using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Animator animator;
    private Rigidbody2D rb2d;
    private SpriteRenderer sprite;
    private Transform groundCheck;

    private bool againstWall = false;
    private bool againstEnemy = false;
    private bool collisionOnRight = false;
    private bool onGround = false;
    
    public float moveForce = 50f; // Since F = ma and m = 1, therefore a = F
    public float maxSpeed = 5f; // Maximum horziontal velocity
    public float jumpForce = 500f;
    public float throwbackForce = 200f; // When hit by enemy

    private float jumpMoveForce; // Half of moveForce, slower horizontal speed in the air
    private float slowdownForce; // Quarter of moveForce, applied on character when no input
    private readonly float deadzoneFactor = 1.0f; // Do not apply certain forces if velocity < or > this
    
    // Use this for initialization
    void Start ()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        groundCheck = transform.Find("groundCheck");
        jumpMoveForce = moveForce * 0.5f;
        slowdownForce = moveForce * 0.25f;
    }

    // Update is called once per frame, independent of the physics engine
    void Update()
    {
    }

    // Update is called in-step with the physics engine
    void FixedUpdate ()
    {
        Move();
        Jump();
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
        onGround = Physics2D.Linecast(transform.position, groundCheck.position,
            1 << LayerMask.NameToLayer("Ground"));

        // Get horizontal input for Left/Right movement
        float inputH = Input.GetAxis("Horizontal");
        float absVelocityX = Mathf.Abs(rb2d.velocity.x);

        // If against wall
        if (againstWall)
        {
            if (collisionOnRight)
            {
                rb2d.AddForce(Vector2.left * moveForce);
            }
            else
            {
                rb2d.AddForce(Vector2.right * moveForce);
            }

            return; // Disallow user-movement
        }

        if (againstEnemy)
        {
            if (collisionOnRight)
            {
                rb2d.AddForce(new Vector2(-1, 0.5f) * throwbackForce);
            }
            else
            {
                rb2d.AddForce(new Vector2(1, 0.5f) * throwbackForce);
            }

            return; // Disallow user-movement
        }

        // If there is no horizontal input
        if (inputH == 0)
        {
            // Slow character down until it comes to a complete rest
            if (absVelocityX > deadzoneFactor)
            {
                // Do not apply slow down force if velocity is smaller than deadzoneFactor
                // Apply slow down force
                if (rb2d.velocity.x > 0)
                {
                    rb2d.AddForce(Vector2.left * slowdownForce);
                }
                else
                {
                    rb2d.AddForce(Vector2.right * slowdownForce);
                }
            }

            if (onGround)
            {
                animator.Play("Idle");
            }
            else
            {
                animator.Play("Jump");
            }

            return; // If there is no input, no point calculating movement velocity
        }

        if (onGround)
        {
            animator.Play("Run");
        }
        else
        {
            animator.Play("Jump");
        }

        // If maxSpeed limit not reached
        if (absVelocityX < maxSpeed)
        {
            // Update facing
            sprite.flipX = inputH < 0;

            // Apply move force
            if (onGround)
            {
                rb2d.AddForce(Vector2.right * inputH * moveForce);
            }
            else
            {
                rb2d.AddForce(Vector2.right * inputH * jumpMoveForce);
            }
        }

        absVelocityX = Mathf.Abs(rb2d.velocity.x); // Update to check if > maxSpeed limit

        // If rb2d.velocity.x > maxSpeed limit
        if (absVelocityX > maxSpeed)
        {
            // Set rb2d.velocity to max velocity based on maxSpeed limit
            rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxSpeed, rb2d.velocity.y);
        }
    }

    // Jump
    void Jump()
    {
        if (Input.GetButton("Jump"))
        {
            // Only apply if y velocity is smaller than deadzoneFactor
            if (onGround && Mathf.Abs(rb2d.velocity.y) < deadzoneFactor) 
            {
                rb2d.AddForce(new Vector2(0f, jumpForce));
            }
        }
    }
}