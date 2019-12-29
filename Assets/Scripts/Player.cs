using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    public float speed = 0.9f;
    public List<Animator> anims;
    public Rigidbody2D projectile;
    public float projectileSpeed = 5f;
    public float projectileAngularSpeed = 50f;
    private Rigidbody2D rigidBody;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        //DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float moveH = alive ? Input.GetAxisRaw("Horizontal") : 0;
        float moveV = alive ? Input.GetAxisRaw("Vertical")*0.7f : 0;
        Vector2 movement = new Vector2(moveH, moveV);
        if (moveH != 0) {
            movement.Normalize();
        }
        //rigidBody.MovePosition(rigidBody.position + movement*speed*Time.fixedDeltaTime);
        rigidBody.velocity = movement * speed * Time.fixedDeltaTime * 100;
        foreach(Animator anim in anims) {
            anim.SetFloat("Horizontal", moveH);
            anim.SetFloat("Vertical", moveV); anim.SetFloat("Speed", movement.magnitude);
        }
    }

    void Update() {
        if (Input.GetButtonDown("Fire1")) {
            Fire();
        }
        if (health < 0.0001f && alive) {
            alive = false;
            foreach(Animator anim in anims) {
                anim.SetTrigger("Die");
            }
        }
    }

    void Fire() {
        Vector3 target = cam.ScreenToWorldPoint(Input.mousePosition);
        target.z = transform.position.z;
        Vector3 start = transform.position + new Vector3(0,1,0);
        Vector3 direction = target - start;
        direction.Normalize();
        Rigidbody2D proj = Instantiate(projectile) as Rigidbody2D;
        proj.transform.position = start;
        proj.velocity = direction * projectileSpeed;
        proj.angularVelocity = (target.x < start.x ? 1 : -1) * projectileAngularSpeed;
        Projectile projClass = proj.GetComponent<Projectile>();
        projClass.owner = transform;
        projClass.damage = 5f;;
        projClass.lifetime = 5f;
        projClass.bounce = true;
    }
}
