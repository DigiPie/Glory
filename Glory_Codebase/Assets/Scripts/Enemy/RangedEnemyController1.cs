using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyController1 : EnemyController
{
    protected override void AI()
    {
        if (IsPlayerWithinAttackRange())
        {
            Attack(true);
        }
        else
        {
            HomeOnLastTargetWithChaseRange();
        }
    }
}