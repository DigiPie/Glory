using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour {

    public GameManager gameManager;
    public StateSystem stateSystem;
    public WaveSystem waveSystem;
    public GameObject popUpHUD;
    public Image objectiveRedFlash;                                   // Reference to an image to flash on the screen on being hurt.
    public Image playerRedFlash;
    public Image bossRedFlash;
    private bool isObjectiveRedFlash = false;
    private bool isPlayerRedFlash = false;
    private bool isBossRedFlash = false;

    public TextMeshProUGUI txtInfo;
    public TextMeshProUGUI txtNextWave;
    public Slider objHealthSlider;                              // Reference to the UI's health bar.
    public Slider healthSlider;                              // Reference to the UI's health bar.
    public Slider bossSlider;                              // Reference to the UI's health bar.

    public int dashWave, spell1Wave, spell2Wave;                // Wave for tutorial to reappear
    public float flashSpeed = 2.0f;                               // The speed the objectiveRedFlash will fade at.
    private bool inputNext;
    private bool allowInputNext = true;

    private void Awake()
    {
        txtInfo.text = "";
    }
    // Update is called in-step with the physics engine
    void FixedUpdate() {
        if (stateSystem.IsGameWave())
        {
            // Hot Fix
            txtInfo.text = gameManager.GetInfo() + 1;

            if (isPlayerRedFlash)
            {
                playerRedFlash.color = Color.Lerp(playerRedFlash.color, Color.clear, flashSpeed * Time.deltaTime);

                if (playerRedFlash.color.a < 0.1f)
                {
                    playerRedFlash.color = Color.clear;
                    isPlayerRedFlash = false;
                }
            }

            if (isObjectiveRedFlash)
            {
                objectiveRedFlash.color = Color.Lerp(objectiveRedFlash.color, Color.clear, flashSpeed * Time.deltaTime);

                if (objectiveRedFlash.color.a < 0.1f)
                {
                    objectiveRedFlash.color = Color.clear;
                    isObjectiveRedFlash = false;
                }
            }

            if (isBossRedFlash)
            {
                bossRedFlash.color = Color.Lerp(bossRedFlash.color, Color.clear, flashSpeed * Time.deltaTime);

                if (bossRedFlash.color.a < 0.1f)
                {
                    bossRedFlash.color = Color.clear;
                    isBossRedFlash = false;
                }
            }

            inputNext = Input.GetButtonDown("Submit");

            if (inputNext)
            {
                OnClickNextWaveBtn();
            }
        }
    }

    public void UpdatePlayerHealth(int health)
    {
        healthSlider.value = health;
        playerRedFlash.color = Color.white;
        isPlayerRedFlash = true;
    }

    public void UpdatePlayerHealth(float health)
    {
        UpdatePlayerHealth((int)health);
    }

    public void UpdateObjectiveHealth(int health)
    {
        objHealthSlider.value = health;
        objectiveRedFlash.color = Color.white;
        isObjectiveRedFlash = true;
    }

    public void UpdateObjectiveHealth(float health)
    {
        UpdateObjectiveHealth((int)health);
    }

    public void ShowBossHealth(int maxHealth)
    {
        bossSlider.gameObject.active = true;
        bossSlider.maxValue = maxHealth;
        bossSlider.value = maxHealth;
    }

    public void HideBossHealth()
    {
        bossSlider.gameObject.active = false;
    }

    public void UpdateBossHealth(int health)
    {
        bossSlider.value = health;
        bossRedFlash.color = Color.white;
        isBossRedFlash = true;
    }

    public void UpdateBossHealth(float health)
    {
        UpdateBossHealth((int)health);
    }

    public void ShowNextWaveBtn()
    {
        if (stateSystem.tutorialEnabled == true && (stateSystem.GetTutorialState() == StateSystem.TutorialState.Intro1))
        {
            stateSystem.SetGameState(StateSystem.GameState.Tutorial);
        }
        else if ((waveSystem.GetDisplayWave() == dashWave) && (stateSystem.GetTutorialState() != StateSystem.TutorialState.Dash3))
        {
            stateSystem.SetTutorialState(StateSystem.TutorialState.Dash1);
            stateSystem.SetGameState(StateSystem.GameState.Tutorial);
        }
        else if ((waveSystem.GetDisplayWave() == spell1Wave) && (stateSystem.GetTutorialState() != StateSystem.TutorialState.FirstSpell3))
        {
            stateSystem.SetTutorialState(StateSystem.TutorialState.FirstSpell1);
            stateSystem.SetGameState(StateSystem.GameState.Tutorial);
        }
        else if ((waveSystem.GetDisplayWave() == spell2Wave) && (stateSystem.GetTutorialState() != StateSystem.TutorialState.SecondSpell3))
        {
            stateSystem.SetTutorialState(StateSystem.TutorialState.SecondSpell1);
            stateSystem.SetGameState(StateSystem.GameState.Tutorial);
        }
        else
        {
            allowInputNext = true;
            popUpHUD.SetActive(true);
            txtNextWave.text = gameManager.GetNextWaveInfo();
        }
    }

    public void OnClickNextWaveBtn()
    {
        
        if (allowInputNext)
        {
            allowInputNext = false;
            popUpHUD.SetActive(false);
            gameManager.StartNextWave();
        }
    }
}
