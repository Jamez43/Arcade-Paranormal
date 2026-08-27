using System;
using UnityEngine;

[Serializable]
public sealed class WaveFitnessSettings
{
    [Min(1)] public int CandidateCount = 20;

    [Header("Enemy Normalization")]
    [Min(0.01f)] public float ReferenceSpeed = 1f;
    [Min(0.01f)] public float ReferenceHealth = 10f;

    [Header("Fitness Weights")]
    [Range(0f, 1f)] public float DifficultyWeight = 0.6f;
    [Range(0f, 1f)] public float VarietyWeight = 0.25f;
    [Range(0f, 1f)] public float PressureWeight = 0.15f;

    [Header("Target Difficulty Rules")]
    [Range(0f, 1f)] public float BaseTargetDifficulty = 0.6f;
    [Range(0f, 1f)] public float HealthyThreshold = 0.8f;
    public float HealthyAdjustment = 0.2f;
    [Min(0f)] public float HighKillRateThreshold = 20f;
    public float HighKillRateAdjustment = 0.2f;
    [Min(0f)] public float RecentDamageThreshold = 20f;
    public float RecentDamageAdjustment = -0.25f;
    [Min(0)] public int CrowdedEnemyThreshold = 30;
    public float CrowdedAdjustment = -0.2f;
}

public struct WaveFitnessResult
{
    public float Score;
    public float TargetDifficulty;
    public float DifficultyFit;
}

public sealed class WaveFitnessScorer
{
    private readonly WaveFitnessSettings settings;

    public WaveFitnessScorer(WaveFitnessSettings settings)
    {
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public WaveFitnessResult Score(WaveCandidate wave, PlayerPerformanceState state)
    {
        if (wave == null)
        {
            throw new ArgumentNullException(nameof(wave));
        }

        float targetDifficulty = GetTargetDifficulty(state);
        float difficultyFit = 1f - Mathf.Abs(wave.EstimatedDifficulty - targetDifficulty);
        float totalWeight = settings.DifficultyWeight + settings.VarietyWeight + settings.PressureWeight;
        float score = 0f;

        if (totalWeight > 0f)
        {
            score = (
                difficultyFit * settings.DifficultyWeight
                + wave.VarietyScore * settings.VarietyWeight
                + wave.PressureScore * settings.PressureWeight) / totalWeight;
        }

        return new WaveFitnessResult
        {
            Score = Mathf.Clamp01(score),
            TargetDifficulty = targetDifficulty,
            DifficultyFit = Mathf.Clamp01(difficultyFit)
        };
    }

    public float GetTargetDifficulty(PlayerPerformanceState state)
    {
        float target = settings.BaseTargetDifficulty;

        if (state.PlayerHealthPercent > settings.HealthyThreshold)
        {
            target += settings.HealthyAdjustment;
        }

        if (state.KillsPerMinute > settings.HighKillRateThreshold)
        {
            target += settings.HighKillRateAdjustment;
        }

        if (state.DamageTakenRecently > settings.RecentDamageThreshold)
        {
            target += settings.RecentDamageAdjustment;
        }

        if (state.CurrentEnemyCount > settings.CrowdedEnemyThreshold)
        {
            target += settings.CrowdedAdjustment;
        }

        return Mathf.Clamp01(target);
    }
}
