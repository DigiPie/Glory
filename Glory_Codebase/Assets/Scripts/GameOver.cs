using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOver : MonoBehaviour {

    public GameObject GameOverScreenUI;
    public TextMeshProUGUI txtGameOver;
    public GameManager gameManager;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (FindObjectOfType<ObjectiveHealth>().isDestroyed)
        {
            GameOverScreenUI.SetActive(true);
            txtGameOver.text = "GAME OVER!";
            Time.timeScale = 0f;
        }
        if (FindObjectOfType<GameManager>().isGameDone)
        {
            GameOverScreenUI.SetActive(true);
            txtGameOver.text = "YOU WON!";
            Time.timeScale = 0f;
        }

    }
    public void LoadMenu()
    {
        gameManager.ExitGame();
        SceneManager.LoadScene("Menu");
    }
    public void ExitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
