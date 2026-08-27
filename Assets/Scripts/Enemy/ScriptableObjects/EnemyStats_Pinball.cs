using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats_Pinball", menuName = "Scriptable Objects/Enemy/Pinball")]
public class EnemyStats_Pinball : EnemyStats
{
    [SerializeField] private float maxHealth = 5f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float speed = .3f;
    [SerializeField] private float attackDelay = 3f;
    [SerializeField] private float defense = 0f;
    [SerializeField] private int cost = 1;
    [SerializeField] private float projectileSpeed = 2f;
    [SerializeField] private float projectileLifetime = 10f;

    public float ProjectileSpeed => projectileSpeed;
    public float ProjectileLifetime => projectileLifetime;
    public override float MaxHealth => maxHealth;
    public override float Damage => damage;
    public override float Speed => speed;
    public override float AttackDelay => attackDelay;
    public override float Defense => defense;
    public override int Cost => cost;
    public override bool IsRanged => true;
}
