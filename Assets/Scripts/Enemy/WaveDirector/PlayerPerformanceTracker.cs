using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerPerformanceTracker : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float recentDamageWindowSeconds = 10f;
    [SerializeField, Min(1f)] private float killRateWindowSeconds = 60f;

    private readonly Queue<TimedDamage> recentDamage = new Queue<TimedDamage>();
    private readonly Queue<float> recentKills = new Queue<float>();
    private PlayerController player;
    private float runStartedAt;
    private float lastHitAt = float.NegativeInfinity;

    public float TotalDamageTaken { get; private set; }
    public event Action PlayerDied;

    public void Initialize(PlayerController playerController)
    {
        if (playerController == null)
        {
            throw new ArgumentNullException(nameof(playerController));
        }

        UnsubscribeFromPlayer();
        player = playerController;
        player.DamageTaken += OnDamageTaken;
        player.Died += OnPlayerDied;
        runStartedAt = Time.time;
        lastHitAt = float.NegativeInfinity;
        TotalDamageTaken = 0f;
        recentDamage.Clear();
        recentKills.Clear();
    }

    private void OnEnable()
    {
        EnemyController.EnemyKilled += OnEnemyKilled;
    }

    private void OnDisable()
    {
        EnemyController.EnemyKilled -= OnEnemyKilled;
    }

    private void OnDestroy()
    {
        EnemyController.EnemyKilled -= OnEnemyKilled;
        UnsubscribeFromPlayer();
    }

    public PlayerPerformanceState CaptureState()
    {
        RemoveExpiredSamples();

        float recentDamageTotal = 0f;
        foreach (TimedDamage sample in recentDamage)
        {
            recentDamageTotal += sample.Amount;
        }

        float elapsedSinceHit = float.IsNegativeInfinity(lastHitAt)
            ? float.PositiveInfinity
            : Time.time - lastHitAt;

        return new PlayerPerformanceState
        {
            PlayerHealthPercent = player != null ? player.HealthPercent : 0f,
            PlayerCurrentHealth = player != null ? player.CurrentHealth : 0f,
            DamageTakenRecently = recentDamageTotal,
            KillsPerMinute = recentKills.Count * (60f / killRateWindowSeconds),
            CurrentEnemyCount = EnemyController.ActiveEnemyCount,
            SurvivalTime = Time.time - runStartedAt,
            TimeSinceLastHit = elapsedSinceHit
        };
    }

    private void OnDamageTaken(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        recentDamage.Enqueue(new TimedDamage(Time.time, amount));
        TotalDamageTaken += amount;
        lastHitAt = Time.time;
        RemoveExpiredSamples();
    }

    private void OnEnemyKilled(EnemyController enemy)
    {
        recentKills.Enqueue(Time.time);
        RemoveExpiredSamples();
    }

    private void OnPlayerDied()
    {
        PlayerDied?.Invoke();
    }

    private void RemoveExpiredSamples()
    {
        float damageCutoff = Time.time - recentDamageWindowSeconds;
        while (recentDamage.Count > 0 && recentDamage.Peek().Time < damageCutoff)
        {
            recentDamage.Dequeue();
        }

        float killCutoff = Time.time - killRateWindowSeconds;
        while (recentKills.Count > 0 && recentKills.Peek() < killCutoff)
        {
            recentKills.Dequeue();
        }
    }

    private void UnsubscribeFromPlayer()
    {
        if (player == null)
        {
            return;
        }

        player.DamageTaken -= OnDamageTaken;
        player.Died -= OnPlayerDied;
        player = null;
    }

    private readonly struct TimedDamage
    {
        public float Time { get; }
        public float Amount { get; }

        public TimedDamage(float time, float amount)
        {
            Time = time;
            Amount = amount;
        }
    }
}
