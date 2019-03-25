using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapManager : MonoBehaviour
{
    public Camera mapCamera;
    public RawImage mapLittleImage;
    public RawImage mapBigImage;

    private RenderTexture renderTexture;
    public int renderTextureWidth = 1024;
    public int renderTextureHeight = 1024;

    // Start is called before the first frame update
    void Start()
    {
        if (mapCamera && mapLittleImage && mapBigImage)
        {
            renderTexture = new RenderTexture(renderTextureWidth, renderTextureHeight, 24, RenderTextureFormat.ARGB32);
            mapCamera.targetTexture = renderTexture;
            mapLittleImage.texture = renderTexture;
            mapBigImage.texture = renderTexture;
        }
        else
            gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        mapLittleImage.transform.parent.gameObject.SetActive(!Input.GetKey(KeyCode.M));
        mapBigImage.transform.parent.gameObject.SetActive(Input.GetKey(KeyCode.M));
    }
}
