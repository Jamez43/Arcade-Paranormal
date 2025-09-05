using UnityEngine;

public abstract class PlayerStats : ScriptableObject
{
    public abstract float MaxHealth { get; }
    public abstract float Damage { get; }
    public abstract float Speed { get; }
    public abstract float AttackDelay { get; }
    public abstract float Defense { get; }
    public abstract string AttackType { get; }
}

[CreateAssetMenu(fileName = "PlayerStats_Testing", menuName = "Scriptable Objects/Player/Testing")]
public class PlayerStats_Testing : PlayerStats
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float attackDelay = 1f;
    [SerializeField] private float defense = 0f;
    [SerializeField] private string attackType = "Melee";

    public override float MaxHealth => maxHealth;
    public override float Damage => damage;
    public override float Speed => speed;
    public override float AttackDelay => attackDelay;
    public override float Defense => defense;
    public override string AttackType => attackType;
}
