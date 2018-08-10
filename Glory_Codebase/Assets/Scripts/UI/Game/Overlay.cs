using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Overlay : MonoBehaviour {
    public StateSystem stateSystem;
    public GameObject hudUI;
    public GameObject gameOverUI;
    public TextMeshProUGUI txtGameOver;

    public GameObject OverlayCanvas;
    public GameObject pauseMenuUI;
    public GameObject optionsUI;
    public GameObject tutorialUI;
    public GameObject mainMenuUI;
    public GameObject creditsUI;

    // Update is called once per frame
    void Update () {
        if (Input.GetButtonDown("Exit"))
        {
            if (stateSystem.IsMenuHidden())
            {
                // Show main pause menu
                ShowPauseUI();
            }
            else if (stateSystem.IsMenuMain())
            {
                // Hide main pause menu and everything else, show HUD
                ShowUnpauseUI();
            }
            else if (stateSystem.IsMenuOptions())
            {
                // Hide options menu, show main pause menu
                ShowPauseUI();
            }
        }
    }

    // Hide all pause menu and sub-menu items, show HUD
    public void ShowUnpauseUI()
    {
        // Unpause game and set game state to game state prior to pause (i.e. Tutorial/Wave)
        stateSystem.Unpause();

        // Update state
        stateSystem.SetMenuState(StateSystem.MenuState.Hidden);

        pauseMenuUI.SetActive(false);
        optionsUI.SetActive(false);

        hudUI.SetActive(true);
    }

    // Hide HUD and sub-menu items, show main pause menu
    public void ShowPauseUI()
    {
        if (!stateSystem.IsGameMenu())
        {
            // Pause game
            stateSystem.Pause();

            // Update state
            stateSystem.SetMenuState(StateSystem.MenuState.Main);

            hudUI.SetActive(false);
            optionsUI.SetActive(false);
            pauseMenuUI.SetActive(true);
        }

        else
        {
            optionsUI.SetActive(false);
        }
    }

    // Hide HUD, pause menu and sub-menu items, show only options menu
    public void ShowOptionsUI()
    {
        // Update state
        stateSystem.SetMenuState(StateSystem.MenuState.Options);
        if (stateSystem.IsGameMenu())
        {
            pauseMenuUI.SetActive(false);
            OverlayCanvas.SetActive(true);
            optionsUI.SetActive(true);
        }
        else {
            hudUI.SetActive(false);
            pauseMenuUI.SetActive(false);
            OverlayCanvas.SetActive(true);
            optionsUI.SetActive(true);
        }
    }

    public void ShowTutorialUI()
    {
        stateSystem.SetGameState(StateSystem.GameState.Tutorial);
        OverlayCanvas.SetActive(true);
        tutorialUI.SetActive(true);
    }
    
    // Hide HUD, pause menu and sub-menu items, show only game over screen
    public void ShowGameoverUI(bool isWin)
    {
        hudUI.SetActive(false);
        pauseMenuUI.SetActive(false);
        optionsUI.SetActive(false);
        gameOverUI.SetActive(true);
        stateSystem.Pause();
        txtGameOver.text = (isWin) ? "VICTORY!" : "DEFEAT!";
    }

    public void ShowCredits()
    {
        creditsUI.SetActive(true);
        // To let credits scroll
        Time.timeScale = 1f;

    }

    public void HideCredits()
    {
        creditsUI.SetActive(false);
        Time.timeScale = 0f;
    }

    // Trigger exit to menu
    public void LoadMenu()
    {
        SceneManager.LoadScene("GameScene");
    }

    // Trigger exit game
    public void ExitGame()
    {
        stateSystem.ExitGame();
    }
}
