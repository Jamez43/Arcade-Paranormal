using System;
using UnityEngine;

[Serializable]
public struct EnemyPressureProfile
{
    public float SwarmPressure;
    public float RangedPressure;
    public float SpeedPressure;
    public float Tankiness;
    public float DamagePressure;
    public float ThreatScore;

    public static EnemyPressureProfile FromStats(
        EnemyStats stats,
        float referenceSpeed,
        float referenceHealth,
        float highestAvailableDps)
    {
        if (stats == null)
        {
            throw new ArgumentNullException(nameof(stats));
        }

        float dps = CalculateDps(stats);
        float effectiveHealth = stats.MaxHealth / Mathf.Max(0.01f, 1f - stats.Defense);

        EnemyPressureProfile profile = new EnemyPressureProfile
        {
            SwarmPressure = stats.Cost > 0 ? Mathf.Clamp01(1f / stats.Cost) : 0f,
            RangedPressure = stats.IsRanged ? 1f : 0f,
            SpeedPressure = Normalize(stats.Speed, referenceSpeed),
            Tankiness = Normalize(effectiveHealth, referenceHealth),
            DamagePressure = Normalize(dps, highestAvailableDps)
        };

        profile.ThreatScore = (
            profile.SwarmPressure
            + profile.RangedPressure
            + profile.SpeedPressure
            + profile.Tankiness
            + profile.DamagePressure) / 5f;

        return profile;
    }

    public static float CalculateDps(EnemyStats stats)
    {
        if (stats == null || stats.AttackDelay <= 0f)
        {
            return 0f;
        }

        return Mathf.Max(0f, stats.Damage) / stats.AttackDelay;
    }

    private static float Normalize(float value, float referenceValue)
    {
        if (referenceValue <= 0f)
        {
            return 0f;
        }

        return Mathf.Clamp01(Mathf.Max(0f, value) / referenceValue);
    }
}

public sealed class EnemySpawnDefinition
{
    public GameObject Prefab { get; }
    public EnemyStats Stats { get; }
    public EnemyPressureProfile Pressure { get; }
    public string Name => Prefab != null ? Prefab.name : Stats.name;

    public EnemySpawnDefinition(GameObject prefab, EnemyStats stats, EnemyPressureProfile pressure)
    {
        Prefab = prefab;
        Stats = stats;
        Pressure = pressure;
    }
}
