using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour {
    private readonly int SPAWN_POINTS = 2;

    public struct Spawn
    {
        public Spawn(int enemyType, int pathChoice, float spawnDelay)
        {
            this.enemyType = enemyType;
            this.pathChoice = pathChoice;
            this.spawnDelay = spawnDelay;
        }

        public int enemyType;
        // 0 - Fox: Attacks objective only
        // 1 - Skeleton: Attacks objective and player if player is in the way
        public int pathChoice;
        // 0 - Left spawn point
        // 1 - Right spawn point
        public float spawnDelay;

        public override string ToString()
        {
            return "[Type: " + enemyType + ", Spawn: " + pathChoice + ", Delay: " + spawnDelay + "]";
        }
    }

    private List<List<Spawn>> waves;
    private List<Spawn> currentWave;
    private List<Spawn> tempWave;
    private int waveNumber = 0;
    private int nextWaveNumber = 1;
    private int totalWaves;

    void CreateNewWave()
    {
        tempWave = new List<Spawn>();
    }

    void AddEnemy (int enemyType, int pathChoice, float spawnDelay)
    {
        tempWave.Add(new Spawn(enemyType, pathChoice, spawnDelay));
    }

    void AddEnemy(int enemyType, int pathChoice, float initialDelay, int count, float subsequentDelay)
    {
        AddEnemy(enemyType, pathChoice, initialDelay);

        for (int i = 1; i < count; i++)
        {
            AddEnemy(enemyType, pathChoice, subsequentDelay);
        }
    }

    void AddEnemyAtAllSpawnPoints(int enemyType, float spawnDelay)
    {
        for (int i = 0; i < SPAWN_POINTS; i++)
        {
            AddEnemy(enemyType, i, spawnDelay);
        }
    }

    void AddEnemyAtAllSpawnPoints(int enemyType, float initialDelay, int count, float subsequentDelay)
    {
        AddEnemyAtAllSpawnPoints(enemyType, initialDelay);

        for (int i = 1; i < count; i++)
        {
            AddEnemyAtAllSpawnPoints(enemyType, subsequentDelay);
        }
    }

    List<Spawn> GetNewWave()
    {
        return tempWave;
    }

    // Use this for initialization
    void Start () {
        waves = new List<List<Spawn>>();

        // Test Wave
        CreateNewWave();

        AddEnemy(0, 0, 1f);
        AddEnemy(1, 0, 1f);

        waves.Add(GetNewWave());

        // Wave 1
        //CreateNewWave();

        // Spawns Fox, at Spawn Point 0, after initial delay of 2s, 2 times, with 2s intervals
        //AddEnemy(0, 0, 2f, 2, 2f);
        //AddEnemy(0, 1, 4f, 2, 2f);

        //AddEnemy(0, 0, 6f, 5, 1.8f);
        //AddEnemy(0, 1, 6f, 5, 1.8f);

        //waves.Add(GetNewWave());

        // Wave 2
        //CreateNewWave();

        // Spawns Fox, at Spawn Point 0, after initial delay of 2s, 3 times, with 2s intervals
        //AddEnemy(0, 0, 2f, 3, 1.8f);
        //AddEnemy(0, 1, 4f, 3, 1.8f);

        // Spawns a Fox at each spawn point, after initial delay of 1.5s
        //AddEnemyAtAllSpawnPoints(0, 3f);

        //AddEnemy(0, 0, 6f, 5, 1.6f);
        //AddEnemy(0, 1, 6f, 5, 1.6f);

        //waves.Add(GetNewWave());

        // Wave 3
        //CreateNewWave();

        //AddEnemy(0, 0, 2f, 6, 1.6f);
        //AddEnemy(0, 1, 5f, 6, 1.6f);

        // Spawns a Fox at each spawn point, after initial delay of 5s, 2 times, with 2s intervals
        //AddEnemyAtAllSpawnPoints(0, 5f, 2, 2.0f);

        //AddEnemy(0, 1, 5f, 8, 1.5f);

        //waves.Add(GetNewWave());

        // Wave 4
        //CreateNewWave();

        //AddEnemyAtAllSpawnPoints(0, 2f, 3, 2.0f);

        //AddEnemy(0, 1, 4f, 3, 1.5f);
        //AddEnemy(0, 0, 3f, 3, 1.5f);
        //AddEnemy(0, 1, 2f, 3, 1.5f);

        //AddEnemyAtAllSpawnPoints(0, 2f, 4, 1.8f);

        //waves.Add(GetNewWave());

        totalWaves = waves.Count;
    }

    public bool IsLastWave()
    {
        return waves.Count == 0;
    }

    public void SetNextWave()
    {
        currentWave = waves[0];
        waves.RemoveAt(0);
        waveNumber++;
        nextWaveNumber++;
    }

    public int GetDisplayWave()
    {
        return waveNumber;
    }

    public int GetWaveCount()
    {
        return currentWave.Count;
    }

    public string GetInfo()
    {
        return "Wave " + waveNumber + " of " + totalWaves;
    }

    public string GetNextWaveInfo()
    {
        return "Begin wave " + nextWaveNumber;
    }

    public bool IsWaveOver()
    {
        return currentWave.Count == 0;
    }

    public Spawn GetSpawn()
    {
        Spawn spawn = currentWave[0];
        currentWave.RemoveAt(0);
        return spawn;
    }
}
