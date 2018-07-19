using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Moves to objective and attacks it.
 * Ignores player.
 */
public class EnemyController1 : EnemyController
{
    protected override void AI()
    {
        // If within attack range of objective
        if (absDistToTargetX < attackRange)
        {
            // Attack objective
            // If ready
            if (Time.timeSinceLevelLoad > attackReadyTime)
            {
                enemyState = EnemyState.AttackingObjective;
                AttackObjective();
            }
        }
        // If not, move to objective
        else
        {
            // Move to objective
            enemyState = EnemyState.Run;
            AImoveH = (distToTargetX < 0) ? -1 : 1;
        }
    }
}
