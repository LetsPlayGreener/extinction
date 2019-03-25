using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraBackgroundSetter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.GetComponent<Image>() && gameObject.GetComponent<Image>().sprite && gameObject.GetComponent<Image>().sprite.texture)
            Camera.main.backgroundColor = gameObject.GetComponent<Image>().sprite.texture.GetPixel(0, 0);
        else
            Camera.main.backgroundColor = Color.black;
    }
}
