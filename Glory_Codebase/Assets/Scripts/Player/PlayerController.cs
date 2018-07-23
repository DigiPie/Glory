using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public GameManager gameManager;
    private Animator animator;
    private Rigidbody2D rb2d;
    private SpriteRenderer sprite;

    public Transform groundCheck;
    public GameObject weapon1;

    // Forces to be applied on character
    private Vector2 bounceWallLeftV, bounceWallRightV;
    private Vector2 bounceEnemyLeftV, bounceEnemyRightV;
    private Vector2 slowdownLeft, slowdownRightV;
    private Vector2 moveLeftV, moveRightV, moveLeftInAirV, moveRightInAirV;
    private Vector2 dashLeftV, dashRightV; // 25% of moveLeftV and moveRightV
    private Vector2 jumpV;

    // States
    private bool againstWall = false;
    private bool againstEnemyAttack = false;
    private bool collisionOnRight = false;
    private bool facingLeft = false;
    private bool onGround = false;

    // Input
    private bool inputJump, inputSlide, inputAttack, inputSpecialAttk, inputSpecialAbility;
    private float inputH;

    // Attack System
    private readonly float comboDuration = 1.0f; // If no next attack within comboDuration of last attack, combo ends
    private bool isCriticalStrike = false; // Every 3rd strike in a combo is a critical strike
    private int combo = 0; // Number of attacks within comboDuration intervals
    private float comboEndTime;

    // Attack 1
    private float attack1Cooldown; // Minimum wait-time before next attack can be triggered
    private bool attack1Ready = true; // Reliant on attack1Cooldown
    private float attack1ReadyTime = 0; // The time at which attack1Ready will be set to true again

    // Dash
    public float dashCooldown; // Minimum wait-time before next dash can be triggered
    public float dashInvulDuration; // How long is the character invulnerable for when dashing
    private bool dashReady = true; // Reliant on dashCooldown
    private float dashReadyTime = 0; // The time at which dashReady will be set to true again

    // Invulnerability
    private bool isInvul = false;
    private float invulEndTime; // The time at which the character is no longer invulnerable

    // Movement
    public float moveForce = 50f; // Since F = ma and m = 1, therefore a = F
    public float maxSpeed = 5f; // Maximum horziontal velocity
    public float throwbackForce = 200f; // When hit by enemy
    private float inAirMoveForce; // Half of moveForce, slower horizontal speed in the air
    private float slowdownForce; // Quarter of moveForce, applied on character when no input
    private readonly float deadzoneFactor = 1.0f; // Do not apply certain forces if velocity < or > this

    // Jump
    public float jumpForce = 500f;
    private bool isJumpRestStarted = true;
    private float jumpRestDuration = 0.05f; // Only allowed to jump again after being on ground for rest duration
    private float jumpReadyTime = 0;

    // Use this for initialization
    void Start ()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        inAirMoveForce = moveForce * 0.5f;
        slowdownForce = moveForce * 0.35f;

        // Calculate the bounce-off vectors here instead of FixedUpdate() so we only
        // calculate them once, as they never change. For optimisation.
        bounceWallLeftV = Vector2.right * moveForce;
        bounceWallRightV = Vector2.left * moveForce;
        bounceEnemyLeftV = new Vector2(1, 0.5f) * throwbackForce;
        bounceEnemyRightV = new Vector2(-1, 0.5f) * throwbackForce;
        slowdownLeft = Vector2.left * slowdownForce;
        slowdownRightV = Vector2.right * slowdownForce;
        moveLeftV = Vector2.left * moveForce;
        moveRightV = Vector2.right * moveForce;
        dashLeftV = moveLeftV * 0.3f;
        dashRightV = moveRightV * 0.3f;
        moveLeftInAirV = Vector2.left * inAirMoveForce;
        moveRightInAirV = Vector2.right * inAirMoveForce;
        jumpV = new Vector2(0f, jumpForce);

        // Attack
        attack1Cooldown = weapon1.GetComponent<PlayerWeapon>().miniCooldown;
    }

    // Update is called in-step with the physics engine
    void FixedUpdate ()
    {
        // Update physics information
        onGround = Physics2D.Linecast(transform.position, groundCheck.position,
            1 << LayerMask.NameToLayer("Ground"));

        // Update input information
        inputH = Input.GetAxisRaw("Horizontal");
        inputJump = Input.GetButton("Jump");
        inputSlide = Input.GetButton("Slide");
        inputAttack = Input.GetButton("Attack");
        inputSpecialAttk = Input.GetButton("SpecialAttack");
        inputSpecialAbility = Input.GetButton("SpecialAbility");

        Move();
        Attack();
        UpdateInvulnerability();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        collisionOnRight = collision.contacts[0].point.x > transform.position.x;

        // If colliding with wall
        if (collision.gameObject.layer == 9)
        {
            againstWall = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // If colliding with wall
        if (collision.gameObject.layer == 9)
        {
            againstWall = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        collisionOnRight = collider.transform.position.x > transform.position.x;

        if (collider.gameObject.layer == 13)
        {
            if (isInvul)
            {
                return;
            }

            Destroy(collider.gameObject, 0.1f);
            gameManager.DamagePlayer(collider.GetComponent<EnemyWeapon>().damage);
            againstEnemyAttack = true;
        }
    }

    // Move character
    void Move()
    {
        // Apply bounce-off forces when colliding with wall and enemies.
        if (HandleBounceOff())
        {
            return; // If bouncing off, do not allow user movement while doing so
        }

        if (inputH == 0)
        {
            // If there is no horizontal input
            HandleSlowdown(); // Slow down velocity on the x-axis
            animator.SetBool("Running", false);
        }
        else
        {
            // If there is horizontal input
            HandleRunning();
            animator.SetBool("Running", true);
            facingLeft = inputH < 0;
            sprite.flipX = facingLeft;
        }

        animator.SetBool("Jumping", !onGround);

        if (inputSlide)
        {
            HandleDash();
            return; // If both dash and jump input are pressed at the same time, dash only
        }

        HandleJumping();
    }

    // Apply bounce-off forces when colliding with wall and enemies.
    bool HandleBounceOff()
    {
        // If against wall
        if (againstWall)
        {
            if (collisionOnRight)
            {
                rb2d.AddForce(bounceWallRightV);
            }
            else
            {
                rb2d.AddForce(bounceWallLeftV);
            }
        }

        if (againstEnemyAttack)
        {
            if (collisionOnRight)
            {
                rb2d.AddForce(bounceEnemyRightV);
            }
            else
            {
                rb2d.AddForce(bounceEnemyLeftV);
            }

            againstEnemyAttack = false;
        }

        // Return true if bouncing off either against wall or enemy
        return againstWall || againstEnemyAttack;
    }

    void HandleDash()
    {
        if (dashReady)
        {
            if (inputSlide)
            {
                if (facingLeft)
                {
                    rb2d.velocity = dashLeftV;
                }
                else
                {
                    rb2d.velocity = dashRightV;
                }

                animator.SetBool("Jumping", false);
                animator.Play("Slide");

                dashReady = false;
                dashReadyTime = Time.timeSinceLevelLoad + dashCooldown;
                isInvul = true;
                invulEndTime = Time.timeSinceLevelLoad + dashInvulDuration;
            }
        }
        else
        {
            if (Time.timeSinceLevelLoad > dashReadyTime)
            {
                dashReady = true;
            }
        }
    }

    void UpdateInvulnerability()
    {
        // Invulnerability can be triggered by dashing and ...
        if (isInvul)
        {
            if (Time.timeSinceLevelLoad > invulEndTime)
            {
                isInvul = false;
            }
        }
    }

    // Apply slowdown forces when no horizontal input is registered.
    void HandleSlowdown()
    {
        // Apply slow down force if velocity is significant.
        if (Mathf.Abs(rb2d.velocity.x) > deadzoneFactor)
        {
            if (rb2d.velocity.x > 0)
            {
                rb2d.AddForce(slowdownLeft);
            }
            else
            {
                rb2d.AddForce(slowdownRightV);
            }
        }
    }

    // Apply horizontal movement forces if horizontal input is registered.
    void HandleRunning()
    {
        // Slows down character if maxSpeed reached
        if (Mathf.Abs(rb2d.velocity.x) > maxSpeed)
        {
            if (rb2d.velocity.x > 0)
            {
                rb2d.AddForce(slowdownLeft);
            }
            else
            {
                rb2d.AddForce(slowdownRightV);
            }
        }
        // Apply forces if max speed is not yet reached
        if (Mathf.Abs(rb2d.velocity.x) < maxSpeed)
        {
            if (inputH < 0)
            {
                if (onGround)
                {
                    rb2d.AddForce(moveLeftV);
                }
                else
                {
                    rb2d.AddForce(moveLeftInAirV);
                }
            }
            else
            {
                if (onGround)
                {
                    rb2d.AddForce(moveRightV);
                }
                else
                {
                    rb2d.AddForce(moveRightInAirV);
                }
            }
        }
    }

    // Apply vertical movement force if jump input is registered.
    void HandleJumping()
    {
        if (onGround)
        {
            if (!isJumpRestStarted)
            {
                isJumpRestStarted = true;
                jumpReadyTime = Time.timeSinceLevelLoad + jumpRestDuration;
            }

            // If on ground for rest duration and y velocity is insignificant
            if (inputJump && Time.timeSinceLevelLoad > jumpReadyTime && Mathf.Abs(rb2d.velocity.y) < deadzoneFactor)
            {
                rb2d.AddForce(jumpV); // Jump
                isJumpRestStarted = false;
            }
        }
    }

    void Attack()
    {
        if (attack1Ready)
        {
            if (inputAttack)
            {
                if (Time.timeSinceLevelLoad < comboEndTime)
                {
                    // If existing combo
                    combo++;
                    comboEndTime = Time.timeSinceLevelLoad + comboDuration;
                    isCriticalStrike = combo % 3 == 0;

                    // For every 3rd hit, play attack 3 (360 backhand strike)
                    if (isCriticalStrike)
                    {
                        // TODO: Critical strike every 3rd hit, increased damage
                        animator.Play("Attack3");
                    }
                    else
                    {
                        // Else randomly select between attack 1 and attack 2 animation
                        if (Random.Range(0, 2) == 0)
                        {
                            animator.Play("Attack");
                        }
                        else
                        {
                            animator.Play("Attack2");
                        }
                    }
                }
                else
                {
                    // If new combo
                    combo = 1;
                    comboEndTime = Time.timeSinceLevelLoad + comboDuration;
                    isCriticalStrike = false;

                    // Randomly select between attack 1 and attack 2 animation
                    if (Random.Range(0, 2) == 0)
                    {
                        animator.Play("Attack");
                    }
                    else
                    {
                        animator.Play("Attack2");
                    }
                }

                // Create a melee projectile
                GameObject projectile = Instantiate(weapon1, this.transform);

                // Assign weapon direction
                if (facingLeft)
                {
                    projectile.GetComponent<PlayerWeapon>().Setup(isCriticalStrike, new Vector2(-1, 0));
                }
                else
                {
                    projectile.GetComponent<PlayerWeapon>().Setup(isCriticalStrike, new Vector2(1, 0));
                }

                // Cooldown
                attack1Ready = false;
                attack1ReadyTime = Time.timeSinceLevelLoad + attack1Cooldown;
            }
        }
        else
        {
            if (Time.timeSinceLevelLoad > attack1ReadyTime)
            {
                attack1Ready = true;
            }
        }
    }
}