using UnityEngine;

public class XP_Pickup : MonoBehaviour
{
    [SerializeField] private CircleCollider2D pickupCollider;
    [SerializeField] private XPController xpController;

    private PlayerRuntimeStats playerStats;
    private PlayerController playerController;

    private void Start()
    {
        // Get runtime stats from PlayerController (use Start to ensure PlayerController.Awake has run)
        playerController = GetComponent<PlayerController>();
        playerStats = playerController.Stats;
        pickupCollider.radius = playerStats.PickupRange;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PauseManager.instance.isPaused) return;
        if (collision.CompareTag("XP"))
        {
            float xpAmount = Random.Range(5, 10);
            GrantXP(xpAmount);
            collision.gameObject.SetActive(false);
            xpController.disabledXP.Add(collision.gameObject);
        }
    }

    private void GrantXP(float amount)
    {
        if (playerController != null)
        {
            playerController.AddXP(amount);
        }
    }
}
