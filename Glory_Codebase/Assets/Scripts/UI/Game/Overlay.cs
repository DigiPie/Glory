using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Overlay : MonoBehaviour {
    public StateSystem stateSystem;
    public GameObject hudUI;
    public GameObject gameOverUI;
    public TextMeshProUGUI txtGameOver;

    public GameObject pauseMenuUI;
    public GameObject optionsUI;

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
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
        // Pause game
        stateSystem.Pause();

        // Update state
        stateSystem.SetMenuState(StateSystem.MenuState.Main);

        hudUI.SetActive(false);
        optionsUI.SetActive(false);

        pauseMenuUI.SetActive(true);
    }

    // Hide HUD, pause menu and sub-menu items, show only options menu
    public void ShowOptionsUI()
    {
        // Update state
        stateSystem.SetMenuState(StateSystem.MenuState.Options);

        hudUI.SetActive(false);
        pauseMenuUI.SetActive(false);

        optionsUI.SetActive(true);
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

    // Trigger exit to menu
    public void LoadMenu()
    {
        stateSystem.LoadMenu();
    }

    // Trigger exit game
    public void ExitGame()
    {
        stateSystem.ExitGame();
    }
}
