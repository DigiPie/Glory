using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public struct EnemySpawnInfo
    {
        public EnemySpawnInfo(int enemyType, int pathChoice, float spawnInterval)
        {
            this.enemyType = enemyType;
            this.pathChoice = pathChoice;
            this.spawnInterval = spawnInterval;
        }

        public int enemyType;
        public int pathChoice;
        public float spawnInterval; // The interval between this enemy spawn and the next's enemy spawn.
    }

    //private ObjectiveHealth objectiveHealth;
    public static GameManager instance = null;
    private ObjectiveHealth objHealth;
    private CameraController camController;
    public GameObject boomEffect, enemy1;

    // Spawning and pathing
    public Transform[] path1, path2;
    private EnemySpawnInfo[][] waves;
    private int currentWave = 0;
    public bool isGameDone = false;
    private bool isWaveFullySpawned = true;
    private bool isWaveCleared = true;
    private float waveInterval = 8.0f; // Delay between each wave
    private float nextWaveReadyTime = 8.0f;
    private int currentSpawn = 0; // Current minion to spawn for this wave
    private float spawnReadyTime;

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
        objHealth = GetComponent<ObjectiveHealth>();
        camController = GetComponent<CameraController>();
        enemies = new List<GameObject>();

        // Enemy spawn type and path
        EnemySpawnInfo harmlessLeft = new EnemySpawnInfo(0, 0, 1.5f); // Does not attack player, spawns at spawn point 0
        EnemySpawnInfo harmlessRight = new EnemySpawnInfo(0, 1, 0.8f); // Does not attack player, spawns at spawn point 0
        EnemySpawnInfo harmlessLeft2 = new EnemySpawnInfo(0, 0, 2.0f);
        EnemySpawnInfo harmlessRight2 = new EnemySpawnInfo(0, 1, 2.0f);

        // Number of waves
        waves = new EnemySpawnInfo[3][];

        // Wave 1
        waves[0] = new EnemySpawnInfo[4];
        waves[0][0] = harmlessLeft;
        waves[0][1] = harmlessLeft;
        waves[0][2] = harmlessLeft;
        waves[0][3] = harmlessLeft;

        // Wave 2
        waves[1] = new EnemySpawnInfo[8];
        waves[1][0] = harmlessRight;
        waves[1][1] = harmlessRight;
        waves[1][2] = harmlessRight;
        waves[1][3] = harmlessRight;
        waves[1][4] = harmlessRight;
        waves[1][5] = harmlessRight;
        waves[1][6] = harmlessRight;
        waves[1][7] = harmlessRight;

        // Wave 3
        waves[2] = new EnemySpawnInfo[12];
        waves[2][0] = harmlessLeft2;
        waves[2][1] = harmlessLeft2;
        waves[2][2] = harmlessRight2;
        waves[2][3] = harmlessRight2;
        waves[2][4] = harmlessLeft2;
        waves[2][5] = harmlessLeft2;
        waves[2][6] = harmlessRight2;
        waves[2][7] = harmlessLeft2;
        waves[2][8] = harmlessRight2;
        waves[2][9] = harmlessLeft2;
        waves[2][10] = harmlessRight2;
        waves[2][11] = harmlessLeft2;
    }

    // Update is called every frame
    void Update()
    {
        Debug.Log(currentWave + " " + currentSpawn);
        Spawn();
        ClearDead();
    }

    void Spawn()
    {
        if (isGameDone)
        {
            return;
        }

        if (isWaveCleared)
        {
            if (currentWave == waves.Length)
            {
                isGameDone = true;
                return;
            }

            if (Time.time > nextWaveReadyTime)
            {
                isWaveFullySpawned = false;
                isWaveCleared = false;
            }

            return;
        }

        if (isWaveFullySpawned)
        {
            if (enemies.Count == 0)
            {
                isWaveCleared = true;
                currentWave++;
                currentSpawn = 0;
                nextWaveReadyTime = Time.time + waveInterval;
            }

            return;
        }

        if (Time.time > spawnReadyTime)
        {
            if (enemies.Count < 40)
            {
                if (currentSpawn == waves[currentWave].Length - 1)
                {
                    isWaveFullySpawned = true;
                    return;
                }

                spawnReadyTime = Time.time + waves[currentWave][currentSpawn].spawnInterval;

                // Get enemy
                GameObject enemy = enemy1; // Default enemy type

                if (waves[currentWave][currentSpawn].enemyType == 1)
                {
                    //TODO enemy type 2
                }

                if (waves[currentWave][currentSpawn].pathChoice == 0)
                {
                    enemy = Instantiate(enemy, path1[0]);
                    enemy.GetComponent<EnemyController1>().Setup(this, path1);
                }
                else if (waves[currentWave][currentSpawn].pathChoice == 1)
                {
                    enemy = Instantiate(enemy, path2[0]);
                    enemy.GetComponent<EnemyController1>().Setup(this, path2);
                }

                enemy.GetComponent<SpriteRenderer>().sortingOrder = 20 + enemies.Count;
                enemies.Add(enemy);
                currentSpawn++;
            }
        }
    }

    void ClearDead()
    {
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
        camController.Shake(0.1f);
        objHealth.TakeDamage(damage);
    }
}
