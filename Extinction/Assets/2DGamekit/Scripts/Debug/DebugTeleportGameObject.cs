using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTeleportGameObject : MonoBehaviour
{
    private Vector3 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            transform.position = initialPosition;
    }
}
