﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour {
    public enum EnemyState { Idle, Run, AttackingObjective, AttackingPlayer, Stunned, Dead }

    // Script references
    protected Rigidbody2D rb2d; // Used for movement
    protected GameManager gameManager; // Used to damage objective and get player position
    protected EnemyAnimator enemyAnimator;
    protected EnemyHealthSystem healthSystem; // Handles health-related matters
    
    // References
    public Transform groundCheck; // Used to check if on the ground
    public GameObject enemyWeapon;
    
    // Forces to be applied on character
    protected Vector2 bounceHurtLeftV, bounceHurtRightV, bounceStunLeftV, bounceStunRightV, 
        bounceDeadLeftV, bounceDeadRightV;
    protected Vector2 moveLeftV, moveRightV;

    // States
    protected EnemyState enemyState = EnemyState.Idle;
    protected bool collisionOnRight = false;
    protected bool onGround = false;

    // Movement
    public float moveForce = 10f; // Since F = ma and m = 1, therefore a = F
    public float maxSpeed = 1f; // Maximum horziontal velocity
    public float throwbackForce = 2f; // When hit by attack
    protected float AImoveH = 0; // Used by the AI to move character

    // Pathing
    protected int currentTarget = 0; // Current target, path[currentTarget]
    protected float distToPlayerX = 0; // Distance from this to player
    protected float absDistToPlayerX = 0; // Absolute value used to compare against attackRange/chaseRange
    public bool targetsPlayer = false; // If true, distToPlayerX and absDistToPlayerX will be updated for sub classes to use
    protected float distToTargetX = 0; // Distance from this to target
    protected float absDistToTargetX = 0; // Absolute value used to compare against attackRange
    public float minAttackRange = 0.5f; // Minimum engagement range
    public float maxAttackRange = 0.7f; // Maximum engagement range
    protected float attackRange; // If within range from target, currentTarget++; if within range from player, attack

    // Attack
    protected float attackCooldown; // Minimum wait-time before next attack can be triggered
    protected float attackReadyTime = 0; // The time at which attack1Ready will be set to true again
    private bool isBoss;

    // Being attacked
    protected string lastCollider;

    // Stunned
    private float stunDuration; // How long is the character stunned when damaged by any attacks
    protected float stunEndTime = 0; // The time at which stunned is set to false again

    // Effects
    public float offsetY; // Offset for effects that are supposed to appear above the enemy's head.
    private GameObject tempEffect;
    private bool isSlowed = false;
    private float maxSpeedWhenSlowed;
    private float slowEndTime;

    // Dead body
    public bool hasDeadBody = false;

    // Use this for initialization
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        

        attackCooldown = enemyWeapon.GetComponent<EnemyWeapon>().cooldown;
        attackRange = Random.Range(minAttackRange, maxAttackRange); // Get a unique engagement range
        isBoss = healthSystem.isBoss; // If boss, shake camera when attacking

        // Calculate the bounce-off vectors here instead of FixedUpdate() so we only
        // calculate them once, as they never change. For optimisation.
        moveLeftV = Vector2.left * moveForce;
        moveRightV = Vector2.right * moveForce;
        bounceHurtLeftV = new Vector2(0.5f, 0.6f) * throwbackForce;
        bounceHurtRightV = new Vector2(-0.5f, 0.6f) * throwbackForce;
        bounceStunLeftV = new Vector2(1.2f, 1.2f) * throwbackForce;
        bounceStunRightV = new Vector2(-1.2f, 1.2f) * throwbackForce;
        bounceDeadLeftV = new Vector2(0.4f, 0.2f) * throwbackForce;
        bounceDeadRightV = new Vector2(-0.4f, 0.2f) * throwbackForce;
    }

    // Used by the gameManager to set up this enemy.
    public void Setup(GameManager gameManager, int sortOrder)
    {
        this.gameManager = gameManager;

        enemyAnimator = GetComponent<EnemyAnimator>();
        healthSystem = GetComponent<EnemyHealthSystem>();

        enemyAnimator.SetSortingOrder(sortOrder);
        healthSystem.SetSortingOrder(sortOrder);
    }

    // Update is called in-step with the physics engine
    void FixedUpdate()
    {
        if (enemyState == EnemyState.Dead)
        {
            // If death animation over
            if (enemyAnimator.IsAnimationOver() && !hasDeadBody)
            {
                // Destroy
                Destroy(gameObject);
            }

            return;
        }

        if (healthSystem.IsDead())
        {
            Destroy(tempEffect);
            AImoveH = 0;
            enemyAnimator.PlayDeath();
            enemyState = EnemyState.Dead;
            return;
        }

        if (enemyState == EnemyState.Stunned)
        {
            HandleStun();
            return;
        }

        if (isSlowed &&Time.timeSinceLevelLoad > slowEndTime)
        {
            CancelSlowEffect();
        }

        distToTargetX = gameManager.GetObjectivePositionX() - transform.position.x;
        absDistToTargetX = Mathf.Abs(distToTargetX);

        AI();

        if (enemyState == EnemyState.Run)
        {
            HandleRunning();
        }
        else if (enemyState == EnemyState.AttackingObjective)
        {
            HandleAttacking(false);
        }
        else if (enemyState == EnemyState.AttackingPlayer)
        {
            HandleAttacking(true);
        }
        else if (enemyState == EnemyState.Idle)
        {
            // Do nothing
        }
    }

    protected void OnTriggerEnter2D(Collider2D collider)
    {
        if (lastCollider == collider.name)
        {
            return;
        }

        lastCollider = collider.name;

        collisionOnRight = collider.transform.position.x > transform.position.x;

        if (enemyState == EnemyState.Dead)
        {
            if (collisionOnRight)
            {
                rb2d.velocity = bounceDeadRightV;
            }
            else
            {
                rb2d.velocity = bounceDeadLeftV;
            }

            return;
        }

        // If colliding with projectile
        if (collider.gameObject.layer == 11)
        {
            PlayerWeapon wep = collider.GetComponent<PlayerWeapon>();
            stunDuration = wep.GetStunDuration();

            // Throwback effect
            if (stunDuration > 0)
            {
                // Unable to move while stunned
                AImoveH = 0;
                enemyState = EnemyState.Stunned;
                stunEndTime = Time.timeSinceLevelLoad + stunDuration;

                if (collisionOnRight)
                {
                    rb2d.velocity = bounceStunRightV;
                }
                else
                {
                    rb2d.velocity = bounceStunLeftV;
                }
            }
            else
            {
                if (collisionOnRight)
                {
                    rb2d.velocity = bounceHurtRightV;
                }
                else
                {
                    rb2d.velocity = bounceHurtLeftV;
                }
            }
            
            if (!healthSystem.isBoss)
                enemyAnimator.PlayHurt();

            // Health deduction
            healthSystem.DeductHealth(wep.GetDamage(), wep.GetBlinkDuration());

            // Damage counter
            wep.SpawnDamageCounter(transform.position);

            // Weapon effect
            PlayerWeaponWithEffect wepEffect = collider.GetComponent<PlayerWeaponWithEffect>();
            
            if (wepEffect != null && wepEffect.effect != null)
            {
                SpawnEffect(wepEffect.effect, wepEffect.isAboveHead, wepEffect.overtimeDamage, 
                    wepEffect.damageInterval, wepEffect.duration);

                if (wepEffect.slowMultiplier != 0)
                {
                    StartSlowEffect(wepEffect.slowMultiplier, wepEffect.duration);
                }
            }
        }
    }

    // Different enemies have unique AI behaviours
    protected abstract void AI();

    void HandleStun()
    {
        // If stun duration over, transition to idle state
        if (Time.timeSinceLevelLoad > stunEndTime)
        {
            enemyState = EnemyState.Idle;
        }
    }

    // Apply horizontal movement forces if horizontal input is registered.
    protected void HandleRunning()
    {
        // Check if on ground
        onGround = Physics2D.Linecast(transform.position, groundCheck.position,
            1 << LayerMask.NameToLayer("Ground"));

        // Apply forces if max speed is not yet reached
        if (Mathf.Abs(rb2d.velocity.x) < ((isSlowed) ? maxSpeedWhenSlowed : maxSpeed))
        {
            if (AImoveH < 0)
            {
                if (onGround)
                {
                    rb2d.AddForce(moveLeftV);
                }
            }
            else
            {
                if (onGround)
                {
                    rb2d.AddForce(moveRightV);
                }
            }
        }
    }

    void HandleAttacking(bool isAttackingPlayer)
    {
        if (enemyAnimator.IsAttackFrame())
        {
            if (isAttackingPlayer)
            {
                SpawnAttackProjectile();
            }
            else
            {
                gameManager.DamageObjective(enemyWeapon.GetComponent<EnemyWeapon>().damage);
            }

            enemyState = EnemyState.Idle;
        }
    }

    protected void AttackPlayer()
    {
        FacePlayer();
        attackReadyTime = Time.timeSinceLevelLoad + attackCooldown;
        enemyAnimator.PlayAttack();
    }

    protected void AttackObjective()
    {
        FaceObjective();
        attackReadyTime = Time.timeSinceLevelLoad + attackCooldown;
        enemyAnimator.PlayAttack();
    }

    protected void FacePlayer()
    {
        enemyAnimator.FaceTarget(distToPlayerX);
    }

    protected void FaceObjective()
    {
        enemyAnimator.FaceTarget(distToTargetX);
    }

    GameObject InstantiateGameObject(GameObject gameObject)
    {
        Vector3 pos = transform.position;
        Quaternion rotation = transform.rotation;
        return Instantiate(gameObject, pos, rotation);
    }

    protected void SpawnAttackProjectile()
    {
        // Create a melee projectile
        GameObject projectile = InstantiateGameObject(enemyWeapon);

        // Assign weapon direction
        if (enemyAnimator.IsFacingLeft())
        {
            projectile.GetComponent<EnemyWeapon>().Setup(new Vector2(-1, 0));
        }
        else
        {
            projectile.GetComponent<EnemyWeapon>().Setup(new Vector2(1, 0));
        }

        if (isBoss)
        {
            gameManager.Shake(0.5f);
        }
    }

    protected void SpawnEffect(GameObject effect, bool isAboveHead, float overtimeDamage, 
        float damageInterval, float duration)
    {
        Destroy(tempEffect);
        tempEffect = Instantiate(effect, transform);

        if (isAboveHead)
        {
            tempEffect.transform.Translate(new Vector2(0, offsetY));
        }

        tempEffect.transform.parent = transform;
        tempEffect.GetComponent<Effect>().Setup(healthSystem, overtimeDamage, damageInterval, duration);
    }

    public void StartSlowEffect(float slowPercentage, float duration)
    {
        slowEndTime = Time.timeSinceLevelLoad + duration;
        maxSpeedWhenSlowed = maxSpeed * slowPercentage;
        isSlowed = true;
    }

    public void CancelSlowEffect()
    {
        isSlowed = false;
    }

    public bool GetAImoveH()
    {
        return AImoveH < 0;
    }

    public EnemyState GetEnemyState()
    {
        return enemyState;
    }

    public bool IsIdle()
    {
        return enemyState == EnemyState.Idle;
    }

    public bool IsStunned()
    {
        return enemyState == EnemyState.Stunned;
    }

    public bool IsRunning()
    {
        return enemyState == EnemyState.Run;
    }

    public bool IsAttacking()
    {
        return enemyState == EnemyState.AttackingObjective || enemyState == EnemyState.AttackingPlayer;
    }

    public bool IsDead()
    {
        return enemyState == EnemyState.Dead;
    }

    public void StartFadeout()
    {
        enemyAnimator.StartDestroy();
    }
}
