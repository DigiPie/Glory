using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public StateSystem stateSystem;
    public GameObject mainMenu;
    public Overlay overlay;

    private void Start()
    {
        FindObjectOfType<AudioManager>().PlaySound("MenuBGM");
    }

    public void PlayGame()
    {
        mainMenu.SetActive(false);
        stateSystem.EnterGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Help()
    {
        overlay.ShowHelp();
    }

    public void Options()
    {
        overlay.ShowOptionsUI();
    }

    public void Credits()
    {
        overlay.ShowCredits();
    }

    public void PlaySound(string name)
    {
        FindObjectOfType<AudioManager>().PlaySound(name);
    }
}
