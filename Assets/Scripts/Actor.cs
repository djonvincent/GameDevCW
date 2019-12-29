using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public float health = 100f;
    public bool alive = true;

    void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Actor: TriggerEnter");
        if (other.tag == "Projectile") {
            Projectile proj = other.GetComponent<Projectile>();
            if (!transform.IsChildOf(proj.owner) && alive) {
                health -= proj.damage;
                if (health <= 0) {
                    alive = false;
                }
                Destroy(other.gameObject);
            }
        }
    }
}
