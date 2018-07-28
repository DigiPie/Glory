using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyAnimator : MonoBehaviour {
    public Animator animator;
    public SpriteRenderer sprite;

    protected EnemyController enemyController;

    public bool spriteFacingLeft = false; // Initial facing of sprite, affects facingLeft boolean
    private bool facingLeft = false;

    public float attackFrame = 0f; // The attack animation frame at which a melee projectile is spawned

    // Use this for initialization
    void Start () {
        enemyController = GetComponent<EnemyController>();
	}
	
	// Update is called once per frame
	void Update () {
        // Update animation
        if (enemyController.IsIdle() || enemyController.IsStunned())
        {
            animator.SetBool("Running", false);
        }
        else if (enemyController.IsRunning())
        {
            FaceForward();
            animator.SetBool("Running", true);
        }
    }

    public void PlayAttack()
    {
        animator.Play("Attack");
    }

    public void PlayHurt()
    {
        animator.Play("Hurt");
    }

    public void PlayDeath()
    {
        animator.Play("Death");
    }

    public bool IsAttackAnim()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
    }

    public bool IsIdleAnim()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
    }

    public bool IsAttackFrame()
    {
        return IsAttackAnim() && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > attackFrame;
    }

    public bool IsAnimationOver()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f;
    }

    public bool IsFacingLeft()
    {
        return facingLeft;
    }

    public void FaceForward()
    {
        // Update facing
        facingLeft = enemyController.GetAImoveH();

        if (spriteFacingLeft)
            sprite.flipX = !facingLeft;
        else
            sprite.flipX = facingLeft;
    }

    public void FaceTarget(float distToTargetX)
    {
        facingLeft = distToTargetX < 0;

        if (spriteFacingLeft)
            sprite.flipX = !facingLeft;
        else
            sprite.flipX = facingLeft;
    }

    public void SetSortingOrder(int sortingOrder)
    {
        sprite.sortingOrder = sortingOrder;
    }
}
