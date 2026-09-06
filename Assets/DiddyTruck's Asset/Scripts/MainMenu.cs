using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{

    public void Start()
    {
        Time.timeScale = 1f;
    }
    public void Play()
    {
        // Unity 6 safe lookup
        if (LevelManager.Instance == null)
        {
            LevelManager.Instance = Object.FindAnyObjectByType<LevelManager>(FindObjectsInactive.Include);
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.gameObject.SetActive(true);
            LevelManager.Instance.LoadScene("Game", "CrossFade");
        }
        else
        {
            Debug.LogError("[MainMenu] No LevelManager instance found in the active scene!");
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}