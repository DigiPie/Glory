using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour {

    public GameManager gameManager;
    public PlayerHealthSystem playerHealthSystem;
    public ObjectiveHealth objectiveHealth;
    public GameObject popUpHUD;
    public Image damageImage;                                   // Reference to an image to flash on the screen on being hurt.
    public Image playerDamageImage;

    public TextMeshProUGUI txtWaveNumber;
    public TextMeshProUGUI txtEnemiesLeft;
    public Slider objHealthSlider;                              // Reference to the UI's health bar.
    public Slider healthSlider;                              // Reference to the UI's health bar.


    public float flashSpeed = 5f;                               // The speed the damageImage will fade at.
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);     // The colour the damageImage is set to, to flash.
    public Color flashColour2 = new Color(1f, 0f, 0f, 0.5f);    // PlayerDamageImage

    // Use this for initialization
    void Start () {

	}

    // Update is called once per frame
    void Update() {
        objHealthSlider.value = objectiveHealth.getCurrentHealth();
        healthSlider.value = playerHealthSystem.getCurrentHealth();
        txtEnemiesLeft.text = "Monsters: " + gameManager.GetEnemyCount().ToString();
        txtWaveNumber.text = "Wave: " + gameManager.GetWave();

        if (objectiveHealth.damaged)
        {
            damageImage.color = flashColour;
        }
        else
        {
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }

        if (playerHealthSystem.damaged)
        {
            playerDamageImage.color = flashColour2;
        }
        else
        {
            playerDamageImage.color = Color.Lerp(playerDamageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
    }

    public void waveTransition()
    {
        popUpHUD.SetActive(true);
    }
}
