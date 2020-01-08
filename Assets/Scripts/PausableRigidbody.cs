using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausableRigidbody : MonoBehaviour
{
    private bool paused;
    private Vector2 velocity;
    private float angularVelocity; 
    private GameManager GM;
    private Rigidbody2D rigidBody;

    void Awake() {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void Start() {
        GM = GameManager.instance;
    }

    void FixedUpdate() {
        if (!paused && GM.paused) {
            paused = true;
            velocity = rigidBody.velocity;
            angularVelocity = rigidBody.angularVelocity;
            rigidBody.velocity = Vector2.zero;
            rigidBody.angularVelocity = 0f;
        }
        if (paused && !GM.paused) {
            paused = false;
            rigidBody.velocity = velocity;
            rigidBody.angularVelocity = angularVelocity;
        }
    }
}
