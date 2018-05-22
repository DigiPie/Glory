using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Rigidbody2D rb2d;
    private Transform groundCheck;

    private bool againstWall = false;
    private bool collisionOnRight;
    private bool facingRight = true;
    private bool onGround = false;
    
    public float moveForce = 50f; // Since F = ma and m = 1, therefore a = F
    public float maxSpeed = 5f; // Maximum horziontal velocity
    public float jumpForce = 500f;

    private float jumpMoveForce; // Half of moveForce, slower horizontal speed in the air
    private float slowdownForce; // Quarter of moveForce, applied on character when no input
    private readonly float deadzoneFactor = 1.0f; // Do not apply certain forces if velocity < or > this
    
    // Use this for initialization
    void Start ()
    {
        rb2d = GetComponent<Rigidbody2D>();
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
        // If colliding with wall
        if (collision.gameObject.layer == 9)
        {
            againstWall = true;
            collisionOnRight = collision.contacts[0].point.x > transform.position.x;
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

    // Move character
    void Move()
    {
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

            return; // If there is no input, no point calculating movement velocity
        }

        // If maxSpeed limit not reached
        if (absVelocityX < maxSpeed)
        {
            // Apply move force
            UpdateFacing(inputH);

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
        onGround = Physics2D.Linecast(transform.position, groundCheck.position,
            1 << LayerMask.NameToLayer("Ground"));

        if (Input.GetButton("Jump"))
        {
            // Only apply if y velocity is smaller than deadzoneFactor
            if (onGround && Mathf.Abs(rb2d.velocity.y) < deadzoneFactor) 
            {
                rb2d.AddForce(new Vector2(0f, jumpForce));
            }
        }
    }

    void UpdateFacing(float inputH)
    {
        // If facing changed
        if (inputH > 0 && !facingRight || inputH < 0 && facingRight)
        {
            facingRight = !facingRight; // Flip boolean switch
            Vector3 tempScale = transform.localScale;
            tempScale.x *= -1;
            transform.localScale = tempScale; // Flip scale on x-axis
        }
    }  
}