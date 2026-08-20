using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        LevelManager.Instance.gameObject.SetActive(true);
        LevelManager.Instance.LoadScene("Game", "CrossFade"); 
    }

    public void Quit()
    {
        Application.Quit();
    }
}
