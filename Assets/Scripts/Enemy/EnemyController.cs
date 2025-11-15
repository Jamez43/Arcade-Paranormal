using UnityEngine;
public class EnemyController : MonoBehaviour
{
    [SerializeField] public EnemyStats stats;
    public float currentHealth { get; private set; }
    private HealthBar healthBar;
    private Collider2D enemyCollider;
    private XPController XPController;

    private void Awake()
    {
        healthBar = GetComponentInChildren<HealthBar>(includeInactive: true);
        enemyCollider = GetComponent<Collider2D>();
        XPController = FindFirstObjectByType<XPController>();
    }
    private void OnEnable()
    {
        currentHealth = stats.MaxHealth;
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
        enemyCollider.enabled = false;
    }

    private void Update()
    {
        CheckDie();
    }

    private void CheckDie()
    {
        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);

            EnemySpawning enemySpawning = FindFirstObjectByType<EnemySpawning>();
            if (enemySpawning != null)
            {
                if (!enemySpawning.disabledEnemies.Contains(gameObject))
                {
                    enemySpawning.disabledEnemies.Add(gameObject);
                }
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
