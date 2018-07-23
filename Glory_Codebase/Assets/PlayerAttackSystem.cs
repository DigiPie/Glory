using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackSystem : MonoBehaviour {
    private readonly Vector2 leftDir = new Vector2(-1, 0);
    private readonly Vector2 rightDir = new Vector2(1, 0);

    private Animator animator;
    public GameObject normalAttack, specialAttack, specialAbility;

    // Normal attack //
    public float attackDmg = 10;
    private float criticalDmg;
    private float comboEndTime;
    private float comboDuration = 1f;
    private int comboCount = 0;
    public float consecAttkCooldown = 0.4f;
    // Consecutive attack cooldown used as long as attacks are within combo duration intervals.
    // Up to 3 consecutive attacks, final consecutive attack is critical strike;
    public float newAttkCooldown = 0.8f; // The cooldown between the last combo (completed or not) and the new
    private float attkReadyTime;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();

        criticalDmg = attackDmg * 1.5f;
	}

    public void NormalAttack(bool isAttackLeft)
    {
        // If cooldown, then don't attack
        if (Time.timeSinceLevelLoad < attkReadyTime)
        {
            return;
        }

        // If combo is ongoing
        if (Time.timeSinceLevelLoad < comboEndTime)
        {
            comboCount++; // Update combo count
            comboEndTime = Time.timeSinceLevelLoad + comboDuration; // Update combo length

            // For every 3rd hit, play attack 3 (360 backhand strike)
            if (comboCount % 3 == 0)
            {
                // Critical Strike projectile
                SpawnCriticalStrike(isAttackLeft);

                // Animate
                animator.Play("Attack3");

                // Combo count reset
                comboCount = 0;

                // Long cooldown to next attack since combo is just completed.
                attkReadyTime = Time.timeSinceLevelLoad + newAttkCooldown;
            }
            else
            {
                // Attack projectile
                SpawnAttack(isAttackLeft);

                // Animate either attack 1 or 2 randomly
                if (Random.Range(0, 2) == 0)
                {
                    animator.Play("Attack");
                }
                else
                {
                    animator.Play("Attack2");
                }

                // Short cooldown to next attack since combo is ongoing.
                attkReadyTime = Time.timeSinceLevelLoad + consecAttkCooldown;
            }
        }
        else
        {
            // If new combo
            comboCount = 1;
            comboEndTime = Time.timeSinceLevelLoad + comboDuration;

            SpawnAttack(isAttackLeft);

            // Randomly select between attack 1 and attack 2 animation
            if (Random.Range(0, 2) == 0)
            {
                animator.Play("Attack");
            }
            else
            {
                animator.Play("Attack2");
            }

            // Short cooldown to next attack since combo is ongoing.
            attkReadyTime = Time.timeSinceLevelLoad + consecAttkCooldown;
        }
    }

    void SpawnAttack(bool isAttackLeft)
    {
        GameObject projectile = Instantiate(normalAttack, transform);
        projectile.GetComponent<PlayerWeapon>().Setup(
            (isAttackLeft) ? leftDir : rightDir, 
            attackDmg);
    }

    void SpawnCriticalStrike(bool isAttackLeft)
    {
        GameObject projectile = Instantiate(normalAttack, transform);
        projectile.GetComponent<PlayerWeapon>().Setup(
            (isAttackLeft) ? leftDir : rightDir, 
            criticalDmg);
    }

    public bool IsAttacking()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3");
    }
}
