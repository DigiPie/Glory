using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour {
    public Transform healthbarFront;
    public Transform healthbarBack;
    private SpriteRenderer frontRend, backRend;

    public float health = 100;
    private float currentHealth;

    // Use this for initialization
    void Start () {
        currentHealth = health;

        frontRend = healthbarFront.GetComponent<SpriteRenderer>();
        backRend = healthbarBack.GetComponent<SpriteRenderer>();

        // Hide it at first, only show when not at full health
        frontRend.enabled = false;
        backRend.enabled = false;
    }

    public void DeductHealth(float damage)
    {
        currentHealth -= damage;
        SetHealthBarScale(currentHealth / health);

        // If invisible, show
        frontRend.enabled = true;
        backRend.enabled = true;
    }

    private void SetHealthBarScale(float scale)
    {
        frontRend.transform.localScale = new Vector3(scale, 1, 1);
    }
}
