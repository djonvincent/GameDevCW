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
        rigidBody.velocity = Vector2.zero;
        bool oldStunned = stunned;
        bool oldImmune = immune;
        stunned = true;
        immune = true;
        for (int t = 0; t < 4; t += 1) {
            foreach (Renderer r in renderers) {
                r.enabled = !r.enabled;
            }
            yield return new WaitForSeconds(0.07f);
        }
        stunned = oldStunned;
        immune = oldImmune;
    }

    void OnTriggerEnter2D(Collider2D other) {
        Debug.Log(other.tag);
        if (other.tag == "Projectile") {
            Debug.Log("Hit");
            Projectile proj = other.GetComponent<Projectile>();
            if (!transform.IsChildOf(proj.owner) && alive) {
                Debug.Log("Enemy projectile");
                if (!immune) {
                    health -= proj.damage;
                }
                if (health <= 0) {
                    alive = false;
                    OnDie();
                }
                if (!stunned) {
                    if (proj.knockBack > 0) {
                        rigidBody.AddForce(
                            other.GetComponent<Rigidbody2D>().velocity.normalized * proj.knockBack
                        );
                    }
                    Debug.Log("Stun");
                    StartCoroutine("Stun");
                }
                Destroy(other.gameObject);
            }
        }
    }
}
