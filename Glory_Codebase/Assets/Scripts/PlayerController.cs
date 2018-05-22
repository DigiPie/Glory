using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Rigidbody2D rb2d;
    private Vector2 charVelocity;
    private bool facingRight = true;
    private bool grounded = false;
    private bool jump = false;
    public float moveForce = 360f;
    public float dragForce = 90f;
    public float maxSpeed = 5f;
    public float jumpForce = 1000f;
    public Transform groundCheck;

    // Use this for initialization
    void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        charVelocity = rb2d.velocity;
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics2D.Linecast(transform.position, 
            groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

        if (Input.GetButtonDown("Jump") && grounded)
        {
            jump = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate () {
        // Get horizontal input for Left/Right movement
        float h = Input.GetAxis("Horizontal");
    
        // If changing direction (h and charVelocity.x have different signs) 
        // or maxSpeed limit not reached
        if (h * charVelocity.x < maxSpeed)
        {
            // Apply move force
            rb2d.AddForce(Vector2.right * h * moveForce);

            // If facing changed
            if (h > 0 && !facingRight || h < 0 && facingRight)
            {
                facingRight = !facingRight; // Flip boolean switch
                Vector3 tempScale = transform.localScale;
                tempScale.x *= -1;
                transform.localScale = tempScale; // Flip scale on x-axis
            }
        }

        // If charVelocity.x > maxSpeed limit
        if (Mathf.Abs(charVelocity.x) > maxSpeed)
        {
            // Set charVelocity to max velocity based on maxSpeed limit
            charVelocity = new Vector2(Mathf.Sign(charVelocity.x) * maxSpeed, charVelocity.y);
        }
       
    }
}