﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerHealthSystem : MonoBehaviour
{
    public int startingHealth = 100;                            // The amount of health the player starts the game with.
    public int currentHealth;                                   // The current health the player has.
    private float displayHealth;
    private float displayChangeSpeed = 20.0f;
    private float diffDeadzone = 1.0f;
    private float diffHealth;
    private float absDiffHealth;
    private bool isDiff = false;

    // public Image damageImage;                                   // Reference to an image to flash on the screen on being hurt.

    //public Slider healthSlider;                                 // Reference to the UI's health bar.
    public AudioClip deathClip;                                 // The audio clip to play when the player dies.
    // public float flashSpeed = 5f;                               // The speed the damageImage will fade at.
    // public Color flashColour = new Color(1f, 0f, 0f, 0.1f);     // The colour the damageImage is set to, to flash.


    Animator anim;                                              // Reference to the Animator component.
    //AudioSource playerAudio;                                    // Reference to the AudioSource component.
    //PlayerMovement playerMovement;                              // Reference to the player's movement.
    //PlayerShooting playerShooting;                              // Reference to the PlayerShooting script.
    PlayerController playerController;
    public bool isDead;                                                // Whether the player is dead.

    void Awake()
    {
        // Setting up the references.
        anim = GetComponent<Animator>();
        //playerAudio = GetComponent<AudioSource>();

        playerController = GetComponent<PlayerController>();
        //playerMovement = GetComponent<PlayerMovement>();
        //playerShooting = GetComponentInChildren<PlayerShooting>();

        // Set the initial health of the player.
        currentHealth = startingHealth;
        displayHealth = currentHealth;
    }


    void FixedUpdate()
    { 
        if (isDiff)
        {
            diffHealth = currentHealth - displayHealth;
            absDiffHealth = Mathf.Abs(diffHealth);

            if (absDiffHealth < diffDeadzone)
            {
                displayHealth = currentHealth;

                if (displayHealth < 10)
                    displayHealth = 10;

                isDiff = false;
                return;
            }

            if (diffHealth > 0)
            {
                displayHealth += Time.deltaTime * displayChangeSpeed;
            }
            else
            {
                displayHealth -= Time.deltaTime * displayChangeSpeed;
            }
        }
    }

    // Important to be public as it is called by other functions.
    public void TakeDamage(int amount)
    {
        isDiff = true;
        // Set the damaged flag so the screen will flash.

        // Reduce the current health by the damage amount.
        currentHealth -= amount;

        // Set the health bar's value to the current health.
        // healthSlider.value = currentHealth;

        // Play the hurt sound effect.
        //playerAudio.Play();

        // If the player has lost all it's health and the death flag hasn't been set yet...
        if (currentHealth <= 0 && !isDead)
        {
            // ... it should die.
            Death();
        }
    }

    public void Heal(int amount)
    {
        isDiff = true;

        if ((currentHealth+amount) > startingHealth)
        {
            currentHealth = startingHealth;
        }
        else
        {
            currentHealth += amount;
        }

        // healthSlider.value = currentHealth;
    }

    public void ResetFullHealth()
    {
        isDiff = true;
        currentHealth = startingHealth;
    }

    public float GetDisplayHealth()
    {
        return displayHealth;
    }


    void Death()
    {
        // Set the death flag so this function won't be called again.
        isDead = true;

        // Turn off any remaining shooting effects.
      //  playerShooting.DisableEffects();

        // Tell the animator that the player is dead.
        //anim.SetTrigger("Die");

        // Set the audiosource to play the death clip and play it (this will stop the hurt sound from playing).
       // playerAudio.clip = deathClip;
      //  playerAudio.Play();

        // Turn off the movement and shooting scripts.
         //playerController.enabled = false;
      //  playerShooting.enabled = false;
    }
}