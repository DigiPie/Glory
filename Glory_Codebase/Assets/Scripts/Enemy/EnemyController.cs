using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour {
    protected GameManager gameManager;
    protected Animator animator;
    protected EnemyHealthSystem healthSystem;
    protected BlinkSystem blinkSystem;
    protected Rigidbody2D rb2d;
    protected SpriteRenderer sprite;
    public Transform groundCheck;

    // Forces to be applied on character
    protected Vector2 bounceHurtLeftV, bounceHurtRightV;

    // AI
    protected Transform[] path; // The AI path, it will move to path[0], path[1]...path[n]
    protected float AImoveH = 0; // Used by the AI to move character
    protected int currentTarget = 0; // Current target, path[currentTarget]
    protected float distToTargetX = 0; // Distance from this to target
    protected float absDistToTargetX = 0; // Absolute value used to compare against deadzone value
    protected float distDeadzone; // If within deadzone unit distance from target, currentTarget++
    protected bool isPathDone = false; // True if reached the end of the designated path

    // Forces to be applied on character
    protected Vector2 moveLeftV, moveRightV;

    // States
    protected bool collisionOnRight = false;
    protected bool facingLeft = false;
    protected bool onGround = false;

    // Attack
    public float minAttackRange = 0.5f;
    public float maxAttackRange = 0.7f;

    // Stunned
    public float defaultStunDuration = 0.3f; // How long is the character stunned when damaged by any attacks
    protected bool isStunned = false;
    protected float stunEndTime = 0; // The time at which stunned is set to false again

    // Movement
    public float moveForce = 50f; // Since F = ma and m = 1, therefore a = F
    public float maxSpeed = 5f; // Maximum horziontal velocity
    public float throwbackForce = 2f; // When hit by attack

    // Objective Attack
    public GameObject enemyWeapon;
    protected float attackCooldown; // Minimum wait-time before next attack can be triggered
    protected bool attackReady = true; // Reliant on attack1Cooldown
    protected float attackReadyTime = 0; // The time at which attack1Ready will be set to true again

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        healthSystem = GetComponent<EnemyHealthSystem>();
        blinkSystem = GetComponent<BlinkSystem>();
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        attackCooldown = enemyWeapon.GetComponent<EnemyWeapon>().cooldown;

        distDeadzone = Random.Range(minAttackRange, maxAttackRange);

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
            facingLeft = AImoveH < 0;
            sprite.flipX = facingLeft;
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
            // Unable to move while stunned
            AImoveH = 0;
            isStunned = true;
            stunEndTime = Time.timeSinceLevelLoad + defaultStunDuration;

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
            blinkSystem.StartBlink(defaultStunDuration);

            // Health deduction
            healthSystem.DeductHealth(
                collider.GetComponent<Weapon>().damage);
        }
    }
}
