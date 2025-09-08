using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawning : MonoBehaviour
{
    public List<GameObject> disabledEnemies = new List<GameObject>();
    private int wallet = 10;
    private Transform playerTransform;
    [SerializeField] private float spawnRadius = 10f;
    private float waveInterval;
    [SerializeField] private float waveIntervalMax = 15f;
    [SerializeField] private float waveIntervalMin = 5f;

    private List<string> enemyNames = new List<string>();

    private void Start()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Enemy"))
            {
                disabledEnemies.Add(obj);
            }
        }
        // Load all enemy prefab names from Resources/Enemies
        enemyNames = Resources.LoadAll<GameObject>("Enemies")
            .Select(go => go.name)
            .ToList();

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
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
            string enemyToSpawnName = affordableEnemies[Random.Range(0, affordableEnemies.Count)].name;

            GameObject disabledEnemy = disabledEnemies.FirstOrDefault(e => e.name.Contains(enemyToSpawnName));
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
                GameObject enemyPrefab = Resources.Load<GameObject>("Enemies/" + enemyToSpawnName);
                enemyInstance = Instantiate(enemyPrefab, GetRandomSpawnPosition(), Quaternion.identity);
            }

            wallet -= enemyInstance.GetComponent<EnemyController>().stats.Cost;
        } while (wallet > 0 && affordableEnemies.Count > 0);
    }


    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        return playerTransform.position + (Vector3)(randomDirection * spawnRadius);
    }
}
