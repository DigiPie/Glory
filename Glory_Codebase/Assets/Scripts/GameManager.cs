using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance = null;

    public Transform spawnPoint1, spawnPoint2;
    public GameObject enemy1;

    private List<GameObject> enemies;
    private bool hasDeadEnemy;
    private bool spawnOnLeft = false;

    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
        {
            // If not, assign to this
            instance = this;
        }
        else if (instance != this)
        {
            // Then destroy this to enforce singleton pattern
            Destroy(gameObject);
        }

        // Do not destroy this when reloading scene
        DontDestroyOnLoad(gameObject);

        // Start game
        InitGame();
    }

    // Initializes the game for each level.
    void InitGame()
    {
        enemies = new List<GameObject>();
    }

    //Update is called every frame.
    void Update()
    {
        if (enemies.Count == 0)
        {
            if (spawnOnLeft)
            {
                enemies.Add(Instantiate(enemy1, spawnPoint1));
            }
            else
            {
                enemies.Add(Instantiate(enemy1, spawnPoint2));
            }

            spawnOnLeft = !spawnOnLeft;
        }

        hasDeadEnemy = false;

        // Runs at least once, terminates if there is no dead enemies in the list
        do
        {
            foreach (GameObject enemy in enemies)
            {
                if (enemy.GetComponent<HealthSystem>().IsDead())
                {
                    enemies.Remove(enemy);
                    Destroy(enemy);
                    hasDeadEnemy = true;
                    return;
                }
            }
        } while (hasDeadEnemy);
    }
}
