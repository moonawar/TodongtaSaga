using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
    public static SceneLoader Instance;
    private Action onLoaderCallback;
    private Action onSceneLoadedCallback;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName) {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone) {
            yield return null;
        }
        // Scene is fully loaded, call the callback if it exists
        onSceneLoadedCallback?.Invoke();
        onSceneLoadedCallback = null;
    }

    public void LoadScene(string sceneName, Action onSceneLoaded = null) {
        Debug.Log("Loading scene: " + sceneName);
        DOTween.KillAll();
        onSceneLoadedCallback = onSceneLoaded;
        onLoaderCallback = () => {
            StartCoroutine(LoadSceneAsync(sceneName));
        };
        SceneManager.LoadScene("LoadingScreen");
    }

    public void DeleteCore() {
        GameObject core = GameObject.FindGameObjectWithTag("Core");
        if (core != null) {
            Destroy(core);
        }
    }

    public void ToMainMenu() {
        DeleteCore();
        LoadScene("MainMenu");
    }

    public void ToGameplayFromMainMenu() {
        SaveManager.Instance.LoadGame();
    }

    public void QuitGame() {
        Application.Quit();
    }

    public static void LoaderCallback()
    {
        if (Instance.onLoaderCallback != null)
        {
            Instance.onLoaderCallback();
            Instance.onLoaderCallback = null;
        }
    }

    public void RestartScene() {
        DeleteCore();
        LoadScene(SceneManager.GetActiveScene().name);
    }
}