using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public Transform owner;
    public float lifetime = 5f;
    public bool bounce = false;
    public float knockBack = 0;
    private Rigidbody2D rigidBody;

    public virtual void Start() {
        Destroy(gameObject, lifetime);
        rigidBody = GetComponent<Rigidbody2D>();
    }

    /*
    void OnTriggerEnter2D(Collider2D other) {
        if (!other.transform.IsChildOf(owner)) {
           Destroy(gameObject);
        }
    }*/

    void OnCollisionEnter2D(Collision2D col) {
        Debug.Log("Projectile: CollisionEnter");
        if (!bounce) {
           Destroy(gameObject);
        }
    }
}
