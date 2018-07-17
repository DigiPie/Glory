using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController2 : EnemyController
{
    protected override void AI()
    {
        if (IsPlayerWithinAttackRange())
        {
            Attack(true);
        }
        else if (IsPlayerWithinChaseRange())
        {
            HomeOnLastTarget();
        }
        else if (isPathDone)
        {
            if (IsTargetWithinAttackRange())
            {
                Attack(false);
            }
            else
            {
                HomeOnLastTarget();
            }
        }
        else
        {
            MoveAlongPath();
        }
    }
}
