using UnityEngine;

public class GamerFuelButton : MonoBehaviour
{
    public void updateSpeed()
    {
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player.Stats.ApplySpeedUpgrade(1.1f);
        Debug.Log("Speed increased to: " + player.Stats.Speed);

        player.CompleteUpgradeSelection();
    }
}
