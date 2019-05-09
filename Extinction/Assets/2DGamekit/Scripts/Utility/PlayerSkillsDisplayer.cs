using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerSkillsDisplayer : MonoBehaviour
{
    private List<KeyValuePair<string, bool>> skills;
    public RectTransform scrollViewContent;
    public TextMeshProUGUI skillGOTemplate;
    private RectTransform skillGOTemplateRT;

    private GameObject tmpGO;
    private RectTransform tmpRT;

    // Start is called before the first frame update
    void Start()
    {
        if (Application.isPlaying)
        {
            if (DataManager.playerData == null)
                DataManager.LoadData();

            skills = DataManager.playerData.Skills;

            if(skills != null && skills.Count > 0 && skillGOTemplate)
            {
                skillGOTemplateRT = skillGOTemplate.GetComponent<RectTransform>();
                scrollViewContent.sizeDelta = new Vector2(scrollViewContent.sizeDelta.x, skills.Count * skillGOTemplateRT.sizeDelta.y);
                skillGOTemplateRT.anchoredPosition = new Vector2(skillGOTemplateRT.anchoredPosition.x, -skillGOTemplateRT.sizeDelta.y * 0.5f);

                for (int i = 1; i < skills.Count; i++)
                {
                    tmpGO = GameObject.Instantiate(skillGOTemplate.gameObject);
                    tmpGO.transform.SetParent(skillGOTemplate.transform.parent);
                    tmpRT = tmpGO.GetComponent<RectTransform>();
                    tmpRT.offsetMin = new Vector2(0, tmpRT.offsetMin.y);
                    tmpRT.offsetMax = new Vector2(0, tmpRT.offsetMax.y);
                    tmpRT.localScale = skillGOTemplateRT.localScale;
                    tmpRT.anchoredPosition = new Vector2(tmpRT.anchoredPosition.x, -skillGOTemplateRT.sizeDelta.y * (i+0.5f));
                    tmpGO.GetComponent<TextMeshProUGUI>().text = string.Concat("<color=", skills[i].Value ? "\"green\">DONE" : "\"red\">FAILED", "</color>", " - ", skills[i].Key.ToUpper());
                }
                skillGOTemplate.text = string.Concat("<color=", skills[0].Value ? "\"green\">DONE" : "\"red\">FAILED", "</color>", " - ", skills[0].Key.ToUpper());
            }
        }
    }
}
