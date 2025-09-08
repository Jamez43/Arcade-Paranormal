using UnityEngine;
using UnityEngine.UI;

public class CooldownBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void UpdateCooldownBar(float currentCooldown, float maxCooldown)
    {
        slider.value = currentCooldown / maxCooldown;
    }
}
