using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingEnemy : Enemy
{
    public Vector3 targetPosition;
    private float targetHeight;

    protected override void Awake() {
        base.Awake();
        targetHeight = Random.value * 0.7f;
    }

    protected void FixedUpdate() {
        Vector3 diff = GM.player.transform.position + new Vector3(0,targetHeight,0) - transform.position;
        if (!stunned && alive && inCombat) {
            if (!hitbox.IsTouching(GM.playerClass.hitbox)) {
                rigidBody.velocity = diff.normalized * speed;
                Vector2 diffNormalised = diff.normalized;
                anim.SetFloat("Horizontal", diffNormalised.x);
                anim.SetFloat("Vertical", diffNormalised.y);
            } else {
                rigidBody.velocity = Vector2.zero;
            }
        }
    }
}
