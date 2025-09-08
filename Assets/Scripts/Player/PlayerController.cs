using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    private float currentHealth;
    private HealthBar healthBar;
    private XPBar xpBar;
    private float currentXP = 0f;
    private float levelUpXPThreshold = 100f;


    private void OnEnable()
    {
        healthBar = GetComponentInChildren<HealthBar>(includeInactive: true);
        xpBar = GetComponentInChildren<XPBar>(includeInactive: true);
        currentHealth = stats.MaxHealth;
        xpBar.UpdateXPBar(currentXP, levelUpXPThreshold);
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

    public void AddXP(float amount)
    {
        currentXP += amount;
        xpBar.UpdateXPBar(currentXP, levelUpXPThreshold);
        if (currentXP >= levelUpXPThreshold)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        currentXP -= levelUpXPThreshold;
        levelUpXPThreshold *= 1.5f;
        xpBar.UpdateXPBar(currentXP, levelUpXPThreshold);
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
        }
    }
}
