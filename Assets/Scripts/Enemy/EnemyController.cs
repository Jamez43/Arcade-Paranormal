using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyStats stats;
    public float currentHealth { get; private set; }
    private HealthBar healthBar;
    private Collider2D enemyCollider;
    private void OnEnable()
    {
        currentHealth = stats.MaxHealth;
        healthBar = GetComponentInChildren<HealthBar>(includeInactive: true);
        enemyCollider = GetComponent<Collider2D>();
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
            Debug.Log("Enemy " + gameObject.name + " died.");
            gameObject.SetActive(false);


            //TODO: Change to pooling system
            //Get disabled xp object from pool instead of instantiating new one
            GameObject xpPrefab = Resources.Load<GameObject>("XP");
            Debug.Log("Instantiating XP prefab at " + transform.position);
            Instantiate(xpPrefab, transform.position, Quaternion.identity);
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
