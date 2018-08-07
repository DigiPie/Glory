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

    public int dashUnlockWave;
    public int spell1UnlockWave;
    public int spell2UnlockWave;

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
        waves.Add(GetNewWave());

        // Test Wave 2
        CreateNewWave();
        AddEnemy(0, 0, 1f);
        waves.Add(GetNewWave());

        // Test Wave 3
        CreateNewWave();
        AddEnemy(0, 0, 1f);
        waves.Add(GetNewWave());

        // Wave 1
        CreateNewWave();
        AddEnemy(0, 0, 0, 3, 2f);
        AddEnemy(0, 0, 4f, 6, 1.5f);
        waves.Add(GetNewWave());

        // Wave 2
        CreateNewWave();
        AddEnemy(0, 0, 0, 6, 1.5f);
        AddEnemy(0, 1, 6f, 6, 1.5f);
        waves.Add(GetNewWave());

        // Wave 3
        CreateNewWave();
        AddEnemy(0, 0, 0, 8, 1f);
        AddEnemy(0, 1, 6f, 8, 1f);
        AddEnemy(0, 0, 6f);
        AddEnemy(0, 1, 0);
        AddEnemy(0, 0, 6f);
        AddEnemy(0, 1, 0);
        waves.Add(GetNewWave());

        // Wave 4
        CreateNewWave();
        AddEnemy(1, 0, 0, 2, 4f);
        AddEnemy(1, 1, 8f, 2, 4f);
        waves.Add(GetNewWave());

        // Wave 5
        CreateNewWave();
        AddEnemy(0, 0, 0, 6, 1f);
        AddEnemy(1, 0, 6f);
        AddEnemy(0, 0, 2f, 6, 1f);
        AddEnemy(1, 0, 4f, 3, 2f);
        waves.Add(GetNewWave());

        // Wave 6
        CreateNewWave();
        AddEnemy(0, 0, 0, 6, 1f);
        AddEnemy(1, 0, 6f);
        AddEnemy(0, 0, 2f, 6, 1f);
        AddEnemy(1, 1, 6f);
        AddEnemy(0, 1, 2f, 8, 1f);
        waves.Add(GetNewWave());

        // Wave 7
        CreateNewWave();
        AddEnemy(2, 0, 0);
        AddEnemy(2, 0, 8f, 2, 3f);
        AddEnemy(0, 1, 2f, 8, 1f);
        AddEnemy(1, 1, 4f, 2, 2f);
        waves.Add(GetNewWave());

        // Wave 8
        CreateNewWave();
        AddEnemy(0, 1, 0, 10, 1f);
        AddEnemy(1, 1, 4f, 4, 2f);
        AddEnemy(2, 0, 8f, 2, 2.5f);
        AddEnemy(0, 0, 12f, 8, 1f);
        waves.Add(GetNewWave());

        // Wave 9
        CreateNewWave();
        AddEnemy(2, 0, 0);
        AddEnemy(2, 1, 0);
        AddEnemy(2, 0, 6f);
        AddEnemy(2, 1, 0);
        AddEnemy(0, 1, 12f, 4, 1f);
        AddEnemy(1, 1, 1f);
        AddEnemy(0, 1, 12f, 4, 1f);
        AddEnemy(1, 1, 1f);
        waves.Add(GetNewWave());

        // Wave 10
        CreateNewWave();
        AddEnemy(0, 0, 0, 6, 0.75f);
        AddEnemy(3, 0, 4f);
        AddEnemy(1, 1, 10f, 2, 3f);
        AddEnemy(3, 0, 6f);
        waves.Add(GetNewWave());

        // Wave 11
        CreateNewWave();
        AddEnemy(0, 0, 0, 14, 0.75f);
        AddEnemy(1, 0, 2f, 6, 1.5f);
        AddEnemy(2, 0, 8f, 2, 2.5f);
        AddEnemy(0, 1, 12f, 14, 0.75f);
        AddEnemy(3, 0, 4f, 2, 4f);
        waves.Add(GetNewWave());

        // Wave 12
        CreateNewWave();
        AddEnemy(0, 0, 0, 12, 0.5f);
        AddEnemy(1, 0, 2f, 3, 1.5f);
        AddEnemy(2, 0, 8f, 2, 2.5f);
        AddEnemy(0, 1, 12f, 6, 0.75f);
        AddEnemy(3, 0, 4f, 2, 4f);
        waves.Add(GetNewWave());

        // Wave 13
        CreateNewWave();
        AddEnemy(1, 0, 0, 6, 1.5f);
        AddEnemy(3, 0, 8f, 2, 4f);
        AddEnemy(2, 1, 12f, 4, 2f);
        waves.Add(GetNewWave());

        // Wave 14
        CreateNewWave();
        AddEnemy(0, 1, 0, 6, 1.5f);
        AddEnemy(2, 1, 4f, 4, 2f);
        AddEnemy(3, 0, 6f, 2, 2f);
        AddEnemy(0, 0, 12f, 12, 0.35f);
        AddEnemy(0, 0, 2f, 12, 0.35f);
        waves.Add(GetNewWave());

        // Wave 15
        CreateNewWave();
        AddEnemy(4, 0, 0);
        waves.Add(GetNewWave());

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

    public int GetWave()
    {
        return waveNumber;
    }

    public bool IsDashUnlockWave()
    {
        return waveNumber == dashUnlockWave;
    }

    public bool IsSpell1UnlockWave()
    {
        return waveNumber == spell1UnlockWave;
    }

    public bool IsSpell2UnlockWave()
    {
        return waveNumber == spell2UnlockWave;
    }

    public int GetWaveCount()
    {
        return currentWave.Count;
    }

    public string GetInfo()
    {
        return "Wave " + nextWaveNumber + " of " + totalWaves;
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
