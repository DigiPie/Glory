using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    // References
    public HUD hud;
    public Overlay overlay;
    public CustomCamController camController;
    public ObjectiveHealth objHealth;
    public PlayerHealthSystem plyHealth;
    public StateSystem stateSystem;
    public WaveSystem waveSystem;
    public GameObject boomEffect, enemy1, enemy2, enemy3, enemy4, player1, objective;

    // Spawning and pathing
    public bool isWaveCleared = true;

    public Transform spawn1, spawn2;
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

    // Set to true after win/lose and game over screen is displayed.
    private bool isGameOver = false;

    void Awake()
    {
        enemies = new List<GameObject>();
    }

    // Update is called every frame
    void Update()
    {
        if (isGameOver)
        {
            return; // If game over screen already displayed, just return
        }

        if (HandleWinLose())
        {
            return; // If game won or lost, display game over screen and stop running the rest of the code
        }


        if(stateSystem.IsGameTutorial())
        {
            overlay.ShowTutorialUI();
        }

        else if (stateSystem.IsGameWave())
        {
            HandleWave();
            HandleDead();
        }
    }

    // Returns true if win/lose, false if haven't won or lost
    bool HandleWinLose()
    {
        if (stateSystem.IsWaveDone())
        {
            stateSystem.SetGameState(StateSystem.GameState.Win);
            overlay.ShowGameoverUI(true);
            camController.StopShake();
            isGameOver = true;
            return true;
        }
        else if(plyHealth.isDead || objHealth.isDestroyed)
        {
            stateSystem.SetGameState(StateSystem.GameState.Lose);
            overlay.ShowGameoverUI(false);
            camController.StopShake();
            isGameOver = true;
            return true;
        }

        return false;
    }

    void HandleWave()
    {
        if (stateSystem.IsWaitingNextWave())
        {
            if (waveSystem.IsLastWave())
            {
                stateSystem.SetWaveState(StateSystem.WaveState.Done);
            }
        }
        else if (stateSystem.IsWaitingWaveClear())
        {
            if (enemies.Count == 0)
            {
                plyHealth.resetFullHealth();
                //player1.GetComponent<PlayerController>().AllowAttack(false);
                stateSystem.SetWaveState(StateSystem.WaveState.WaitingNextWave);
                hud.ShowNextWaveBtn();
            }
        }
        else if (stateSystem.IsWaitingWaveSpawn())
        {
            if (enemies.Count < 40)
            {
                if (getNewSpawn)
                {
                    if (waveSystem.IsWaveOver())
                    {
                        stateSystem.SetWaveState(StateSystem.WaveState.WaitingWaveClear);
                        return;
                    }

                    spawn = waveSystem.GetSpawn();
                    spawnReadyTime = Time.timeSinceLevelLoad + spawn.spawnDelay;
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

    // Spawn an enemy using the spawn system
    void Spawn()
    {
        // Get enemy
        GameObject enemy = enemy1; // Default enemy type

        if (spawn.enemyType == 1)
        {
            enemy = enemy2;
        }
        else if (spawn.enemyType == 2)
        {
            enemy = enemy3;
        }
        else if (spawn.enemyType == 3)
        {
            enemy = enemy4;
        }

        if (spawn.pathChoice == 0)
        {
            enemy = Instantiate(enemy, spawn1);
        }
        else
        {
            enemy = Instantiate(enemy, spawn2);
        }

        enemy.GetComponent<EnemyController>().Setup(this);
        enemy.GetComponent<EnemyAnimator>().SetSortingOrder(20 + enemies.Count);
        enemies.Add(enemy);
    }

    // Clear dead enemies
    void HandleDead()
    {
        hasDeadEnemy = false;

        // Runs at least once, terminates if there is no dead enemies in the list
        do
        {
            foreach (GameObject enemy in enemies)
            {
                if (enemy == null || enemy.GetComponent<EnemyController>().IsDead())
                {
                    camController.Shake(0.15f, 0.1f);
                    //Instantiate(boomEffect, enemy.transform.position, enemy.transform.rotation);
                    enemies.Remove(enemy);
                    hasDeadEnemy = true;
                    waveKilled++;
                    return;
                }
            }
        } while (hasDeadEnemy);
    }

    // Called by the HUD script upon clicking of next wave button or Enter key press
    public void StartNextWave()
    {
        if (stateSystem.IsGameWave())
        {
            stateSystem.SetWaveState(StateSystem.WaveState.WaitingWaveSpawn);
            waveSystem.SetNextWave();
            waveCount = waveSystem.GetWaveCount();
            waveKilled = 0;
            getNewSpawn = true;
            //player1.GetComponent<PlayerController>().AllowAttack(true);
        }
    }

    // Called by spawned enemies to damage the objective
    public void DamageObjective(int damage)
    {
        //camController.Shake(0.05f, 0.15f);
        objHealth.TakeDamage(damage);
    }

    public void DamageObjective(float damage)
    {
        DamageObjective((int)damage);
    }

    // Used by spawned enemies to damage the player
    public void DamagePlayer(int damage)
    {
        camController.Shake(0.1f, 0.2f);
        plyHealth.TakeDamage(damage);
    }

    public void DamagePlayer(float damage)
    {
        DamagePlayer((int) damage);
    }

    public float GetPlayerPositionX()
    {
        return player1.transform.position.x;
    }

    public float GetObjectivePositionX()
    {
        return objective.transform.position.x;
    }

    // Used by the HUD script and displayed by txtInfo under the objective health
    public string GetInfo()
    {
        if (stateSystem.IsWaitingNextWave())
        {
            return waveSystem.GetInfo();
        }
        else
        {
            return "Defeated " + waveKilled + " of " + waveCount;
        }
    }

    // Used by the HUD script and displayed by txtNextWave which is on the next wave button
    public string GetNextWaveInfo()
    {
        return waveSystem.GetNextWaveInfo();
    }
}
