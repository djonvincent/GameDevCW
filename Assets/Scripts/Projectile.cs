using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public Actor owner;
    public bool dieWithOwner = false;
    public float lifetime = 5f;
    public bool bounce = false;
    public float knockBack = 0;
    public float stunDuration = 0.5f;
    public Transform body {get; private set;}

    protected Rigidbody2D rigidBody;
    protected GameManager GM;
    private float dieTime;
    private Vector2 velocity;

    protected void Awake() {
        body = transform.Find("Body");
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void Start() {
        GM = GameManager.instance;
        dieTime = Time.time + lifetime;
    }

    protected void Update() {
        if (Time.time >= dieTime) {
            Destroy(gameObject);
        }
        if (GM.paused) {
            dieTime += Time.deltaTime;
        }
        if (dieWithOwner && !owner.alive) {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        //Debug.Log("Projectile: CollisionEnter");
        if (!bounce) {
           Destroy(gameObject);
        }
    }
}
