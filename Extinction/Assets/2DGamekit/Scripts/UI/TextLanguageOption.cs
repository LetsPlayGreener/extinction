using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextLanguageOption : MonoBehaviour
{

    public TextMeshProUGUI buttonText;
    public Button button;

    // Start is called before the first frame update
    void Start()
    {
        if (buttonText && button)
        {
            if (DataManager.playerData == null)
                DataManager.LoadData();

            buttonText.text = DataManager.playerData.PlayerLanguage.ToString().ToUpper();

            button.onClick.AddListener(SwitchPlayerLanguage);
        }
        else
            gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SwitchPlayerLanguage()
    {
        switch (DataManager.playerData.PlayerLanguage)
        {
            case PlayerData.Language.english:
                DataManager.playerData.PlayerLanguage = PlayerData.Language.french;
                break;

            case PlayerData.Language.french:
                DataManager.playerData.PlayerLanguage = PlayerData.Language.italian;
                break;

            case PlayerData.Language.italian:
                DataManager.playerData.PlayerLanguage = PlayerData.Language.romanian;
                break;

            case PlayerData.Language.romanian:
                DataManager.playerData.PlayerLanguage = PlayerData.Language.english;
                break;

            default:
                break;
        }

        buttonText.text = DataManager.playerData.PlayerLanguage.ToString().ToUpper();
    }
}
