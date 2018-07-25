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
        Debug.Log("QUIT!");
        Application.Quit();
    }

    public void PlaySound(string name)
    {
        FindObjectOfType<AudioManager>().PlaySound(name);
    }
}
