using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats_Testing", menuName = "Scriptable Objects/Enemy")]

public abstract class EnemyStats : ScriptableObject
{
    public abstract float MaxHealth { get; }
    public abstract float Damage { get; }
    public abstract float Speed { get; }
    public abstract float AttackDelay { get; }
    public abstract float Defense { get; }
    public abstract string AttackType { get; }
}

[CreateAssetMenu(fileName = "EnemyStats_Testing", menuName = "Scriptable Objects/Enemy/Testing")]
public class EnemyStats_Testing : EnemyStats
{
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float attackDelay = 0f;
    [SerializeField] private float defense = 0f;
    [SerializeField] private string attackType = "Melee";

    public override float MaxHealth => maxHealth;
    public override float Damage => damage;
    public override float Speed => speed;
    public override float AttackDelay => attackDelay;
    public override float Defense => defense;
    public override string AttackType => attackType;
}