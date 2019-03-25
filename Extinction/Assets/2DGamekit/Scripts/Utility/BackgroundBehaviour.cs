using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundBehaviour : MonoBehaviour
{
    private Vector3 previousCameraPos;

    // Start is called before the first frame update
    void Start()
    {
        previousCameraPos = Camera.main.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.right * (Camera.main.transform.position.x - previousCameraPos.x) / 2 + Vector3.up * (Camera.main.transform.position.y - previousCameraPos.y) / 1.025f;
        previousCameraPos = Camera.main.transform.position;
    }
}
