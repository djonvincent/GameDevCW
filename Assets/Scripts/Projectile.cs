using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public Actor owner;
    public float lifetime = 5f;
    public bool bounce = false;
    public float knockBack = 0;
    public float stunDuration = 0.5f;
    protected Rigidbody2D rigidBody;
    public Transform body {get; private set;}

    protected void Awake() {
        body = transform.Find("Body");
        rigidBody = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter2D(Collision2D col) {
        //Debug.Log("Projectile: CollisionEnter");
        if (!bounce) {
           Destroy(gameObject);
        }
    }
}
