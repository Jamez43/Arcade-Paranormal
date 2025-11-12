using UnityEngine;

public class CheatCodeButton : MonoBehaviour
{
    private GameObject parentPanel;

    public void updateDamage()
    {
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player.Stats.ApplyDamageUpgrade(1.1f);
        Debug.Log("Damage increased to: " + player.Stats.Damage);

        parentPanel = transform.parent.gameObject;
        parentPanel.SetActive(false);
        PauseManager.instance.UnPauseGame();
    }
}
