using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningProjectile : Projectile
{
    public float angularVelocity = 0;
    public float angle = 0;
    private Transform child;

    void Start() {
        child = transform.GetChild(0);
    }
    
    void FixedUpdate() {
        angle = (angularVelocity*Time.fixedDeltaTime + angle) % 360;
        child.eulerAngles = new Vector3(0,0,angle);
    }
}
