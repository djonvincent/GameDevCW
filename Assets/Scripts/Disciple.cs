using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disciple : Enemy
{
    public float speed = 0.2f;
    public Transform firePoint;
    public GameObject projectile;
    public float damage;
    public float fireInterval;
    public Animator anim;
    public Vector3 targetPosition;
    public float fireCooldown = 3f;
    private float nextFireTime = 0;
    private float _aggroRadius = 5f;
    public override float aggroRadius {
        get {return _aggroRadius;}
        set {_aggroRadius = value;}
    }

    public override void Awake()
    {
        base.Awake();
        targetPosition = transform.position;
    }

    void FixedUpdate() {
        Vector3 diff = targetPosition - transform.position;
        if (!stunned && alive) {
            if (diff.magnitude >= Time.fixedDeltaTime * speed) {
                rigidBody.velocity = diff.normalized * speed;
            } else {
                rigidBody.velocity = Vector2.zero;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(
            GM.player.transform.position.x < transform.position.x ? -1 : 1,
            1,
            1
        );
        anim.SetFloat("Health", health);
        // Engage in combat
        float playerDistance =
            (transform.position - GM.player.transform.position).magnitude;
        if (playerDistance <= aggroRadius && !inCombat) {
            inCombat = true;
            GM.StartCombat(this);
        } else if (playerDistance > aggroRadius + 1 && inCombat) {
            inCombat = false;
            GM.Flee();
        }

        if (inCombat && Time.time >= nextFireTime) {
            nextFireTime = Time.time + fireCooldown;
            Invoke("Fire", 0.52f);
            anim.SetTrigger("Attack");
        }
    }

    public void Fire()
    {
        Vector3 start = firePoint.position;
        float height = firePoint.localPosition.y;
        Vector3 target = GM.player.transform.position + new Vector3(0,1,0);
        Vector3 direction = target - start;
        direction.Normalize();
        GameObject proj = (GameObject)Instantiate(projectile);
        Rigidbody2D projRB = proj.GetComponent<Rigidbody2D>();
        proj.transform.position = start - new Vector3(0,height,0) + direction;
        Transform child = proj.transform.GetChild(0).transform;
        child.localPosition = new Vector3(0,height,0);
        child.up = -direction;
        projRB.velocity = direction * 5f;
        Projectile projClass = proj.GetComponent<Projectile>();
        projClass.owner = transform;
        projClass.damage = damage;
        projClass.lifetime = 5f;
        projClass.knockBack = 30;
    }
}
