using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour {
    private Transform healthBar;
    private Renderer healthBarR, healthBarBackR;

    public float startingHealth = 100;
    private float currentHealth;

    // Use this for initialization
    void Start () {
        healthBar = transform.Find("Health_Enemy_1_red");
        healthBarR = healthBar.GetComponent<Renderer>();
        healthBarBackR = transform.Find("Health_Enemy_1_black").GetComponent<Renderer>();
        currentHealth = startingHealth;

        healthBarR.enabled = false;
        healthBarBackR.enabled = false;
    }

    public void DeductHealth(float damage)
    {
        currentHealth -= damage;
        SetHealthBarScale(currentHealth / startingHealth);

        healthBarR.enabled = true;
        healthBarBackR.enabled = true;
    }

    private void SetHealthBarScale(float scale)
    {
        Vector3 tempScale = healthBar.localScale;
        tempScale.x *= scale;
        healthBar.localScale = tempScale;
    }

	// Update is called once per frame
	void Update () {
	}
}
