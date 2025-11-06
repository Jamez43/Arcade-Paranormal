using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileCollisions : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    private ProjectileWeaponController projectileWeaponController;

    private void Awake()
    {
        projectileWeaponController = FindFirstObjectByType<ProjectileWeaponController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyController>().ApplyDamage(playerStats.Damage);
            DisableProjectile();
        }
        else if (collision.CompareTag("Obstacle"))
        {
            DisableProjectile();
        }
    }

    private void DisableProjectile()
    {
        gameObject.SetActive(false);
        gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        projectileWeaponController.disabledProjectiles.Add(gameObject);
        projectileWeaponController.activeProjectiles.Remove(gameObject);
        transform.SetParent(projectileWeaponController.transform.Find("Projectiles"));
    }
}
