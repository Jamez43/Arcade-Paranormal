using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawning : MonoBehaviour
{
    public List<GameObject> disabledEnemies = new List<GameObject>();
    private int wallet = 10;
    private Transform playerTransform;
    [SerializeField] private float spawnRadius = 8f;
    private float waveInterval;
    [SerializeField] private float waveIntervalMax = 15f;
    [SerializeField] private float waveIntervalMin = 5f;

    [SerializeField] private int spawnRetryAttempts = 8;

    [Header("Spawn Blocking")]
    [Tooltip("Layers that should block enemy spawns (recommended: create an 'Obstacle' layer and assign to walls/objects)")]
    [SerializeField] private LayerMask obstacleLayers;
    [Tooltip("Radius used to check for collisions when picking a spawn point")]
    [SerializeField] private float spawnClearRadius = 0.6f;

    private List<string> enemyNames = new List<string>();
    private Transform enemiesParent;

    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;

    private void Awake()
    {
        enemiesParent = GameObject.Find("Enemies").transform;
        // Only pool inactive enemies already in the scene hierarchy
        foreach (Transform child in enemiesParent)
        {
            if (child != null && child.CompareTag("Enemy") && !child.gameObject.activeInHierarchy)
            {
                if (!disabledEnemies.Contains(child.gameObject))
                    disabledEnemies.Add(child.gameObject);
            }

        }

        // Preload enemy prefab names for quick lookups
        enemyNames = Resources.LoadAll<GameObject>("Enemies")
            .Select(go => go.name)
            .ToList();

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start()
    {
        SpawnEnemies();
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        while (true)
        {
            yield return new WaitForSeconds(waveInterval);
            SpawnEnemies();
            waveInterval = Random.Range(waveIntervalMin, waveIntervalMax);
            wallet += (int)Random.Range(waveInterval, waveInterval + 5);
        }
    }


    private void SpawnEnemies()
    {
        // You will need to load prefabs by name when spawning
        List<GameObject> affordableEnemies;
        do
        {
            affordableEnemies = enemyNames
                .Select(name => Resources.Load<GameObject>("Enemies/" + name))
                .Where(e => e.GetComponent<EnemyController>().stats.Cost <= wallet)
                .ToList();

            if (affordableEnemies.Count == 0) break;

            // Spawn logic here
            // Choose a prefab to spawn
            GameObject prefabToSpawn = affordableEnemies[Random.Range(0, affordableEnemies.Count)];
            string enemyToSpawnName = prefabToSpawn.name;

            // Prefer matching by EnemyStats reference to avoid name suffix issues
            EnemyController prefabEC = prefabToSpawn.GetComponent<EnemyController>();
            GameObject disabledEnemy = disabledEnemies.FirstOrDefault(e =>
            {
                var ec = e != null ? e.GetComponent<EnemyController>() : null;
                if (ec != null && ec.stats != null && prefabEC != null && prefabEC.stats != null)
                {
                    return ec.stats == prefabEC.stats; // match by ScriptableObject reference
                }
                // Fallback to sanitized name match
                return SanitizeName(e.name) == enemyToSpawnName;
            });
            GameObject enemyInstance;

            if (disabledEnemy != null)
            {
                enemyInstance = disabledEnemy;
                enemyInstance.transform.position = GetRandomSpawnPosition();
                enemyInstance.SetActive(true);
                disabledEnemies.Remove(disabledEnemy);
            }
            else
            {
                GameObject enemyPrefab = prefabToSpawn; // already loaded
                // Parent under Enemies container if available to keep hierarchy tidy
                if (enemiesParent != null)
                {
                    enemyInstance = Instantiate(enemyPrefab, GetRandomSpawnPosition(), Quaternion.identity, enemiesParent);
                }
                else
                {
                    enemyInstance = Instantiate(enemyPrefab, GetRandomSpawnPosition(), Quaternion.identity);
                }
            }

            wallet -= enemyInstance.GetComponent<EnemyController>().stats.Cost;
        } while (wallet > 0 && affordableEnemies.Count > 0);
    }


    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 candidate;
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float radius;

        BoundsInt bounds = floorTilemap.cellBounds;

        // Preferred behavior: pick a point on a circle around the player; if out of bounds, retry
        // Generate a candidate on the circle (or slightly randomized ring)
        for (int attempt = 0; attempt < Mathf.Max(1, spawnRetryAttempts); attempt++)
        {
            angle = Random.Range(0f, Mathf.PI * 2f);
            // small jitter on radius to avoid perfect ring overlap
            radius = spawnRadius * Random.Range(0.8f, 1.2f);
            candidate = playerTransform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;

            if (IsWithinBounds(candidate) && !IsBlocked(candidate))
            {
                return candidate;
            }
        }

        // If we failed all attempts, find the nearest valid tile
        candidate = playerTransform.position;
        Vector3Int startCell = floorTilemap.WorldToCell(candidate);

        // Spiral search outward from player position to find nearest valid tile
        int maxSearchRadius = Mathf.Max(bounds.size.x, bounds.size.y);
        for (int searchRadius = 1; searchRadius < maxSearchRadius; searchRadius++)
        {
            for (int x = -searchRadius; x <= searchRadius; x++)
            {
                for (int y = -searchRadius; y <= searchRadius; y++)
                {
                    if (Mathf.Abs(x) == searchRadius || Mathf.Abs(y) == searchRadius)
                    {
                        Vector3Int checkCell = startCell + new Vector3Int(x, y, 0);
                        if (floorTilemap.HasTile(checkCell))
                        {
                            return floorTilemap.GetCellCenterWorld(checkCell);
                        }
                    }
                }
            }
        }

        // Fallback: return player position if no valid tile found
        return playerTransform.position;
    }

    // Check if the candidate position overlaps any obstacle
    private bool IsBlocked(Vector3 position)
    {
        // Preferred: layer-based check (fast)
        if (obstacleLayers.value != 0)
        {
            var hit = Physics2D.OverlapCircle(position, spawnClearRadius, obstacleLayers);
            if (hit != null) return true;
        }

        // Fallback: tag-based check (if layers not configured)
        var hits = Physics2D.OverlapCircleAll(position, spawnClearRadius);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] != null && hits[i].CompareTag("Obstacle"))
                return true;
        }
        return false;
    }

    // Helper to compare pooled instances and prefabs by base name
    private string SanitizeName(string n)
    {
        return string.IsNullOrEmpty(n) ? string.Empty : n.Replace("(Clone)", string.Empty).Trim();
    }

    private bool IsWithinBounds(Vector3 position)
    {
        Vector3Int cellPosition = floorTilemap.WorldToCell(position);

        // Check if there's actually a floor tile at this position
        return floorTilemap.HasTile(cellPosition) && !wallTilemap.HasTile(cellPosition);
    }
}
