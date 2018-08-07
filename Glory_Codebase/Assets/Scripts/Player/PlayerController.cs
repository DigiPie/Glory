using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public GameManager gameManager;
    private Rigidbody2D rb2d;
    private PlayerActionSystem actionSystem;
    private PlayerAnimator playerAnimator;
    public Transform groundCheck;

    // Forces to be applied on character
    private Vector2 bounceEnemyLeftV, bounceEnemyRightV;
    private Vector2 slowdownLeft, slowdownRightV;
    private Vector2 moveLeftV, moveRightV;
    private Vector2 jumpV;

    // States
    private bool canAttack = true;
    private bool againstWall = false;
    private bool againstEnemyAttack = false;
    private bool collisionOnRight = false;
    private bool exceedOnRight = false; // Exceed the bounds of the map
    private bool onGround = false;

    // Input
    private bool inputJump, inputSlide, inputAttack, inputSpell1, inputSpell2;
    private float inputH;

    // Movement
    public float moveForce = 50f; // Since F = ma and m = 1, therefore a = F
    public float maxSpeed = 5f; // Maximum horziontal velocity
    private float preBuffMaxSpeed;
    private float maxSpeedInAir;
    private float maxSpeedWhileAttk;
    public float throwbackForce = 200f; // When hit by enemy
    private float whileAttkMoveForce; // Half of moveForce, slower horizontal speed in the air
    private float inAirMoveForce; // Half of moveForce, slower horizontal speed in the air
    private float slowdownForce; // Quarter of moveForce, applied on character when no input
    private readonly float deadzoneFactor = 1.0f; // Do not apply certain forces if velocity < or > this

    // Jump
    public float jumpForce = 500f;
    private bool isJumpRestStarted = true;
    private float jumpRestDuration = 0.02f; // Only allowed to jump again after being on ground for rest duration
    private float jumpReadyTime = 0;

    // Use this for initialization
    void Start ()
    {
        rb2d = GetComponent<Rigidbody2D>();
        actionSystem = GetComponent<PlayerActionSystem>();
        playerAnimator = GetComponent<PlayerAnimator>();

        // Calculate static values of forces here instead of FixedUpdate() so we
        // only calculate them once, as they never change. For optimsation.
        inAirMoveForce = moveForce * 0.5f;
        whileAttkMoveForce = moveForce * 0.4f;
        slowdownForce = moveForce;

        preBuffMaxSpeed = maxSpeed;
        maxSpeedInAir = maxSpeed * 0.7f;
        maxSpeedWhileAttk = maxSpeed * 0.5f;

        // Calculate the static vectors here instead of FixedUpdate() so we only
        // calculate them once, as they never change. For optimisation.
        bounceEnemyLeftV = new Vector2(1, 0.5f) * throwbackForce;
        bounceEnemyRightV = new Vector2(-1, 0.5f) * throwbackForce;
        slowdownLeft = Vector2.left * slowdownForce;
        slowdownRightV = Vector2.right * slowdownForce;
        moveLeftV = Vector2.left * moveForce;
        moveRightV = Vector2.right * moveForce;
        jumpV = new Vector2(0f, jumpForce);

        // Abilities
        actionSystem.Setup(moveLeftV, moveRightV);
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
        inputSlide = Input.GetButton("Slide");
        inputAttack = Input.GetButton("Attack");
        inputSpell1 = Input.GetButton("Spell1");
        inputSpell2 = Input.GetButton("Spell2");

        Move();

        if (canAttack)
        {
            Attack();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        collisionOnRight = collider.transform.position.x > transform.position.x;

        if (collider.gameObject.layer == 9)
        {
            exceedOnRight = collisionOnRight;
            againstWall = true;
        }
        else if (collider.gameObject.layer == 13)
        {
            collider.GetComponent<EnemyWeapon>().StartDestroy();

            if (actionSystem.IsInvul())
            {
                return;
            }

            gameManager.DamagePlayer(collider.GetComponent<EnemyWeapon>().damage);
            againstEnemyAttack = true;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        // If colliding with wall
        if (collider.gameObject.layer == 9)
        {
            againstWall = false;
        }
    }

    // Move character
    void Move()
    {
        // Apply bounce-off forces when colliding with wall and enemies.
        HandleBounceOff();

        // If there is no horizontal input or player is casting a spell
        if (inputH == 0 || playerAnimator.IsCastAnim())
        {
            // Do not run and start slowing down
            HandleSlowdown();
        }
        else
        {
            // If there is horizontal input
            playerAnimator.FaceForward();
            HandleRunning();
        }

        if (inputSlide)
        {
            HandleSlide();
            return; // If both slide and jump input are pressed at the same time, slide only
        }

        HandleJumping();
    }

    // Apply bounce-off forces when colliding with wall and enemies.
    bool HandleBounceOff()
    {
        if (againstEnemyAttack)
        {
            if (collisionOnRight)
            {
                rb2d.AddForce(bounceEnemyRightV);
            }
            else
            {
                rb2d.AddForce(bounceEnemyLeftV);
            }

            againstEnemyAttack = false;
        }

        // Return true if bouncing off either against wall or enemy
        return againstEnemyAttack;
    }

    void HandleSlide()
    {
        if (inputSlide)
        {
            actionSystem.Slide();
        }
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

        playerAnimator.PlayRun(false);
    }

    // Apply horizontal movement forces if horizontal input is registered.
    void HandleRunning()
    {
        float tempMaxSpd;

        if (playerAnimator.IsCasting())
            return;
        else if (playerAnimator.IsAttacking())
            tempMaxSpd = maxSpeedWhileAttk;
        else if (!onGround)
            tempMaxSpd = maxSpeedInAir;
        else
            tempMaxSpd = maxSpeed;

        // Slows down character if maxSpeed reached
        if (Mathf.Abs(rb2d.velocity.x) > tempMaxSpd)
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
                // Prevent further left movement if against wall
                if (againstWall && !exceedOnRight)
                {
                    playerAnimator.PlayRun(false);
                    return;
                }

                rb2d.AddForce(moveLeftV);
                playerAnimator.PlayRun(true);
            }
            else
            {
                // Prevent further right movement if against wall
                if (againstWall && exceedOnRight)
                {
                    playerAnimator.PlayRun(false);
                    return;
                }

                rb2d.AddForce(moveRightV);
                playerAnimator.PlayRun(true);
            }
        }
    }

    // Apply vertical movement force if jump input is registered.
    void HandleJumping()
    {
        if (onGround)
        {
            if (!isJumpRestStarted)
            {
                isJumpRestStarted = true;
                jumpReadyTime = Time.timeSinceLevelLoad + jumpRestDuration;
            }

            // If on ground for rest duration and y velocity is insignificant
            if (inputJump && Time.timeSinceLevelLoad > jumpReadyTime && Mathf.Abs(rb2d.velocity.y) < deadzoneFactor)
            {
                rb2d.AddForce(jumpV); // Jump
                isJumpRestStarted = false;
            }
        }
    }

    void Attack()
    {
        // If is not sliding or jumping
        if (!playerAnimator.IsSliding() && onGround)
        {
            if (inputAttack)
            {
                actionSystem.NormalAttack();
            }
            else if (inputSpell1)
            {
                actionSystem.Spell1();
            }
            else if (inputSpell2)
            {
                actionSystem.Spell2();
            }
        }
    }

    public void AllowAttack(bool canAttack)
    {
        this.canAttack = canAttack;
    }

    public bool GetMoveH()
    {
        return inputH < 0;
    }

    public bool GetOnGround()
    {
        return onGround;
    }

    public void ResetMaxSpeed()
    {
        maxSpeed = preBuffMaxSpeed;
    }

    public void ApplySpeedBuff(float speedMultiplier)
    {
        maxSpeed = preBuffMaxSpeed * speedMultiplier;
    }
}