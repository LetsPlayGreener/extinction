using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RememberTeleporterPosition : MonoBehaviour
{
    private static Vector3 position;
    private static bool initialized = false;

    // Start is called before the first frame update
    void Start()
    {
        if (initialized)
            transform.position = position;
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        position = transform.position;
    }
}
