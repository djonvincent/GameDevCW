using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    public float speed = 0.9f;
    public List<Animator> anims;
    public GameObject projectile;
    public float projectileSpeed = 5f;
    public float projectileAngularSpeed = 50f;
    private Camera cam;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        cam = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!stunned && alive) {
            HandleMovement();
        }
    }
void Update() {
        if (Input.GetButtonDown("Fire1")) {
            Fire();
        }
    }

    private void HandleMovement() {
        float moveH;
        float moveV;
        if (stunned) {
            moveH = 0;
            moveV = 0;
        } else {
            moveH = alive ? Input.GetAxisRaw("Horizontal") : 0;
            moveV = alive ? Input.GetAxisRaw("Vertical")*0.7f : 0;
        }

        Vector2 movement = new Vector2(moveH, moveV);
        if (moveH != 0) {
            movement.Normalize();
        }
        //rigidBody.MovePosition(rigidBody.position + movement*speed*Time.fixedDeltaTime);
        rigidBody.velocity = movement * speed * Time.fixedDeltaTime * 100;
        foreach(Animator anim in anims) {
            anim.SetFloat("Horizontal", moveH);
            anim.SetFloat("Vertical", moveV);
            anim.SetFloat("Speed", movement.magnitude);
        }
    }

    public override void OnDie() {
        foreach(Animator anim in anims) {
            anim.SetFloat("Horizontal", 0);
            anim.SetFloat("Vertical", 0);
            anim.SetFloat("Speed", 0);
            anim.SetTrigger("Die");
        }
    }

    new public IEnumerator Stun() {
        foreach (Animator anim in anims) {
            anim.SetFloat("Speed", 0f);
        }
        return base.Stun();
    }

    void Fire() {
        Debug.Log("Fire");
        Vector3 target = cam.ScreenToWorldPoint(Input.mousePosition) - new Vector3(0,1,0);
        target.z = transform.position.z;
        Vector3 start = transform.position;
        Vector3 direction = target - start;
        direction.Normalize();
        Transform proj = ((GameObject)Instantiate(projectile, start, Quaternion.identity)).transform;
        Debug.Log(proj);
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
