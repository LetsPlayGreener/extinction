using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InfoDisplayer : MonoBehaviour
{
    public static InfoDisplayer instance;

    //the text mesh pro used to display info
    public TextMeshProUGUI infoDisplayerText;
    //strings used to set the text to bold and centered
    string[] boldCentered;
    //rectTransform to move to follow mouse position
    private RectTransform displayerRect;
    [HideInInspector]
    //contains the currently displayed info
    public Displayable displayedInfo = null;

    //variables used for raycast
    public EventSystem eventSystem;
    public GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private List<RaycastResult> raycastResults;
    private Displayable raycastedDisplayable = null;

    private void Awake()
    {
        if(!instance)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //initialize variables
        if (!infoDisplayerText || !eventSystem || !raycaster)
            this.enabled = false;
        else
            displayerRect = infoDisplayerText.transform.parent.GetComponent<RectTransform>();

        boldCentered = new string[2] { "<align=\"center\"><b>", "</b></align>" };
        raycastResults = new List<RaycastResult>();
        pointerEventData = new PointerEventData(eventSystem);
    }

    // Update is called once per frame
    void Update()
    {
        if(displayedInfo)
            displayedInfo.focused = false;

        pointerEventData.position = Input.mousePosition;
        raycastResults.Clear();
        raycaster.Raycast(pointerEventData, raycastResults);
        if (raycastResults.Count > 0)
        {
            foreach(RaycastResult result in raycastResults)
            {
                raycastedDisplayable = result.gameObject.GetComponent<Displayable>();
                if (raycastedDisplayable)
                    break;
                if (result.gameObject.transform.parent)
                    raycastedDisplayable = result.gameObject.transform.parent.GetComponent<Displayable>();
                else
                    continue;
                if (raycastedDisplayable)
                    break;
                if (result.gameObject.transform.parent.parent)
                    raycastedDisplayable = result.gameObject.transform.parent.parent.GetComponent<Displayable>();
                else
                    continue;
                if (raycastedDisplayable)
                    break;
            }

            if (raycastedDisplayable)
            {

                if (raycastedDisplayable != displayedInfo)
                {
                    displayedInfo = raycastedDisplayable;
                    displayerRect.gameObject.SetActive(true);

                    //display new info
                    infoDisplayerText.text = string.Concat(boldCentered[0], displayedInfo.elementName, boldCentered[1], System.Environment.NewLine, displayedInfo.description);
                    //display gauge percentage
                    if (displayedInfo is GaugeInfo)
                    {
                        if (displayedInfo.elementName == "" && displayedInfo.description == "")
                            infoDisplayerText.text = string.Concat("<align=\"center\">", ((GaugeInfo)displayedInfo).Value, "%</align>");
                        else
                            infoDisplayerText.text = string.Concat(infoDisplayerText.text, System.Environment.NewLine, "<align=\"center\">Score: ", ((GaugeInfo)displayedInfo).Value, "%</align>");
                    }
                }
            }
            else
                displayedInfo = null;
        }
        else
            displayedInfo = null;

        if (displayedInfo)
        {
            displayedInfo.focused = true;
            //set displayer position depending on mouse position
            Vector3 offset = Vector3.zero;
            if (Input.mousePosition.x > Screen.width/2)
                offset = new Vector3(-displayerRect.rect.width / 2, offset.y, 0);
            else
                offset = new Vector3(displayerRect.rect.width / 2, offset.y, 0);
            if (Input.mousePosition.y > Screen.height/2)
                offset = new Vector3(offset.x, -displayerRect.rect.height / 2, 0);
            else
                offset = new Vector3(offset.x, displayerRect.rect.height / 2, 0);
            displayerRect.anchoredPosition = Input.mousePosition - new Vector3(Screen.width, Screen.height,0)/2 + offset;
        }
        else if(!displayedInfo)
            displayerRect.gameObject.SetActive(false);
    }
}
