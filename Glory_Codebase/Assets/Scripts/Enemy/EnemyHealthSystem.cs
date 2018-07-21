using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthSystem : MonoBehaviour {
    public SpriteRenderer outlineRend, frontRend, backRend;

    public float health = 100;
    private float currentHealth = 1;

    // Use this for initialization
    void Start () {
        currentHealth = health;

        // Hide it at first, only show when not at full health
        outlineRend.enabled = false;
        frontRend.enabled = false;
        backRend.enabled = false;
    }

    public void DeductHealth(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            outlineRend.enabled = false;
            frontRend.enabled = false;
            backRend.enabled = false;
        }
        else
        {
            SetHealthBarScale((currentHealth + 10) / health);

            // If invisible, show
            outlineRend.enabled = true;
            frontRend.enabled = true;
            backRend.enabled = true;
        }
    }

    private void SetHealthBarScale(float scale)
    {
        frontRend.transform.localScale = new Vector3(scale, 1, 1);
    }

    public bool IsDead()
    {
        return currentHealth < 1;
    }
}
