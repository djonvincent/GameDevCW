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
    public Rigidbody2D rigidBody;
    private SpriteRenderer[] renderers;
    public GameManager GM;

    public virtual void Awake() {
        rigidBody = GetComponent<Rigidbody2D>();
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    void Start() {
        GM = GameManager.instance;
    }

    public virtual void OnDie(){}

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

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Projectile") {
            Projectile proj = other.GetComponent<Projectile>();
            if (!transform.IsChildOf(proj.owner) && alive) {
                if (!immune) {
                    health -= proj.damage;
                }
                if (health <= 0) {
                    alive = false;
                    OnDie();
                } else if (!stunned) {
                    if (proj.knockBack > 0) {
                        rigidBody.velocity = Vector2.zero;
                        rigidBody.AddForce(
                            other.GetComponent<Rigidbody2D>().velocity.normalized * proj.knockBack
                        );
                    }
                    StartCoroutine("Stun", proj.stunDuration);
                }
                Destroy(other.gameObject);
            }
        }
    }
}
