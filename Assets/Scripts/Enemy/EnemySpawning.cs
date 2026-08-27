using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawning : MonoBehaviour
{
    public List<GameObject> disabledEnemies = new List<GameObject>();

    [Header("Wave Budget")]
    [SerializeField, Min(1)] private int startingWallet = 10;
    [SerializeField] private float waveIntervalMax = 15f;
    [SerializeField] private float waveIntervalMin = 5f;
    [SerializeField] private WaveFitnessSettings directorSettings = new WaveFitnessSettings();

    [Header("Spawn Placement")]
    [SerializeField] private float spawnRadius = 8f;
    [SerializeField] private int spawnRetryAttempts = 8;

    [Header("Spawn Blocking")]
    [Tooltip("Layers that should block enemy spawns (recommended: create an 'Obstacle' layer and assign to walls/objects)")]
    [SerializeField] private LayerMask obstacleLayers;
    [Tooltip("Radius used to check for collisions when picking a spawn point")]
    [SerializeField] private float spawnClearRadius = 0.6f;

    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;

    private readonly List<EnemySpawnDefinition> availableEnemies = new List<EnemySpawnDefinition>();
    private int wallet;
    private int nextWaveId = 1;
    private float waveInterval;
    private Transform playerTransform;
    private Transform enemiesParent;
    private PlayerPerformanceTracker performanceTracker;
    private WaveTelemetryLogger telemetryLogger;
    private WaveGenerator waveGenerator;
    private WaveFitnessScorer fitnessScorer;
    private System.Random candidateRandom;
    private bool isInitialized;

    private void Awake()
    {
        wallet = startingWallet;
        candidateRandom = new System.Random();
        waveGenerator = new WaveGenerator();
        fitnessScorer = new WaveFitnessScorer(directorSettings);

        GameObject enemiesContainer = GameObject.Find("Enemies");
        enemiesParent = enemiesContainer != null ? enemiesContainer.transform : null;
        CacheDisabledEnemies();

        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("EnemySpawning requires an active PlayerController.", this);
            enabled = false;
            return;
        }

        playerTransform = player.transform;
        performanceTracker = GetComponent<PlayerPerformanceTracker>();
        if (performanceTracker == null)
        {
            performanceTracker = gameObject.AddComponent<PlayerPerformanceTracker>();
        }
        performanceTracker.Initialize(player);

        telemetryLogger = GetComponent<WaveTelemetryLogger>();
        if (telemetryLogger == null)
        {
            telemetryLogger = gameObject.AddComponent<WaveTelemetryLogger>();
        }
        telemetryLogger.Initialize(performanceTracker);

        CacheEnemyDefinitions();
        isInitialized = ValidateConfiguration();
        if (!isInitialized)
        {
            enabled = false;
        }
    }

    private void Start()
    {
        if (!isInitialized)
        {
            return;
        }

        SpawnAdaptiveWave();
        waveInterval = Random.Range(waveIntervalMin, waveIntervalMax);
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        while (true)
        {
            yield return new WaitForSeconds(waveInterval);
            wallet += (int)Random.Range(waveInterval, waveInterval + 5f);
            SpawnAdaptiveWave();
            waveInterval = Random.Range(waveIntervalMin, waveIntervalMax);
        }
    }

    public void ReturnToPool(GameObject enemy)
    {
        if (enemy != null && !disabledEnemies.Contains(enemy))
        {
            disabledEnemies.Add(enemy);
        }
    }

    private void SpawnAdaptiveWave()
    {
        PlayerPerformanceState state = performanceTracker.CaptureState();
        List<WaveCandidate> candidates = waveGenerator.GenerateCandidates(
            availableEnemies,
            wallet,
            directorSettings.CandidateCount,
            candidateRandom);

        if (candidates.Count == 0)
        {
            Debug.LogWarning($"Wave director could not generate a wave with wallet {wallet}.", this);
            return;
        }

        List<ScoredCandidate> rankedCandidates = candidates
            .Select(candidate => new ScoredCandidate(candidate, fitnessScorer.Score(candidate, state)))
            .OrderByDescending(candidate => candidate.Fitness.Score)
            .ToList();

        LogTopCandidates(rankedCandidates);
        ScoredCandidate winner = rankedCandidates[0];
        List<EnemyController> spawnedEnemies = SpawnCandidate(winner.Candidate);

        if (spawnedEnemies.Count == 0)
        {
            return;
        }

        wallet -= winner.Candidate.TotalCost;
        telemetryLogger.BeginWave(nextWaveId, winner.Candidate, state, winner.Fitness, spawnedEnemies);
        nextWaveId++;
    }

    private List<EnemyController> SpawnCandidate(WaveCandidate candidate)
    {
        List<EnemyController> spawned = new List<EnemyController>();

        foreach (EnemySpawnDefinition enemy in candidate.Enemies)
        {
            EnemyController controller = SpawnEnemy(enemy);
            if (controller != null)
            {
                spawned.Add(controller);
            }
        }

        return spawned;
    }

    private EnemyController SpawnEnemy(EnemySpawnDefinition definition)
    {
        disabledEnemies.RemoveAll(enemy => enemy == null);
        GameObject pooledEnemy = disabledEnemies.FirstOrDefault(enemy =>
        {
            EnemyController controller = enemy.GetComponent<EnemyController>();
            return controller != null && controller.stats == definition.Stats;
        });

        GameObject enemyInstance;
        Vector3 spawnPosition = GetRandomSpawnPosition();

        if (pooledEnemy != null)
        {
            disabledEnemies.Remove(pooledEnemy);
            enemyInstance = pooledEnemy;
            enemyInstance.transform.position = spawnPosition;
            enemyInstance.SetActive(true);
        }
        else
        {
            enemyInstance = enemiesParent != null
                ? Instantiate(definition.Prefab, spawnPosition, Quaternion.identity, enemiesParent)
                : Instantiate(definition.Prefab, spawnPosition, Quaternion.identity);
        }

        return enemyInstance.GetComponent<EnemyController>();
    }

    private void CacheDisabledEnemies()
    {
        if (enemiesParent == null)
        {
            return;
        }

        foreach (Transform child in enemiesParent)
        {
            if (child != null && child.CompareTag("Enemy") && !child.gameObject.activeInHierarchy)
            {
                ReturnToPool(child.gameObject);
            }
        }
    }

    private void CacheEnemyDefinitions()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Enemies");
        List<PrefabStatsPair> validPrefabs = new List<PrefabStatsPair>();

        foreach (GameObject prefab in prefabs)
        {
            EnemyController controller = prefab != null ? prefab.GetComponent<EnemyController>() : null;
            if (controller == null || controller.stats == null)
            {
                Debug.LogWarning($"Skipping enemy prefab '{(prefab != null ? prefab.name : "null")}' because it has no EnemyController stats.", this);
                continue;
            }

            if (controller.stats.Cost <= 0)
            {
                Debug.LogWarning($"Skipping enemy prefab '{prefab.name}' because its cost must be greater than zero.", this);
                continue;
            }

            if (controller.stats.AttackDelay <= 0f)
            {
                Debug.LogWarning($"Skipping enemy prefab '{prefab.name}' because DPS requires an attack delay greater than zero.", this);
                continue;
            }

            validPrefabs.Add(new PrefabStatsPair(prefab, controller.stats));
        }

        float highestAvailableDps = validPrefabs.Count > 0
            ? validPrefabs.Max(pair => EnemyPressureProfile.CalculateDps(pair.Stats))
            : 0f;

        foreach (PrefabStatsPair pair in validPrefabs)
        {
            EnemyPressureProfile pressure = EnemyPressureProfile.FromStats(
                pair.Stats,
                directorSettings.ReferenceSpeed,
                directorSettings.ReferenceHealth,
                highestAvailableDps);

            availableEnemies.Add(new EnemySpawnDefinition(pair.Prefab, pair.Stats, pressure));
            Debug.Log(
                $"AI profile {pair.Prefab.name}: threat={pressure.ThreatScore:0.00}, "
                + $"swarm={pressure.SwarmPressure:0.00}, ranged={pressure.RangedPressure:0.00}, "
                + $"speed={pressure.SpeedPressure:0.00}, tank={pressure.Tankiness:0.00}, "
                + $"damage={pressure.DamagePressure:0.00}",
                this);
        }
    }

    private bool ValidateConfiguration()
    {
        if (availableEnemies.Count == 0)
        {
            Debug.LogError("EnemySpawning found no valid enemy prefabs in Resources/Enemies.", this);
            return false;
        }

        if (floorTilemap == null || wallTilemap == null)
        {
            Debug.LogError("EnemySpawning requires both floor and wall tilemaps.", this);
            return false;
        }

        if (waveIntervalMin <= 0f || waveIntervalMax < waveIntervalMin)
        {
            Debug.LogError("EnemySpawning wave intervals are invalid.", this);
            return false;
        }

        return true;
    }

    private void LogTopCandidates(IReadOnlyList<ScoredCandidate> rankedCandidates)
    {
        int count = Mathf.Min(3, rankedCandidates.Count);
        IEnumerable<string> lines = Enumerable.Range(0, count).Select(index =>
        {
            ScoredCandidate entry = rankedCandidates[index];
            string composition = string.Join(", ", entry.Candidate.Enemies
                .GroupBy(enemy => enemy.Name)
                .OrderBy(group => group.Key)
                .Select(group => $"{group.Key} x{group.Count()}"));

            return $"#{index + 1} fitness={entry.Fitness.Score:0.000}, "
                + $"difficulty={entry.Candidate.EstimatedDifficulty:0.000}, "
                + $"target={entry.Fitness.TargetDifficulty:0.000}, "
                + $"variety={entry.Candidate.VarietyScore:0.000}, "
                + $"pressure={entry.Candidate.PressureScore:0.000}: {composition}";
        });

        Debug.Log("Wave director top candidates:\n" + string.Join("\n", lines), this);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 candidate;
        BoundsInt bounds = floorTilemap.cellBounds;

        for (int attempt = 0; attempt < Mathf.Max(1, spawnRetryAttempts); attempt++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float radius = spawnRadius * Random.Range(0.8f, 1.2f);
            candidate = playerTransform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;

            if (bounds.Contains(floorTilemap.WorldToCell(candidate)) && IsWithinBounds(candidate) && !IsBlocked(candidate))
            {
                return candidate;
            }
        }

        const int uniformSamples = 24;
        for (int i = 0; i < uniformSamples; i++)
        {
            float angle = Mathf.PI * 2f * (i / (float)uniformSamples);
            float radius = spawnRadius * Mathf.Sqrt(Random.Range(0f, 1f));
            Vector3 point = playerTransform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;

            if (IsWithinBounds(point) && !IsBlocked(point))
            {
                return point;
            }
        }

        Vector3Int startCell = floorTilemap.WorldToCell(playerTransform.position);
        int cellRadius = Mathf.CeilToInt(spawnRadius);
        for (int searchRadius = 1; searchRadius <= cellRadius; searchRadius++)
        {
            for (int x = -searchRadius; x <= searchRadius; x++)
            {
                for (int y = -searchRadius; y <= searchRadius; y++)
                {
                    if (Mathf.Abs(x) != searchRadius && Mathf.Abs(y) != searchRadius)
                    {
                        continue;
                    }

                    Vector3Int checkCell = startCell + new Vector3Int(x, y, 0);
                    if (!floorTilemap.HasTile(checkCell) || wallTilemap.HasTile(checkCell))
                    {
                        continue;
                    }

                    Vector3 center = floorTilemap.GetCellCenterWorld(checkCell);
                    if (Vector3.Distance(center, playerTransform.position) <= spawnRadius && !IsBlocked(center))
                    {
                        return center;
                    }
                }
            }
        }

        float fallbackAngle = Random.Range(0f, Mathf.PI * 2f);
        return playerTransform.position
            + new Vector3(Mathf.Cos(fallbackAngle), Mathf.Sin(fallbackAngle), 0f) * spawnRadius;
    }

    private bool IsBlocked(Vector3 position)
    {
        if (obstacleLayers.value != 0)
        {
            Collider2D hit = Physics2D.OverlapCircle(position, spawnClearRadius, obstacleLayers);
            if (hit != null)
            {
                return true;
            }
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(position, spawnClearRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit != null && hit.CompareTag("Obstacle"))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsWithinBounds(Vector3 position)
    {
        Vector3Int cellPosition = floorTilemap.WorldToCell(position);
        return floorTilemap.HasTile(cellPosition) && !wallTilemap.HasTile(cellPosition);
    }

    private readonly struct PrefabStatsPair
    {
        public GameObject Prefab { get; }
        public EnemyStats Stats { get; }

        public PrefabStatsPair(GameObject prefab, EnemyStats stats)
        {
            Prefab = prefab;
            Stats = stats;
        }
    }

    private readonly struct ScoredCandidate
    {
        public WaveCandidate Candidate { get; }
        public WaveFitnessResult Fitness { get; }

        public ScoredCandidate(WaveCandidate candidate, WaveFitnessResult fitness)
        {
            Candidate = candidate;
            Fitness = fitness;
        }
    }
}
