using UnityEngine;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private string defaultTransitionName = "Fade"; // Name of your SceneTransition child object

    public void Next()
    {
        Time.timeScale = 1f;
        int nextSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1;

        // Pass scene index or scene name to LevelManager
        string nextSceneName = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(nextSceneIndex);
        nextSceneName = System.IO.Path.GetFileNameWithoutExtension(nextSceneName);

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LoadScene(nextSceneName, defaultTransitionName);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneIndex);
        }
    }

    public void Back()
    {
        Time.timeScale = 1f; // Always ensure time scale is unpaused!

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LoadScene("Menu", defaultTransitionName);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }
    }
}