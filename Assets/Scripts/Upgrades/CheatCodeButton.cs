using UnityEngine;

public class CheatCodeButton : MonoBehaviour
{
    public void updateDamage()
    {
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player.Stats.ApplyDamageUpgrade(1.1f);
        Debug.Log("Damage increased to: " + player.Stats.Damage);

        player.CompleteUpgradeSelection();
    }
}
