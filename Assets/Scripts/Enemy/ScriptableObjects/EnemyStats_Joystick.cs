using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats_Joystick", menuName = "Scriptable Objects/Enemy/Joystick")]
public class EnemyStats_Joystick : EnemyStats
{
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float attackDelay = .8f;
    [SerializeField] private float defense = 0f;
    [SerializeField] private int cost = 1;

    public override float MaxHealth => maxHealth;
    public override float Damage => damage;
    public override float Speed => speed;
    public override float AttackDelay => attackDelay;
    public override float Defense => defense;
    public override int Cost => cost;
}
