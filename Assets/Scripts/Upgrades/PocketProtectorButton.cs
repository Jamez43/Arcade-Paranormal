using UnityEngine;

public class PocketProtectorButton : MonoBehaviour
{
    private GameObject parentPanel;

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

        parentPanel = transform.parent.gameObject;
        parentPanel.SetActive(false);
        PauseManager.instance.UnPauseGame();
    }
}
