using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController2 : EnemyController
{
    public GameObject enemyWeapon;

    private bool isAttackingPlayer = false;
    private bool isAttackingObjective = false;

    protected override void AI()
    {
        // Stunned for defaultStunDuration when damaged by any attacks.
        // If stunned by weapon effect, weapon stun duration is used instead
        if (isStunned)
        {
            if (Time.time > stunEndTime)
            {
                isStunned = false;
                isAttackingObjective = false; // Cancel any ongoing attack
            }

            return;
        }

        // Distance calculations to check proximity to player
        distToTargetX = gameManager.GetPlayerPosition().transform.position.x - this.transform.position.x;
        absDistToTargetX = Mathf.Abs(distToTargetX);

        // If attack for player was started and is at weapon slam frame, spawn a melee projectile that damages player
        if (isAttackingPlayer && this.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")
            && this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f)
        {
            AttackPlayer();
            isAttackingPlayer = false;
        }

        // If within attack range to player
        if (absDistToTargetX < distDeadzone)
        {
            // Stop
            AImoveH = 0;

            // Face Player
            if(this.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                facingLeft = distToTargetX < 0;
                sprite.flipX = facingLeft;
            }

            // Attack
            if (attackReady)
            {
                attackReady = false;
                attackReadyTime = Time.time + attackCooldown;
                animator.Play("Attack");
                isAttackingPlayer = true;
            }
            else
            {
                if (Time.time > attackReadyTime)
                {
                    attackReady = true;
                }
            }

            return;
        }

        // If attack for objective was started and is at weapon slam frame, damage objective
        if (isAttackingObjective && this.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")
            && this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f)
        {
            gameManager.DamageObjective(attackDamage);
            isAttackingObjective = false;
        }

        // Distance calculations for path movement and objective attack
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
                    isAttackingObjective = true;
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

    void AttackPlayer()
    {
        // Create a melee projectile
        GameObject projectile = Instantiate(enemyWeapon, this.transform);

        // Assign weapon direction
        if (facingLeft)
        {
            projectile.GetComponent<EnemyWeapon>().Setup(new Vector2(-1, 0));
        }
        else
        {
            projectile.GetComponent<EnemyWeapon>().Setup(new Vector2(1, 0));
        }
    }
}
