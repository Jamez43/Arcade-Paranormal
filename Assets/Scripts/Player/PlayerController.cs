using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerStats baseStats;
    [SerializeField] GameObject upgradesMenuContainer;

    // Runtime stats that can be modified during gameplay
    private PlayerRuntimeStats runtimeStats;

    private HealthBar healthBar;
    private XPBar xpBar;
    private float currentXP = 0f;
    [SerializeField] private float levelUpXPThreshold = 100f;
    private List<string> upgradesNames = new List<string>();
    private GameObject gameOverMenu;
    private GameObject coolDownBarCanvas;
    private GameObject xpCanvas;
    private float currentHealth;
    private bool deathReported;

    // Public property to access runtime stats from other scripts
    public PlayerRuntimeStats Stats => runtimeStats;
    public float CurrentHealth => Mathf.Max(0f, currentHealth);
    public float HealthPercent => Mathf.Clamp01(CurrentHealth / runtimeStats.MaxHealth);
    public event Action<float> DamageTaken;
    public event Action Died;

    private void Awake()
    {
        // Initialize runtime stats from base stats
        runtimeStats = new PlayerRuntimeStats(baseStats);
        currentHealth = runtimeStats.MaxHealth;
        deathReported = false;

        healthBar = GetComponentInChildren<HealthBar>(includeInactive: true);
        healthBar.UpdateHealthBar(currentHealth, runtimeStats.MaxHealth);
        healthBar.gameObject.SetActive(true);

        coolDownBarCanvas = GameObject.Find("CooldownBarCanvas");
        coolDownBarCanvas.SetActive(true);

        xpCanvas = GameObject.Find("XPCanvas");
        xpCanvas.SetActive(true);

        xpBar = GetComponentInChildren<XPBar>(includeInactive: true);
        xpBar.UpdateXPBar(currentXP, levelUpXPThreshold);
        upgradesNames = Resources.LoadAll<GameObject>("Upgrades")
        .Select(go => go.name)
        .ToList();
        gameOverMenu = GameObject.FindWithTag("Game Over Menu");

        gameOverMenu.SetActive(false);

    }

    private void Start()
    {
        // Unpause game after all Awake methods have run
        if (PauseManager.instance != null)
        {
            PauseManager.instance.UnPauseGame();
        }
    }


    public void ApplyDamage(float damageAmount)
    {
        if (PauseManager.instance.isPaused) return;
        float damageAfterDefense = damageAmount * (1 - runtimeStats.Defense);
        if (damageAfterDefense > 0)
        {
            float appliedDamage = Mathf.Min(CurrentHealth, damageAfterDefense);
            currentHealth = Mathf.Max(0f, currentHealth - damageAfterDefense);
            healthBar.gameObject.SetActive(true);
            healthBar.UpdateHealthBar(currentHealth, runtimeStats.MaxHealth);
            DamageTaken?.Invoke(appliedDamage);
        }
    }

    public void AddXP(float amount)
    {
        currentHealth = Mathf.Min(runtimeStats.MaxHealth, currentHealth + amount / 2f);
        healthBar.UpdateHealthBar(currentHealth, runtimeStats.MaxHealth);
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
        .OrderBy(x => UnityEngine.Random.value)
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
        if (currentHealth <= 0 && !deathReported)
        {
            deathReported = true;
            Died?.Invoke();
            gameOverMenu.SetActive(true);
            healthBar.gameObject.SetActive(false);
            coolDownBarCanvas.SetActive(false);
            xpCanvas.SetActive(false);
            PauseManager.instance.PauseGame();
        }
    }
}
