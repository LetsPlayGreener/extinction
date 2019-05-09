using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnemiesDisplayer : MonoBehaviour
{
    public static List<EnemyMenuInfo> enemyList;

    public RectTransform scrollViewContent;
    public RectTransform enemyElementTemplateRT;

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

            enemyElementTemplateRT.gameObject.SetActive(true);

            scrollViewContent.sizeDelta = new Vector2(enemyList.Count * enemyElementTemplateRT.sizeDelta.x, scrollViewContent.sizeDelta.y);
            enemyElementTemplateRT.anchoredPosition = new Vector2(enemyElementTemplateRT.sizeDelta.x * 0.5f, enemyElementTemplateRT.anchoredPosition.y);

            for (int i = 0; i < enemyList.Count; i++)
            {
                tmpGO = enemyList[i].gameObject;
                tmpRT = tmpGO.GetComponent<RectTransform>();
                tmpRT.anchorMin = enemyElementTemplateRT.anchorMin;
                tmpRT.anchorMax = enemyElementTemplateRT.anchorMax;
                tmpRT.offsetMin = new Vector2(tmpRT.offsetMin.x, 0);
                tmpRT.offsetMax = new Vector2(tmpRT.offsetMax.x, 0);
                tmpRT.localScale = enemyElementTemplateRT.localScale;
                tmpRT.anchoredPosition = new Vector2(enemyElementTemplateRT.sizeDelta.x * (i + 0.5f), tmpRT.anchoredPosition.y);
                tmpRT.localPosition += Vector3.back * tmpRT.localPosition.z;
            }
        }
    }
}
