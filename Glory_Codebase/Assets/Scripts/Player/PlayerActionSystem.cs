using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionSystem : MonoBehaviour
{
    // References //
    private Animator animator;
    private Rigidbody2D rb2d;
    public CustomCamController camController;
    public GameObject normalAttack, criticalAttack, specialAttack, specialAbility;

    // Forces
    private readonly Vector2 leftDir = new Vector2(-1, 0);
    private readonly Vector2 rightDir = new Vector2(1, 0);
    private readonly float slideMultiplier = 0.4f;
    private Vector2 slideLeftV, slideRightV; // 25% of moveLeftV and moveRightV

    // Abilities and Attacks
    private bool isSlideEnabled = true;
    private bool isSpecialAttackEnabled = true;

    /*** Abilities ***/
    // Slide //
    public float slideCooldown = 2f; // Minimum wait-time before next slide can be triggered
    public float slideInvulDuration = 1f; // How long is the character invulnerable for when slideing
    private bool slideReady = true; // Reliant on slideCooldown
    private float slideReadyTime; // The time at which slideReady will be set to true again

    // Invulnerability //
    private bool isInvul = false; // Currently only slide triggers invulnerability
    private float invulEndTime; // The time at which the character is no longer invulnerable

    /*** Attacks ***/
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

    // Special attack //
    private float specialDmg;
    public float specialAttkCooldown = 8f;
    private float specialAttkReadyTime;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();

        criticalDmg = attackDmg * 1.5f;
        specialDmg = attackDmg * 2f;
    }

    public void Setup(Vector2 moveLeftV, Vector2 moveRightV)
    {
        slideLeftV = moveLeftV * slideMultiplier;
        slideRightV = moveRightV * slideMultiplier;
    }

    private void Update()
    {
        HandleInvul();
    }

    void HandleInvul()
    {
        // Invulnerability can be triggered by sliding and ...
        if (isInvul)
        {
            if (Time.timeSinceLevelLoad > invulEndTime)
            {
                isInvul = false;
            }
        }
    }

    /*** Abilities ***/
    public void Slide(bool isFacingLeft)
    {
        if (isSlideEnabled && slideReady)
        {
            if (isFacingLeft)
            {
                rb2d.velocity = slideLeftV;
            }
            else
            {
                rb2d.velocity = slideRightV;
            }

            animator.SetBool("Jumping", false);
            animator.Play("Slide");

            slideReady = false;
            slideReadyTime = Time.timeSinceLevelLoad + slideCooldown;
            isInvul = true;
            invulEndTime = Time.timeSinceLevelLoad + slideInvulDuration;
        }
        else
        {
            if (Time.timeSinceLevelLoad > slideReadyTime)
            {
                slideReady = true;
            }
        }
    }

    public bool IsInvul()
    {
        return isInvul;
    }

    public bool IsSliding()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Slide");
    }

    public void EnableSlide()
    {
        isSlideEnabled = true;
    }

    public void DisableSlide()
    {
        isSlideEnabled = true;
    }

    /*** Attacks ***/
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

    public void SpecialAttack(bool isAttackLeft)
    {
        if (!isSpecialAttackEnabled)
        {
            return;
        }

        // If cooldown, then don't attack
        if (Time.timeSinceLevelLoad < specialAttkReadyTime)
        {
            return;
        }

        // If new combo
        SpawnSpecialAttack(isAttackLeft);

        // Animate with special attack
        animator.Play("SpecialAttack");
        camController.Shake(0.05f, 0.3f);

        specialAttkReadyTime = Time.timeSinceLevelLoad + specialAttkCooldown;
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
        GameObject projectile = Instantiate(criticalAttack, transform);
        projectile.GetComponent<PlayerWeapon>().Setup(
            (isAttackLeft) ? leftDir : rightDir,
            criticalDmg);
    }

    void SpawnSpecialAttack(bool isAttackLeft)
    {
        GameObject projectile = Instantiate(specialAttack, transform);
        projectile.GetComponent<PlayerWeapon>().Setup(
            (isAttackLeft) ? leftDir : rightDir,
            specialDmg);
    }

    public bool IsAttacking()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("SpecialAttack");
    }
}
