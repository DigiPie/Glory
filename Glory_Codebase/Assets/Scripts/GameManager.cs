using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    // Game State
    private bool isGameRunning = false;

    //private ObjectiveHealth objectiveHealth;
    public static GameManager instance = null;

    public CustomCamController camController;
    public ObjectiveHealth objHealth;
    public PlayerHealthSystem plyHealth;
    public GameObject boomEffect, enemy1, enemy2, player1;

    // Spawning and pathing
    public Transform[] path1, path2;
    private EnemySpawnInfo[][] waves;
    public int currentWave = 0;
    public bool isGameDone = false;
    private bool isWaveFullySpawned = true;
    private bool isWaveCleared = true;
    private float waveInterval = 5.0f; // Delay between each wave
    private float nextWaveReadyTime = 5.0f;
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
        //DontDestroyOnLoad(gameObject);

        // Start game
        InitGame();
    }

    public void RunGame(bool isGameRunning)
    {
        this.isGameRunning = isGameRunning;
    }

    public void ExitGame()
    {
        isGameRunning = false;
    }


    // Initializes the game for each level.
    void InitGame()
    {
        enemies = new List<GameObject>();

        // Enemy spawn type and path
        // Fox - Does not attack player
        EnemySpawnInfo foxL0 = new EnemySpawnInfo(0, 0, 0);
        EnemySpawnInfo foxL0_5 = new EnemySpawnInfo(0, 0, 0.5f);
        EnemySpawnInfo foxL1_5 = new EnemySpawnInfo(0, 0, 1.5f);
        EnemySpawnInfo foxL5 = new EnemySpawnInfo(0, 0, 5.0f);

        EnemySpawnInfo foxR0 = new EnemySpawnInfo(0, 1, 0);
        EnemySpawnInfo foxR0_5 = new EnemySpawnInfo(0, 1, 0.5f);
        EnemySpawnInfo foxR1_5 = new EnemySpawnInfo(0, 1, 1.5f);
        EnemySpawnInfo foxR5 = new EnemySpawnInfo(0, 1, 5.0f);

        // Fox - Does not attack player
        EnemySpawnInfo skeL0 = new EnemySpawnInfo(1, 0, 0);
        EnemySpawnInfo skeL1 = new EnemySpawnInfo(1, 0, 1.0f);
        EnemySpawnInfo skeL3 = new EnemySpawnInfo(1, 0, 3.0f);

        EnemySpawnInfo skeR0 = new EnemySpawnInfo(1, 1, 0);
        EnemySpawnInfo skeR1 = new EnemySpawnInfo(1, 1, 1.0f);
        EnemySpawnInfo skeR3 = new EnemySpawnInfo(1, 1, 3.0f);

        // Number of waves
        waves = new EnemySpawnInfo[5][];

        /*waves[0] = new EnemySpawnInfo[4];
        waves[0][0] = harmlessLeft;
        waves[0][1] = harmfulRight;
        waves[0][2] = harmfulLeft;
        waves[0][3] = harmlessRight;*/

        // Wave 1
        waves[0] = new EnemySpawnInfo[10];
        waves[0][0] = foxL1_5;
        waves[0][1] = foxL1_5;
        waves[0][2] = foxR1_5;
        waves[0][3] = foxR5;
        waves[0][4] = foxL0;
        waves[0][5] = foxR5;
        waves[0][6] = foxL0;
        waves[0][7] = foxR1_5;
        waves[0][8] = foxL0;
        waves[0][9] = foxR0;

        // Wave 2
        waves[1] = new EnemySpawnInfo[16];
        waves[1][0] = foxL0_5;
        waves[1][1] = foxL0_5;
        waves[1][2] = foxL0_5;
        waves[1][3] = foxL5;
        waves[1][4] = foxR0_5;
        waves[1][5] = foxR0_5;
        waves[1][6] = foxR0_5;
        waves[1][7] = foxR5;

        waves[1][8] = foxL0;
        waves[1][9] = foxR0;
        waves[1][10] = foxL0;
        waves[1][11] = foxR0_5;
        waves[1][12] = foxL0;
        waves[1][13] = foxR0_5;
        waves[1][14] = foxL0;
        waves[1][15] = foxL5;

        // Wave 3
        waves[2] = new EnemySpawnInfo[20];
        waves[2][0] = foxL0_5;
        waves[2][1] = foxL0_5;
        waves[2][2] = foxL1_5;
        waves[2][3] = foxR0_5;
        waves[2][4] = foxR0_5;
        waves[2][5] = foxR5;

        waves[2][6] = foxL0;
        waves[2][7] = foxR0_5;
        waves[2][8] = foxL0;
        waves[2][9] = foxR5;

        waves[2][7] = foxL0;
        waves[2][8] = foxR0_5;
        waves[2][9] = foxL0;
        waves[2][10] = foxR0_5;
        waves[2][11] = foxL0;
        waves[2][12] = foxR0_5;
        waves[2][13] = foxL0;
        waves[2][14] = foxR0_5;
        waves[2][15] = foxL0;
        waves[2][16] = foxR0_5;
        waves[2][17] = foxL0;
        waves[2][18] = foxR0_5;
        waves[2][19] = foxL5;

        // Wave 4
        waves[3] = new EnemySpawnInfo[12];
        waves[3][0] = foxL0_5;
        waves[3][1] = foxL0_5;
        waves[3][2] = foxL0_5;
        waves[3][3] = foxL1_5;
        waves[3][4] = foxR0_5;
        waves[3][5] = foxR0_5;
        waves[3][6] = foxR0_5;
        waves[3][7] = foxR5;

        waves[3][8] = skeL3;
        waves[3][9] = skeL3;
        waves[3][10] = skeL0;
        waves[3][11] = skeR0;

        // Wave 5
        waves[4] = new EnemySpawnInfo[12];
        waves[4][0] = skeL1;
        waves[4][1] = skeL3;

        waves[4][2] = foxR0_5;
        waves[4][3] = foxR0_5;
        waves[4][4] = foxR0_5;
        waves[4][5] = foxR5;

        waves[4][6] = skeL1;
        waves[4][7] = foxL0_5;
        waves[4][8] = foxL0_5;
        waves[4][9] = foxL5;

        waves[4][10] = skeL0;
        waves[4][11] = skeR0;
    }

    // Update is called every frame
    void Update()
    {
        /*if (!isGameRunning)
        {
            return;
        }*/

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

            if (Time.timeSinceLevelLoad > nextWaveReadyTime)
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
                nextWaveReadyTime = Time.timeSinceLevelLoad + waveInterval;
            }

            return;
        }

        if (Time.timeSinceLevelLoad > spawnReadyTime)
        {
            if (enemies.Count < 40)
            {
                if (currentSpawn == waves[currentWave].Length - 1)
                {
                    isWaveFullySpawned = true;
                    return;
                }

                spawnReadyTime = Time.timeSinceLevelLoad + waves[currentWave][currentSpawn].spawnInterval;

                // Get enemy
                GameObject enemy = enemy1; // Default enemy type

                if (waves[currentWave][currentSpawn].enemyType == 1)
                {
                    enemy = enemy2;
                }

                if (waves[currentWave][currentSpawn].pathChoice == 0)
                {
                    enemy = Instantiate(enemy, path1[0]);
                    enemy.GetComponent<EnemyController>().Setup(this, path1);
                }
                else if (waves[currentWave][currentSpawn].pathChoice == 1)
                {
                    enemy = Instantiate(enemy, path2[0]);
                    enemy.GetComponent<EnemyController>().Setup(this, path2);
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
                    camController.Shake(0.01f);
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

    public void DamageObjective(float damage)
    {
        DamageObjective((int)damage);
    }


    public void DamagePlayer(int damage)
    {
        camController.Shake(0.2f);
        plyHealth.TakeDamage(damage);
    }

    public void DamagePlayer(float damage)
    {
        DamagePlayer((int) damage);
    }

    public Transform GetPlayerPosition()
    {
        return player1.transform;
    }
}
