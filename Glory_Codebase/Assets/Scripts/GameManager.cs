using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private PlayerHealthSystem playerHealthSystem;
    public static GameManager instance = null;

    public Transform[] path1, path2;
    public GameObject boomEffect, enemy1, enemy2;
    
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
        playerHealthSystem = GetComponent<PlayerHealthSystem>();
        enemies = new List<GameObject>();

    }

    //Update is called every frame.
    void Update()
    {
        if (enemies.Count < 2)
        {
            GameObject enemy;
            if (spawnOnLeft)
            {
                enemy = Instantiate(enemy1, path1[0]);
                enemy.GetComponent<EnemyController1>().Setup(this, path1);
                enemy.GetComponent<SpriteRenderer>().sortingOrder = 20 + enemies.Count;
            }
            else
            {
                enemy = Instantiate(enemy1, path2[0]);
                enemy.GetComponent<EnemyController1>().Setup(this, path2);
                enemy.GetComponent<SpriteRenderer>().sortingOrder = 20 + enemies.Count;
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
                if (enemy.GetComponent<EnemyHealthSystem>().IsDead())
                {
                    Instantiate(boomEffect, enemy.transform.position, enemy.transform.rotation);
                    enemies.Remove(enemy);
                    Destroy(enemy);
                    hasDeadEnemy = true;
                    return;
                }
            }
        } while (hasDeadEnemy);
    }

    public void DamageObjective(int damage)
    {
        playerHealthSystem.TakeDamage(damage);
    }
}
