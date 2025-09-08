using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    private float currentHealth;
    private HealthBar healthBar;


    private void OnEnable()
    {
        healthBar = GetComponentInChildren<HealthBar>(includeInactive: true);
        currentHealth = stats.MaxHealth;
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
