using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GamerFuelButton : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    private GameObject parentPanel;

    public void updateSpeed()
    {
        playerStats.Speed *= 1.1f;
        Debug.Log("Speed increased to: " + playerStats.Speed);

        parentPanel = transform.parent.gameObject;
        parentPanel.SetActive(false);
        PauseManager.instance.UnPauseGame();

    }
}
