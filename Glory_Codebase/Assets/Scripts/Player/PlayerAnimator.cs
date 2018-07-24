using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {
    public Animator animator;
    public SpriteRenderer sprite;

    protected PlayerController playerController;

    public bool spriteFacingLeft = false; // Initial facing of sprite, affects facingLeft boolean
    private bool facingLeft = false;

    // Use this for initialization
    void Start () {
        playerController = GetComponent<PlayerController>();
    }
	
	// Update is called once per frame
	void Update () {
        animator.SetBool("Jumping", !playerController.GetOnGround());
    }

    public void FaceForward()
    {
        // Update facing
        facingLeft = playerController.GetMoveH();

        if (spriteFacingLeft)
            sprite.flipX = !facingLeft;
        else
            sprite.flipX = facingLeft;
    }

    public void PlayRun(bool isRun)
    {
        animator.SetBool("Running", isRun);
    }

    public void PlaySlide()
    {
        animator.SetBool("Jumping", false);
        animator.Play("Slide");
    }

    public void PlayAttack()
    {
        if (Random.Range(0, 2) == 0)
        {
            animator.Play("Attack");
        }
        else
        {
            animator.Play("Attack2");
        }
    }

    public void PlayAttackCriticalStrike()
    {
        animator.Play("Attack3");
    }

    public void PlayCast()
    {
        animator.Play("Cast");
    }

    public bool IsSliding()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Slide");
    }

    public bool IsAttacking()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3");

    }

    public bool IsFacingLeft()
    {
        return facingLeft;
    }

    public bool IsCasting()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Cast");
    }
}
