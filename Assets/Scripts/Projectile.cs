using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public Transform owner;
    public float lifetime;
    public bool bounce = false;

    void Start() {
      Destroy(gameObject, lifetime);
    }

    /*
    void OnTriggerEnter2D(Collider2D other) {
        if (!other.transform.IsChildOf(owner)) {
           Destroy(gameObject);
        }
    }*/

    void OnCollisionEnter2D(Collision2D other) {
        Debug.Log("Projectile: CollisionEnter");
        if (!bounce) {
           Destroy(gameObject);
        }
    }
}
