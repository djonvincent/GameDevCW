﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    public float speed = 0.9f;
    public Animator anim;
    public GameObject projectile;
    public GameObject flashlightLight;
    public float projectileSpeed = 5f;
    public float projectileAngularSpeed = 50f;
    public float fireCooldown = 0.5f;
    private float nextFireTime = 0;
    private Camera cam;

    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        cam = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HandleMovement();
    }

    void Update() {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime) {
            nextFireTime = Time.time + fireCooldown;
            Fire();
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            flashlightLight.SetActive(!flashlightLight.activeSelf);
        }
    }

    private void HandleMovement() {
        float moveH;
        float moveV;
        if (stunned || !alive) {
            moveH = 0;
            moveV = 0;
        } else {
            moveH = Input.GetAxisRaw("Horizontal");
            moveV = Input.GetAxisRaw("Vertical")*0.7f;
        }

        Vector2 movement = new Vector2(moveH, moveV);
        if (moveH != 0) {
            movement.Normalize();
        }
        //rigidBody.MovePosition(rigidBody.position + movement*speed*Time.fixedDeltaTime);
        if (!stunned) {
            rigidBody.velocity = movement * speed * Time.fixedDeltaTime * 100;
        }
        anim.SetFloat("Horizontal", moveH);
        anim.SetFloat("Vertical", moveV);
        anim.SetFloat("Speed", movement.magnitude);
    }

    private void StopAnimations() {
        anim.SetFloat("Horizontal", 0);
        anim.SetFloat("Vertical", 0);
        anim.SetFloat("Speed", 0);
    }

    public override void OnDie() {
        StopAnimations();
        anim.SetTrigger("Die");
    }

    void Fire() {
        Vector3 target = cam.ScreenToWorldPoint(Input.mousePosition) - new Vector3(0,1,0);
        target.z = transform.position.z;
        Vector3 start = transform.position;
        Vector3 direction = target - start;
        direction.Normalize();
        Transform proj = ((GameObject)Instantiate(projectile, start, Quaternion.identity)).transform;
        Rigidbody2D projRB = proj.GetComponent<Rigidbody2D>();
        projRB.velocity = direction * projectileSpeed;
        SpinningProjectile projClass = proj.GetComponent<SpinningProjectile>();
        projClass.angularVelocity = (target.x < start.x ? 1 : -1) * projectileAngularSpeed;
        projClass.owner = transform;
        projClass.damage = 5f;
        projClass.bounce = true;
        projClass.knockBack = 30;
    }
}
