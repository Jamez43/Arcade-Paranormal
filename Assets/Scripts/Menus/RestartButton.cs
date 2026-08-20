using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    private PlayerController playerController;

    private void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
    }

    public void RestartLevel()
    {
        // Don't unpause yet - let the scene reload first
        // PauseManager.instance.UnPauseGame(); // Removed - game will unpause naturally on scene load

        // Reset runtime stats back to base stats (not needed since scene reloads, but doesn't hurt)
        playerController.Stats.ResetToBaseStats();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
