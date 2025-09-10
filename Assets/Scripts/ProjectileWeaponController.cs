using UnityEngine;
using System.Collections.Generic;
using System.Numerics;

public class ProjectileWeaponController : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private GameObject projectilePrefab;
    private CooldownBar cooldownBar;
    private float elapsedTime;
    public List<GameObject> disabledProjectiles = new List<GameObject>();
    public List<GameObject> activeProjectiles = new List<GameObject>();

    private void Awake()
    {
        cooldownBar = Object.FindAnyObjectByType<CooldownBar>();
        cooldownBar.UpdateCooldownBar(playerStats.AttackDelay, playerStats.AttackDelay);

        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Projectile"))
            {
                disabledProjectiles.Add(obj);
            }
        }
    }

    private void Update()
    {
        RemoveOffScreenProjectiles();
        elapsedTime += Time.deltaTime;
        cooldownBar.UpdateCooldownBar(elapsedTime, playerStats.AttackDelay);

        if (elapsedTime >= playerStats.AttackDelay)
        {
            elapsedTime = 0f;
            FireWeapon();
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
        }

        UnityEngine.Vector2 direction = GetClosestEnemyDirection();
        projectile.transform.position = transform.position + 0.5f * UnityEngine.Vector3.right; // Offset to avoid immediate collision with player
        projectile.transform.up = direction; // Align the projectile's up direction with the firing direction

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
        }

        activeProjectiles.Add(projectile);
    }

    private UnityEngine.Vector2 GetClosestEnemyDirection()
    {
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        UnityEngine.Vector3 weaponPosition = transform.position;

        foreach (EnemyController enemy in FindObjectsByType<EnemyController>(FindObjectsSortMode.None))
        {
            UnityEngine.Vector3 enemyScreenPos = Camera.main.WorldToViewportPoint(enemy.transform.position);
            bool onScreen = enemyScreenPos.z > 0 &&
                            enemyScreenPos.x > 0 && enemyScreenPos.x < 1 &&
                            enemyScreenPos.y > 0 && enemyScreenPos.y < 1;

            if (onScreen)
            {
                float distance = UnityEngine.Vector3.Distance(weaponPosition, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy.gameObject;
                }
            }
        }

        if (closestEnemy != null)
        {
            return (closestEnemy.transform.position - transform.position).normalized;
        }
        else
        {
            return Random.insideUnitCircle.normalized;
        }
    }

    private void RemoveOffScreenProjectiles()
    {
        for (int i = activeProjectiles.Count - 1; i >= 0; i--)
        {
            GameObject projectile = activeProjectiles[i];
            UnityEngine.Vector3 screenPos = Camera.main.WorldToViewportPoint(projectile.transform.position);
            float buffer = .5f;
            bool onScreen = screenPos.z > 0 &&
                            screenPos.x > -buffer && screenPos.x < 1 + buffer &&
                            screenPos.y > -buffer && screenPos.y < 1 + buffer;

            if (!onScreen)
            {
                projectile.SetActive(false);
                disabledProjectiles.Add(projectile);
                activeProjectiles.RemoveAt(i);
            }
        }
    }

    /*
        private void OnDrawGizmos()
        {
            if (Camera.main == null) return;
            float buffer = .5f; // Use your current buffer value

            // Draw viewport boundary with buffer
            UnityEngine.Vector3[] corners = new UnityEngine.Vector3[4];
            corners[0] = Camera.main.ViewportToWorldPoint(new UnityEngine.Vector3(-buffer, -buffer, 10));
            corners[1] = Camera.main.ViewportToWorldPoint(new UnityEngine.Vector3(1 + buffer, -buffer, 10));
            corners[2] = Camera.main.ViewportToWorldPoint(new UnityEngine.Vector3(1 + buffer, 1 + buffer, 10));
            corners[3] = Camera.main.ViewportToWorldPoint(new UnityEngine.Vector3(-buffer, 1 + buffer, 10));

            Gizmos.color = Color.red;
            for (int i = 0; i < 4; i++)
            {
                Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
            }
        }
        */
}
