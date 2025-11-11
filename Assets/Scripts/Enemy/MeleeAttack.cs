using UnityEngine;
public class MeleeAttack : MonoBehaviour
{

    [SerializeField] public EnemyStats stats;
    private float lastDamageTime;

    private void Awake()
    {
        lastDamageTime = -stats.AttackDelay; // Initialize to allow immediate attack
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (PauseManager.instance.isPaused)
        {
            return;
        }
        if (collision.collider.CompareTag("Player"))
        {
            // Check if enough time has passed since last damage
            if (Time.time >= lastDamageTime + stats.AttackDelay)
            {
                PlayerController player = collision.collider.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.ApplyDamage(stats.Damage);
                    lastDamageTime = Time.time; // Update last damage time
                }
            }
        }
    }


}
