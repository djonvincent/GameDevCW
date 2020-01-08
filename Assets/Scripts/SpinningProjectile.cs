using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningProjectile : Projectile
{
    public float angularVelocity = 0;
    public float angle = 0;
    
    protected void FixedUpdate() {
        if (GM.paused) {
            return;
        }
        angle = (angularVelocity*Time.fixedDeltaTime + angle) % 360;
        body.eulerAngles = new Vector3(0,0,angle);
    }
}
