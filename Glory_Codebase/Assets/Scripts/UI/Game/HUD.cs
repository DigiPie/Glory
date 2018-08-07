using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour {

    public GameManager gameManager;
    public StateSystem stateSystem;
    public WaveSystem waveSystem;
    public PlayerActionSystem playerActionSystem;
    public GameObject popUpHUD;
    public Image objectiveRedFlash;                                   // Reference to an image to flash on the screen on being hurt.
    public Image playerRedFlash;
    public Image bossRedFlash;
    private bool isObjectiveRedFlash = false;
    private bool isPlayerRedFlash = false;
    private bool isBossRedFlash = false;

    public TextMeshProUGUI txtInfo;
    public TextMeshProUGUI txtNextWave;
    public Slider objHealthSlider;
    public Slider healthSlider;
    public GameObject bossSliderObj;
    public GameObject slideSliderObj;
    public GameObject spell1SliderObj;
    public GameObject spell2SliderObj;
    private Slider bossSlider;
    private Slider slideSlider;
    private Slider spell1Slider;
    private Slider spell2Slider;

    public float flashSpeed = 2.0f;                               // The speed the objectiveRedFlash will fade at.
    private bool inputNext;
    private bool allowInputNext = true;

    private void Awake()
    {
        txtInfo.text = "";
        bossSliderObj.SetActive(false);
        slideSliderObj.SetActive(false);
        spell1SliderObj.SetActive(false);
        spell2SliderObj.SetActive(false);
        bossSlider = bossSliderObj.GetComponent<Slider>();
        slideSlider = slideSliderObj.GetComponent<Slider>();
        spell1Slider = spell1SliderObj.GetComponent<Slider>();
        spell2Slider = spell2SliderObj.GetComponent<Slider>();
        slideSlider.value = 0;
        spell1Slider.value = 0;
        spell2Slider.value = 0;
    }
    // Update is called in-step with the physics engine
    void FixedUpdate() {
        txtInfo.text = gameManager.GetInfo();

        HandleCooldown();
        HandleFlash();

        if (stateSystem.IsGameWave())
        {
            inputNext = Input.GetButtonDown("Submit");

            if (inputNext)
            {
                OnClickNextWaveBtn();
            }
        }
    }

    public void HandleCooldown()
    {
        if (slideSlider.value != 0)
        {
            slideSlider.value -= (slideSlider.maxValue * Time.deltaTime) / playerActionSystem.slideCooldown;
        }

        if (spell1Slider.value != 0)
        {
            spell1Slider.value -= (spell1Slider.maxValue * Time.deltaTime) / playerActionSystem.spell1Cooldown;
        }

        if (spell2Slider.value != 0)
        {
            spell2Slider.value -= (spell2Slider.maxValue * Time.deltaTime) / playerActionSystem.spell2Cooldown;
        }
    }

    public void HandleFlash()
    {
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
    }

    public void ResetAllCooldownIndicators()
    {
        spell1Slider.value = 0;
        spell2Slider.value = 0;
    }

    public void RedFlash()
    {
        playerRedFlash.color = Color.white;
        isPlayerRedFlash = true;
    }

    public void UpdatePlayerHealth(int health)
    {
        healthSlider.value = health;
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
        bossSliderObj.SetActive(true);
        bossSlider.maxValue = maxHealth;
        bossSlider.value = maxHealth;
    }

    public void HideBossHealth()
    {
        bossSliderObj.SetActive(true);
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
        else if ((waveSystem.IsDashUnlockWave()) && (stateSystem.GetTutorialState() != StateSystem.TutorialState.Dash3))
        {
            stateSystem.SetTutorialState(StateSystem.TutorialState.Dash1);
            stateSystem.SetGameState(StateSystem.GameState.Tutorial);
        }
        else if ((waveSystem.IsSpell1UnlockWave()) && (stateSystem.GetTutorialState() != StateSystem.TutorialState.FirstSpell3))
        {
            stateSystem.SetTutorialState(StateSystem.TutorialState.FirstSpell1);
            stateSystem.SetGameState(StateSystem.GameState.Tutorial);
        }
        else if ((waveSystem.IsSpell2UnlockWave()) && (stateSystem.GetTutorialState() != StateSystem.TutorialState.SecondSpell3))
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

    public void ShowDashCooldownIndicator()
    {
        slideSliderObj.SetActive(true);
    }

    public void ShowSpell1CooldownIndicator()
    {
        spell1SliderObj.SetActive(true);
    }

    public void ShowSpell2CooldownIndicator()
    {
        spell2SliderObj.SetActive(true);
    }

    public void StartSlideCooldownAnim()
    {
        slideSlider.value = slideSlider.maxValue;
    }

    public void StartSpell1CooldownAnim()
    {
        spell1Slider.value = spell1Slider.maxValue;
    }

    public void StartSpell2CooldownAnim()
    {
        spell2Slider.value = spell2Slider.maxValue;
    }
}
