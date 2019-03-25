using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLauncher : MonoBehaviour
{
    public static SceneLauncher instance;

    public static string loadingSceneName = "MyLoading";

    private string sceneNameToLoad;
    private bool loadingLoadingScene = false;
    private bool loadingScene = false;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (loadingLoadingScene)
        {
            if(SceneManager.GetActiveScene().name == loadingSceneName)
            {
                try
                {
                    loadingLoadingScene = false;
                    loadingScene = true;
                    SceneManager.LoadSceneAsync(sceneNameToLoad);
                }
                catch (System.Exception)
                {
                    Debug.LogError(string.Concat("Unable to load scene \"", sceneNameToLoad, "\""));
                }
            }
        }
        else if (loadingScene)
        {
            if (SceneManager.GetActiveScene().name == sceneNameToLoad)
            {
                loadingScene = false;
                if (instance != this)
                    Destroy(this.gameObject);
            }
        }
    }

    public void LoadScene(string sceneName)
    {
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            try
            {
                DontDestroyOnLoad(this);
                sceneNameToLoad = sceneName;
                loadingLoadingScene = true;
                SceneManager.LoadSceneAsync(SceneLauncher.loadingSceneName);
            }
            catch (System.Exception)
            {
                Debug.LogError(string.Concat("Unable to load scene \"", sceneName, "\""));
            }
        }
    }
}
