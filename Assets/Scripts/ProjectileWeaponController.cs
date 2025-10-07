using UnityEngine;
using System.Collections.Generic;

public class ProjectileWeaponController : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float spawnOffset = 0.5f; // distance in front of the shooter along aim direction
    [Header("Aiming (like melee)")]
    [SerializeField] private LayerMask enemyLayer;            // set to Enemy layer
    [SerializeField] private float targetSearchRadius = 30f;  // how far to search
    [SerializeField] private float arcAngle = 20f;            // cone half-angle (degrees)
    private CooldownBar cooldownBar;
    private float elapsedTime;
    public List<GameObject> disabledProjectiles = new List<GameObject>();
    public List<GameObject> activeProjectiles = new List<GameObject>();
    // Aiming helpers
    private IndicatorController indicator;
    private Transform center;                                  // usually the player (indicator's parent)
    private readonly Collider2D[] _overlapResults = new Collider2D[128];

    private void Awake()
    {
        cooldownBar = Object.FindAnyObjectByType<CooldownBar>();
        cooldownBar.UpdateCooldownBar(playerStats.AttackDelay, playerStats.AttackDelay);

        // Find the aim indicator (same pattern as melee)
        indicator = GetComponentInChildren<IndicatorController>();
        center = indicator != null ? indicator.transform.parent : transform;

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

        // Aim like melee: pick nearest enemy in cone; else straight along indicator
        UnityEngine.Vector2 direction = GetFireDirectionLikeMelee();
        if (direction.sqrMagnitude < 0.0001f)
        {
            direction = (UnityEngine.Vector2)transform.up; // fallback to forward if direction == 0
        }
        // Spawn from the same origin used for aiming (indicator's parent if present)
        projectile.transform.position = transform.position + (UnityEngine.Vector3)(direction.normalized * spawnOffset);
        projectile.transform.up = direction; // Align the projectile's up direction with the firing direction

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
        }

        activeProjectiles.Add(projectile);
    }

    private UnityEngine.Vector2 GetFireDirectionLikeMelee()
    {
        // Forward direction comes from the indicator; fallback to this object's up
        Vector2 forward = indicator.transform.up;
        // Offset origin downward so cone starts at bottom of model
        Vector3 origin = transform.position;

        // Query nearby enemies using physics for efficiency
        var filter = new ContactFilter2D { useLayerMask = true };
        filter.SetLayerMask(enemyLayer);
        int count = Physics2D.OverlapCircle(origin, targetSearchRadius, filter, _overlapResults);

        float shortestDistance = float.PositiveInfinity;
        Transform best = null;

        for (int i = 0; i < count; i++)
        {
            var col = _overlapResults[i];
            if (col == null) continue;
            Transform transform;
            //ensure you're using the parent transform
            if (col.attachedRigidbody != null)
            {
                transform = col.attachedRigidbody.transform;
            }
            else
            {
                transform = col.transform;
            }
            if (!transform.gameObject.activeInHierarchy) continue;
            if (!transform.CompareTag("Enemy")) continue;

            Vector2 toEnemy = (Vector2)(transform.position - origin);
            float angle = Vector2.Angle(forward, toEnemy);
            if (angle > arcAngle) continue;

            float distance = toEnemy.sqrMagnitude;
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                best = transform;
            }
        }

        if (best != null)
        {
            return ((Vector2)(best.position - origin)).normalized;
        }

        // No enemy in cone: shoot straight along indicator
        return forward.normalized;
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

    private void OnDrawGizmosSelected()
    {
        // Visualize targeting radius and aim cone in Scene view
        Transform c = center != null ? center : transform;
        Vector3 centerPos = c.position;
        Vector3 forward = indicator != null ? indicator.transform.up : transform.up;

        // Cone
        int segments = 40;
        float angleStep = (arcAngle * 2f) / segments;
        Vector3 prevPoint = centerPos + (Quaternion.AngleAxis(-arcAngle, Vector3.forward) * forward * targetSearchRadius);
        Gizmos.color = Color.yellow;
        for (int i = 1; i <= segments; i++)
        {
            float angle = -arcAngle + angleStep * i;
            Vector3 nextPoint = centerPos + (Quaternion.AngleAxis(angle, Vector3.forward) * forward * targetSearchRadius);
            Gizmos.DrawLine(centerPos, nextPoint);
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}
