using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public class EnemySpawning : MonoBehaviour
{
    public List<GameObject> disabledEnemies = new List<GameObject>();
    private int wallet = 10;
    private Transform playerTransform;
    [SerializeField] private float spawnRadius = 8f;
    private float waveInterval;
    [SerializeField] private float waveIntervalMax = 15f;
    [SerializeField] private float waveIntervalMin = 5f;

    [Header("Spawn Bounds")]
    [SerializeField] private Transform[] spawnBoundsMarkers = new Transform[4];
    [SerializeField] private int spawnRetryAttempts = 8;

    private float minX, maxX, minY, maxY;

    private List<string> enemyNames = new List<string>();
    private Transform enemiesParent;

    private void Awake()
    {
        // One-time setup and cache references
        getSpawnBounds();

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
        // Preferred behavior: pick a point on a circle around the player; if out of bounds, retry
        // Generate a candidate on the circle (or slightly randomized ring)
        for (int attempt = 0; attempt < Mathf.Max(1, spawnRetryAttempts); attempt++)
        {
            angle = Random.Range(0f, Mathf.PI * 2f);
            // small jitter on radius to avoid perfect ring overlap
            radius = spawnRadius * Random.Range(0.8f, 1.2f);
            candidate = playerTransform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;

            if (IsWithinBounds(candidate, minX, maxX, minY, maxY))
            {
                return candidate;
            }
        }

        // If we failed all attempts and we have bounds, clamp to the nearest in-bounds point
        candidate = playerTransform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * spawnRadius;
        float clampedX = Mathf.Clamp(candidate.x, minX, maxX);
        float clampedY = Mathf.Clamp(candidate.y, minY, maxY);
        return new Vector3(clampedX, clampedY, 0f);
    }

    // Helper to compare pooled instances and prefabs by base name
    private static string SanitizeName(string n)
    {
        return string.IsNullOrEmpty(n) ? string.Empty : n.Replace("(Clone)", string.Empty).Trim();
    }

    private static bool IsWithinBounds(Vector3 position, float minX, float maxX, float minY, float maxY)
    {
        return position.x >= minX && position.x <= maxX && position.y >= minY && position.y <= maxY;
    }

    private void getSpawnBounds()
    {
        foreach (Transform marker in spawnBoundsMarkers)
        {
            if (marker == null) continue;
            if (marker.position.x < minX) minX = marker.position.x;
            if (marker.position.x > maxX) maxX = marker.position.x;
            if (marker.position.y < minY) minY = marker.position.y;
            if (marker.position.y > maxY) maxY = marker.position.y;
        }
    }

    // Visualize the spawn bounds in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.pink;
        Vector3 a = new Vector3(minX, minY, 0f);
        Vector3 b = new Vector3(maxX, minY, 0f);
        Vector3 c = new Vector3(maxX, maxY, 0f);
        Vector3 d = new Vector3(minX, maxY, 0f);

        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(d, a);
    }
}
