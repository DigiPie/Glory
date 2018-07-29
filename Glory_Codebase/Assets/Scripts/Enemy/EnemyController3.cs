using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Moves to player and attacks player only.
 */
public class EnemyController3 : EnemyController
{
    protected override void AI()
    {
        // Get distance to player
        distToPlayerX = gameManager.GetPlayerPositionX() - transform.position.x;
        absDistToPlayerX = Mathf.Abs(distToPlayerX);

        // If within attack range of player
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
        // If not, move to player wherever player is at
        else
        {
            // Chase player
            enemyState = EnemyState.Run;
            AImoveH = (distToPlayerX < 0) ? -1 : 1;
        }
    }
}