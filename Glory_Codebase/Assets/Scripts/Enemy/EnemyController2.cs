using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController2 : EnemyController
{
    private bool isAttackAnimStarted = false;

    override protected void AI()
    {
        // Stunned for defaultStunDuration when damaged by any attacks.
        // If stunned by weapon effect, weapon stun duration is used instead
        if (isStunned)
        {
            if (Time.time > stunEndTime)
            {
                isStunned = false;
                isAttackAnimStarted = false; // Cancel any ongoing attack
            }

            return;
        }

        // If attack animation was started and is at weapon slam frame, damage objective
        if (isAttackAnimStarted && this.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")
            && this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.45f)
        {
            gameManager.DamageObjective(attackDamage);
            isAttackAnimStarted = false;
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
                    attackReady = false;
                    attackReadyTime = Time.time + attackCooldown;
                    animator.Play("Attack");
                    isAttackAnimStarted = true;
                }
                else
                {
                    if (Time.time > attackReadyTime)
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
