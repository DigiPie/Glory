using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour {
    public enum EnemyState { Idle, Run, AttackingObjective, AttackingPlayer, Stunned, Dead }

    // Script references
    protected Rigidbody2D rb2d; // Used for movement
    protected GameManager gameManager; // Used to damage objective and get player position
    protected EnemyAnimator enemyAnimator;
    protected EnemyHealthSystem healthSystem; // Handles health-related matters
    protected BlinkSystem blinkSystem; // Handles blinking effect
    
    // References
    public Transform groundCheck; // Used to check if on the ground
    public GameObject enemyWeapon;

    // Forces to be applied on character
    protected Vector2 bounceHurtLeftV, bounceHurtRightV;
    protected Vector2 moveLeftV, moveRightV;

    // States
    protected EnemyState enemyState = EnemyState.Idle;
    protected bool collisionOnRight = false;
    protected bool onGround = false;

    // Movement
    public float moveForce = 50f; // Since F = ma and m = 1, therefore a = F
    public float maxSpeed = 5f; // Maximum horziontal velocity
    public float throwbackForce = 2f; // When hit by attack
    protected float AImoveH = 0; // Used by the AI to move character

    // Pathing
    protected Transform[] path; // The AI path, it will move to path[0], path[1]...path[n]
    protected bool isPathDone = false; // True if reached the end of the designated path
    protected int currentTarget = 0; // Current target, path[currentTarget]
    protected float distToTargetX = 0; // Distance from this to target
    protected float absDistToTargetX = 0; // Absolute value used to compare against deadzone value
    public float minAttackRange = 0.5f; // Minimum engagement range
    public float maxAttackRange = 0.7f; // Maximum engagement range
    protected float attackRange; // If within range from target, currentTarget++; if within range from player, attack
    public float chasePlayerRange = 2.0f;

    // Attack
    protected float attackCooldown; // Minimum wait-time before next attack can be triggered
    protected float attackReadyTime = 0; // The time at which attack1Ready will be set to true again

    // Stunned
    private float stunDuration; // How long is the character stunned when damaged by any attacks
    protected float stunEndTime = 0; // The time at which stunned is set to false again

    // Use this for initialization
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<EnemyAnimator>();
        healthSystem = GetComponent<EnemyHealthSystem>();
        blinkSystem = GetComponent<BlinkSystem>();

        attackCooldown = enemyWeapon.GetComponent<EnemyWeapon>().cooldown;
        attackRange = Random.Range(minAttackRange, maxAttackRange); // Get a unique engagement range

        // Calculate the bounce-off vectors here instead of FixedUpdate() so we only
        // calculate them once, as they never change. For optimisation.
        moveLeftV = Vector2.left * moveForce;
        moveRightV = Vector2.right * moveForce;
        bounceHurtLeftV = new Vector2(0.5f, 0.6f) * throwbackForce;
        bounceHurtRightV = new Vector2(-0.5f, 0.6f) * throwbackForce;
    }

    // Used by the gameManager to set up this enemy.
    public void Setup(GameManager gameManager, Transform[] path)
    {
        this.gameManager = gameManager;
        this.path = path;
    }

    // Update is called in-step with the physics engine
    void FixedUpdate()
    {
        if (enemyState == EnemyState.Dead)
        {
            // Do nothing
            return;
        }

        if (enemyState == EnemyState.Stunned)
        {
            HandleStun();
            return;
        }

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
        collisionOnRight = collider.transform.position.x > transform.position.x;

        // If colliding with projectile
        if (collider.gameObject.layer == 11)
        {
            stunDuration = collider.GetComponent<Weapon>().stunDuration;

            // Unable to move while stunned
            AImoveH = 0;
            enemyState = EnemyState.Stunned;
            stunEndTime = Time.timeSinceLevelLoad + stunDuration;

            // Throwback effect
            if (collisionOnRight)
            {
                rb2d.velocity = bounceHurtRightV;
            }
            else
            {
                rb2d.velocity = bounceHurtLeftV;
            }

            enemyAnimator.PlayHurt();

            // Blink effect
            blinkSystem.StartBlink(collider.GetComponent<Weapon>().blinkDuration);

            // Health deduction
            healthSystem.DeductHealth(
                collider.GetComponent<Weapon>().damage);
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
        if (Mathf.Abs(rb2d.velocity.x) < maxSpeed)
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

    protected void Attack(bool isAttackingPlayer)
    {
        if (enemyState == EnemyState.AttackingObjective || enemyState == EnemyState.AttackingPlayer ||
            Time.timeSinceLevelLoad < attackReadyTime)
        {
            return;
        }

        FacePlayer();
        attackReadyTime = Time.timeSinceLevelLoad + attackCooldown;
        enemyAnimator.PlayAttack();
        enemyState = (isAttackingPlayer) ? EnemyState.AttackingPlayer : EnemyState.AttackingObjective;
    }

    protected bool IsPlayerWithinAttackRange()
    {
        distToTargetX = gameManager.GetPlayerPosition().transform.position.x - this.transform.position.x;
        absDistToTargetX = Mathf.Abs(distToTargetX);
        return absDistToTargetX < attackRange;
    }

    protected bool IsPlayerWithinChaseRange()
    {
        distToTargetX = gameManager.GetPlayerPosition().transform.position.x - this.transform.position.x;
        absDistToTargetX = Mathf.Abs(distToTargetX);
        return absDistToTargetX < chasePlayerRange;
    }

    protected bool IsTargetWithinAttackRange()
    {
        distToTargetX = path[currentTarget].position.x - this.transform.position.x;
        absDistToTargetX = Mathf.Abs(distToTargetX);
        return absDistToTargetX < attackRange;
    }

    // Should be called right after IsPlayerWithinRange
    protected void FacePlayer()
    {
        enemyAnimator.FaceTarget(distToTargetX);
    }

    protected void SpawnAttackProjectile()
    {
        // Create a melee projectile
        GameObject projectile = Instantiate(enemyWeapon, this.transform);

        // Assign weapon direction
        if (enemyAnimator.IsFacingLeft())
        {
            Debug.Log("Facing left");
            projectile.GetComponent<EnemyWeapon>().Setup(new Vector2(-1, 0));
        }
        else
        {
            projectile.GetComponent<EnemyWeapon>().Setup(new Vector2(1, 0));
        }
    }

    // Can be used by child classes in the AI class
    protected void MoveAlongPath()
    {
        // If reached current target
        if (IsTargetWithinAttackRange())
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
            AImoveH = (distToTargetX > 0) ? 1 : -1;
            enemyState = EnemyState.Run;
        }
    }

    // Home on the last target for which range calculation was done for
    // IsPlayerWithinAttackRange, IsPlayerWithinChaseRange, IsTargetWithinAttackRange
    protected void HomeOnLastTarget()
    {
        AImoveH = (distToTargetX > 0) ? 1 : -1;
        enemyState = EnemyState.Run;
    }

    protected void HomeOnLastTargetWithChaseRange()
    {
        if (absDistToTargetX < chasePlayerRange)
        {
            return;
        }

        AImoveH = (distToTargetX > 0) ? 1 : -1;
        enemyState = EnemyState.Run;
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
}
