using UnityEngine;

public class XPController : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            // Grant XP for defeating the enemy
            float xpAmount = Random.Range(5, 10);
            GrantXP(xpAmount);
            Debug.Log("Granted " + xpAmount + " XP to player.");
            gameObject.SetActive(false);
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
