using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController1 : MonoBehaviour {
    private Animator animator;
    private HealthSystem healthSystem;
    private Rigidbody2D rb2d;
    public Transform groundCheck;

    // AI
    private Transform[] path;
    private float AImoveH = 0; // Used by AI to move character
    private int currentWaypointTarget = 0;
    private float distToTargetX = 0;
    private float absDistToTargetX = 0;
    private float distDeadzone = 2;
    private bool isDone = false;

    // Forces to be applied on character
    private Vector2 moveLeftV, moveRightV;

    // States
    private bool collisionOnRight = false;
    private bool onGround = false;
    private bool stunned = false;

    // Movement
    public float moveForce = 50f; // Since F = ma and m = 1, therefore a = F
    public float maxSpeed = 5f; // Maximum horziontal velocity
    public float throwbackForce = 100f; // When hit by attack

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();
        rb2d = GetComponent<Rigidbody2D>();

        // Calculate the bounce-off vectors here instead of FixedUpdate() so we only
        // calculate them once, as they never change. For optimisation.
        moveLeftV = Vector2.left * moveForce;
        moveRightV = Vector2.right * moveForce;
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
            HandleStun();
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
        if (isDone)
        {
            return;
        }

        distToTargetX = path[currentWaypointTarget].position.x - transform.position.x;
        absDistToTargetX = Mathf.Abs(distToTargetX);

        if (absDistToTargetX < distDeadzone)
        {
            currentWaypointTarget++;

            if (path.Length == currentWaypointTarget)
            {
                isDone = true;
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

    void HandleStun()
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
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        collisionOnRight = collider.transform.position.x > transform.position.x;

        // If colliding with projectile
        if (collider.gameObject.layer == 11)
        {
            stunned = true;
            healthSystem.DeductHealth(
                collider.GetComponent<Weapon>().damage);
        }
    }
}
