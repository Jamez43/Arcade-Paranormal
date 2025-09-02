using UnityEngine;

[CreateAssetMenu(menuName = "Player/InGameStats_Default")]
public class PlayerInGameStats_Default : ScriptableObject
{
    public float maxHealth = 100f;
    public float damage = 5f;
    public float speed = 3f;
    public float attackDelay = 1f;
    public float defense = 0f;
    public string attackType = "Melee";

}

public class PlayerStats_InGame : MonoBehaviour
{
    [SerializeField] private PlayerInGameStats_Default stats;
    public float currentHealth { get; private set; }

    private void OnEnable()
    {
        //currentHealth = stats.maxHealth;
    }
}
