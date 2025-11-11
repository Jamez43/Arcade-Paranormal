using UnityEngine;

public class CheatCodeButton : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    private GameObject parentPanel;

    public void updateDamage()
    {
        playerStats.Damage *= 1.1f;
        Debug.Log("Damage increased to: " + playerStats.Damage);

        parentPanel = transform.parent.gameObject;
        parentPanel.SetActive(false);
    }
}
