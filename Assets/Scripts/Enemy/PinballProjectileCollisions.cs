using System.Collections.Generic;
using UnityEngine;

public class PinballProjectileCollisions : MonoBehaviour
{
    [SerializeField] private EnemyStats enemyStats;
    private PinballAttack pinballAttack;
    private Rigidbody2D rb;

    private void Awake()
    {
        pinballAttack = GetComponentInParent<PinballAttack>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().ApplyDamage(enemyStats.Damage);
            rb.linearVelocity = -rb.linearVelocity;
        }
        else if (collision.CompareTag("Obstacle"))
        {
            rb.linearVelocity = -rb.linearVelocity;
        }
    }

    private void DisableProjectile()
    {
        // Reparent to the Projectiles folder before disabling
        transform.SetParent(pinballAttack.transform.Find("Projectiles"));

        gameObject.SetActive(false);
        pinballAttack.disabledProjectiles.Add(gameObject);
        pinballAttack.activeProjectiles.Remove(gameObject);

    }

}
