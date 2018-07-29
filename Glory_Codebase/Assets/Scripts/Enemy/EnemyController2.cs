using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Moves to objective and attacks it.
 * If player appears within chase range of this enemy, chase player.
 * If player is within attack range of this enemy, attack player.
 * If player exits chase range of this enemy, resume movement to objective.
 */
public class EnemyController2 : EnemyController
{
    public float chasePlayerRange = 2.0f;

    protected override void AI()
    {
        // Get distance to player
        distToPlayerX = gameManager.GetPlayerPositionX() - transform.position.x;
        absDistToPlayerX = Mathf.Abs(distToPlayerX);

        // If attacking player or objective
        if (absDistToPlayerX < attackRange)
        {
            // Attack player
            // If ready
            if (Time.timeSinceLevelLoad > attackReadyTime)
            {
                enemyState = EnemyState.AttackingPlayer;
                AttackPlayer();
            }
        }
        // If not, if within chase range of player
        else if (absDistToPlayerX < chasePlayerRange)
        {
            // Chase player
            enemyState = EnemyState.Run;
            AImoveH = (distToPlayerX < 0) ? -1 : 1;
        }
        // If not, if within attack range of objective
        else if (absDistToTargetX < attackRange)
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
