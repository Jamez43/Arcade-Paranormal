using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

public sealed class WaveTelemetryLogger : MonoBehaviour
{
    [SerializeField, Range(0.01f, 1f)] private float mostlyClearedThreshold = 0.8f;
    [SerializeField, Min(0f)] private float postWaveDeathWindowSeconds = 10f;
    [SerializeField] private string telemetryFileName = "adaptive_wave_telemetry.csv";

    private readonly Dictionary<int, ActiveWave> activeWaves = new Dictionary<int, ActiveWave>();
    private readonly Dictionary<EnemyController, int> enemyWaveIds = new Dictionary<EnemyController, int>();
    private PlayerPerformanceTracker performanceTracker;
    private string telemetryPath;
    private string runTimestamp;

    public string TelemetryPath => telemetryPath;

    public void Initialize(PlayerPerformanceTracker tracker)
    {
        if (tracker == null)
        {
            throw new ArgumentNullException(nameof(tracker));
        }

        Unsubscribe();
        performanceTracker = tracker;
        performanceTracker.PlayerDied += OnPlayerDied;
        EnemyController.EnemyKilled += OnEnemyKilled;
        runTimestamp = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);

        string telemetryDirectory = Path.Combine(Application.persistentDataPath, "Telemetry");
        Directory.CreateDirectory(telemetryDirectory);
        telemetryPath = Path.Combine(telemetryDirectory, telemetryFileName);
        EnsureHeaderExists();
        Debug.Log($"Adaptive wave telemetry: {telemetryPath}");
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void Update()
    {
        int[] wavesPastDeathWindow = activeWaves
            .Where(pair => pair.Value.MostlyClearedAt >= 0f
                && Time.time - pair.Value.MostlyClearedAt >= postWaveDeathWindowSeconds)
            .Select(pair => pair.Key)
            .ToArray();

        foreach (int waveId in wavesPastDeathWindow)
        {
            CompleteWave(waveId, false);
        }
    }

