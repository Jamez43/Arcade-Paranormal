using UnityEngine;

public class StickyFingersButton : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    private GameObject parentPanel;

    public void updatePickupRange()
    {
        playerStats.PickupRange *= 1.1f;
        Debug.Log("Pickup Range increased to: " + playerStats.PickupRange);

        parentPanel = transform.parent.gameObject;
        parentPanel.SetActive(false);
    }
}
