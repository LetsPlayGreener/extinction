using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelInfo : Displayable
{
    public string levelSceneName;
    [HideInInspector]
    public bool alreadyDone = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!Application.CanStreamedLevelBeLoaded(levelSceneName))
        {
            Debug.LogError(string.Concat("The scene \"", levelSceneName, "\"was not found."));
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonUp(0) && focused)
        {
            if (SceneLauncher.instance)
                SceneLauncher.instance.LoadScene(levelSceneName);
            else
                Debug.LogError("There is no SceneLauncher component in the scene. Add one from the prefab");
        }
    }
}
