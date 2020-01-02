using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle : Enemy
{
    private Vector2 attackPosition;

    protected override void Update()
    {
        base.Update();
        if (canAttack) {
            StartAttack();
        }
        if (!inCombat && GM.playerClass.alive) {
            anim.SetBool("Dormant", true);
        }
    }

    public void StartAttack() {
        nextAttackTime = Time.time + attackCooldown;
        anim.SetBool("Dormant", true);
        float spawnTime = 0.5f + Random.value * 2;
        Invoke("GetNextAttackPosition", spawnTime);
        Invoke("Attack", spawnTime + 0.1f);
    }

    public override void StartCombat() {
        base.StartCombat();
        anim.SetBool("Dormant", false);
    }

    private void GetNextAttackPosition() {
        attackPosition = GM.player.transform.position + new Vector3(0, -0.2f, 0);
    }

    void OnTriggerStay2D(Collider2D col) {
        if (col == GM.playerClass.hitbox && !GM.playerClass.stunned &&
            !GM.playerClass.immune) {
            GM.playerClass.health -= damage;
            int knockDirection =
                GM.player.transform.position.x > transform.position.x ? 1 : -1;
            Vector2 knockForce = new Vector2(knockDirection * 300f, 0);
            GM.playerClass.rigidBody.AddForce(knockForce);
            StartCoroutine(GM.playerClass.Jump(3f));
            StartCoroutine(GM.playerClass.Stun(1f));
        }
    }

    public void Attack() {
        if (!inCombat) {
            return;
        }
        transform.position = attackPosition;
        anim.SetBool("Dormant", false);
    }

    protected override void OnAttacked() {
        base.OnAttacked();
        nextAttackTime = Time.time;
        if (canAttack) {
            StartAttack();
        }
    }

}
