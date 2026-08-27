using System.Collections.Generic;
using System.Linq;

public sealed class WaveCandidate
{
    public List<EnemySpawnDefinition> Enemies { get; } = new List<EnemySpawnDefinition>();
    public int TotalCost { get; private set; }
    public float EstimatedDifficulty { get; private set; }
    public float VarietyScore { get; private set; }
    public float PressureScore { get; private set; }

    public void Add(EnemySpawnDefinition enemy)
    {
        Enemies.Add(enemy);
        TotalCost += enemy.Stats.Cost;
    }

    public void CalculateMetrics(int wallet, int availableEnemyTypeCount, int minimumEnemyCost)
    {
        if (Enemies.Count == 0)
        {
            EstimatedDifficulty = 0f;
            VarietyScore = 0f;
            PressureScore = 0f;
            return;
        }

        int maximumEnemyCount = minimumEnemyCost > 0 ? wallet / minimumEnemyCost : 0;
        float totalThreat = Enemies.Sum(enemy => enemy.Pressure.ThreatScore);
        EstimatedDifficulty = maximumEnemyCount > 0
            ? UnityEngine.Mathf.Clamp01(totalThreat / maximumEnemyCount)
            : 0f;

        int uniqueTypes = Enemies.Select(enemy => enemy.Stats).Distinct().Count();
        VarietyScore = availableEnemyTypeCount > 0
            ? UnityEngine.Mathf.Clamp01(uniqueTypes / (float)availableEnemyTypeCount)
            : 0f;

        float count = Enemies.Count;
        float swarm = Enemies.Sum(enemy => enemy.Pressure.SwarmPressure) / count;
        float ranged = Enemies.Sum(enemy => enemy.Pressure.RangedPressure) / count;
        float speed = Enemies.Sum(enemy => enemy.Pressure.SpeedPressure) / count;
        float tankiness = Enemies.Sum(enemy => enemy.Pressure.Tankiness) / count;
        float damage = Enemies.Sum(enemy => enemy.Pressure.DamagePressure) / count;
        float minimum = UnityEngine.Mathf.Min(swarm, ranged, speed, tankiness, damage);
        float maximum = UnityEngine.Mathf.Max(swarm, ranged, speed, tankiness, damage);
        PressureScore = 1f - UnityEngine.Mathf.Clamp01(maximum - minimum);
    }
}
