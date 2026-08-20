using UnityEngine;

public class MeleeWeaponController : MonoBehaviour
{
    [SerializeField] private float radius = 1.5f;
    [SerializeField] private float arcAngle = 75f;        // half-angle of cone
    [SerializeField] private LayerMask enemyLayer;        // assign Enemy layer for efficiency

    private PlayerRuntimeStats playerStats;
    private CooldownBar cooldownBar;
    private Transform center;
    private IndicatorController indicator;
    private float elapsedTime;


    private void Start()
    {
        // Get runtime stats from PlayerController
        playerStats = GetComponentInParent<PlayerController>().Stats;

        indicator = GetComponentInChildren<IndicatorController>();
        if (indicator != null)
        {
            center = indicator.transform.parent; // usually the player
        }
        cooldownBar = FindAnyObjectByType<CooldownBar>();
        cooldownBar.UpdateCooldownBar(playerStats.Cooldown, playerStats.Cooldown);
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        cooldownBar.UpdateCooldownBar(elapsedTime, playerStats.Cooldown);

        // Indicator’s forward direction (up axis after rotation)
        Vector2 forwardDir = indicator.transform.up;

        if (elapsedTime >= playerStats.Cooldown)
        {
            elapsedTime = 0f;
            DetectEnemies(center.position, forwardDir);
        }
    }

    private void DetectEnemies(Vector3 center, Vector2 forwardDir)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius, enemyLayer);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Vector2 toEnemy = (hit.transform.position - center).normalized;
                float angleToEnemy = Vector2.Angle(forwardDir, toEnemy);

                if (angleToEnemy <= arcAngle)
                {
                    dealAttack(hit.GetComponent<EnemyController>());
                }
            }
        }
    }

    private void dealAttack(EnemyController enemyController)
    {
        if (enemyController != null)
        {
            enemyController.ApplyDamage(playerStats.Damage);
        }
    }

    // === Debug Drawing ===
    private void OnDrawGizmos()
    {
        if (indicator == null || center == null) return;

        Vector3 centerPos = center.position;
        Vector3 forward = indicator.transform.up;

        // Draw the arc sector (cone)
        int segments = 40;
        float angleStep = (arcAngle * 2) / segments;
        Vector3 prevPoint = centerPos + (Quaternion.AngleAxis(-arcAngle, Vector3.forward) * forward * radius);
        Gizmos.color = Color.yellow;
        for (int i = 1; i <= segments; i++)
        {
            float angle = -arcAngle + angleStep * i;
            Vector3 nextPoint = centerPos + (Quaternion.AngleAxis(angle, Vector3.forward) * forward * radius);
            Gizmos.DrawLine(centerPos, nextPoint);
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }

        // Draw arc edges
        Gizmos.color = Color.blue;
        Quaternion leftRot = Quaternion.AngleAxis(-arcAngle, Vector3.forward);
        Quaternion rightRot = Quaternion.AngleAxis(arcAngle, Vector3.forward);
        Vector3 leftDir = leftRot * forward * radius;
        Vector3 rightDir = rightRot * forward * radius;
        Gizmos.DrawLine(centerPos, centerPos + leftDir);
        Gizmos.DrawLine(centerPos, centerPos + rightDir);
    }
}
