using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {
    public Animator animator;
    public SpriteRenderer sprite;

    protected PlayerController playerController;

    public bool spriteFacingLeft = false; // Initial facing of sprite, affects facingLeft boolean
    private bool facingLeft = false;

    public float attackFrame = 0f; // The attack animation frame at which a melee projectile is spawned
    public float attack2Frame = 0f; // The attack animation frame at which a melee projectile is spawned
    public float attack3Frame = 0f; // The attack animation frame at which a melee projectile is spawned
    public float castFrame = 0f; // The attack animation frame at which a magic projectile is spawned

    // Use this for initialization
    void Start() {
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update() {
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

    public bool IsAttackAnim()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
    }

    public bool IsAttack2Anim()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2");
    }

    public bool IsAttack3Anim()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3");
    }

    public bool IsCastAnim()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Cast");

    }
    public bool IsAttackFrame()
    {
        return (IsAttackAnim() && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > attackFrame)
            || (IsAttack2Anim() && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > attack2Frame);
    }

    public bool IsCriticalAttackFrame()
    {
        return (IsAttack3Anim() && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > attack3Frame);
    }

    public bool IsCastFrame()
    {
        return (IsCastAnim() && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > castFrame);
    }
}
