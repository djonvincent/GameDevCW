using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    public float speed = 0.9f;
    public Animator anim;
    public GameObject projectile;
    public GameObject flashlightLight;
    public float projectileSpeed = 8f;
    public float projectileAngularSpeed = 50f;
    public float attackCooldown = 0.5f;
    public float bookDamage = 20f;
    private float nextAttackTime = 0;
    private Camera cam;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        cam = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HandleMovement();
    }

    protected override void Update() {
        base.Update();
        if (Input.GetButtonDown("Fire1") && canAttack) {
            nextAttackTime = Time.time + attackCooldown;
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

    protected override void OnDie() {
        base.OnDie();
        StopAnimations();
        anim.SetTrigger("Die");
    }
    
    protected bool canAttack {
        get {
            return alive && Time.time >= nextAttackTime;
        }
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
        projClass.damage = bookDamage;
        projClass.bounce = true;
        projClass.knockBack = 30;
    }
}
