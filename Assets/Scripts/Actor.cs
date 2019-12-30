using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public float health = 100f;
    public bool alive = true;
    public bool stunned = false;
    public bool immune = false;
    public Rigidbody2D rigidBody;
    private SpriteRenderer[] renderers;

    public virtual void Start() {
        rigidBody = GetComponent<Rigidbody2D>();
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public virtual void OnDie(){}

    public IEnumerator Stun() {
        bool oldStunned = stunned;
        bool oldImmune = immune;
        stunned = true;
        immune = true;
        for (int t = 0; t < 8; t += 1) {
            foreach (Renderer r in renderers) {
                r.enabled = !r.enabled;
            }
            yield return new WaitForSeconds(0.07f);
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
                        rigidBody.AddForce(
                            other.GetComponent<Rigidbody2D>().velocity.normalized * proj.knockBack
                        );
                    }
                    StartCoroutine("Stun");
                }
                Destroy(other.gameObject);
            }
        }
    }
}
