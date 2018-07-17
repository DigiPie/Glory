using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController2 : EnemyController
{
    protected override void AI()
    {
        if (IsPlayerWithinRange())
        {
            Attack(true);
        }
        else if (isPathDone)
        {
            if (IsTargetWithinRange())
            {
                Attack(false);
            }
            else
            {
                HomeOnFinalTarget();
            }
        }
        else
        {
            MoveAlongPath();
        }
    }
}
