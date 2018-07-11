using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    // Game State
    public enum GameState { GameDone, WaitingNextWave, WaitingWaveClear, WaitingWaveSpawn };
    public GameState gameState = GameState.WaitingNextWave;
    private bool isGameRunning = false;

    // References
    private WaveSystem waveSystem;
    public static GameManager instance = null;

    public HUD hud;
    public CustomCamController camController;
    public ObjectiveHealth objHealth;
    public PlayerHealthSystem plyHealth;
    public GameObject boomEffect, enemy1, enemy2, player1;

    // Spawning and pathing
    public bool isGameDone = false;
    public bool isWaveCleared = true;

    public Transform[] path1, path2;
    private WaveSystem.Spawn spawn;
    private bool isWaveFullySpawned = true;
    private bool startNextWave = false;
    private float spawnReadyTime;
    private int waveKilled;
    private int waveCount;
    private bool getNewSpawn;

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

        waveSystem = GetComponent<WaveSystem>();

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

        hud.ShowNextWaveBtn();
    }

    // Update is called every frame
    void Update()
    {
        /*if (!isGameRunning)
        {
            return;
        }*/

        Wave();
        ClearDead();
    }

    public void StartNextWave()
    {
        if (gameState != GameState.GameDone)
        {
            gameState = GameState.WaitingWaveSpawn;
            waveSystem.SetNextWave();
            waveCount = waveSystem.GetWaveCount();
            waveKilled = 0;

            getNewSpawn = true;
        }
    }

    void Wave()
    {
        Debug.Log(gameState);

        if (gameState == GameState.GameDone)
        {
            isGameDone = true;
        }
        else if (gameState == GameState.WaitingNextWave)
        {
            if (waveSystem.IsLastWave())
            {
                gameState = GameState.GameDone;
            }
        }
        else if (gameState == GameState.WaitingWaveClear)
        {
            if (enemies.Count == 0)
            {
                gameState = GameState.WaitingNextWave;
                hud.ShowNextWaveBtn();
            }
        }
        else if (gameState == GameState.WaitingWaveSpawn)
        {
            if (enemies.Count < 40)
            {
                if (getNewSpawn)
                {
                    if (waveSystem.IsWaveOver())
                    {
                        gameState = GameState.WaitingWaveClear;
                        return;
                    }

                    spawn = waveSystem.GetSpawn();
                    spawnReadyTime = Time.timeSinceLevelLoad + spawn.spawnDelay;
                    //Debug.Log(spawn);
                    getNewSpawn = false;
                }

                if (Time.timeSinceLevelLoad > spawnReadyTime)
                {
                    Spawn();
                    getNewSpawn = true;
                }

            }
        }
    }

    void Spawn()
    {
        // Get enemy
        GameObject enemy = enemy1; // Default enemy type

        if (spawn.enemyType == 1)
        {
            enemy = enemy2;
        }

        if (spawn.pathChoice == 0)
        {
            enemy = Instantiate(enemy, path1[0]);
            enemy.GetComponent<EnemyController>().Setup(this, path1);
        }
        else
        {
            enemy = Instantiate(enemy, path2[0]);
            enemy.GetComponent<EnemyController>().Setup(this, path2);
        }

        enemy.GetComponent<SpriteRenderer>().sortingOrder = 20 + enemies.Count;
        enemies.Add(enemy);
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
                    waveKilled++;
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

    public string GetInfo()
    {
        if (gameState == GameState.WaitingNextWave)
        {
            return waveSystem.GetInfo();
        }
        else
        {
            return "Defeated " + waveKilled + " of " + waveCount;
        }
    }

    public string GetNextWaveInfo()
    {
        return waveSystem.GetNextWaveInfo();
    }
}
