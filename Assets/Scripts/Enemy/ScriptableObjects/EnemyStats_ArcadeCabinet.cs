using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats_ArcadeCabinet", menuName = "Scriptable Objects/Enemy/ArcadeCabinet")]
public class EnemyStats_ArcadeCabinet : EnemyStats
{
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private float speed = .75f;
    [SerializeField] private float attackDelay = 1f;
    [SerializeField] private float defense = 0f;
    [SerializeField] private int cost = 1;
    [SerializeField] private float projectileSpeed = 5f;

    public override float MaxHealth => maxHealth;
    public override float Damage => damage;
    public override float Speed => speed;
    public override float AttackDelay => attackDelay;
    public float ProjectileSpeed => projectileSpeed;
    public override float Defense => defense;
    public override int Cost => cost;
}
