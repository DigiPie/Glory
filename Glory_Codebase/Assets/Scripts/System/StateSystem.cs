using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateSystem : MonoBehaviour {
    public enum GameState { Menu, Paused, Tutorial, Wave, Lose, Win };
    /*
     * Menu: GameManager does not run, only main menu canvas is displayed
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
    public enum TutorialState { Intro1, Intro2, Walk, Jump, Attack, Done, Dash1, Dash2, Dash3,
                                FirstSpell1, FirstSpell2, FirstSpell3, SecondSpell1, SecondSpell2, SecondSpell3}

    private GameState gameState;
    private MenuState menuState;
    private WaveState waveState;
    private TutorialState tutorialState;

    private GameState beforePauseGameState;

    public HUD hud;
    public GameObject HudUI;
    public GameObject mainMenu;

    public volatile bool tutorialEnabled;

    // Use this for initialization
    public void Start () {
        Time.timeScale = 0f;
        tutorialEnabled = true;
        gameState = GameState.Menu;
        menuState = MenuState.Hidden;
        waveState = WaveState.WaitingNextWave;
        tutorialState = TutorialState.Intro1;
    }

    // Timescale
    public void Pause()
    {
        Time.timeScale = 0f;
        beforePauseGameState = GetGameState();
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
        SetGameState(GameState.Menu);
        
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
    }

    public void SetToBeforePauseGameState()
    {
        gameState = beforePauseGameState;
    }
    
    public bool IsGameMenu()
    {
        return gameState == GameState.Menu;
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
    
    public void EnterGame()
    {
        Unpause();
        HudUI.SetActive(true);
        if (tutorialEnabled)
        {
            gameState = GameState.Tutorial;
        }
        else
        {
            StartGameWave(); // Call this at end of tutorial
        }
    }

    public void StartGameWave()
    {
        // Order v important dont change
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

    public void EnableTutorial(bool check)
    {
        tutorialEnabled = check;
    }

    public void SetTutorialState(TutorialState newTutorialState)
    {
        this.tutorialState = newTutorialState;
    }

    public bool IsIntro1()
    {
        return tutorialState == TutorialState.Intro1;
    }

    public bool IsIntro2()
    {
        return tutorialState == TutorialState.Intro2;
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

    public bool IsDone()
    {
        return tutorialState == TutorialState.Done;
    }

    public bool IsDash1()
    {
        return tutorialState == TutorialState.Dash1;
    }

    public bool IsDash2()
    {
        return tutorialState == TutorialState.Dash2;
    }

    public bool IsDash3()
    {
        return tutorialState == TutorialState.Dash3;
    }

    public bool IsFirstSpell1()
    {
        return tutorialState == TutorialState.FirstSpell1;
    }

    public bool IsFirstSpell2()
    {
        return tutorialState == TutorialState.FirstSpell2;
    }

    public bool IsFirstSpell3()
    {
        return tutorialState == TutorialState.FirstSpell3;
    }

    public bool IsSecondSpell1()
    {
        return tutorialState == TutorialState.SecondSpell1;
    }

    public bool IsSecondSpell2()
    {
        return tutorialState == TutorialState.SecondSpell2;
    }

    public bool IsSecondSpell3()
    {
        return tutorialState == TutorialState.SecondSpell3;
    }
}
