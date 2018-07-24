using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilitySystem : MonoBehaviour {
    // References //
    private Animator animator;
    private Rigidbody2D rb2d;

    // Slide //
    public float slideCooldown = 2f; // Minimum wait-time before next slide can be triggered
    public float slideInvulDuration = 1f; // How long is the character invulnerable for when slideing
    private bool slideReady = true; // Reliant on slideCooldown
    private float slideReadyTime; // The time at which slideReady will be set to true again

    // Invulnerability //
    private bool isInvul = false; // Currently only slide triggers invulnerability
    private float invulEndTime; // The time at which the character is no longer invulnerable

    // Forces
    private readonly float slideMultiplier = 0.4f;
    private Vector2 slideLeftV, slideRightV; // 25% of moveLeftV and moveRightV

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Use this for initialization
    public void Setup (Vector2 moveLeftV, Vector2 moveRightV) {
        slideLeftV = moveLeftV * slideMultiplier;
        slideRightV = moveRightV * slideMultiplier;
    }

    private void Update()
    {
        HandleInvul();
    }

    void HandleInvul()
    {
        // Invulnerability can be triggered by sliding and ...
        if (isInvul)
        {
            if (Time.timeSinceLevelLoad > invulEndTime)
            {
                isInvul = false;
            }
        }
    }

    public void Slide (bool isFacingLeft) {
        if (slideReady)
        {
            if (isFacingLeft)
            {
                rb2d.velocity = slideLeftV;
            }
            else
            {
                rb2d.velocity = slideRightV;
            }

            animator.SetBool("Jumping", false);
            animator.Play("Slide");

            slideReady = false;
            slideReadyTime = Time.timeSinceLevelLoad + slideCooldown;
            isInvul = true;
            invulEndTime = Time.timeSinceLevelLoad + slideInvulDuration;
        }
        else
        {
            if (Time.timeSinceLevelLoad > slideReadyTime)
            {
                slideReady = true;
            }
        }
    }

    public bool IsInvul()
    {
        return isInvul;
    }

    public bool IsSliding()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Slide");
    }
}
