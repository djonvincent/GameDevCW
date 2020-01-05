using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Player : Actor
{
    public float speed = 90f;
    public Animator anim;
    public GameObject projectile;
    public GameObject flashlightLight;
    public GameObject flashlight;
    public float projectileSpeed = 8f;
    public float projectileAngularSpeed = 50f;
    public float attackCooldown = 0.6f;
    public float bookDamage = 20f;
    public Joystick joystick;
    
    private bool attacking = false;
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
        if (Input.GetButtonDown("Fire1") && canAttack &&
            !EventSystem.current.IsPointerOverGameObject()) {
            StartCoroutine("Attack");
        }
        if (Input.GetKeyDown(KeyCode.Space) && alive && !attacking) {
            flashlightLight.SetActive(!flashlightLight.activeSelf);
        }
    }

    private IEnumerator Attack() {
        Debug.Log(Screen.width);
        Debug.Log(cam.scaledPixelWidth);
        Debug.Log(Screen.height);
        Vector2 viewportPosition = new Vector2(
            (Input.mousePosition.x/Screen.width - 0.5f) / cam.rect.width,
            (Input.mousePosition.y/Screen.height - 0.5f) / cam.rect.height
        );
        Debug.Log(Input.mousePosition);
        Debug.Log($"{viewportPosition.x}, {viewportPosition.y}");
        //Vector2 target = (cam.orthographicSize * 2 * viewportPosition) +
        //   (Vector2)cam.transform.position;
        //Vector2 target = (Vector2)cam.ViewportToWorldPoint(viewportPosition);
        Vector2 target = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 diff = (Vector2)target - ((Vector2)transform.position + new Vector2(0, 0.7f));
        Debug.Log(diff);
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
        anim.SetTrigger("Attack");
        attacking = true;
        yield return new WaitForSeconds(0.3f);
        Fire(target, offset, clockwise, rotation);
        yield return new WaitForSeconds(0.3f);
        anim.SetBool("Attacking", false);
        attacking = false;
        flashlight.SetActive(oldFlashlightActive);
        flashlightLight.SetActive(oldFlashlightLightActive);
    }

    private void HandleMovement() {
        float moveH;
        float moveV;
        if (stunned || !alive || attacking) {
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
        if (!stunned) {
            rigidBody.velocity = velocity * speed * Time.fixedDeltaTime;
        }
        if (!attacking && !(moveH == 0 && moveV == 0)) {
            anim.SetFloat("Horizontal", movement.x);
            anim.SetFloat("Vertical", movement.y);
        }

        float moveSpeed = velocity.magnitude;
        anim.SetFloat("Speed", moveSpeed);
        if (moveSpeed > 0) {
            anim.speed = moveSpeed;
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
        anim.SetTrigger("Die");
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
