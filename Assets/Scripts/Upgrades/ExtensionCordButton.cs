using UnityEngine;

public class ExtensionCordButton : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    private GameObject parentPanel;

    public void updateCooldown()
    {
        playerStats.Cooldown *= 0.9f;
        Debug.Log("Cooldown increased to: " + playerStats.Cooldown);

        parentPanel = transform.parent.gameObject;
        parentPanel.SetActive(false);
        PauseManager.instance.UnPauseGame();

    }
}
