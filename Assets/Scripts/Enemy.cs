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
    protected float timeToStartAttack = 0;
    protected float nextAttackTime = 0;

    protected override void Update()
    {
        base.Update();
        if (facePlayer) {
            transform.localScale = new Vector3(
                GM.player.transform.position.x < transform.position.x ? -1 : 1,
                1,
                1
            );
        }
        anim.SetFloat("Health", health);
        float playerDistance =
            (GM.player.transform.position - transform.position).magnitude; 
        if (!GM.playerClass.alive) {
            inCombat = false;
        } else if (playerDistance <= aggroRadius && !inCombat) {
            StartCombat();
        } else if (playerDistance > aggroRadius + 1 && inCombat) {
            inCombat = false;
            GM.Flee();
        }
    }

    protected override void OnDie() {
        base.OnDie();
        GM.StopCombat();
    }

    public virtual void StartCombat() {
        inCombat = true;
        timeToStartAttack = Time.time + idlePeriod;
        GM.StartCombat(this);
    }

    protected bool canAttack {
        get {
            return alive && inCombat && 
                Time.time >= nextAttackTime &&
                Time.time >= timeToStartAttack;
        }
    }
}
