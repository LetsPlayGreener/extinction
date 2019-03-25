using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableFallenObjects : MonoBehaviour
{
    public GameObject disabled;
    public float heigthBeforeDisable = -100;

    // Start is called before the first frame update
    void Start()
    {
        if (!disabled)
            this.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position.y < heigthBeforeDisable)
        {
            disabled.SetActive(false);
            this.enabled = false;
        }
    }
}
