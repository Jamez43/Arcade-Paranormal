using UnityEngine;
using UnityEngine.UI;

public class XPBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void UpdateXPBar(float currentXP, float maxXP)
    {
        slider.value = currentXP / maxXP;
    }
}
