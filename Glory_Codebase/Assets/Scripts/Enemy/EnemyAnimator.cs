using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(EnemyController))]
public class EnemyAnimator : MonoBehaviour {
    protected Animator animator;
    protected SpriteRenderer sprite;

    protected EnemyController enemyController;

    public bool spriteFacingLeft = false; // Initial facing of sprite, affects facingLeft boolean
    private bool facingLeft = false;

    public float attackFrame = 0f; // The attack animation frame at which a melee projectile is spawned

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        enemyController = GetComponent<EnemyController>();
	}
	
	// Update is called once per frame
	void Update () {
        // Update facing
        facingLeft = enemyController.AImoveH < 0;

        if (spriteFacingLeft)
            facingLeft = !facingLeft;

        sprite.flipX = facingLeft;

        // Update animation
        if (enemyController.enemyState == EnemyController.EnemyState.Idle)
        {
            animator.SetBool("Running", false);
        }
        else if (enemyController.enemyState == EnemyController.EnemyState.Run)
        {
            animator.SetBool("Running", true);
        }
        else if (enemyController.enemyState == EnemyController.EnemyState.AttackPlayer ||
            enemyController.enemyState == EnemyController.EnemyState.AttackObjective)
        {

        }
        else if (enemyController.enemyState == EnemyController.EnemyState.Dead)
        {

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
        animator.Play("Attack");
    }

    public bool IsAttackAnim()
    {
        return this.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
    }

    public bool IsIdleAnim()
    {
        return this.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
    }

    public bool IsAttackFrame()
    {
        return IsAttackAnim() && this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > attackFrame;
    }

    public bool IsFacingLeft()
    {
        return facingLeft;
    }

    public void FaceTarget(float distToTargetX)
    {
        facingLeft = distToTargetX < 0;

        if (spriteFacingLeft)
            facingLeft = !facingLeft;

        sprite.flipX = facingLeft;
    }
}
