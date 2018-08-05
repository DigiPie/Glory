using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthSystem : MonoBehaviour {
    protected BlinkSystem blinkSystem; // Handles blinking effect
    private HUD hud;
    public SpriteRenderer outlineRend, frontRend, backRend;
    public float health = 100;
    public bool isBoss = false;
    private float currentHealth = 1;

    // Use this for initialization
    void Start () {
        blinkSystem = GetComponent<BlinkSystem>();
        currentHealth = health;
    }

    public void Setup()
    {
        // Hide it at first, only show when not at full health
        outlineRend.enabled = false;
        frontRend.enabled = false;
        backRend.enabled = false;
    }

    public void Setup(HUD hud)
    {
        this.hud = hud;
    }

    public void DeductHealth(float damage)
    {
        currentHealth -= damage;

        if (isBoss)
        {
            hud.UpdateBossHealth(currentHealth);

            if (currentHealth <= 0)
            {
                hud.HideBossHealth();
            }
        }
        else
        {
            if (currentHealth <= 0)
            {
                outlineRend.enabled = false;
                frontRend.enabled = false;
                backRend.enabled = false;
            }
            else
            {
                SetHealthBarScale((currentHealth + 1) / health);

                // If invisible, show
                outlineRend.enabled = true;
                frontRend.enabled = true;
                backRend.enabled = true;
            }
        }
    }

    public void DeductHealth(float damage, float blinkDuration)
    {
        DeductHealth(damage);
        blinkSystem.StartBlink();
    }

    private void SetHealthBarScale(float scale)
    {
        if (isBoss)
            return;

        frontRend.transform.localScale = new Vector3(scale, 1, 1);
    }

    public bool IsDead()
    {
        return currentHealth < 1;
    }

    public bool IsBoss()
    {
        return isBoss;
    }

    public int GetCurrentHealth()
    {
        return (int)currentHealth;
    }

    public int GetMaxHealth()
    {
        return (int)health;
    }

    public void SetSortingOrder(int sortingOrder)
    {
        backRend.sortingOrder = sortingOrder;
        frontRend.sortingOrder = backRend.sortingOrder + 1;
        outlineRend.sortingOrder = frontRend.sortingOrder + 1;
    }
}
