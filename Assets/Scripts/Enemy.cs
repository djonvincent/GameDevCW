using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Actor
{
    public float aggroRadius = 8f;
    public float speed = 0.2f;
    public float damage;
    public float attackCooldown = 3f;
    public bool damageOnTouch = true;
    public float touchKnockAmount = 100f;
    public float idlePeriod = 1f;
    public Animator anim;
    public bool facePlayer = true;
    public Healthbar healthbar;
    public float distanceToPlayer {get; private set;}
    public bool focusCamera = true;
    protected float timeToStartAttack = 0;
    protected float nextAttackTime = 0;

    protected override void Update()
    {
        if (GM.paused) {
            nextAttackTime += Time.deltaTime;
            timeToStartAttack += Time.deltaTime;
            return;
        }
        base.Update();
        healthbar.health = health/maxHealth;
        if (facePlayer && alive && !stunned) {
            body.localScale = new Vector3(
                GM.player.transform.position.x < transform.position.x ? -1 : 1,
                1,
                1
            );
        }
        anim.SetFloat("Health", health);
        distanceToPlayer =
            (GM.player.transform.position - transform.position).magnitude; 
        if (!GM.playerClass.alive) {
            inCombat = false;
        } else if (distanceToPlayer <= aggroRadius && !inCombat && alive) {
            timeToStartAttack = Time.time;
            StartCombat();
        } else if (distanceToPlayer > aggroRadius + 1 && inCombat) {
            StopCombat();
        }
    }

    protected override void OnDie() {
        base.OnDie();
        healthbar.gameObject.SetActive(false);
        StopCombat();
    }

    public virtual void StartCombat() {
        inCombat = true;
        timeToStartAttack = Time.time + idlePeriod;
        GM.AddEnemy(this);
    }

    public virtual void StopCombat() {
        inCombat = false;
        GM.RemoveEnemy(this);
    }

    protected bool canAttack {
        get {
            return alive && inCombat && 
                Time.time >= nextAttackTime &&
                Time.time >= timeToStartAttack;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D col) {
        base.OnTriggerEnter2D(col);
        if (col.tag == "Sword") {
            Vector2 diff = (transform.position + new Vector3(0, 0f, 0)) - GM.player.transform.position;
            Attacked(15, 0.4f, diff.normalized * 300f);
        }
    }

    protected void OnTriggerStay2D(Collider2D col) {
        if (col == GM.playerClass.hitbox && damageOnTouch) {
            int knockDirection =
                GM.player.transform.position.x > transform.position.x ? 1 : -1;
            Vector2 knockForce = new Vector2(knockDirection * touchKnockAmount, 0);
            GM.playerClass.Attacked(damage, 1f, knockForce, 3f);
        }
    }
}
