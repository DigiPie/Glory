using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController2 : EnemyController
{
    private bool isAttackingPlayer = false;
    private bool isAttackingObjective = false;

    protected override void AI()
    {
        if (HandleStun())
        {
            return; // If stunned don't bother executing any AI behaviour.
        }

        // Distance calculations to check proximity to player
        distToTargetX = gameManager.GetPlayerPosition().transform.position.x - this.transform.position.x;
        absDistToTargetX = Mathf.Abs(distToTargetX);

        if (HandleAttackPlayer())
        {
            return; // If attacking player, don't attack objective.
        }

        if (HandleAttackObjective())
        {
            return; // If attacking objective, don't bother pathing.
        }

        // Distance calculations for path movement and objective attack
        distToTargetX = path[currentTarget].position.x - transform.position.x;
        absDistToTargetX = Mathf.Abs(distToTargetX);

        if (isPathDone)
        {
            // If completed path and targetting final target aka city gate or player
            // If within deadzone distance to the final target
            if (absDistToTargetX < range)
            {
                HandleStartAttack();
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
            MoveAlongPath();
        }
    }

    bool HandleAttackPlayer()
    {
        // If attack for player was started and is at weapon slam frame, spawn a melee projectile that damages player
        if (isAttackingPlayer && enemyAnimator.IsAttackFrame())
        {
            SpawnAttackProjectile();
            isAttackingPlayer = false;
        }

        // If within attack range to player
        if (absDistToTargetX < range)
        {
            // Stop
            AImoveH = 0;

            // Face Player
            if (enemyAnimator.IsIdleAnim())
            {
                enemyAnimator.FaceTarget(distToTargetX);
            }

            // Attack
            if (attackReady)
            {
                attackReady = false;
                attackReadyTime = Time.timeSinceLevelLoad + attackCooldown;
                enemyAnimator.PlayAttack();
                isAttackingPlayer = true;
            }
            else
            {
                if (Time.timeSinceLevelLoad > attackReadyTime)
                {
                    attackReady = true;
                }
            }

            return true;
        }

        return false;
    }

    void SpawnAttackProjectile()
    {
        // Create a melee projectile
        GameObject projectile = Instantiate(enemyWeapon, this.transform);

        // Assign weapon direction
        if (enemyAnimator.IsFacingLeft())
        {
            projectile.GetComponent<EnemyWeapon>().Setup(new Vector2(-1, 0));
        }
        else
        {
            projectile.GetComponent<EnemyWeapon>().Setup(new Vector2(1, 0));
        }
    }

    bool HandleAttackObjective()
    {
        // If attack for objective was started and is at weapon slam frame, damage objective
        if (isAttackingObjective && enemyAnimator.IsAttackFrame())
        {
            gameManager.DamageObjective(enemyWeapon.GetComponent<EnemyWeapon>().damage);
            isAttackingObjective = false;
            return true;
        }

        return false;
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
                isAttackingObjective = false; // Cancel any ongoing attack
            }

            return true;
        }

        return false;
    }

    void HandleStartAttack()
    {
        // Stop
        AImoveH = 0;

        // Attack
        if (attackReady)
        {
            attackReady = false;
            attackReadyTime = Time.timeSinceLevelLoad + attackCooldown;
            enemyAnimator.PlayAttack();
            isAttackingObjective = true;
        }
        else
        {
            if (Time.timeSinceLevelLoad > attackReadyTime)
            {
                attackReady = true;
            }
        }
    }
}
