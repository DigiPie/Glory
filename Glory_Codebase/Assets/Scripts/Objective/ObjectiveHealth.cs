using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObjectiveHealth : MonoBehaviour
{
    public int startingHealth = 200;                            // The amount of health the objective starts the game with.
    public int currentHealth;                                   // The current health the player has.
    // The health that is displayed
    //public Slider objHealthSlider;                              // Reference to the UI's health bar.
    // public Image damageImage;                                   // Reference to an image to flash on the screen on being hurt.
    // public AudioClip deathClip;                                 // The audio clip to play when the player dies.
    // public float flashSpeed = 5f;                               // The speed the damageImage will fade at.
    // public Color flashColour = new Color(1f, 0f, 0f, 0.1f);     // The colour the damageImage is set to, to flash.

    // Animator anim;                                              // Reference to the Animator component.
    // AudioSource playerAudio;                                    // Reference to the AudioSource component.
    public bool isDestroyed;                                       // Whether the objective is destroyed.
    public bool damaged;                                                  // True when the objective gets damaged.


    void Awake()
    {
        // Setting up the references.
        // anim = GetComponent();
        // playerAudio = GetComponent();
        // playerMovement = GetComponent();
        // playerShooting = GetComponentInChildren();

        // Set the initial health of the objective.

        currentHealth = startingHealth;
    }


    void Update()
    {
        /*
        // If the player has just been damaged...
        if (damaged)
        {
            // ... set the colour of the damageImage to the flash colour.
            damageImage.color = flashColour;
        }
        // Otherwise...
        else
        {
            // ... transition the colour back to clear.
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        */
        // Reset the damaged flag.
        damaged = false;
    }

    // Self implemented function for future use
    public void HealDamage(int amount)
    {
        if (currentHealth <= 0)
        {
            isDestroyed = true; // Prevents healing when already dead
        }
        else if ((currentHealth + amount) > startingHealth) // Prevents overheal
        {
            currentHealth = startingHealth; 
        }
        else
        {
            currentHealth += amount;
        }
        // objHealthSlider.value = currentHealth;
    }


    public void TakeDamage(int amount)
    {
        // Set the damaged flag so the screen will flash.
        damaged = true;

        // Reduce the current health by the damage amount.
        currentHealth -= amount;

        // Set the health bar's value to the current health.
        // objHealthSlider.value = currentHealth;

        // Play the hurt sound effect.
        // playerAudio.Play();

        // If the player has lost all it's health and the death flag hasn't been set yet...
        if (currentHealth <= 0 && !isDestroyed)
        {
            // ... it should die.
            Destroyed();
        }
    }
    
    public int getCurrentHealth()
    {
        return currentHealth;
    }


    void Destroyed()
    {
        // Set the death flag so this function won't be called again.
        isDestroyed = true;

        // Turn off any remaining shooting effects.

        // Tell the animator that the player is dead.
        // anim.SetTrigger("Die");

        // Set the audiosource to play the death clip and play it (this will stop the hurt sound from playing).
        // playerAudio.clip = deathClip;
        // playerAudio.Play();

        // Turn off the movement and shooting scripts.
        // playerController.enabled = false;
    }
}