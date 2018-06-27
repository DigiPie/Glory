using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController1 : MonoBehaviour {
    private GameManager gameManager;
    private Animator animator;
    private EnemyHealthSystem healthSystem;
    private BlinkSystem blinkSystem;
    private Rigidbody2D rb2d;
    private SpriteRenderer sprite;
    public Transform groundCheck;

    // Forces to be applied on character
    private Vector2 bounceHurtLeftV, bounceHurtRightV;

    // AI
    private Transform[] path; // The AI path, it will move to path[0], path[1]...path[n]
    private float AImoveH = 0; // Used by the AI to move character
    private int currentTarget = 0; // Current target, path[currentTarget]
    private float distToTargetX = 0; // Distance from this to target
    private float absDistToTargetX = 0; // Absolute value used to compare against deadzone value
    private float distDeadzone; // If within deadzone unit distance from target, currentTarget++
    private bool isPathDone = false; // True if reached the end of the designated path

    // Forces to be applied on character
    private Vector2 moveLeftV, moveRightV;

    // States
    private bool collisionOnRight = false;
    private bool facingLeft = false;
    private bool onGround = false;

    // Stunned
    public float defaultStunDuration = 0.3f; // How long is the character stunned when damaged by any attacks
    private bool isStunned = false;
    private float stunEndTime = 0; // The time at which stunned is set to false again

    // Movement
    public float moveForce = 50f; // Since F = ma and m = 1, therefore a = F
    public float maxSpeed = 5f; // Maximum horziontal velocity
    public float throwbackForce = 200f; // When hit by attack

    // Objective Attack
    public float attackCooldown = 1.5f; // Minimum wait-time before next attack can be triggered
    public int attackDamag = 7;
    private bool attackReady = true; // Reliant on attack1Cooldown
    private float attackReadyTime = 0; // The time at which attack1Ready will be set to true again

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        healthSystem = GetComponent<EnemyHealthSystem>();
        blinkSystem = GetComponent<BlinkSystem>();
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        distDeadzone = 0.5f + Random.Range(0, 0.2f);

        // Calculate the bounce-off vectors here instead of FixedUpdate() so we only
        // calculate them once, as they never change. For optimisation.
        moveLeftV = Vector2.left * moveForce;
        moveRightV = Vector2.right * moveForce;
        bounceHurtLeftV = new Vector2(0.5f, 0.6f) * throwbackForce;
        bounceHurtRightV = new Vector2(-0.5f, 0.6f) * throwbackForce;
    }

    // Update is called once per frame, independent of the physics engine
    void Update()
    {
    }

    // Update is called in-step with the physics engine
    void FixedUpdate()
    {
        onGround = Physics2D.Linecast(transform.position, groundCheck.position,
            1 << LayerMask.NameToLayer("Ground"));

        AI();
        Move();
    }

    public void Setup(GameManager gameManager, Transform[] path)
    {
        this.gameManager = gameManager;
        this.path = path;
    }

    void AI()
    {
        // Stunned for defaultStunDuration when damaged by any attacks.
        // If stunned by weapon effect, weapon stun duration is used instead
        if (isStunned)
        {
            if (Time.time > stunEndTime)
            {
                isStunned = false;
            }

            return;
        }

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
                    gameManager.DamageObjective(12);
                    attackReady = false;
                    attackReadyTime = Time.time + attackCooldown;
                }
                else
                {
                    if (Time.time > attackReadyTime)
                    {
                        attackReady = true;
                    }
                }

                animator.Play("Attack");
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

    void Move()
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
    void HandleRunning()
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

    void OnTriggerEnter2D(Collider2D collider)
    {
        collisionOnRight = collider.transform.position.x > transform.position.x;

        // If colliding with projectile
        if (collider.gameObject.layer == 11)
        {
            // Unable to move while stunned
            AImoveH = 0;
            isStunned = true;
            stunEndTime = Time.time + defaultStunDuration;

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
