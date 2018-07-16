using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController1 : EnemyController
{
    protected override void AI()
    {
        if (HandleStun())
        {
            return; // If stunned don't bother executing any AI behaviour.
        }

        distToTargetX = path[currentTarget].position.x - transform.position.x;
        absDistToTargetX = Mathf.Abs(distToTargetX);

        // If at objective
        if (isPathDone)
        {
            HandleAttackObjective();
        }
        else
        {
            // If not at final target and still enroute
            MoveAlongPath();
        }
    }

    bool HandleStun()
    {
        // Stunned for defaultStunDuration when damaged by any attacks.
        // If stunned by weapon effect, weapon stun duration is used instead
        if (isStunned)
        {
            if (Time.timeSinceLevelLoad > stunEndTime)
            {
                isStunned = false;
            }

            return true;
        }

        return false;
    }

    void HandleAttackObjective()
    {
        // If completed path and targetting final target aka city gate or player
        // If within deadzone distance to the final target
        if (absDistToTargetX < range)
        {
            // Stop
            AImoveH = 0;

            // Attack
            if (attackReady)
            {
                gameManager.DamageObjective(enemyWeapon.GetComponent<EnemyWeapon>().damage);
                attackReady = false;
                attackReadyTime = Time.timeSinceLevelLoad + attackCooldown;
                enemyAnimator.PlayAttack();
            }
            else
            {
                if (Time.timeSinceLevelLoad > attackReadyTime)
                {
                    attackReady = true;
                }
            }
        }
        else
        {
            // Move to the final target
            if (distToTargetX > 0)
            {
                AImoveH = 1;
            }
            else
            {
                AImoveH = -1;
            }
        }
    }
}
