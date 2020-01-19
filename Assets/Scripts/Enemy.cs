using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : Actor
{
    public float aggroRadius = 8f;
    public float speed = 0.2f;
    public float damage;
    public float attackCooldown = 3f;
    public bool damageOnTouch = true;
    public float touchKnockAmount = 200f;
    public float idlePeriod = 1f;
    public bool facePlayer = true;
    public Healthbar healthbar;
    public float distanceToPlayer {get; private set;}
    public bool focusCamera = true;
    public GameObject[] lootList;
    public float lootAvg = 0.5f;
    public float swordKnockBackAmount = 300f;
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
        distanceToPlayer =
            (GM.player.transform.position - transform.position).magnitude; 
        if (!GM.playerClass.alive) {
            inCombat = false;
        } else if (distanceToPlayer <= aggroRadius && !inCombat && alive &&
            GM.playerClass.alive) {
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

    protected override void DropLoot() {
        if (lootList.Length == 0) {
            return;
        }
        double lootCount = Math.Round(UnityEngine.Random.value * 2 * lootAvg);
        Debug.Log(lootCount);
        for (int i=0; i < (int)lootCount; i++) {
            GameObject loot = lootList[UnityEngine.Random.Range(0, lootList.Length)];
            GameObject lootObj = Instantiate(loot);
            Vector2 pos = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle*0.5f;
            lootObj.transform.position = pos;
            //Vector2 diff = transform.position - GM.player.transform.position;
            //Vector2 lootVelocity = diff.normalized * 5f;
            //lootObj.GetComponent<Rigidbody2D>().velocity = lootVelocity;
        }
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
            Attacked(15, 0.4f, diff.normalized * swordKnockBackAmount);
        }
    }

    protected void OnTriggerStay2D(Collider2D col) {
        if (col == GM.playerClass.hitbox && damageOnTouch && alive) {
            int knockDirection =
                GM.player.transform.position.x > transform.position.x ? 1 : -1;
            Vector2 knockForce = new Vector2(knockDirection * touchKnockAmount, 0);
            GM.playerClass.Attacked(damage, 1f, knockForce, 3f);
        }
    }
}
