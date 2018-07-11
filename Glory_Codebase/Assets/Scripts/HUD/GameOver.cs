using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;
using TMPro;

public class GameOver : MonoBehaviour {

    public GameObject GameOverScreenUI;
    public TextMeshProUGUI txtGameOver;
    public GameManager gameManager;
    public GameObject hud;

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
            hud.SetActive(false);
        }
        if (FindObjectOfType<PlayerHealthSystem>().isDead)
        {
            GameOverScreenUI.SetActive(true);
            txtGameOver.text = "GAME OVER!";
            Time.timeScale = 0f;
            hud.SetActive(false);
        }
        if (FindObjectOfType<GameManager>().gameState == GameManager.GameState.GameDone)
        {
            GameOverScreenUI.SetActive(true);
            txtGameOver.text = "YOU WIN!";
            Time.timeScale = 0f;
            hud.SetActive(false);
        }
        
    }
    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void ExitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

    // GameOver Function Version
    public void GameOverCall(int state)
    {
        // State == 0 ? Players all dead : Objective dead
        if (state == 0)
        {
            GameOverScreenUI.SetActive(true);
            txtGameOver.text = "GAME OVER!";
            Time.timeScale = 0f;
            hud.SetActive(false);
        }

        // State == 1 : Game Completed
        if (state == 1)
        {
            GameOverScreenUI.SetActive(true);
            txtGameOver.text = "YOU WIN!";
            Time.timeScale = 0f;
            hud.SetActive(false);
        }
    }
}
