using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController1 : MonoBehaviour {
    private Animator animator;
    private HealthSystem healthSystem;
    private Rigidbody2D rb2d;
    public Transform groundCheck;

    // Forces to be applied on character
    private Vector2 bounceHurtLeftV, bounceHurtRightV;

    // AI
    private Transform[] path; // The AI path, it will move to path[0], path[1]...path[n]
    private float AImoveH = 0; // Used by the AI to move character
    private int currentTarget = 0; // Current target, path[currentTarget]
    private float distToTargetX = 0; // Distance from this to target
    private float absDistToTargetX = 0; // Absolute value used to compare against deadzone value
    private readonly float distDeadzone = 2; // If within 2 unit distance from target, currentTarget++
    private bool isPathDone = false; // True if reached the end of the designated path

    // Forces to be applied on character
    private Vector2 moveLeftV, moveRightV;

    // States
    private bool collisionOnRight = false;
    private bool onGround = false;

    // Stunned
    public float stunDuration = 1; // How long is the character stunned each time it is damaged
    private bool isStunned = false;
    private float stunEndTime = 0; // The time at which stunned is set to false again

    // Movement
    public float moveForce = 50f; // Since F = ma and m = 1, therefore a = F
    public float maxSpeed = 5f; // Maximum horziontal velocity
    public float throwbackForce = 200f; // When hit by attack

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();
        rb2d = GetComponent<Rigidbody2D>();

        // Calculate the bounce-off vectors here instead of FixedUpdate() so we only
        // calculate them once, as they never change. For optimisation.
        moveLeftV = Vector2.left * moveForce;
        moveRightV = Vector2.right * moveForce;
        bounceHurtLeftV = new Vector2(1, 0.5f) * throwbackForce;
        bounceHurtRightV = new Vector2(-1, 0.5f) * throwbackForce;
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

        if (isStunned)
        {
            UpdateStun();
            return; // Disallow AI-movement
        }

        AI();

        if (AImoveH != 0)
        {
            HandleRunning();
        }
    }

    public void Setup(Transform[] designatedPath)
    {
        path = designatedPath;
    }

    void AI()
    {
        if (isPathDone)
        {
            return;
        }

        distToTargetX = path[currentTarget].position.x - transform.position.x;
        absDistToTargetX = Mathf.Abs(distToTargetX);

        if (absDistToTargetX < distDeadzone)
        {
            currentTarget++;

            if (path.Length == currentTarget)
            {
                isPathDone = true;
                AImoveH = 0;
                return;
            }
        }

        if (distToTargetX > 0)
        {
            AImoveH = 1;
        } else
        {
            AImoveH = -1;
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

    void UpdateStun()
    {
        if (isStunned)
        {
            if (Time.time > stunEndTime)
            {
                isStunned = false;
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
            isStunned = true;
            stunEndTime = Time.time + stunDuration;

            // Throwback effect
            if (collisionOnRight)
            {
                rb2d.AddForce(bounceHurtRightV);
            }
            else
            {
                rb2d.AddForce(bounceHurtLeftV);
            }

            // Health deduction
            healthSystem.DeductHealth(
                collider.GetComponent<Weapon>().damage);
        }
    }
}
