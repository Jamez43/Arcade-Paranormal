using UnityEngine;

public class XP_Pickup : MonoBehaviour
{
    [SerializeField] private CircleCollider2D pickupCollider;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private XPController xpController;

    private void Awake()
    {
        pickupCollider = GetComponent<CircleCollider2D>();
        pickupCollider.radius = playerStats.PickupRange;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.AddXP(amount);
        }
    }
}
