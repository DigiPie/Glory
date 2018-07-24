using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionSystem : MonoBehaviour
{
    // References //
    private Rigidbody2D rb2d;
    public CustomCamController camController;
    public GameObject normalAttack, criticalAttack, specialAttack, specialAbility;
    private PlayerAnimator playerAnimator;

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
    private float comboDuration = 0.6f;
    private int comboCount = 0;
    public float consecAttkCooldown = 0.4f;
    // Consecutive attack cooldown used as long as attacks are within combo duration intervals.
    // Up to 3 consecutive attacks, final consecutive attack is critical strike;
    public float newAttkCooldown = 0.8f; // The cooldown between the last combo (completed or not) and the new
    private float attkReadyTime;
    private bool isAttack = false;
    private bool isCriticalAttk = false;

    // Special attack //
    private float specialDmg;
    public float specialAttkCooldown = 8f;
    private float specialAttkReadyTime;
    private bool isSpecialAttk = false;

    // Use this for initialization
    void Start()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
        rb2d = GetComponent<Rigidbody2D>();

        criticalDmg = attackDmg * 2f;
        specialDmg = attackDmg * 3f;
    }

    public void Setup(Vector2 moveLeftV, Vector2 moveRightV)
    {
        slideLeftV = moveLeftV * slideMultiplier;
        slideRightV = moveRightV * slideMultiplier;
    }

    private void FixedUpdate()
    {
        HandleInvul();
        HandleProjectiles();
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

    void HandleProjectiles()
    {
        if (isAttack && (playerAnimator.IsAttackAnim() || playerAnimator.IsAttack2Anim()))
        {
            if (playerAnimator.IsAttackFrame())
            {
                // Attack projectile
                SpawnAttack(playerAnimator.IsFacingLeft());
                isAttack = false;
            }
        }
        else if (isCriticalAttk && playerAnimator.IsAttack3Anim())
        {
            if (playerAnimator.IsCriticalAttackFrame())
            {
                // Critical attack projectile
                SpawnCriticalStrike(playerAnimator.IsFacingLeft());
                isCriticalAttk = false;
            }
        }
        else if (isSpecialAttk && playerAnimator.IsCastAnim())
        {
            if (playerAnimator.IsCastFrame())
            {
                SpawnSpecialAttack(playerAnimator.IsFacingLeft());
                isSpecialAttk = false;
                camController.Shake(0.015f, 0.15f);
            }
        }
    }

    /*** Abilities ***/
    public void Slide()
    {
        if (isSlideEnabled && slideReady)
        {
            if (playerAnimator.IsFacingLeft())
            {
                rb2d.velocity = slideLeftV;
            }
            else
            {
                rb2d.velocity = slideRightV;
            }

            playerAnimator.PlaySlide();

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

    public void EnableSlide()
    {
        isSlideEnabled = true;
    }

    public void DisableSlide()
    {
        isSlideEnabled = true;
    }

    /*** Attacks ***/
    public void NormalAttack()
    {
        // If cooldown, then don't attack
        if (Time.timeSinceLevelLoad < attkReadyTime)
        {
            return;
        }

        // If combo is ongoing
        if (Time.timeSinceLevelLoad < comboEndTime)
        {
            if (playerAnimator.IsAttacking())
                return;

            comboCount++; // Update combo count
            comboEndTime = Time.timeSinceLevelLoad + comboDuration; // Update combo length

            // For every 3rd hit, play attack 3 (360 backhand strike)
            if (comboCount % 3 == 0)
            {
                isCriticalAttk = true;

                // Animate
                playerAnimator.PlayAttackCriticalStrike();

                // Combo count reset
                comboCount = 0;

                // Long cooldown to next attack since combo is just completed.
                attkReadyTime = Time.timeSinceLevelLoad + newAttkCooldown;
            }
            else
            {
                isAttack = true;

                // Animate either attack 1 or 2 randomly
                playerAnimator.PlayAttack();

                // Short cooldown to next attack since combo is ongoing.
                attkReadyTime = Time.timeSinceLevelLoad + consecAttkCooldown;
            }
        }
        else
        {
            if (playerAnimator.IsAttacking())
                return;

            // If new combo
            comboCount = 1;
            comboEndTime = Time.timeSinceLevelLoad + comboDuration;

            isAttack = true;

            // Randomly select between attack 1 and attack 2 animation
            playerAnimator.PlayAttack();

            // Short cooldown to next attack since combo is ongoing.
            attkReadyTime = Time.timeSinceLevelLoad + consecAttkCooldown;
        }
    }

    public void SpecialAttack()
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

        isSpecialAttk = true;

        // Animate with special attack
        playerAnimator.PlayCast();

        specialAttkReadyTime = Time.timeSinceLevelLoad + specialAttkCooldown;
    }

    void SpawnAttack(bool isAttackLeft)
    {
        if (isAttackLeft)
        {
            GameObject projectile = Instantiate(normalAttack, transform);
            projectile.GetComponent<PlayerWeapon>().Setup(leftDir, attackDmg);
        }
        else
        {
            GameObject projectile = Instantiate(normalAttack, transform);
            projectile.GetComponent<PlayerWeapon>().Setup(rightDir, attackDmg);
        }
    }

    void SpawnCriticalStrike(bool isAttackLeft)
    {
        if (isAttackLeft)
        {
            GameObject projectile = Instantiate(criticalAttack, transform);
            projectile.GetComponent<PlayerWeapon>().Setup(leftDir, criticalDmg);
        }
        else
        {
            GameObject projectile = Instantiate(criticalAttack, transform);
            projectile.GetComponent<PlayerWeapon>().Setup(rightDir, criticalDmg);
        }
    }

    void SpawnSpecialAttack(bool isAttackLeft)
    {
        if (isAttackLeft)
        {
            GameObject projectile = Instantiate(specialAttack, transform);
            projectile.GetComponent<PlayerWeapon>().Setup(leftDir, specialDmg);
        }
        else
        {
            GameObject projectile = Instantiate(specialAttack, transform);
            projectile.GetComponent<PlayerWeapon>().Setup(rightDir, specialDmg);
        }
    }
}
