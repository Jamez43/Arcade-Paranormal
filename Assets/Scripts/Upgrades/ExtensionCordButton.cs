using UnityEngine;

public class ExtensionCordButton : MonoBehaviour
{
    private GameObject parentPanel;

    public void updateCooldown()
    {
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player.Stats.ApplyCooldownUpgrade(0.9f);
        Debug.Log("Cooldown decreased to: " + player.Stats.Cooldown);

        parentPanel = transform.parent.gameObject;
        parentPanel.SetActive(false);
        PauseManager.instance.UnPauseGame();
    }
}
