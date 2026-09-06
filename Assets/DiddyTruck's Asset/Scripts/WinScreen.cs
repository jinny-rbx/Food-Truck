using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    public void Next()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("Game 2");
    }
    public void Back()
    {
        SceneManager.LoadScene("Menu");
    }

}
