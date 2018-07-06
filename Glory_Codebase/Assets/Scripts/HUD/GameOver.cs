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
        if (FindObjectOfType<GameManager>().isGameDone)
        {
            GameOverScreenUI.SetActive(true);
            txtGameOver.text = "YOU WON!";
            Time.timeScale = 0f;
            hud.SetActive(false);
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
