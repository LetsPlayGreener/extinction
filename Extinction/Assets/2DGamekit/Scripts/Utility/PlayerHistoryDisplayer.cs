using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHistoryDisplayer : MonoBehaviour
{
    private List<KeyValuePair<string, List<Vector3>>> history;
    public RectTransform scrollViewContent;
    public GameObject historyElementPrefab;
    public RectTransform historyElementTemplateRT;

    private GameObject tmpGO;
    private RectTransform tmpRT;
    private LineRenderer tmpLR;

    // Start is called before the first frame update
    void Start()
    {
        if (Application.isPlaying)
        {
            if (DataManager.playerData == null)
                DataManager.LoadData();

            history = DataManager.playerData.LevelsHistoryToList;

            if (history.Count == 0)
                historyElementTemplateRT.gameObject.SetActive(false);
            else if (history != null && historyElementPrefab)
            {
                historyElementTemplateRT.gameObject.SetActive(true);

                scrollViewContent.sizeDelta = new Vector2(history.Count * historyElementTemplateRT.sizeDelta.x, scrollViewContent.sizeDelta.y);
                historyElementTemplateRT.anchoredPosition = new Vector2(historyElementTemplateRT.sizeDelta.x * 0.5f, historyElementTemplateRT.anchoredPosition.y);

                for (int i = 1; i < history.Count; i++)
                {
                    tmpGO = GameObject.Instantiate(historyElementPrefab.gameObject);
                    tmpGO.transform.SetParent(historyElementTemplateRT.transform.parent);
                    tmpRT = tmpGO.GetComponent<RectTransform>();
                    tmpRT.anchorMin = historyElementTemplateRT.anchorMin;
                    tmpRT.anchorMax = historyElementTemplateRT.anchorMax;
                    tmpRT.offsetMin = new Vector2(tmpRT.offsetMin.x, 0);
                    tmpRT.offsetMax = new Vector2(tmpRT.offsetMax.x, 0);
                    tmpRT.localScale = historyElementTemplateRT.localScale;
                    tmpRT.anchoredPosition = new Vector2(historyElementTemplateRT.sizeDelta.x * (i + 0.5f), tmpRT.anchoredPosition.y);
                    tmpRT.localPosition += Vector3.back * tmpRT.localPosition.z;

                    tmpGO.GetComponentInChildren<TextMeshProUGUI>().text = history[i].Key;
                    ChangeCoordToFitInSquare(tmpGO.GetComponentInChildren<Image>().GetComponent<RectTransform>().sizeDelta.y, history[i].Value);
                    tmpLR = tmpGO.GetComponentInChildren<LineRenderer>();
                    tmpLR.positionCount = history[i].Value.Count;
                    tmpLR.SetPositions(history[i].Value.ToArray());
                    tmpGO.GetComponent<PlayerHistoryElement>().startDot.rectTransform.localPosition = history[i].Value[0];
                    tmpGO.GetComponent<PlayerHistoryElement>().endDot.rectTransform.localPosition = history[i].Value[history[i].Value.Count-1];

                }
                historyElementTemplateRT.GetComponentInChildren<TextMeshProUGUI>().text = history[0].Key;
                ChangeCoordToFitInSquare(historyElementTemplateRT.GetComponentInChildren<Image>().GetComponent<RectTransform>().sizeDelta.y, history[0].Value);
                tmpLR = historyElementTemplateRT.GetComponentInChildren<LineRenderer>();
                tmpLR.positionCount = history[0].Value.Count;
                tmpLR.SetPositions(history[0].Value.ToArray());
                historyElementTemplateRT.GetComponent<PlayerHistoryElement>().startDot.rectTransform.localPosition = history[0].Value[0];
                historyElementTemplateRT.GetComponent<PlayerHistoryElement>().endDot.rectTransform.localPosition = history[0].Value[history[0].Value.Count - 1];
            }
        }
    }

    private void ChangeCoordToFitInSquare(float squareLenght, List<Vector3> positions)
    {
        float positionWidth, positionHeight;

        float offsetX, offsetY;
        float scale;

        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;

        foreach(Vector3 pos in positions)
        {
            if (pos.x > maxX)
                maxX = pos.x;
            if (pos.x < minX)
                minX = pos.x;
            if (pos.y > maxY)
                maxY = pos.y;
            if (pos.y < minY)
                minY = pos.y;
        }

        positionWidth = maxX - minX;
        positionHeight = maxY - minY;

        offsetX = -(maxX - positionWidth / 2);
        offsetY = -(maxY - positionHeight / 2);

        if (positionWidth > positionHeight)
            scale = squareLenght / positionWidth;
        else
            scale = squareLenght / positionHeight;

        for (int i = 0; i < positions.Count; i++)
        {
            positions[i] += new Vector3(offsetX, offsetY, 0);
            positions[i] *= scale;
        }
    }
}
