using System;
using System.Collections.Generic;
using System.Linq;

public sealed class WaveGenerator
{
    public List<WaveCandidate> GenerateCandidates(
        IReadOnlyList<EnemySpawnDefinition> availableEnemies,
        int wallet,
        int candidateCount,
        Random random)
    {
        if (availableEnemies == null)
        {
            throw new ArgumentNullException(nameof(availableEnemies));
        }

        if (random == null)
        {
            throw new ArgumentNullException(nameof(random));
        }

        List<EnemySpawnDefinition> validEnemies = availableEnemies
            .Where(enemy => enemy != null
                && enemy.Stats != null
                && enemy.Stats.Cost > 0
                && enemy.Stats.Cost <= wallet)
            .ToList();

        List<WaveCandidate> candidates = new List<WaveCandidate>();
        if (wallet <= 0 || candidateCount <= 0 || validEnemies.Count == 0)
        {
            return candidates;
        }

        int minimumEnemyCost = validEnemies.Min(enemy => enemy.Stats.Cost);

        for (int i = 0; i < candidateCount; i++)
        {
            WaveCandidate candidate = new WaveCandidate();
            int remainingBudget = wallet;

            while (remainingBudget >= minimumEnemyCost)
            {
                List<EnemySpawnDefinition> affordable = validEnemies
                    .Where(enemy => enemy.Stats.Cost <= remainingBudget)
                    .ToList();

                if (affordable.Count == 0)
                {
                    break;
                }

                EnemySpawnDefinition chosen = affordable[random.Next(affordable.Count)];
                candidate.Add(chosen);
                remainingBudget -= chosen.Stats.Cost;
            }

            candidate.CalculateMetrics(wallet, validEnemies.Count, minimumEnemyCost);
            candidates.Add(candidate);
        }

        return candidates;
    }
}
