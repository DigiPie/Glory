using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour {
    protected Animator animator; // Used to animate the sprite
    protected GameManager gameManager; // Used to damage objective and get player position
    protected Rigidbody2D rb2d; // Used for movement
    protected SpriteRenderer sprite;

    protected EnemyHealthSystem healthSystem; // Handles health-related matters
    protected BlinkSystem blinkSystem; // Handles blinking effect
    
    public Transform groundCheck; // Used to check if on the ground

    // Forces to be applied on character
    protected Vector2 bounceHurtLeftV, bounceHurtRightV;
    protected Vector2 moveLeftV, moveRightV;

    // States
    public bool spriteFacingLeft = false;
    protected bool collisionOnRight = false;
    protected bool facingLeft = false;
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

    public float minEngagementRange = 0.5f; // Minimum engagement range
    public float maxEngagementRange = 0.7f; // Maximum engagement range
    protected float range; // If within range from target, currentTarget++; if within range from player, attack

    // Attack
    public GameObject enemyWeapon;
    
    protected float attackCooldown; // Minimum wait-time before next attack can be triggered
    protected bool attackReady = true; // Reliant on attack1Cooldown
    protected float attackReadyTime = 0; // The time at which attack1Ready will be set to true again

    // Stunned
    private float stunDuration; // How long is the character stunned when damaged by any attacks
    protected bool isStunned = false;
    protected float stunEndTime = 0; // The time at which stunned is set to false again

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        healthSystem = GetComponent<EnemyHealthSystem>();
        blinkSystem = GetComponent<BlinkSystem>();

        attackCooldown = enemyWeapon.GetComponent<EnemyWeapon>().cooldown;
        range = Random.Range(minEngagementRange, maxEngagementRange); // Get a unique engagement range

        // Calculate the bounce-off vectors here instead of FixedUpdate() so we only
        // calculate them once, as they never change. For optimisation.
        moveLeftV = Vector2.left * moveForce;
        moveRightV = Vector2.right * moveForce;
        bounceHurtLeftV = new Vector2(0.5f, 0.6f) * throwbackForce;
        bounceHurtRightV = new Vector2(-0.5f, 0.6f) * throwbackForce;
    }

    // Update is called in-step with the physics engine
    void FixedUpdate()
    {
        GroundCheck();
        AI();
        Move();
    }


    // Used by the gameManager to set up this enemy.
    public void Setup(GameManager gameManager, Transform[] path)
    {
        this.gameManager = gameManager;
        this.path = path;
    }

    protected void GroundCheck()
    {
        onGround = Physics2D.Linecast(transform.position, groundCheck.position,
            1 << LayerMask.NameToLayer("Ground"));
    }

    // Different enemies have unique AI behaviours
    protected abstract void AI();

    protected void Move()
    {
        if (AImoveH == 0)
        {
            animator.SetBool("Running", false);
        }
        else
        {
            HandleRunning();
            animator.SetBool("Running", true);

            if (spriteFacingLeft)
            {
                facingLeft = AImoveH > 0;
                sprite.flipX = facingLeft;
            }
            else
            {
                facingLeft = AImoveH < 0;
                sprite.flipX = facingLeft;
            }
        }
    }

    // Apply horizontal movement forces if horizontal input is registered.
    protected void HandleRunning()
    {
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

    protected void OnTriggerEnter2D(Collider2D collider)
    {
        collisionOnRight = collider.transform.position.x > transform.position.x;

        // If colliding with projectile
        if (collider.gameObject.layer == 11)
        {
            stunDuration = collider.GetComponent<Weapon>().stunDuration;

            // Unable to move while stunned
            AImoveH = 0;
            isStunned = true;
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

            // Hurt animation
            animator.Play("Hurt");

            // Blink effect
            blinkSystem.StartBlink(collider.GetComponent<Weapon>().blinkDuration);

            // Health deduction
            healthSystem.DeductHealth(
                collider.GetComponent<Weapon>().damage);
        }
    }

    // Can be used by child classes in the AI class
    protected void MoveAlongPath()
    {
        // If reached current target
        if (absDistToTargetX < range)
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
