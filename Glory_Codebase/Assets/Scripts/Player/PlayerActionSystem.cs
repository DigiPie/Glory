using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionSystem : MonoBehaviour
{
    // References //
    private Rigidbody2D rb2d;
    public CustomCamController camController;
    public GameObject normalAttack, criticalAttack;
    public GameObject fireSpell, iceSpell, earthSpell, airSpell;
    private GameObject spell1, spell2;
    private PlayerController playerController;
    private PlayerAnimator playerAnimator;
    public AudioManager audioManager;
    public HUD hud;

    // Forces
    private readonly Vector2 leftDir = new Vector2(-1, 0);
    private readonly Vector2 rightDir = new Vector2(1, 0);
    private readonly float slideMultiplier = 0.4f;
    private Vector2 slideLeftV, slideRightV; // 25% of moveLeftV and moveRightV

    // Abilities and Attacks
    private bool isSlideEnabled = false;
    private bool isSpell1Enabled = false;
    private bool isSpell2Enabled = false;

    // Invulnerability //
    private bool isInvul = false; // Currently only slide triggers invulnerability
    private float invulEndTime; // The time at which the character is no longer invulnerable

    // Attack speed //
    private bool isFaster = false;
    private float fasterEndTime;

    /*** Actions ***/
    // Slide //
    public float slideCooldown = 2f; // Minimum wait-time before next slide can be triggered
    public float slideInvulDuration = 1f; // How long is the character invulnerable for when slideing
    public bool slideReady = true; // Reliant on slideCooldown
    private float slideReadyTime; // The time at which slideReady will be set to true again

    /*** Attacks ***/
    // Normal attack //
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

    // Spells //
    public float spell1Cooldown = 8f;
    private float spell1ReadyTime;
    private bool isSpell1 = false;

    public float spell2Cooldown = 8f;
    private float spell2ReadyTime;
    private bool isSpell2 = false;

    private bool isFireSpell = false;
    private bool isEarthSpell = false;

    // Use this for initialization
    void Start()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
        playerController = GetComponent<PlayerController>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void Setup(Vector2 moveLeftV, Vector2 moveRightV)
    {
        slideLeftV = moveLeftV * slideMultiplier;
        slideRightV = moveRightV * slideMultiplier;
    }

    private void FixedUpdate()
    {
        HandleInvul();
        HandleFaster();
        HandleProjectiles();
    }

    void HandleInvul()
    {
        // Invulnerability can be triggered by sliding and air spell
        if (isInvul)
        {
            if (Time.timeSinceLevelLoad > invulEndTime)
            {
                isInvul = false;
            }
        }
    }

    public bool IsInvul()
    {
        return isInvul;
    }

    void HandleFaster()
    {
        // Faster is triggered by air spell
        if (isFaster)
        {
            if (Time.timeSinceLevelLoad > fasterEndTime)
            {
                playerController.ResetMaxSpeed();
                isFaster = false;
            }
        }
    }

    public bool IsFaster()
    {
        return isFaster;
    }


    void HandleProjectiles()
    {
        if (isAttack && (playerAnimator.IsAttackAnim() || playerAnimator.IsAttack2Anim()))
        {
            if (playerAnimator.IsAttackFrame())
            {
                // Attack projectile
                StartNormalAttack(playerAnimator.IsFacingLeft());
                isAttack = false;
            }
        }
        else if (isCriticalAttk && playerAnimator.IsAttack3Anim())
        {
            if (playerAnimator.IsCriticalAttackFrame())
            {
                // Critical attack projectile
                StartCriticalStrike(playerAnimator.IsFacingLeft());
                isCriticalAttk = false;
            }
        }
        else if (isSpell1 && playerAnimator.IsCastAnim())
        {
            if (playerAnimator.IsCastFrame())
            {
                StartSpell1(playerAnimator.IsFacingLeft());
                isSpell1 = false;
                camController.Shake(0.015f, 0.15f);
            }
        }
        else if (isSpell2 && playerAnimator.IsCastAnim())
        {
            if (playerAnimator.IsCastFrame())
            {
                StartSpell2(playerAnimator.IsFacingLeft());
                isSpell2 = false;
                camController.Shake(0.015f, 0.15f);
            }
        }
    }

    /*** Actions ***/
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
            hud.StartSlideCooldownAnim();
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

    public void EnableSlide()
    {
        isSlideEnabled = true;
        hud.ShowDashCooldownIndicator();
    }

    public void EnableSpell1(bool isFireSpell)
    {
        isSpell1Enabled = true;

        if (isFireSpell)
            spell1 = fireSpell;
        else
            spell1 = iceSpell;

        this.isFireSpell = isFireSpell;

        hud.ShowSpell1CooldownIndicator();
    }

    public void EnableSpell2(bool isEarthSpell)
    {
        isSpell2Enabled = true;

        if (isEarthSpell)
            spell2 = earthSpell;
        else
            spell2 = airSpell;

        this.isEarthSpell = isEarthSpell;

        hud.ShowSpell2CooldownIndicator();
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
                audioManager.PlaySound("Sword3SFX");

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
                audioManager.PlaySound("Sword2SFX");

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
            audioManager.PlaySound("Sword1SFX");

            // Short cooldown to next attack since combo is ongoing.
            attkReadyTime = Time.timeSinceLevelLoad + consecAttkCooldown;
        }
    }

    public void Spell1()
    {
        if (!isSpell1Enabled)
        {
            return;
        }

        // If cooldown, then don't attack
        if (Time.timeSinceLevelLoad < spell1ReadyTime)
        {
            return;
        }

        isSpell1 = true;

        // Animate with special attack
        playerAnimator.PlayCast();

        // Update HUD
        hud.StartSpell1CooldownAnim();

        spell1ReadyTime = Time.timeSinceLevelLoad + spell1Cooldown;
    }

    public void Spell2()
    {
        if (!isSpell2Enabled)
        {
            return;
        }

        // If cooldown, then don't attack
        if (Time.timeSinceLevelLoad < spell2ReadyTime)
        {
            return;
        }

        isSpell2 = true;

        // Animate with special attack
        playerAnimator.PlayCast();

        // Update HUD
        hud.StartSpell2CooldownAnim();

        spell2ReadyTime = Time.timeSinceLevelLoad + spell2Cooldown;
    }

    public void ResetAllCooldowns()
    {
        spell1ReadyTime = 0;
        spell2ReadyTime = 0;
    }

    GameObject InstantiateGameObject(GameObject gameObject)
    {
        Vector3 pos = transform.position;
        Quaternion rotation = transform.rotation;
        return Instantiate(gameObject, pos, rotation);
    }

    void StartNormalAttack(bool isAttackLeft)
    {
        GameObject projectile = InstantiateGameObject(normalAttack);

        if (isAttackLeft)
        {
            projectile.GetComponent<PlayerWeapon>().Setup(leftDir);
        }
        else
        {
            projectile.GetComponent<PlayerWeapon>().Setup(rightDir);
        }
    }

    void StartCriticalStrike(bool isAttackLeft)
    {
        GameObject projectile = InstantiateGameObject(criticalAttack);

        if (isAttackLeft)
        {
            projectile.GetComponent<PlayerWeapon>().Setup(leftDir);
        }
        else
        {
            projectile.GetComponent<PlayerWeapon>().Setup(rightDir);
        }
    }

    void StartSpell(GameObject spell, bool isFacingLeft)
    {
        GameObject tempSpell = InstantiateGameObject(spell);

        if (tempSpell.GetComponent<PlayerWeapon>() != null)
        {
            if (isFacingLeft)
            {
                tempSpell.GetComponent<PlayerWeapon>().Setup(leftDir);
            }
            else
            {
                tempSpell.GetComponent<PlayerWeapon>().Setup(rightDir);
            }
        }
        else if (tempSpell.GetComponent<PlayerBuff>() != null)
        {
            PlayerBuff tempBuff = tempSpell.GetComponent<PlayerBuff>();

            tempBuff.Setup();
            
            if (tempBuff.invulDuration > 0)
            {
                isInvul = true;
                invulEndTime = Time.timeSinceLevelLoad + tempBuff.invulDuration;
            }

            if (tempBuff.fasterSpeedDuration > 0)
            {
                isFaster = true;
                fasterEndTime = Time.timeSinceLevelLoad + tempBuff.fasterSpeedDuration;
                playerController.ApplySpeedBuff(tempBuff.speedMultiplier);

                // When the player animator animates slide, it will call the SetToSlideOffset() in PlayerBuffs cript
                playerAnimator.AddPlayerBuff(tempBuff);
            }
        }
    }

    void StartSpell1(bool isFacingLeft)
    {
        StartSpell(spell1, isFacingLeft);

        // Play Sound
        if (isFireSpell)
        {
            audioManager.PlaySound("FireSpellSFX");
        }
        else
        {
            audioManager.PlaySound("IceSpellSFX");
        }
    }

    void StartSpell2(bool isFacingLeft)
    {
        StartSpell(spell2, isFacingLeft);

        // PlaySound
        if (isEarthSpell)
        {
            audioManager.PlaySound("EarthSpellSFX");
        }
        else
        {
            audioManager.PlaySound("AirSpellSFX");
        }
    }
}
