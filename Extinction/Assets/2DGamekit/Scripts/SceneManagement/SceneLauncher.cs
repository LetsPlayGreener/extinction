using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLauncher : MonoBehaviour
{
    public static SceneLauncher instance;

    public static string loadingSceneName = "MyLoading";

    private void Awake()
    {
        if (!instance)
        {
            if(this.transform.parent == null)
                instance = this;
            else
            {
                SceneLauncher sceneLauncher = new GameObject().AddComponent<SceneLauncher>();
                instance = sceneLauncher;
                this.enabled = false;
            }
        }
        else
            this.enabled = false;
    }

    private void Update()
    {
    }

    public void LoadScene(string sceneName)
    {
        if (Application.CanStreamedLevelBeLoaded(sceneName))
            instance.StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    public IEnumerator LoadSceneCoroutine(string sceneName)
    {
        DontDestroyOnLoad(instance);
        yield return SceneManager.LoadSceneAsync(SceneLauncher.loadingSceneName);
        yield return SceneManager.LoadSceneAsync(sceneName);
        Time.timeScale = 1;
    }
}
