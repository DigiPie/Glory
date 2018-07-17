using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController1 : EnemyController
{
    protected override void AI()
    {
        if (isPathDone)
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
