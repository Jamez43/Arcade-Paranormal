using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{

    [SerializeField] private string sceneName;
    public void StartGame()
    {
        SceneManager.LoadScene(sceneName);
    }
}
