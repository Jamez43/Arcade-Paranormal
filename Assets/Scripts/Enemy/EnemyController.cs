using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyStats stats;
    public float currentHealth { get; private set; }
    private HealthBar healthBar;

    private void OnEnable()
    {
        currentHealth = stats.MaxHealth;
        healthBar = GetComponentInChildren<HealthBar>(includeInactive: true);
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

    private void Update()
    {
        CheckDie();
    }

    private void CheckDie()
    {
        if (currentHealth <= 0)
        {
            Debug.Log("Enemy " + gameObject.name + " died.");
            gameObject.SetActive(false);
        }
    }
}
