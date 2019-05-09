using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyMenuInfo : Displayable
{
    public Sprite enemyDiscoveredImage;
    public Sprite enemyNotDiscoveredImage;

    public bool hideInfoIfNotDiscovered = false;

    private Image image;

    private List<string> levels;
    private string metInText;

    private void Awake()
    {
        if (Application.isPlaying)
        {
            if (PlayerEnemiesDisplayer.enemyList == null)
                PlayerEnemiesDisplayer.enemyList = new List<EnemyMenuInfo>();
            if (!PlayerEnemiesDisplayer.enemyList.Contains(this))
                PlayerEnemiesDisplayer.enemyList.Add(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (DataManager.playerData == null)
            DataManager.LoadData();

        image = GetComponentInChildren<Image>();

        levels = DataManager.playerData.CheckEnemyMet(elementName);
        if (DataManager.playerData.CheckEnemyMet(elementName) != null)
        {
            metInText = string.Concat("Met in : ", levels[0]);
            for (int i = 1; i < levels.Count; i++)
                metInText = string.Concat(metInText, ", ", levels[i]);
            description = string.Concat(description, description == "" ? "" : System.Environment.NewLine, metInText);

            InfoCanBeDisplayed = true;
            image.sprite = enemyDiscoveredImage;
        }
        else
        {
            InfoCanBeDisplayed = !hideInfoIfNotDiscovered;
            image.sprite = enemyNotDiscoveredImage;
        }

        if (GetComponentInChildren<TextMeshProUGUI>().text == "")
            GetComponentInChildren<TextMeshProUGUI>().text = elementName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
