using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMarker : MonoBehaviour
{
    public SpriteRenderer sprite;
    public float scale = 4;

    private Vector3 previousLossyScale;

    private bool shrinkToScale = false;
    private float tmpScale;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.one;
        transform.localScale = new Vector3(scale / transform.lossyScale.x, scale / transform.lossyScale.y, scale / transform.lossyScale.z);
        previousLossyScale = transform.lossyScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (previousLossyScale != transform.lossyScale)
        {
            transform.localScale = Vector3.one;
            transform.localScale = new Vector3(scale / transform.lossyScale.x, scale / transform.lossyScale.y, scale / transform.lossyScale.z);
        }
        if (shrinkToScale)
        {
            //interpolate tmpScale toward scale and set new scale
            tmpScale -= scale * Time.deltaTime *10;
            tmpScale = tmpScale < scale ? scale : tmpScale;
            transform.localScale = Vector3.one;
            transform.localScale = new Vector3(tmpScale / transform.lossyScale.x, tmpScale / transform.lossyScale.y, tmpScale / transform.lossyScale.z);
            if (tmpScale == scale)
            {
                shrinkToScale = false;
            }
        }
    }

    public void ShowMarkOnMap()
    {
        shrinkToScale = true;
        tmpScale = scale * 10;
        transform.localScale = Vector3.one;
        transform.localScale = new Vector3(tmpScale / transform.lossyScale.x, tmpScale / transform.lossyScale.y, tmpScale / transform.lossyScale.z);
    }

    public void HideMark()
    {
            sprite.color = Color.grey - Color.black * 0.5f;
    }
}
