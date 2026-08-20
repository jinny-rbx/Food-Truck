using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public Slider progressBar;
    public GameObject transitionsContainer;

    private SceneTransition[] transitions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Fetch transitions in Awake so they are ready immediately
            FetchTransitions();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void FetchTransitions()
    {
        if (transitionsContainer != null)
        {
            // Include true to find inactive transitions too
            transitions = transitionsContainer.GetComponentsInChildren<SceneTransition>(true);
        }
        else
        {
            Debug.LogError("LevelManager: 'transitionsContainer' is not assigned in the Inspector!", this);
        }
    }

    public void LoadScene(string sceneName, string transitionName)
    {
        StartCoroutine(LoadSceneAsync(sceneName, transitionName));
    }

    private IEnumerator LoadSceneAsync(string sceneName, string transitionName)
    {
        // Safety Fallback if transitions wasn't populated
        if (transitions == null || transitions.Length == 0)
        {
            FetchTransitions();
        }

        SceneTransition transition = transitions?.FirstOrDefault(t => t.name == transitionName);

        if (transition == null)
        {
            Debug.LogError($"LevelManager: Transition '{transitionName}' could not be found!");
            yield break;
        }

        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        yield return transition.AnimateTransitionIn();

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
        }

        // Scene progress stops at 0.9f while allowSceneActivation is false. 
        // Divide by 0.9f to get a smooth 0.0 to 1.0 range for the progress bar.
        while (scene.progress < 0.9f)
        {
            if (progressBar != null)
            {
                progressBar.value = Mathf.Clamp01(scene.progress / 0.9f);
            }
            yield return null;
        }

        if (progressBar != null)
        {
            progressBar.value = 1f;
        }

        yield return new WaitForSeconds(0.5f);

        scene.allowSceneActivation = true;

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
        }

        yield return transition.AnimateTransitionOut();
    }
}