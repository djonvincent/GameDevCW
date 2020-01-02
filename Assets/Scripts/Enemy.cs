using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Actor
{
    public float aggroRadius = 8f;
    public float speed = 0.2f;
    public float damage;
    public float attackCooldown = 3f;
    public float idlePeriod = 1f;
    public Animator anim;
    public bool facePlayer = true;
    public Healthbar healthbar;
    public float distanceToPlayer {get; private set;}
    protected float timeToStartAttack = 0;
    protected float nextAttackTime = 0;

    protected override void Update()
    {
        base.Update();
        healthbar.health = health/100;
        if (facePlayer && alive && !stunned) {
            transform.localScale = new Vector3(
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
        } else if (distanceToPlayer <= aggroRadius && !inCombat) {
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
}
