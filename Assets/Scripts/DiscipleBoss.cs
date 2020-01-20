using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscipleBoss : RangedEnemy
{
    public GameObject bossLoot;

    protected override void OnDie() {
        base.OnDie();
        Vector2 position = transform.position + new Vector3(0, -0.3f, 0);
        Instantiate(bossLoot, position, Quaternion.identity);
    }
}
