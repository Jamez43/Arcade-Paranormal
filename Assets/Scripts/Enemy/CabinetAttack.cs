using UnityEngine;
using System.Collections.Generic;
public class CabinetAttack : MonoBehaviour
{
    [SerializeField] private EnemyStats_ArcadeCabinet stats;
    private float elapsedTime;
    [SerializeField] private GameObject projectilePrefab;
    public List<GameObject> disabledProjectiles = new List<GameObject>();
    public List<GameObject> activeProjectiles = new List<GameObject>();

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
            if (child != transform && child.CompareTag("Enemy Cabinet Projectile"))
            {
                disabledProjectiles.Add(child.gameObject);
            }
        }

    }

    private void Update()
    {
        if (PauseManager.instance.isPaused)
        {
            return;
        }
        RemoveOffScreenProjectiles();
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
                transform.SetParent(transform.Find("Projectiles"));
            }
        }
    }

}
