using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Actor
{
    public float aggroRadius = 8f;
    public float speed = 0.2f;
    public float damage;
    public Animator anim;

    public virtual void Update()
    {
        transform.localScale = new Vector3(
            GM.player.transform.position.x < transform.position.x ? -1 : 1,
            1,
            1
        );
        anim.SetFloat("Health", health);
        float playerDistance =
            (GM.player.transform.position - transform.position).magnitude; 
        if (!GM.playerClass.alive) {
            inCombat = false;
        } else if (playerDistance <= aggroRadius && !inCombat) {
            inCombat = true;
            GM.StartCombat(this);
        } else if (playerDistance > aggroRadius + 1 && inCombat) {
            inCombat = false;
            GM.Flee();
        }
    }
}
