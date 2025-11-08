using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
public class PinballAttack : MonoBehaviour
{
    [SerializeField] private EnemyStats_Pinball stats;
    private float elapsedTime;
    [SerializeField] private GameObject projectilePrefab;
    public List<GameObject> disabledProjectiles = new List<GameObject>();
    public List<GameObject> activeProjectiles = new List<GameObject>();
    // Track when each projectile was fired
    private readonly Dictionary<GameObject, float> spawnTimes = new Dictionary<GameObject, float>();

    private void Start()
    {
        elapsedTime = stats.AttackDelay; // Initialize elapsed time
    }

    private void Awake()
    {
        // Search only in children of this GameObject
        Transform[] childTransforms = GetComponentsInChildren<Transform>(true);

        foreach (Transform child in childTransforms)
        {
            if (child != transform && child.CompareTag("Enemy Pinball Projectile"))
            {
                disabledProjectiles.Add(child.gameObject);
            }
        }

    }

    private void Update()
    {
        RemoveExpiredProjectiles();
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= stats.AttackDelay)
        {
            FireWeapon();
            elapsedTime = 0f; // Reset elapsed time after firing
        }
    }


    private void FireWeapon()
    {
        GameObject projectile;
        if (disabledProjectiles.Count > 0)
        {
            projectile = disabledProjectiles[0];
            disabledProjectiles.RemoveAt(0);
            projectile.SetActive(true);
        }
        else
        {
            projectile = Instantiate(projectilePrefab);
            projectile.SetActive(true);
        }

        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        UnityEngine.Vector2 direction = (playerTransform.position - transform.position).normalized;

        // Spawn from the same origin used for aiming (indicator's parent if present)
        projectile.transform.position = transform.position;
        projectile.transform.up = direction; // Align the projectile's up direction with the firing direction

        // Detach from parent so cabinet movement doesn't affect projectile
        projectile.transform.SetParent(null);

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * stats.ProjectileSpeed;

        activeProjectiles.Add(projectile);
        spawnTimes[projectile] = Time.time;
    }

    // Disable projectiles that exceed their uptime (lifetime)
    private void RemoveExpiredProjectiles()
    {
        for (int i = activeProjectiles.Count - 1; i >= 0; i--)
        {
            GameObject projectile = activeProjectiles[i];
            if (Time.time - spawnTimes[projectile] >= stats.ProjectileLifetime)
            {
                projectile.SetActive(false);
                disabledProjectiles.Add(projectile);
                activeProjectiles.RemoveAt(i);
                spawnTimes.Remove(projectile);
                projectile.transform.SetParent(transform.Find("Projectiles"));
            }
        }
    }

}
