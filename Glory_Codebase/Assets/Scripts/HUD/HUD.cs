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

    public TextMeshProUGUI txtInfo;
    public TextMeshProUGUI txtNextWave;
    public Slider objHealthSlider;                              // Reference to the UI's health bar.
    public Slider healthSlider;                              // Reference to the UI's health bar.


    public float flashSpeed = 5f;                               // The speed the damageImage will fade at.
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);     // The colour the damageImage is set to, to flash.
    public Color flashColour2 = new Color(1f, 0f, 0f, 0.5f);    // PlayerDamageImage

    private bool inputNext;
    private bool allowInputNext = true;

    // Use this for initialization
    void Start () {

	}

    // Update is called in-step with the physics engine
    void FixedUpdate() {
        objHealthSlider.value = objectiveHealth.getCurrentHealth();
        healthSlider.value = playerHealthSystem.getCurrentHealth();
        txtInfo.text = gameManager.GetInfo();

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

        inputNext = Input.GetButton("Submit");

        if (inputNext)
        {
            OnClickNextWaveBtn();
        }
    }


    public void ShowNextWaveBtn()
    {
        allowInputNext = true;
        popUpHUD.SetActive(true);
        txtNextWave.text = gameManager.GetNextWaveInfo();

    }

    public void OnClickNextWaveBtn()
    {
        if (allowInputNext)
        {
            allowInputNext = false;
            Debug.Log("Click");
            popUpHUD.SetActive(false);
            gameManager.StartNextWave();
        }
    }
}
