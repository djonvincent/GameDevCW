using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public float health = 100f;
    public bool alive = true;
    public bool stunned = false;
    public bool immune = false;
    public bool inCombat = false;
    public Rigidbody2D rigidBody {get; private set;}
    protected SpriteRenderer[] renderers;
    protected GameManager GM;
    protected Transform body;
    protected bool jumping = false;
    public Collider2D hitbox {get; private set;}

    protected virtual void Awake() {
        rigidBody = GetComponent<Rigidbody2D>();
        body = transform.Find("Body");
        renderers = body.GetComponentsInChildren<SpriteRenderer>();
        Debug.Log(renderers.Length);
        hitbox = body.Find("Hitbox").GetComponent<Collider2D>();
    }

    protected virtual void Start() {
        GM = GameManager.instance;
    }

    protected virtual void OnDie(){}
    protected virtual void OnAttacked(){}

    protected virtual void Update() {
        if (alive && health <= 0) {
            alive = false;
            Debug.Log("Dead");
            OnDie();
        }
    }

    protected virtual void Attacked(
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
        }
        rigidBody.AddForce(force);
        if (!jumping && jumpSpeed > 0) {
            StartCoroutine(Jump(jumpSpeed));
        }
    }

    public IEnumerator Jump(float velocity) {
        jumping = true;
        float v = velocity;
        float y = 0;
        do {
            body.localPosition = new Vector2(0, y);
            y += v * Time.deltaTime;
            v -= 10 * Time.deltaTime;
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
        for (float t = 0f; t < duration; t += 0.07f) {
            foreach (Renderer r in renderers) {
                r.enabled = !r.enabled;
            }
            yield return new WaitForSeconds(0.07f);
        }
        foreach (Renderer r in renderers) {
            r.enabled = true;
        }
        stunned = oldStunned;
        immune = oldImmune;
    }

    void OnTriggerEnter2D(Collider2D col) {
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
