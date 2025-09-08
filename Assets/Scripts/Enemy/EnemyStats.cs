using UnityEngine;

public abstract class EnemyStats : ScriptableObject
{
    public abstract float MaxHealth { get; }
    public abstract float Damage { get; }
    public abstract float Speed { get; }
    public abstract float AttackDelay { get; }
    public abstract float Defense { get; }
    public abstract string AttackType { get; }
}