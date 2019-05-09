using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Displayable : MonoBehaviour
{
    public string elementName;
    public string description;

    [HideInInspector]
    public bool focused = false;

    private bool infoCanBeDisplayed = true;

    public bool InfoCanBeDisplayed
    {
        get
        {
            return infoCanBeDisplayed;
        }

        set
        {
            infoCanBeDisplayed = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
