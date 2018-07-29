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

    private bool isFadingIn = true;
    private bool isFadingOut = false;
    private float opacity = 0f;
    private float fadeInSpeed = 6.0f;
    private float fadeOutSpeed = 3.0f;

    // Use this for initialization
    void Start () {
        enemyController = GetComponent<EnemyController>();
        sprite.color = new Color(1.0f, 1.0f, 1.0f, 0f);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (isFadingOut)
        {
            if (opacity > 0.1f)
            {
                opacity -= fadeOutSpeed * Time.fixedDeltaTime;
                sprite.color = new Color(1.0f, 1.0f, 1.0f, opacity);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else if (isFadingIn)
        {
            if (opacity < 1.0f)
            {
                opacity += fadeInSpeed * Time.fixedDeltaTime;
            }
            else
            {
                opacity = 1.0f;
                isFadingIn = false;
            }

            sprite.color = new Color(1.0f, 1.0f, 1.0f, opacity);
        }

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

    public void StartDestroy()
    {
        if (sprite == null)
        {
            Destroy(gameObject);
            return;
        }

        opacity = 1.0f;

        isFadingOut = true;
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
