using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscipleBoss : RangedEnemy
{
    public GameObject bossLoot;

    protected override void OnDie() {
        base.OnDie();
        Instantiate(bossLoot, transform.position, Quaternion.identity);
    }
}
