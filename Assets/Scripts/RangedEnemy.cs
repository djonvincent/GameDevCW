using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    public Transform firePoint;
    public GameObject projectile;
    public float fireInterval;
    public Vector3 targetPosition;
    public float projectileSpeed = 4f;

    protected override void Awake()
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

    protected override void Update()
    {
        base.Update();
        if (canAttack) {
            nextAttackTime = Time.time + attackCooldown;
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
        // Projectile must have a child object named "Body"
        projRB.velocity = direction * projectileSpeed;
        Projectile projClass = proj.GetComponent<Projectile>();
        projClass.body.localPosition = new Vector3(0,height,0);
        projClass.body.up = -direction;
        projClass.owner = transform;
        projClass.damage = damage;
        projClass.lifetime = 5f;
        projClass.knockBack = 30;
    }
}
