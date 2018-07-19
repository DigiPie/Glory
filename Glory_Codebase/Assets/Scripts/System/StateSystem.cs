using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateSystem : MonoBehaviour {
    public enum GameState { Paused, Tutorial, Wave, Lose, Win };
    /*
     * Paused:  GameManager does not run, Overlay displays Pause menu
     * Tutorial: GameManager runs tutorial code (Line 48)
     * Wave: GameManager runs wave code (Line 43)
     * Lose/Win: GameManager does not run, Overlay displays game-over screen
     */
    public enum MenuState { Hidden, Main, Options };
    /*
     * Hidden: Main pause menu and sub-menu items hidden
     * Main: Main pause menu shown
     * Options: Options sub-menu shown, pause menu hidden
     */
    public enum WaveState { WaitingNextWave, WaitingWaveClear, WaitingWaveSpawn, Done };
    public enum TutorialState { Intro, Walk, Jump, Dash, Attack, Done }

    public bool skipTutorial = false;

    private GameState gameState;
    private MenuState menuState;
    private WaveState waveState;
    private TutorialState tutorialState;

    private GameState beforePauseGameState;

    public HUD hud;

    // Use this for initialization
    void Start () {
        Unpause();

        menuState = MenuState.Hidden;
        waveState = WaveState.WaitingNextWave;
        tutorialState = TutorialState.Intro;

        if (skipTutorial)
        {
            StartGameWave(); // Call this at end of tutorial
        }
        else
        {
            gameState = GameState.Tutorial;
        }
	}

    // Timescale
    public void Pause()
    {
        Time.timeScale = 0f;
        gameState = GameState.Paused;
    }

    public void Unpause()
    {
        Time.timeScale = 1f;
        SetToBeforePauseGameState();
    }

    // Scene transition
    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    // GameState
    public GameState GetGameState()
    {
        return gameState;
    }

    public void SetGameState(GameState gameState)
    {
        this.gameState = gameState;

        // Remember game state prior to pause, to be used when game is unpaused.
        if (gameState != GameState.Paused)
        {
            beforePauseGameState = gameState;
        }
    }

    public GameState GetBeforePauseGameState()
    {
        return beforePauseGameState;
    }

    public void SetToBeforePauseGameState()
    {
        gameState = beforePauseGameState;
    }

    public bool IsGamePaused()
    {
        return gameState == GameState.Paused;
    }

    public bool IsGameTutorial()
    {
        return gameState == GameState.Tutorial;
    }

    public bool IsGameWave()
    {
        return gameState == GameState.Wave;
    }

    public bool IsGameWin()
    {
        return gameState == GameState.Win;
    }

    public bool IsGameLose()
    {
        return gameState == GameState.Lose;
    }

    public void StartGameWave()
    {
        hud.ShowNextWaveBtn();
        gameState = GameState.Wave;
        waveState = WaveState.WaitingNextWave;
    }

    // Menu State
    public MenuState GetMenuState()
    {
        return menuState;
    }

    public void SetMenuState(MenuState menuState)
    {
        this.menuState = menuState;
    }

    public bool IsMenuHidden()
    {
        return menuState == MenuState.Hidden;
    }

    public bool IsMenuMain()
    {
        return menuState == MenuState.Main;
    }

    public bool IsMenuOptions()
    {
        return menuState == MenuState.Options;
    }

    // Wave State
    public WaveState GetWaveState()
    {
        return waveState;
    }

    public void SetWaveState(WaveState waveState)
    {
        this.waveState = waveState;
    }

    public bool IsWaitingNextWave()
    {
        return waveState == WaveState.WaitingNextWave;
    }

    public bool IsWaitingWaveClear()
    {
        return waveState == WaveState.WaitingWaveClear;
    }

    public bool IsWaitingWaveSpawn()
    {
        return waveState == WaveState.WaitingWaveSpawn;
    }

    public bool IsWaveDone()
    {
        return waveState == WaveState.Done;
    }
    // Tutorial State
    public TutorialState GetTutorialState()
    {
        return tutorialState;
    }

    public void SetTutorialState(TutorialState newTutorialState)
    {
        Debug.Log(newTutorialState);
        this.tutorialState = newTutorialState;
    }

    public bool IsIntro()
    {
        return tutorialState == TutorialState.Intro;
    }

    public bool IsWalk()
    {
        return tutorialState == TutorialState.Walk;
    }

    public bool IsJump()
    {
        return tutorialState == TutorialState.Jump;
    }

    public bool IsAttack()
    {
        return tutorialState == TutorialState.Attack;
    }

    public bool IsDash()
    {
        return tutorialState == TutorialState.Dash;
    }

    public bool IsDone()
    {
        return tutorialState == TutorialState.Done;
    }
}
