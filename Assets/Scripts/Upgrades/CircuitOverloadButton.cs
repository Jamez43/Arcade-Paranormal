using UnityEngine;

public class CircuitOverloadButton : MonoBehaviour
{
    private GameObject parentPanel;

    public void updateMaxHealth()
    {
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player.Stats.ApplyMaxHealthUpgrade(1.1f);
        Debug.Log("Max Health increased to: " + player.Stats.MaxHealth);

        parentPanel = transform.parent.gameObject;
        parentPanel.SetActive(false);
        PauseManager.instance.UnPauseGame();
    }
}
