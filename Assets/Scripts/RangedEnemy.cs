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
    public bool killProjectileOnDie = false;

    protected override void Awake()
    {
        base.Awake();
        targetPosition = transform.position;
    }

    protected void FixedUpdate() {
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
        if (GM.paused) {
            return;
        }
        if (canAttack) {
            nextAttackTime = Time.time + attackCooldown;
            StartCoroutine(FireIn(0.52f));
            anim.SetTrigger("Attack");
        }
    }

    protected IEnumerator FireIn(float wait) {
        float waited = 0f;
        while (waited < wait) {
            if (GM.paused) {
                yield return null;
                continue;
            }
            waited += Time.deltaTime;
        }
        Fire();
    }

    public void Fire()
    {
        Vector3 start = firePoint.position;
        //float height = firePoint.localPosition.y;
        float height = 1.1f;
        Vector3 target = GM.player.transform.position + new Vector3(0,1.2f,0);
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
        projClass.owner = this;
        projClass.damage = damage;
        projClass.lifetime = 5f;
        projClass.knockBack = 30;
        projClass.dieWithOwner = killProjectileOnDie;
    }
}
