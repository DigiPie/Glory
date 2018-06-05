using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
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
    private Vector2 jumpV;

    private bool againstWall = false;
    private bool againstEnemy = false;
    private bool collisionOnRight = false;
    private bool facingLeft = false;
    private bool onGround = false;
    private bool inputJump, inputAttack1, inputDash;
    // Dash
    public float dashCooldown;
    private bool dashReady = true;
    private float dashReadyTime = 0;
    // Attack1
    private float attack1Cooldown;
    private bool attack1Ready = true;
    private float attack1ReadyTime = 0;
    // Invulnerabilty Time
    public float dashInvuln;
    private float dashInvulnTime;

    private float inputH;

    public float moveForce = 50f; // Since F = ma and m = 1, therefore a = F
    public float maxSpeed = 5f; // Maximum horziontal velocity
    public float jumpForce = 500f;
    public float throwbackForce = 200f; // When hit by enemy

    private float inAirMoveForce; // Half of moveForce, slower horizontal speed in the air
    private float slowdownForce; // Quarter of moveForce, applied on character when no input
    private readonly float deadzoneFactor = 1.0f; // Do not apply certain forces if velocity < or > this
    
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
        moveLeftInAirV = Vector2.left * inAirMoveForce;
        moveRightInAirV = Vector2.right * inAirMoveForce;
        jumpV = new Vector2(0f, jumpForce);
        // Dash
        dashCooldown = 2f;
        dashInvulnTime = 0.5f;
        // Attack
        attack1Cooldown = weapon1.GetComponent<Projectile>().cooldown;

    }

    // Update is called once per frame, independent of the physics engine
    void Update()
    {
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
        inputDash = Input.GetButton("Dash");
        inputAttack1 = Input.GetButton("Attack1");

        Move();
        Attack();
        Dash();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        collisionOnRight = collision.contacts[0].point.x > transform.position.x;

        // If colliding with wall
        if (collision.gameObject.layer == 9)
        {
            againstWall = true;
        }

        if (collision.gameObject.layer == 10)
        {
            againstEnemy = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // If colliding with wall
        if (collision.gameObject.layer == 9)
        {
            againstWall = false;
        }

        if (collision.gameObject.layer == 10)
        {
            againstEnemy = false;
        }
    }

    // Move character
    void Move()
    {
        // Apply bounce-off forces when colliding with wall and enemies.
        HandleBounceOff();

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

        if(inputJump)
        {
            HandleJumping();
        }

        animator.SetBool("Jumping", !onGround);
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

        if (againstEnemy)
        {
            if (collisionOnRight)
            {
                rb2d.AddForce(bounceEnemyRightV);
            }
            else
            {
                rb2d.AddForce(bounceEnemyLeftV);
            }
        }

        // Return true if bouncing off either against wall or enemy
        return againstWall || againstEnemy;
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
        // Apply force if on ground and y velocity is insignificant.
        if (onGround && Mathf.Abs(rb2d.velocity.y) < deadzoneFactor)
        {
            rb2d.AddForce(jumpV);
        }
    }

    void Attack()
    {
        if (attack1Ready)
        {
            if (inputAttack1)
            {
                // Create a melee projectile
                GameObject projectile = Instantiate(weapon1, this.transform);

                // Assign weapon direction
                if (facingLeft)
                {
                    projectile.GetComponent<Projectile>().SetDir(new Vector2(-1, 0));
                }
                else
                {
                    projectile.GetComponent<Projectile>().SetDir(new Vector2(1, 0));
                }

                // Cooldown
                attack1Ready = false;
                attack1ReadyTime = Time.time + attack1Cooldown;
            }
        }
        else
        {
            if (Time.time > attack1ReadyTime)
            {
                attack1Ready = true;
            }
        }
    }

    void Dash()
    {
        if (Time.time > dashReadyTime)
        {
            dashReady = true;
        }
        if (inputDash && dashReady)
        {
            Physics2D.IgnoreLayerCollision(10, 12, true);

            if (facingLeft)
            {
                // Float number determines dash length
                rb2d.velocity = moveLeftV*0.25f;
                //rb2d.AddForce(moveLeftV * 12);
            }
            else
            {
                rb2d.velocity = moveRightV*0.25f;
                //rb2d.AddForce(moveRightV * 12);
            }
            dashReady = false;
            dashReadyTime = Time.time + dashCooldown;
            dashInvulnTime = Time.time + dashInvuln;
        }
        if (Time.time > dashInvulnTime)
        {
            Physics2D.IgnoreLayerCollision(10, 12, false);
        }
    }
}