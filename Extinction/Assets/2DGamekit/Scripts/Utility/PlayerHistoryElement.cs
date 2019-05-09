using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerHistoryElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image startDot;
    public Image endDot;

    private LineRenderer lineRenderer;

    private bool pointerOver = false;
    private Vector3[] positions;
    private bool initialized = false;

    private float timer;
    private float timeBeforeNextDot = 0.15f;
    private float timeBeforeStartingOver = 3;
    private int positionCounter;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!initialized)
        {
            positions = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(positions);
            initialized = true;
        }

        positionCounter = 0;
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(positionCounter, positions[positionCounter]);
        timer = Time.time;
        pointerOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerOver = false;
        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Application.isPlaying)
        {
            lineRenderer = gameObject.GetComponentInChildren<LineRenderer>();
            if (!lineRenderer)
                enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (pointerOver)
        {
            if (Time.time - timer > timeBeforeNextDot && lineRenderer.positionCount < positions.Length)
            {
                lineRenderer.positionCount++;
                positionCounter++;
                lineRenderer.SetPosition(positionCounter, positions[positionCounter]);
                timer = Time.time;
            }
            else if(Time.time - timer > timeBeforeStartingOver && lineRenderer.positionCount >= positions.Length)
            {
                positionCounter = 0;
                lineRenderer.positionCount = 1;
                lineRenderer.SetPosition(positionCounter, positions[positionCounter]);
                timer = Time.time;
            }
        }
    }
}
