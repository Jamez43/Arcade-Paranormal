using UnityEngine;

public class XPCollisions : MonoBehaviour
{

    private XPController XPController;

    private void Awake()
    {
        XPController = FindFirstObjectByType<XPController>();
    }

    private void OnEnable()
    {
        XPController.disabledXP.Remove(gameObject);

    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            // Grant XP for defeating the enemy
            float xpAmount = Random.Range(5, 10);
            GrantXP(xpAmount);
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        XPController.disabledXP.Add(gameObject);
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
