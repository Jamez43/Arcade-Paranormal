using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats_Testing", menuName = "Scriptable Objects/Player/Testing")]
public class PlayerStats_Testing : PlayerStats
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private float defense = 0f;
    [SerializeField] private string attackType = "Melee";
    [SerializeField] private float pickupRange = .5f;

    // Read-only properties - base stats should never be modified at runtime
    public override float MaxHealth { get => maxHealth; set { } }
    public override float Damage { get => damage; set { } }
    public override float Speed { get => speed; set { } }
    public override float Cooldown { get => cooldown; set { } }
    public override float Defense { get => defense; set { } }
    public override string AttackType { get => attackType; set { } }
    public override float PickupRange { get => pickupRange; set { } }

    public override void ResetStats()
    {
        // This method is no longer needed since base stats don't change
        // Runtime stats will be reset via PlayerRuntimeStats.ResetToBaseStats()
    }
}