using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController1 : MonoBehaviour {
    private Animator animator;
    private HealthSystem healthSystem;
    private Rigidbody2D rb2d;

    public Transform groundCheck;

    private bool collisionOnRight = false;
    private bool onGround = false;
    private bool stunned = false;

    public float throwbackForce = 200f; // When hit by attack

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();
        rb2d = GetComponent<Rigidbody2D>();
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
            return; // Disallow AI-movement
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        collisionOnRight = collision.contacts[0].point.x > transform.position.x;

        // If colliding with projectile
        if (collision.gameObject.layer == 11)
        {
            stunned = true;
            healthSystem.DeductHealth(
                collision.gameObject.GetComponent<Projectile>().damage);
        }
    }
}
