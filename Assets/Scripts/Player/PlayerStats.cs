using UnityEngine;

public abstract class PlayerStats : ScriptableObject
{
    public abstract float MaxHealth { get; set; }
    public abstract float Damage { get; set; }
    public abstract float Speed { get; set; }
    public abstract float Cooldown { get; set; }
    public abstract float Defense { get; set; } //Percentage
    public abstract string AttackType { get; set; }
    public abstract float PickupRange { get; set; }

    public abstract void ResetStats();
}
