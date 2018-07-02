using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController1 : EnemyController
{
    protected override void AI()
    {
        // Stunned for defaultStunDuration when damaged by any attacks.
        // If stunned by weapon effect, weapon stun duration is used instead
        if (isStunned)
        {
            if (Time.timeSinceLevelLoad > stunEndTime)
            {
                isStunned = false;
            }

            return;
        }

        distToTargetX = path[currentTarget].position.x - transform.position.x;
        absDistToTargetX = Mathf.Abs(distToTargetX);

        if (isPathDone)
        {
            // If completed path and targetting final target aka city gate or player
            // If within deadzone distance to the final target
            if (absDistToTargetX < distDeadzone)
            {
                // Stop
                AImoveH = 0;

                // Attack
                if (attackReady)
                {
                    gameManager.DamageObjective(attackDamage);
                    attackReady = false;
                    attackReadyTime = Time.timeSinceLevelLoad + attackCooldown;
                    animator.Play("Attack");
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
        else
        {
            // If not at final target and still enroute
            // If reached current target
            if (absDistToTargetX < distDeadzone)
            {
                // If at final target
                if (currentTarget + 1 == path.Length)
                {
                    // Indicate that character is at final target
                    isPathDone = true;
                }
                else
                {
                    // Move on to next target
                    currentTarget++;
                }
            }
            else
            {
                // Move to the current target
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
}
