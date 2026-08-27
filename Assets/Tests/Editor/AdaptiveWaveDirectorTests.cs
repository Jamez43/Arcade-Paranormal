using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class AdaptiveWaveDirectorTests
{
    private readonly List<ScriptableObject> createdStats = new List<ScriptableObject>();

    [TearDown]
    public void TearDown()
    {
        foreach (ScriptableObject stats in createdStats)
        {
            UnityEngine.Object.DestroyImmediate(stats);
        }

        createdStats.Clear();
    }

    [Test]
    public void PressureProfileNormalizesDpsAgainstHighestAvailableEnemy()
    {
        AdaptiveWaveTestEnemyStats stats = CreateStats(
            "Ranged",
            health: 10f,
            damage: 5f,
            speed: 1f,
            attackDelay: 2f,
            defense: 0f,
            cost: 2,
            isRanged: true);

        EnemyPressureProfile profile = EnemyPressureProfile.FromStats(
            stats,
            referenceSpeed: 1f,
            referenceHealth: 10f,
            highestAvailableDps: 3.125f);

        Assert.That(profile.SwarmPressure, Is.EqualTo(0.5f).Within(0.0001f));
        Assert.That(profile.RangedPressure, Is.EqualTo(1f));
        Assert.That(profile.SpeedPressure, Is.EqualTo(1f));
        Assert.That(profile.Tankiness, Is.EqualTo(1f));
        Assert.That(profile.DamagePressure, Is.EqualTo(0.8f).Within(0.0001f));
        Assert.That(profile.ThreatScore, Is.EqualTo(0.86f).Within(0.0001f));
    }

    [Test]
    public void GeneratorBuildsRequestedCandidatesWithinWallet()
    {
        AdaptiveWaveTestEnemyStats cheapStats = CreateStats(
            "Cheap",
            health: 5f,
            damage: 2f,
            speed: 1f,
            attackDelay: 1f,
            defense: 0f,
            cost: 1,
            isRanged: false);
        AdaptiveWaveTestEnemyStats expensiveStats = CreateStats(
            "Expensive",
            health: 10f,
            damage: 4f,
            speed: 0.5f,
            attackDelay: 1f,
            defense: 0f,
            cost: 3,
            isRanged: true);

        List<EnemySpawnDefinition> enemies = new List<EnemySpawnDefinition>
        {
            CreateDefinition(cheapStats, 4f),
            CreateDefinition(expensiveStats, 4f)
        };

        WaveGenerator generator = new WaveGenerator();
        List<WaveCandidate> candidates = generator.GenerateCandidates(
            enemies,
            wallet: 10,
            candidateCount: 20,
            random: new System.Random(1234));

        Assert.That(candidates, Has.Count.EqualTo(20));
        foreach (WaveCandidate candidate in candidates)
        {
            Assert.That(candidate.TotalCost, Is.EqualTo(10));
            Assert.That(candidate.Enemies, Is.Not.Empty);
            Assert.That(candidate.EstimatedDifficulty, Is.InRange(0f, 1f));
            Assert.That(candidate.VarietyScore, Is.InRange(0f, 1f));
            Assert.That(candidate.PressureScore, Is.InRange(0f, 1f));
        }
    }

    [Test]
    public void GeneratorReturnsNoCandidatesWhenNothingIsAffordable()
    {
        AdaptiveWaveTestEnemyStats stats = CreateStats(
            "Expensive",
            health: 10f,
            damage: 4f,
            speed: 0.5f,
            attackDelay: 1f,
            defense: 0f,
            cost: 3,
            isRanged: true);

        WaveGenerator generator = new WaveGenerator();
        List<WaveCandidate> candidates = generator.GenerateCandidates(
            new List<EnemySpawnDefinition> { CreateDefinition(stats, 4f) },
            wallet: 2,
            candidateCount: 20,
            random: new System.Random(1234));

        Assert.That(candidates, Is.Empty);
    }

    [Test]
    public void TargetDifficultyRespondsToDominatingAndStrugglingStates()
    {
        WaveFitnessScorer scorer = new WaveFitnessScorer(new WaveFitnessSettings());

        PlayerPerformanceState dominating = new PlayerPerformanceState
        {
            PlayerHealthPercent = 0.9f,
            KillsPerMinute = 21f,
            DamageTakenRecently = 0f,
            CurrentEnemyCount = 0
        };
        PlayerPerformanceState struggling = new PlayerPerformanceState
        {
            PlayerHealthPercent = 0.5f,
            KillsPerMinute = 0f,
            DamageTakenRecently = 21f,
            CurrentEnemyCount = 31
        };

        Assert.That(scorer.GetTargetDifficulty(dominating), Is.EqualTo(0.9f).Within(0.0001f));
        Assert.That(scorer.GetTargetDifficulty(struggling), Is.EqualTo(0.05f).Within(0.0001f));
    }

    private AdaptiveWaveTestEnemyStats CreateStats(
        string name,
        float health,
        float damage,
        float speed,
        float attackDelay,
        float defense,
        int cost,
        bool isRanged)
    {
        AdaptiveWaveTestEnemyStats stats = ScriptableObject.CreateInstance<AdaptiveWaveTestEnemyStats>();
        stats.name = name;
        stats.Configure(health, damage, speed, attackDelay, defense, cost, isRanged);
        createdStats.Add(stats);
        return stats;
    }

    private static EnemySpawnDefinition CreateDefinition(EnemyStats stats, float highestDps)
    {
        EnemyPressureProfile pressure = EnemyPressureProfile.FromStats(stats, 1f, 10f, highestDps);
        return new EnemySpawnDefinition(null, stats, pressure);
    }
}

public sealed class AdaptiveWaveTestEnemyStats : EnemyStats
{
    private float maxHealth;
    private float damage;
    private float speed;
    private float attackDelay;
    private float defense;
    private int cost;
    private bool isRanged;

    public override float MaxHealth => maxHealth;
    public override float Damage => damage;
    public override float Speed => speed;
    public override float AttackDelay => attackDelay;
    public override float Defense => defense;
    public override int Cost => cost;
    public override bool IsRanged => isRanged;

    public void Configure(
        float health,
        float damageValue,
        float speedValue,
        float attackDelayValue,
        float defenseValue,
        int costValue,
        bool ranged)
    {
        maxHealth = health;
        damage = damageValue;
        speed = speedValue;
        attackDelay = attackDelayValue;
        defense = defenseValue;
        cost = costValue;
        isRanged = ranged;
    }
}
