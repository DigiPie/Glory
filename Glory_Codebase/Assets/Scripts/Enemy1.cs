using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : MonoBehaviour {
    private Animator animator;
    private HealthSystem healthSystem;
    private Rigidbody2D rb2d;
    private Transform groundCheck;

    private bool collisionOnRight = false;
    private bool stunned = false;
    private bool onGround = false;

    public float throwbackForce = 200f; // When hit by projectile

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();
        rb2d = GetComponent<Rigidbody2D>();
        groundCheck = transform.Find("groundCheck");
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        onGround = Physics2D.Linecast(transform.position, groundCheck.position,
            1 << LayerMask.NameToLayer("Ground"));

        if (onGround)
        {
            animator.Play("Idle");
        }
        else
        {
            animator.Play("Jump");
        }

        if (stunned)
        {
            if (collisionOnRight)
            {
                rb2d.AddForce(new Vector2(-0.25f, 1) * throwbackForce);
            }
            else
            {
                rb2d.AddForce(new Vector2(0.25f, 1) * throwbackForce);
            }

            stunned = false;
            return; // Disallow user-movement
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        collisionOnRight = collision.contacts[0].point.x > transform.position.x;

        // If colliding with projectile
        if (collision.gameObject.layer == 11)
        {
            stunned = true;
            healthSystem.DeductHealth(10);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {

    }
}
