using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance = null;

    public Transform[] path1, path2;
    public GameObject enemy1, enemy2, objective;

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
            GameObject enemy;
            if (spawnOnLeft)
            {
                enemy = Instantiate(enemy1, path1[0]);
                enemy.GetComponent<EnemyController1>().Setup(path1);
            }
            else
            {
                enemy = Instantiate(enemy1, path2[0]);
                enemy.GetComponent<EnemyController1>().Setup(path2);
            }

            enemies.Add(enemy);
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
