using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle : Enemy
{
    private Vector2 attackPosition;
    private bool dormant = true;

    protected override void Update()
    {
        base.Update();
        if (GM.paused) {
            return;
        }
        anim.SetBool("Dormant", dormant);
        if (!dormant) {
            Vector2 diff = GM.player.transform.position - transform.position;
            if (diff.magnitude > 3) {
                nextAttackTime = Time.time;
            }
            if (canAttack) {
                StartAttack();
            }
        }
    }

    public void StartAttack() {
        nextAttackTime = Time.time + attackCooldown;
        dormant = true;
        float spawnTime = 1.5f + Random.value * 2;
        StartCoroutine(GetNextAttackPosition(spawnTime));
        StartCoroutine(Attack(spawnTime + 0.1f));
    }

    public override void StartCombat() {
        focusCamera = true;
        base.StartCombat();
        dormant = false;
    }

    public override void StopCombat() {
        base.StopCombat();
        if (GM.playerClass.alive) {
            dormant = true;
        }
    }

    private IEnumerator GetNextAttackPosition(float wait) {
        float waited = 0f;
        while (waited < wait) {
            if (!inCombat) {
                yield break;
            }
            if (!GM.paused) {
                waited += Time.deltaTime;
            }
            yield return null;
        }
        focusCamera = false;
        attackPosition = GM.player.transform.position + new Vector3(0, -0.2f, 0);
    }

    public IEnumerator Attack(float wait) {
        float waited = 0f;
        while (waited < wait) {
            if (!inCombat) {
                yield break;
            }
            if (!GM.paused) {
                waited += Time.deltaTime;
            }
            yield return null;
        }
        focusCamera = true;
        transform.position = attackPosition;
        dormant = false;
    }

    protected override void OnAttacked() {
        base.OnAttacked();
        nextAttackTime = Time.time;
        timeToStartAttack = Time.time;
        if (canAttack) {
            StartAttack();
        }
    }

}
