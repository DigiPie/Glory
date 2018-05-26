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

    public float throwbackForce = 100f; // When hit by attack

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame, independent of the physics engine
    void Update()
    {
        if (healthSystem.IsDead())
        {
            Object.Destroy(this.gameObject);
        }
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

    void OnTriggerEnter2D(Collider2D collider)
    {
        collisionOnRight = collider.transform.position.x > transform.position.x;

        // If colliding with projectile
        if (collider.gameObject.layer == 11)
        {
            stunned = true;
            healthSystem.DeductHealth(
                collider.GetComponent<Projectile>().damage);
        }
    }
}
