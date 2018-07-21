using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {
    public StateSystem stateSystem;
    public GameObject mainMenu;
    public Overlay overlay;

    public void PlayGame()
    {
        mainMenu.SetActive(false);
        stateSystem.EnterGame();
        if (stateSystem.IsGameTutorial())
        {
            overlay.ShowTutorialUI();
        }
    }
    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
