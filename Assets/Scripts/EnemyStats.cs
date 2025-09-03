using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/DefaultStats")]
public class EnemyDefaultStats : ScriptableObject
{
    public float maxHealth = 10f;
    public float damage = 5f;
    public float speed = 1f;
    public float attackDelay = 0f;
    public float defense = 0f;
    public string attackType = "Melee";

}

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private EnemyDefaultStats stats;
    public float currentHealth { get; private set; }

    private void OnEnable()
    {
        currentHealth = stats.maxHealth;
    }

    public void ApplyDamage(float damageAmount)
    {
        float damageAfterDefense = damageAmount * (1 - stats.defense);
        if (damageAfterDefense > 0)
        {
            currentHealth -= damageAfterDefense;
            HealthBar healthBar = GetComponentInChildren<HealthBar>(includeInactive: true);
            healthBar.UpdateHealthBar(currentHealth, stats.maxHealth);
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