    public void BeginWave(
        int waveId,
        WaveCandidate candidate,
        PlayerPerformanceState startState,
        WaveFitnessResult fitness,
        IReadOnlyList<EnemyController> spawnedEnemies)
    {
        if (candidate == null || spawnedEnemies == null || spawnedEnemies.Count == 0)
        {
            return;
        }

        ActiveWave wave = new ActiveWave
        {
            WaveId = waveId,
            StartedAt = Time.time,
            SurvivalTimeAtStart = startState.SurvivalTime,
            HealthBefore = startState.PlayerCurrentHealth,
            RecentDamageBefore = startState.DamageTakenRecently,
            TotalDamageBefore = performanceTracker.TotalDamageTaken,
            KillsPerMinuteAtStart = startState.KillsPerMinute,
            ActiveEnemiesAtStart = startState.CurrentEnemyCount,
            TotalCost = candidate.TotalCost,
            EstimatedDifficulty = candidate.EstimatedDifficulty,
            TargetDifficulty = fitness.TargetDifficulty,
            FitnessScore = fitness.Score,
            VarietyScore = candidate.VarietyScore,
            PressureScore = candidate.PressureScore,
            Composition = BuildComposition(candidate),
            TotalEnemies = spawnedEnemies.Count
        };

        activeWaves[waveId] = wave;
        foreach (EnemyController enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                enemyWaveIds[enemy] = waveId;
                wave.Members.Add(enemy);
            }
        }
    }

    private void OnEnemyKilled(EnemyController enemy)
    {
        if (enemy == null || !enemyWaveIds.TryGetValue(enemy, out int waveId))
        {
            return;
        }

        enemyWaveIds.Remove(enemy);
        if (!activeWaves.TryGetValue(waveId, out ActiveWave wave))
        {
            return;
        }

        wave.KilledEnemies++;
        float clearedRatio = wave.KilledEnemies / (float)wave.TotalEnemies;
        if (clearedRatio >= mostlyClearedThreshold && wave.MostlyClearedAt < 0f)
        {
            wave.MostlyClearedAt = Time.time;
        }
    }

    private void OnPlayerDied()
    {
        int[] activeIds = activeWaves.Keys.ToArray();
        foreach (int waveId in activeIds)
        {
            CompleteWave(waveId, true);
        }
    }

    private void CompleteWave(int waveId, bool playerDied)
    {
        if (!activeWaves.TryGetValue(waveId, out ActiveWave wave))
        {
            return;
        }

        PlayerPerformanceState endState = performanceTracker.CaptureState();
        float outcomeAt = wave.MostlyClearedAt >= 0f ? wave.MostlyClearedAt : Time.time;
        float duration = outcomeAt - wave.StartedAt;
        float damageTaken = performanceTracker.TotalDamageTaken - wave.TotalDamageBefore;

        string row = string.Join(",",
            Escape(runTimestamp),
            wave.WaveId.ToString(CultureInfo.InvariantCulture),
            Format(wave.SurvivalTimeAtStart),
            Format(wave.HealthBefore),
            Format(endState.PlayerCurrentHealth),
            Format(damageTaken),
            Format(wave.RecentDamageBefore),
            Format(wave.KillsPerMinuteAtStart),
            wave.ActiveEnemiesAtStart.ToString(CultureInfo.InvariantCulture),
            wave.TotalCost.ToString(CultureInfo.InvariantCulture),
            Format(wave.EstimatedDifficulty),
            Format(wave.TargetDifficulty),
            Format(wave.FitnessScore),
            Format(wave.VarietyScore),
            Format(wave.PressureScore),
            wave.TotalEnemies.ToString(CultureInfo.InvariantCulture),
            wave.KilledEnemies.ToString(CultureInfo.InvariantCulture),
            Format(duration),
            Escape(wave.Composition),
            playerDied ? "1" : "0");

        File.AppendAllText(telemetryPath, row + Environment.NewLine);
        activeWaves.Remove(waveId);

        foreach (EnemyController member in wave.Members)
        {
            if (member != null && enemyWaveIds.TryGetValue(member, out int memberWaveId) && memberWaveId == waveId)
            {
                enemyWaveIds.Remove(member);
            }
        }
    }

    private void EnsureHeaderExists()
    {
        if (File.Exists(telemetryPath) && new FileInfo(telemetryPath).Length > 0)
        {
            return;
        }

        const string header = "run_timestamp,wave_id,survival_time_at_start,health_before,health_after,damage_taken,recent_damage_at_start,kills_per_minute,active_enemies_at_start,wave_cost,estimated_difficulty,target_difficulty,fitness_score,variety_score,pressure_score,total_enemies,killed_enemies,time_to_outcome,enemies,player_died";
        File.WriteAllText(telemetryPath, header + Environment.NewLine);
    }

    private static string BuildComposition(WaveCandidate candidate)
    {
        return string.Join("|", candidate.Enemies
            .GroupBy(enemy => enemy.Name)
            .OrderBy(group => group.Key)
            .Select(group => $"{group.Key}:{group.Count()}"));
    }

    private static string Format(float value)
    {
        return value.ToString("0.####", CultureInfo.InvariantCulture);
    }

    private static string Escape(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        return $"\"{value.Replace("\"", "\"\"")}\"";
    }

    private void Unsubscribe()
    {
        EnemyController.EnemyKilled -= OnEnemyKilled;
        if (performanceTracker != null)
        {
            performanceTracker.PlayerDied -= OnPlayerDied;
        }
    }

    private sealed class ActiveWave
    {
        public int WaveId;
        public float StartedAt;
        public float SurvivalTimeAtStart;
        public float HealthBefore;
        public float RecentDamageBefore;
        public float TotalDamageBefore;
        public float KillsPerMinuteAtStart;
        public int ActiveEnemiesAtStart;
        public int TotalCost;
        public float EstimatedDifficulty;
        public float TargetDifficulty;
        public float FitnessScore;
        public float VarietyScore;
        public float PressureScore;
        public string Composition;
        public int TotalEnemies;
        public int KilledEnemies;
        public float MostlyClearedAt = -1f;
        public HashSet<EnemyController> Members = new HashSet<EnemyController>();
    }
}
