using System;
using UnityEngine;
public class EnemyController : MonoBehaviour
{
    public static event Action<EnemyController> EnemyKilled;
    public static int ActiveEnemyCount { get; private set; }

    [SerializeField] public EnemyStats stats;
    public float currentHealth { get; private set; }
    private HealthBar healthBar;
    private Collider2D enemyCollider;
    private XPController XPController;
    private EnemySpawning enemySpawning;
    private bool isCountedAsActive;
    private bool deathReported;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        EnemyKilled = null;
        ActiveEnemyCount = 0;
    }

    private void Awake()
    {
        healthBar = GetComponentInChildren<HealthBar>(includeInactive: true);
        enemyCollider = GetComponent<Collider2D>();
        XPController = FindAnyObjectByType<XPController>();
        enemySpawning = FindAnyObjectByType<EnemySpawning>();
    }
    private void OnEnable()
    {
        currentHealth = stats.MaxHealth;
        deathReported = false;
        if (!isCountedAsActive)
        {
            ActiveEnemyCount++;
            isCountedAsActive = true;
        }
        enemyCollider.enabled = true;
        healthBar.gameObject.SetActive(false);
    }

    public void ApplyDamage(float damageAmount)
    {
        float damageAfterDefense = damageAmount * (1 - stats.Defense);
        if (damageAfterDefense > 0)
        {
            currentHealth -= damageAfterDefense;
            healthBar.gameObject.SetActive(true);
            healthBar.UpdateHealthBar(currentHealth, stats.MaxHealth);
        }
    }

    private void OnDisable()
    {
        if (isCountedAsActive)
        {
            ActiveEnemyCount = Mathf.Max(0, ActiveEnemyCount - 1);
            isCountedAsActive = false;
        }
        enemyCollider.enabled = false;
    }

    private void Update()
    {
        CheckDie();
    }

    private void CheckDie()
    {
        if (currentHealth <= 0 && !deathReported)
        {
            deathReported = true;
            gameObject.SetActive(false);
            EnemyKilled?.Invoke(this);

            enemySpawning ??= FindAnyObjectByType<EnemySpawning>();
            if (enemySpawning != null)
            {
                enemySpawning.ReturnToPool(gameObject);
            }

            if (XPController.disabledXP.Count == 0)
            {
                GameObject xpPrefab = Resources.Load<GameObject>("XP");
                Instantiate(xpPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                GameObject xpInstance = XPController.disabledXP[0];
                xpInstance.transform.position = transform.position;
                xpInstance.SetActive(true);
                XPController.disabledXP.RemoveAt(0);
            }
        }
    }
}
