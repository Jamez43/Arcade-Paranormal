using System.Collections.Generic;
using UnityEngine;

public class CabinetProjectileCollisions : MonoBehaviour
{
    [SerializeField] private EnemyStats enemyStats;
    private CabinetAttack cabinetAttack;

    private void Awake()
    {
        cabinetAttack = GetComponentInParent<CabinetAttack>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().ApplyDamage(enemyStats.Damage);
            DisableProjectile();
        }
        else if (collision.CompareTag("Obstacle"))
        {
            DisableProjectile();
        }
    }

    private void DisableProjectile()
    {
        // Reparent to the Projectiles folder before disabling
        transform.SetParent(cabinetAttack.transform.Find("Projectiles"));

        gameObject.SetActive(false);
        cabinetAttack.disabledProjectiles.Add(gameObject);
        cabinetAttack.activeProjectiles.Remove(gameObject);

    }
}
