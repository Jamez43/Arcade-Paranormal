using UnityEngine;

public class PocketProtectorButton : MonoBehaviour
{
    public void updateDefense()
    {
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if (player.Stats.Defense == 0)
        {
            player.Stats.ApplyDefenseUpgrade(0.1f);
        }
        else if (player.Stats.Defense < 0.6f)
        {
            player.Stats.ApplyDefenseUpgrade(0.1f);
        }
        Debug.Log("Defense increased to: " + player.Stats.Defense);

        player.CompleteUpgradeSelection();
    }
}
