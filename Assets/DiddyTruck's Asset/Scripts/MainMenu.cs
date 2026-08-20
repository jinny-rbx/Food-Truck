using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        // Try finding Instance if null
        if (LevelManager.Instance == null)
        {
            LevelManager.Instance = FindFirstObjectByType<LevelManager>();
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.gameObject.SetActive(true);
            LevelManager.Instance.LoadScene("Game", "CrossFade");
        }
        else
        {
            Debug.LogError("MainMenu: No LevelManager found in the scene!");
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}