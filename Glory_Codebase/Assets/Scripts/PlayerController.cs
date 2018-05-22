using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Rigidbody2D rb2d;
    private Transform groundCheck;

    private bool facingRight = true;
    private bool grounded = false;
    private bool jump = false;

    public float moveForce = 30f; // Since F = ma and m = 1, therefore a = F
    private float slowdownForce; // Half of moveForce
    private float slowdownDeadzone = 1.0f; // Do not apply slowdownForce if velocity < this
    public float maxSpeed = 5f; // Maximum horziontal velocity
    public float jumpForce = 100f;

    // Use this for initialization
    void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        slowdownForce = moveForce * 0.25f;
    }

    // Update is called once per frame
    void Update()
    {
        //grounded = Physics2D.Linecast(transform.position, 
            //groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

        if (Input.GetButtonDown("Jump") && grounded)
        {
            jump = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate () {
        Move();
    }

    // Move character
    void Move()
    {
        Debug.Log(rb2d.velocity);

        // Get horizontal input for Left/Right movement
        float inputH = Input.GetAxis("Horizontal");
        float absVelocityX = Mathf.Abs(rb2d.velocity.x);

        // If there is no horizontal input
        if (inputH == 0)
        {
            // Slow character down until it comes to a complete rest
            if (absVelocityX > slowdownDeadzone)
            {
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
            rb2d.AddForce(Vector2.right * inputH * moveForce);
            UpdateFacing(inputH);
        }

        absVelocityX = Mathf.Abs(rb2d.velocity.x); // Update to check if > maxSpeed limit

        // If rb2d.velocity.x > maxSpeed limit
        if (absVelocityX > maxSpeed)
        {
            // Set rb2d.velocity to max velocity based on maxSpeed limit
            rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxSpeed, rb2d.velocity.y);
        }
    }

    void UpdateFacing(float inputH) {
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