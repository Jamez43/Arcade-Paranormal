using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollisions : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    private List<GameObject> disabledProjectiles;

    private void Awake()
    {
        ProjectileWeaponController projectileWeaponController = FindFirstObjectByType<ProjectileWeaponController>();
        disabledProjectiles = projectileWeaponController.disabledProjectiles;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyController>().ApplyDamage(playerStats.Damage);
            gameObject.SetActive(false);
            disabledProjectiles.Add(gameObject);
        }
        else if (collision.CompareTag("Obstacle"))
        {
            gameObject.SetActive(false);
            disabledProjectiles.Add(gameObject);
        }
    }
}
