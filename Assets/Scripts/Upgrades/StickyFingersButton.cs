using UnityEngine;

public class StickyFingersButton : MonoBehaviour
{
    private GameObject parentPanel;

    public void updatePickupRange()
    {
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player.Stats.ApplyPickupRangeUpgrade(1.1f);
        Debug.Log("Pickup Range increased to: " + player.Stats.PickupRange);

        // Update the pickup collider radius
        player.GetComponent<CircleCollider2D>().radius = player.Stats.PickupRange;

        parentPanel = transform.parent.gameObject;
        parentPanel.SetActive(false);
        PauseManager.instance.UnPauseGame();
    }
}
