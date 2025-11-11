using UnityEngine;

public class CircuitOverloadButton : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    private GameObject parentPanel;

    public void updateMaxHealth()
    {
        playerStats.MaxHealth *= 1.1f;
        Debug.Log("Max Health increased to: " + playerStats.MaxHealth);

        parentPanel = transform.parent.gameObject;
        parentPanel.SetActive(false);
        PauseManager.instance.UnPauseGame();

    }
}
