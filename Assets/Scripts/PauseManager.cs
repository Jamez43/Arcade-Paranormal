using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;
public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;
    private PlayerMovement playerMovement;
    private GameObject joystick;
    private GameObject indicator;

    public bool isPaused { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        joystick = GameObject.FindWithTag("Joystick");
        indicator = GameObject.FindWithTag("Indicator");
    }


    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        playerMovement.SwitchInputMap("Gameplay", "UI");
        joystick.SetActive(false);
        indicator.SetActive(false);

    }

    public void UnPauseGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        playerMovement.SwitchInputMap("UI", "Gameplay");
        joystick.SetActive(true);
        indicator.SetActive(true);
    }

}