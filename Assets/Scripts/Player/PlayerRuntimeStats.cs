using UnityEngine;

/// <summary>
/// Runtime stats for the player that can be modified during gameplay.
/// Initialized from a base PlayerStats ScriptableObject.
/// </summary>
[System.Serializable]
public class PlayerRuntimeStats
{
    private PlayerStats baseStats;

    // Current runtime values (mutable)
    public float MaxHealth { get; set; }
    public float Damage { get; set; }
    public float Speed { get; set; }
    public float Cooldown { get; set; }
    public float Defense { get; set; }
    public float PickupRange { get; set; }
    public string AttackType { get; private set; }

    public PlayerRuntimeStats(PlayerStats baseStats)
    {
        this.baseStats = baseStats;
        ResetToBaseStats();
    }

    /// <summary>
    /// Reset all runtime stats back to base values from ScriptableObject
    /// </summary>
    public void ResetToBaseStats()
    {
        MaxHealth = baseStats.MaxHealth;
        Damage = baseStats.Damage;
        Speed = baseStats.Speed;
        Cooldown = baseStats.Cooldown;
        Defense = baseStats.Defense;
        PickupRange = baseStats.PickupRange;
        AttackType = baseStats.AttackType;
    }

    // Upgrade methods for easier upgrade management
    public void ApplyDamageUpgrade(float multiplier)
    {
        Damage *= multiplier;
    }

    public void ApplySpeedUpgrade(float multiplier)
    {
        Speed *= multiplier;
    }

    public void ApplyMaxHealthUpgrade(float multiplier)
    {
        MaxHealth *= multiplier;
    }

    public void ApplyCooldownUpgrade(float multiplier)
    {
        Cooldown *= multiplier;
    }

    public void ApplyDefenseUpgrade(float amount)
    {
        Defense = Mathf.Min(Defense + amount, 0.9f); // Cap at 90%
    }

    public void ApplyPickupRangeUpgrade(float multiplier)
    {
        PickupRange *= multiplier;
    }
}
