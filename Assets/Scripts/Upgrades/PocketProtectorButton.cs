using UnityEngine;

public class PocketProtectorButton : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    private GameObject parentPanel;

    public void updateDefense()
    {
        if (playerStats.Defense == 0)
        {
            playerStats.Defense = 0.1f;
        }
        else if (playerStats.Defense < 0.6f)
        {
            playerStats.Defense += .1f;
        }
        Debug.Log("Defense increased to: " + playerStats.Defense);

        parentPanel = transform.parent.gameObject;
        parentPanel.SetActive(false);
        PauseManager.instance.UnPauseGame();

    }
}
