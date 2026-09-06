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
            FetchTransitions();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void FetchTransitions()
    {
        if (transitionsContainer != null)
        {
            transitions = transitionsContainer.GetComponentsInChildren<SceneTransition>(true);
        }
    }

    public void LoadScene(string sceneName, string transitionName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName, transitionName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName, string transitionName)
    {
        if (transitions == null || transitions.Length == 0) FetchTransitions();

        SceneTransition transition = transitions?.FirstOrDefault(t => t.name == transitionName);

        if (transition == null)
        {
            Debug.LogError($"[LevelManager] Could not find transition named: {transitionName}");
            yield break;
        }

        transition.gameObject.SetActive(true);

        // 1. Play Transition In (e.g. Fade to Black)
        yield return transition.AnimateTransitionIn();

        // 2. Start Async Load
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.value = 0f;
        }

        // 3. Track Load Progress
        while (asyncLoad.progress < 0.9f)
        {
            if (progressBar != null)
            {
                progressBar.value = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            }
            yield return null;
        }

        if (progressBar != null)
        {
            progressBar.value = 1f;
            yield return new WaitForSecondsRealtime(0.2f);
            progressBar.gameObject.SetActive(false);
        }

        // 4. Activate New Scene
        asyncLoad.allowSceneActivation = true;

        // Wait until new scene is fully loaded into memory
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 5. Play Transition Out (e.g. Fade Clear)
        yield return transition.AnimateTransitionOut();

        transition.gameObject.SetActive(false);
    }
}