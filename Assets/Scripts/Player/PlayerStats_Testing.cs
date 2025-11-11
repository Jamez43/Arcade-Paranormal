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

    public override float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public override float Damage { get => damage; set => damage = value; }
    public override float Speed { get => speed; set => speed = value; }
    public override float Cooldown { get => cooldown; set => cooldown = value; }
    public override float Defense { get => defense; set => defense = value; }
    public override string AttackType { get => attackType; set => attackType = value; }
    public override float PickupRange { get => pickupRange; set => pickupRange = value; }

    private float initialMaxHealth = 100f;
    private float initialDamage = 5f;
    private float initialSpeed = 5f;
    private float initialCooldown = 1f;
    private float initialDefense = 0f;
    private float initialPickupRange = .5f;

    public override void ResetStats()
    {
        maxHealth = initialMaxHealth;
        damage = initialDamage;
        speed = initialSpeed;
        cooldown = initialCooldown;
        defense = initialDefense;
        pickupRange = initialPickupRange;
    }
}