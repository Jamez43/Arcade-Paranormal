using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats_Testing", menuName = "Scriptable Objects/Enemy/Testing")]
public class EnemyStats_Testing : EnemyStats
{
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float damage = .2f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float attackDelay = 0f;
    [SerializeField] private float defense = 0f;
    [SerializeField] private int cost = 1;

    public override float MaxHealth => maxHealth;
    public override float Damage => damage;
    public override float Speed => speed;
    public override float AttackDelay => attackDelay;
    public override float Defense => defense;
    public override int Cost => cost;
    public override bool IsRanged => false;
}
