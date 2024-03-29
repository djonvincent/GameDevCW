﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine; using UnityEngine.EventSystems;
using System;

public class Player : Actor
{
    public float speed = 90f;
    public GameObject projectile;
    public GameObject flashlightLight;
    public GameObject flashlight;
    public GameObject jacket;
    public GameObject sword;
    public float projectileSpeed = 8f;
    public float projectileAngularSpeed = 50f;
    public float attackCooldown = 0.5f;
    public float bookDamage = 20f;
    public Joystick joystick;
    public bool onStairs{get; private set;}
    public int stairsDirection{get; private set;}
    public Collider2D movementCollider;
    public int apples = 1;
    public int books = 0;
    public AudioClip[] swordSounds;
    public AudioClip[] bookSounds;
    public AudioClip appleSound;
    public AudioSource itemAudio;
    public AudioSource attackAudio;
    
    private bool attacking = false;
    private float nextAttackTime = 0;
    private Camera cam;
    private AudioSource audio;
    public bool hasJacket = false;
    public bool hasSword = false;
    public bool hasFlashlight = false;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        cam = Camera.main;
    }

    // Update is called once per frame
    protected void FixedUpdate()
    {
        HandleMovement();
    }

    protected override void Update() {
        base.Update();
        if (GM.paused || !alive) {
            return;
        }
        if (Input.GetMouseButtonDown(0) && canAttack && hasSword) {
            StartCoroutine("AttackSword");
        }
        if (Input.GetMouseButtonDown(1) && canAttack && books > 0) {
            StartCoroutine("AttackBook");
        }
        if (Input.GetKeyDown(KeyCode.Space) && alive && !attacking && hasFlashlight) {
            flashlightLight.SetActive(!flashlightLight.activeSelf);
            flashlight.SetActive(!flashlight.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            if (health < maxHealth && apples > 0) {
                itemAudio.clip = appleSound;
                itemAudio.Play();
                apples -= 1;
                health = Math.Min(maxHealth, health + 50);
            }
        }
    }

    public void GiveItem(int item) {
        switch (item) {
            case 0:
                hasJacket = true;
                jacket.SetActive(true);
                maxHealth *= 1.5f;
                break;
            case 1:
                hasSword = true;
                break;
            case 2:
                books += 10;
                break;
            case 3:
                hasFlashlight = true;
                break;
            case 4:
                apples += 3;
                break;
        }
        renderers = body.GetComponentsInChildren<SpriteRenderer>();
    }

    private IEnumerator AttackSword() {
        Vector2 target = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 diff = (Vector2)target - ((Vector2)transform.position + new Vector2(0, 0.7f));
        if (Math.Abs(diff.y) > Math.Abs(diff.x)) {
            bool up = diff.y > 0;
            anim.SetFloat("Vertical", up ? 1 : -1);
            anim.SetFloat("Horizontal", 0);
        } else {
            bool right = diff.x > 0;
            anim.SetFloat("Horizontal", right ? 1 : -1);
            anim.SetFloat("Vertical", 0);
        }
        nextAttackTime = Time.time + attackCooldown;
        bool oldFlashlightActive = flashlight.activeSelf;
        bool oldFlashlightLightActive = flashlightLight.activeSelf;
        flashlight.SetActive(false);
        flashlightLight.SetActive(false);
        anim.SetBool("Attacking", true);
        anim.SetTrigger("Slash");
        attacking = true;
        attackAudio.clip = swordSounds[UnityEngine.Random.Range(0, swordSounds.Length)];
        attackAudio.Play();
        yield return new PausableWaitForSeconds(0.5f);
        anim.SetBool("Attacking", false);
        attacking = false;
        flashlight.SetActive(oldFlashlightActive);
        flashlightLight.SetActive(oldFlashlightLightActive);
    }

    private IEnumerator AttackBook() {
        Vector2 target = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 diff = (Vector2)target - ((Vector2)transform.position + new Vector2(0, 0.7f));
        Vector2 offset;
        float rotation;
        bool clockwise;
        if (Math.Abs(diff.y) < 0.6 && Math.Abs(diff.x) < 0.6) {
            yield break;
        }
        if (Math.Abs(diff.y) > Math.Abs(diff.x)) {
            bool up = diff.y > 0;
            offset = new Vector2(0.14f, up ? 1.23f : 0.41f);
            rotation = up ? -90f : 90f;
            anim.SetFloat("Vertical", up ? 1 : -1);
            anim.SetFloat("Horizontal", 0);
            clockwise = up;
        } else {
            bool right = diff.x > 0;
            offset = new Vector2(right ? 0.5f : -0.5f, 0.8f);
            rotation = 180f;
            anim.SetFloat("Horizontal", right ? 1 : -1);
            anim.SetFloat("Vertical", 0);
            clockwise = !right;
        }
        nextAttackTime = Time.time + attackCooldown;
        bool oldFlashlightActive = flashlight.activeSelf;
        bool oldFlashlightLightActive = flashlightLight.activeSelf;
        flashlight.SetActive(false);
        flashlightLight.SetActive(false);
        anim.SetBool("Attacking", true);
        anim.SetTrigger("Throw");
        attacking = true;
        books -= 1;
        attackAudio.clip = bookSounds[UnityEngine.Random.Range(0, bookSounds.Length)];
        attackAudio.Play();
        yield return new PausableWaitForSeconds(0.3f);
        Fire(target, offset, clockwise, rotation);
        yield return new PausableWaitForSeconds(0.5f);
        anim.SetBool("Attacking", false);
        attacking = false;
        flashlight.SetActive(oldFlashlightActive);
        flashlightLight.SetActive(oldFlashlightLightActive);
    }

    private void HandleMovement() {
        float moveH;
        float moveV;
        if (stunned || !alive || attacking || GM.paused) {
            moveH = 0;
            moveV = 0;
        } else {
            moveH = Input.GetAxisRaw("Horizontal");
            moveV = Input.GetAxisRaw("Vertical");
            if (moveH == 0 && moveV == 0 && joystick != null) {
                moveH = joystick.Horizontal;
                moveV = joystick.Vertical;
            }
        }

        Vector2 movement = new Vector2(moveH, moveV);
        movement.Normalize();
        Vector2 velocity = new Vector2(movement.x, movement.y * 0.7f);
        if (moveH != 0 && onStairs) {
            velocity.y += (moveH > 0 ? 1 : -1) * stairsDirection * 0.5f;
            velocity.Normalize();
        }
        if (!stunned) {
            rigidBody.velocity = velocity * speed * Time.fixedDeltaTime;
        }
        if (!(moveH == 0 && moveV == 0)) {
            anim.SetFloat("Horizontal", movement.x);
            anim.SetFloat("Vertical", movement.y);
        }

        float moveSpeed = velocity.magnitude;
        anim.SetFloat("Speed", moveSpeed);
        if (!attacking && moveSpeed > 0) {
            anim.speed = moveSpeed;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D col) {
        base.OnTriggerEnter2D(col);
        switch (col.tag) {
            case "Apple":
                apples += 1;
                Destroy(col.gameObject);
                break;
            case "Book":
                books += UnityEngine.Random.Range(3, 6);
                Destroy(col.gameObject);
                break;
            case "Book Pile":
                books += UnityEngine.Random.Range(6, 12);
                Destroy(col.gameObject);
                break;
            case "Stairs Right":
                onStairs = true;
                stairsDirection = 1;
                break;
            case "Stairs Left":
                onStairs = true;
                stairsDirection = -1;
                break;
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if (!movementCollider.IsTouchingLayers(LayerMask.GetMask("Stairs"))) {
            onStairs= false;
        }
    }

    private void StopAnimations() {
        anim.SetFloat("Horizontal", 0);
        anim.SetFloat("Vertical", 0);
        anim.SetFloat("Speed", 0);
    }

    protected override void OnDie() {
        base.OnDie();
        StopAnimations();
        GM.OnDie();
    }
    
    protected bool canAttack {
        get {
            return alive && Time.time >= nextAttackTime && !attacking;
        }
    }

    void Fire(Vector2 target, Vector2 offset, bool clockwise, float rotation) {
        Vector2 start = (Vector2)transform.position + offset;
        Vector2 direction = target - start;
        direction.Normalize();
        Transform proj = ((GameObject)Instantiate(
            projectile,
            new Vector2(start.x, start.y - 0.8f),
            Quaternion.identity
        )).transform;
        Rigidbody2D projRB = proj.GetComponent<Rigidbody2D>();
        projRB.velocity = direction * projectileSpeed;
        SpinningProjectile projClass = proj.GetComponent<SpinningProjectile>();
        projClass.angle = rotation;
        projClass.angularVelocity = (clockwise ? -1 : 1) * projectileAngularSpeed;
        projClass.owner = this;
        projClass.damage = bookDamage;
        projClass.bounce = true;
        projClass.knockBack = 30;
    }
}
