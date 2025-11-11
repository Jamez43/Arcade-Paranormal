using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    [SerializeField] GameObject upgradesMenuContainer;
    private float currentHealth;
    private HealthBar healthBar;
    private XPBar xpBar;
    private float currentXP = 0f;
    [SerializeField] private float levelUpXPThreshold = 100f;
    private List<string> upgradesNames = new List<string>();
    private Camera camera;


    private void OnEnable()
    {
        healthBar = GetComponentInChildren<HealthBar>(includeInactive: true);
        healthBar.gameObject.SetActive(true);
        xpBar = GetComponentInChildren<XPBar>(includeInactive: true);
        currentHealth = stats.MaxHealth;
        xpBar.UpdateXPBar(currentXP, levelUpXPThreshold);
        upgradesNames = Resources.LoadAll<GameObject>("Upgrades")
        .Select(go => go.name)
        .ToList();
        stats.ResetStats();
        camera = Camera.main;
    }


    public void ApplyDamage(float damageAmount)
    {
        float damageAfterDefense = damageAmount * (1 - stats.Defense);
        if (damageAfterDefense > 0)
        {
            currentHealth -= damageAfterDefense;
            healthBar.gameObject.SetActive(true);
            healthBar.UpdateHealthBar(currentHealth, stats.MaxHealth);
        }
    }

    public void AddXP(float amount)
    {
        Debug.Log("Gained " + amount + " XP");
        currentXP += amount;
        xpBar.UpdateXPBar(currentXP, levelUpXPThreshold);
        if (currentXP >= levelUpXPThreshold)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        PauseManager.instance.PauseGame();
        NewUpgradesWindow();
        currentXP -= levelUpXPThreshold;
        levelUpXPThreshold *= 1.5f;
        xpBar.UpdateXPBar(currentXP, levelUpXPThreshold);

    }

    private void NewUpgradesWindow()
    {
        List<string> selectedUpgradesName = upgradesNames
        .OrderBy(x => Random.value)
        .Take(3)
        .ToList();

        for (int i = 0; i < 3; i++)
        {
            GameObject upgrade = upgradesMenuContainer.GetComponentsInChildren<Transform>(true)
                    .FirstOrDefault(x => x.name == selectedUpgradesName[i]).gameObject;
            Vector3 newPosition = upgrade.transform.localPosition;
            newPosition.x = -300 + (i * 300);
            upgrade.transform.localPosition = newPosition;

            upgrade.SetActive(true);
        }
        upgradesMenuContainer.SetActive(true);
    }


    private void Update()
    {
        if (PauseManager.instance.isPaused)
        {
            return;
        }
        CheckDie();
    }

    private void CheckDie()
    {
        if (currentHealth <= 0)
        {
            camera.transform.SetParent(null);
            healthBar.gameObject.SetActive(false);
            GameObject.Find("CooldownBarCanvas").SetActive(false);
            GameObject.Find("XPCanvas").SetActive(false);
            PauseManager.instance.PauseGame();
        }
    }
}
