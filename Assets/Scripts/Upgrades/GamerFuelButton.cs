using UnityEngine;

public class GamerFuelButton : MonoBehaviour
{
    private GameObject parentPanel;

    public void updateSpeed()
    {
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player.Stats.ApplySpeedUpgrade(1.1f);
        Debug.Log("Speed increased to: " + player.Stats.Speed);

        parentPanel = transform.parent.gameObject;
        parentPanel.SetActive(false);
        PauseManager.instance.UnPauseGame();
    }
}
