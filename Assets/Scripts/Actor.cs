using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public float maxHealth = 100f;
    public float health = 100f;
    public Animator anim;
    public bool alive{get; private set;}
    public bool stunned = false;
    public bool immune = false;
    public bool inCombat = false;
    public bool destroyOnDeath = false;
    public Rigidbody2D rigidBody {get; private set;}
    protected SpriteRenderer[] renderers;
    protected GameManager GM;
    protected Transform body;
    protected bool jumping = false;
    public Collider2D hitbox {get; private set;}
    public AudioSource hurtAudio;
    public AudioClip[] hurtSounds;

    protected virtual void Awake() {
        alive = true;
        rigidBody = GetComponent<Rigidbody2D>();
        body = transform.Find("Body");
        renderers = body.GetComponentsInChildren<SpriteRenderer>();
        hitbox = body.Find("Hitbox").GetComponent<Collider2D>();
    }

    protected virtual void Start() {
        GM = GameManager.instance;
    }

    protected virtual void OnDie(){}
    protected virtual void OnAttacked(){}

    protected virtual void Update() {
        if (GM.paused) {
            return;
        }
        anim.SetFloat("Health", health);
        if (health > 0) {
            alive = true;
        } else if (alive && health <= 0) {
            alive = false;
            OnDie();
        }
    }

    protected virtual void DropLoot(){}

    public virtual void Attacked(
        float damage,
        float stunDuration = 0f,
        Vector2 force = new Vector2(),
        float jumpSpeed = 0f
    ) {
        if (immune) {
            return;
        }
        health -= damage;
        OnAttacked();
        if (!stunned && stunDuration > 0) {
            StartCoroutine(Stun(stunDuration));
        } else if (health == 0) {
            DropLoot();
            if (destroyOnDeath) {
                OnDie();
                Destroy(gameObject);
            }
        }
        rigidBody.AddForce(force);
        if (!jumping && jumpSpeed > 0) {
            StartCoroutine(Jump(jumpSpeed));
        }
        if (hurtSounds.Length > 0 && hurtAudio != null) {
            hurtAudio.clip = hurtSounds[Random.Range(0, hurtSounds.Length)];
            hurtAudio.Play();
        }
    }

    public IEnumerator Jump(float velocity) {
        jumping = true;
        float v = velocity;
        float y = 0;
        do {
            if (!GM.paused) {
                body.localPosition = new Vector2(0, y);
                y += v * Time.deltaTime;
                v -= 10 * Time.deltaTime;
            }
            yield return null;
        } while (y > 0);
        body.localPosition = Vector2.zero;
        jumping = false;
    }

    public IEnumerator Stun(float duration) {
        bool oldStunned = stunned;
        bool oldImmune = immune;
        stunned = true;
        immune = true;
        float waited = 0;
        float t = 0;
        while (t < duration) {
            foreach (Renderer r in renderers) {
                r.enabled = !r.enabled;
            }
            while (waited < 0.07f) {
                if (!GM.paused) {
                    waited += Time.deltaTime;
                    t += Time.deltaTime;
                }
                yield return null;
            }
            waited = 0f;
        }
        if (!alive) {
            DropLoot();
            OnDie();
            if (destroyOnDeath) {
                Destroy(gameObject);
            }
        }
        foreach (Renderer r in renderers) {
            r.enabled = true;
        }
        stunned = oldStunned;
        immune = oldImmune;
    }

    protected virtual void OnTriggerEnter2D(Collider2D col) {
        if (col.attachedRigidbody == null) {
            return;
        }
        GameObject other = col.attachedRigidbody.gameObject;
        if (other.tag != "Projectile") {
            return;
        }
        Projectile proj = other.GetComponent<Projectile>();
        if (proj.owner == this || !alive) {
            return;
        }
        Vector2 knockBack = Vector2.zero;
        if (proj.knockBack > 0) {
            rigidBody.velocity = Vector2.zero;
            knockBack = other.GetComponent<Rigidbody2D>().velocity.normalized *
                proj.knockBack;
        }
        Attacked(proj.damage, proj.stunDuration, knockBack);
        Destroy(other.gameObject);
    }
}
