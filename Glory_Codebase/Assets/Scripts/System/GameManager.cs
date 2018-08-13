using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // References
    public HUD hud;
    public Overlay overlay;
    public CustomCamController camController;
    public GameObject enemy1, enemy2, enemy3, enemy4, boss1;
    public GameObject player1;
    public GameObject objective;
    private PlayerActionSystem playerAction; // GetComponent from player1

    private ObjectiveHealthSystem objectiveHealth;
    private PlayerHealthSystem playerHealth;
    private StateSystem stateSystem;
    private WaveSystem waveSystem;

    // States
    private bool isWaveCleared = true;
    private bool isWaveFullySpawned = true;
    private bool startNextWave = false;
    private bool isGameOver = false; // Set to true after win/lose and game over screen is displayed.

    // Spawning and pathing
    public Transform spawn1, spawn2;
    private WaveSystem.Spawn spawn;
    private float spawnReadyTime;
    private bool spawnOnLeft = false;
    private int waveKilled;
    private int waveCount;
    private bool getNewSpawn;

    // For visual
    private readonly int defaultSpawnSortOrder = 20;
    private readonly int maxSpawnSortOrder = 100;
    private readonly int spawnSortOrderIncrement = 3;
    private int spawnSortOrder = 20;

    // Enemy tracking
    private EnemyHealthSystem bossHealth;
    private List<GameObject> enemies;
    private List<GameObject> deadBodies;
    private int maxBodyCount = 10;
    private bool hasDeadEnemy;
    private bool hasEnemyOnLeft; // and beyond camera. Used to trigger enemy off-screen indicator
    private bool hasEnemyOnRight;

    void Awake()
    {
        objectiveHealth = GetComponent<ObjectiveHealthSystem>();
        playerHealth = GetComponent<PlayerHealthSystem>();
        stateSystem = GetComponent<StateSystem>();
        waveSystem = GetComponent<WaveSystem>();

        playerAction = player1.GetComponent<PlayerActionSystem>();

        enemies = new List<GameObject>();
        deadBodies = new List<GameObject>();

        // Testing
        //EnableSpell1(true);
        //EnableSpell2(true);
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

        HandleHUD();

        if (stateSystem.IsGameTutorial())
        {
            overlay.ShowTutorialUI();
        }

        else if (stateSystem.IsGameWave())
        {
            HandleWave();
            HandleEnemies();
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
        else if (playerHealth.isDead || objectiveHealth.isDestroyed)
        {
            stateSystem.SetGameState(StateSystem.GameState.Lose);
            overlay.ShowGameoverUI(false);
            camController.StopShake();
            isGameOver = true;
            return true;
        }

        return false;
    }

    void HandleHUD()
    {
        hud.UpdatePlayerHealth(playerHealth.GetDisplayHealth());
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
                playerHealth.ResetFullHealth();
                playerAction.ResetAllCooldowns();
                hud.ResetAllCooldownIndicators();

                //player1.GetComponent<PlayerController>().AllowAttack(false);
                stateSystem.SetWaveState(StateSystem.WaveState.WaitingNextWave);

                // Reset monster spawn sort order
                spawnSortOrder = defaultSpawnSortOrder;

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
        else if (spawn.enemyType == 4)
        {
            enemy = boss1;
        }

        if (spawn.pathChoice == 0)
        {
            enemy = InstantiateGameObject(enemy, spawn1);
        }
        else
        {
            enemy = InstantiateGameObject(enemy, spawn2);
        }

        enemy.GetComponent<EnemyController>().Setup(this, spawnSortOrder);

        spawnSortOrder += spawnSortOrderIncrement;

        if (spawnSortOrder > maxSpawnSortOrder)
        {
            spawnSortOrder = defaultSpawnSortOrder;
        }

        if (enemy.GetComponent<EnemyHealthSystem>().IsBoss())
        {
            bossHealth = enemy.GetComponent<EnemyHealthSystem>();
            bossHealth.Setup(hud);
            hud.ShowBossHealth(bossHealth.GetMaxHealth());
            Shake(1.0f);
        }
        else
        {
            enemy.GetComponent<EnemyHealthSystem>().Setup();
        }

        enemies.Add(enemy);
    }

    GameObject InstantiateGameObject(GameObject gameObject, Transform newTransform)
    {
        Vector3 pos = newTransform.position;
        Quaternion rotation = newTransform.rotation;
        return Instantiate(gameObject, pos, rotation);
    }

    // Clear dead enemies
    void HandleEnemies()
    {
        hasDeadEnemy = false;
        hasEnemyOnLeft = false;
        hasEnemyOnRight = false;

        // Runs at least once, terminates if there is no dead enemies in the list
        do
        {
            foreach (GameObject enemy in enemies)
            {
                if (enemy == null || enemy.GetComponent<EnemyController>().IsDead())
                {
                    enemies.Remove(enemy);
                    deadBodies.Add(enemy);

                    if (deadBodies.Count > maxBodyCount)
                    {
                        GameObject tempObj = deadBodies[0];
                        deadBodies.RemoveAt(0);

                        if (tempObj != null)
                            tempObj.GetComponent<EnemyController>().StartFadeout();
                    }

                    hasDeadEnemy = true;
                    waveKilled++;
                    return;
                }
                else
                {
                    float xDiff = enemy.transform.position.x - player1.transform.position.x;

                    // If alive, check if out of camera bounds
                    if(!hasEnemyOnLeft && xDiff < -10)
                    {
                        hasEnemyOnLeft = true;
                    }
                    else if (!hasEnemyOnRight && xDiff > 10)
                    {
                        hasEnemyOnRight = true;
                    }
                }
            }
        } while (hasDeadEnemy);

        hud.ShowIndicatorLeft(hasEnemyOnLeft);
        hud.ShowIndicatorRight(hasEnemyOnRight);
    }

    // Called by the HUD script upon clicking of next wave button or Enter key press
    public void StartNextWave()
    {
        if (stateSystem.IsGameWave())
        {
            // Clear all dead bodies
            foreach (GameObject enemy in deadBodies)
            {
                if (enemy != null)
                {
                    enemy.GetComponent<EnemyController>().StartFadeout();
                }
            }

            deadBodies.Clear();

            // Reset cooldown for all abilities
            playerAction.ResetAllCooldowns();
            hud.ResetAllCooldownIndicators();

            // Update states
            stateSystem.SetWaveState(StateSystem.WaveState.WaitingWaveSpawn);
            waveSystem.SetNextWave();
            waveCount = waveSystem.GetWaveCount();
            waveKilled = 0;
            getNewSpawn = true;
        }
    }

    // Damage Dealing //
    public void DamageObjective(int damage)
    {
        hud.UpdateObjectiveHealth(objectiveHealth.TakeDamage(damage));
    }

    public void DamageObjective(float damage)
    {
        DamageObjective((int)damage);
    }

    public void DamagePlayer(int damage)
    {
        Shake(0.3f);
        playerHealth.TakeDamage(damage);
        hud.RedFlash();
    }

    public void DamagePlayer(float damage)
    {
        DamagePlayer((int)damage);
    }

    // Used by DamagePlayer and PlayerActionSystem to shake the camera
    public void Shake(float addShakeAmount)
    {
        camController.Shake(addShakeAmount);
    }

    // Positions //
    public float GetPlayerPositionX()
    {
        return player1.transform.position.x;
    }

    public float GetObjectivePositionX()
    {
        return objective.transform.position.x;
    }

    // HUD //
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

    public string GetNextWaveInfo()
    {
        return waveSystem.GetNextWaveInfo();
    }

    public void SetupHUD(float slideCooldown, float spell1Cooldown, float spell2Cooldown)
    {
        hud.Setup(slideCooldown, spell1Cooldown, spell2Cooldown);
    }

    // Called by PlayerActionSystem
    public void StartSlideAnim(float slideInvulDuration)
    {
        hud.StartInvulDurationAnim(slideInvulDuration);
        hud.StartSlideCooldownAnim();
    }

    // Called by PlayerActionSystem
    public void StartInvulDurationAnim(float slideInvulDuration)
    {
        hud.StartInvulDurationAnim(slideInvulDuration);
    }

    // Called by PlayerActionSystem
    public void ShowDashCooldownIndicator()
    {
        hud.ShowDashCooldownIndicator();
    }

    // Called by PlayerActionSystem
    public void ShowSpell1CooldownIndicator()
    {
        hud.ShowSpell1CooldownIndicator();
    }

    // Called by PlayerActionSystem
    public void ShowSpell2CooldownIndicator()
    {
        hud.ShowSpell2CooldownIndicator();
    }

    // Called by PlayerActionSystem
    public void StartSpell1CooldownAnim()
    {
        hud.StartSpell1CooldownAnim();
    }

    // Called by PlayerActionSystem
    public void StartSpell2CooldownAnim()
    {
        hud.StartSpell2CooldownAnim();
    }

    // Player Action System //
    public void EnableSlide()
    {
        playerAction.EnableSlide();
    }

    public void EnableSpell1(bool isFireSpell)
    {
        playerAction.EnableSpell1(isFireSpell);
    }

    public void EnableSpell2(bool isEarthSpell)
    {
        playerAction.EnableSpell2(isEarthSpell);
    }
}