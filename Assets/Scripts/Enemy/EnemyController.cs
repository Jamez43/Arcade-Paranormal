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
                enemySpawning.disabledEnemies.Add(gameObject);
            }

            GameObject xpPrefab = Resources.Load<GameObject>("XP");
            if (XPController.disabledXP.Count == 0)
            {
                Instantiate(xpPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                GameObject xpInstance = XPController.disabledXP[0];
                xpInstance.transform.position = transform.position;
                xpInstance.SetActive(true);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerController player = collision.collider.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ApplyDamage(stats.Damage);
            }
        }
    }
}
