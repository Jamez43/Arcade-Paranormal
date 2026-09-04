using UnityEngine;

public class ExtensionCordButton : MonoBehaviour
{
    public void updateCooldown()
    {
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player.Stats.ApplyCooldownUpgrade(0.9f);
        Debug.Log("Cooldown decreased to: " + player.Stats.Cooldown);

        player.CompleteUpgradeSelection();
    }
}
