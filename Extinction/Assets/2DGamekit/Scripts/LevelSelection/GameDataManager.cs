using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Gamekit2D;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance;

    public GaugeInfo airGauge;
    public GaugeInfo waterGauge;
    public GaugeInfo bioGauge;

    public GaugeInfo lifeGauge;
    public Damageable player;

    public bool levelIsLevelSelection = false;

    private float continuousTimer;
    private float frequencyForTimer = 1;

    // Start is called before the first frame update
    void Start()
    {
        if (!instance)
        {
            instance = this;

            DataManager.LoadData();

            if (airGauge)
            {
                if (DataManager.playerData.AirGaugeValue < 0)
                    DataManager.playerData.AirGaugeValue = UnityEngine.Random.Range(40, 60);
                else
                    airGauge.Value = DataManager.playerData.AirGaugeValue;
            }
            if (waterGauge)
            {
                if (DataManager.playerData.WaterGaugeValue < 0)
                    DataManager.playerData.WaterGaugeValue = UnityEngine.Random.Range(40, 60);
                else
                    waterGauge.Value = DataManager.playerData.WaterGaugeValue;
            }
            if (bioGauge)
            {
                if (DataManager.playerData.BioGaugeValue < 0)
                    DataManager.playerData.BioGaugeValue = UnityEngine.Random.Range(40, 60);
                else
                    bioGauge.Value = DataManager.playerData.BioGaugeValue;
            }

            if (!levelIsLevelSelection)
            {
                DataManager.playerData.AddLevelToHistory(SceneManager.GetActiveScene().name);
                if (player)
                {
                    DataManager.playerData.AddPosition(player.transform.position);
                    continuousTimer = Time.time;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (airGauge && DataManager.playerData.AirGaugeValue != airGauge.Value)
            DataManager.playerData.AirGaugeValue = airGauge.Value;
        if (waterGauge && DataManager.playerData.WaterGaugeValue != waterGauge.Value)
            DataManager.playerData.WaterGaugeValue = waterGauge.Value;
        if (bioGauge && DataManager.playerData.BioGaugeValue != bioGauge.Value)
            DataManager.playerData.BioGaugeValue = bioGauge.Value;
        if (lifeGauge && player)
            lifeGauge.Value = (float)player.CurrentHealth / player.startingHealth * 100;

        if(!levelIsLevelSelection && Time.time - continuousTimer > frequencyForTimer && player)
        {
            DataManager.playerData.AddPosition(player.transform.position);
            continuousTimer = Time.time;
        }
    }

    public void PlayerAcquiredSkill(string skillName)
    {
        if(DataManager.playerData != null)
            DataManager.playerData.SetSkill(skillName, true);
    }

    public void PlayerAttemptedSkill(string skillName)
    {
        if(DataManager.playerData != null)
            DataManager.playerData.SetSkill(skillName, false);
    }

    public void PlayerMetEnemy(string enemyType)
    {
        DataManager.playerData.PlayerMetEnemy(enemyType, SceneManager.GetActiveScene().name);
    }
}

public class DataManager
{
    private static string dataPath = "Data/playerProgression.txt";

    public static PlayerData playerData;

    public static void LoadData()
    {
        Serializer.LoadData<PlayerData>(dataPath, out playerData);
        if(playerData == null)
        {
            playerData = new PlayerData();
            SaveData();
        }
    }

    public static void SaveData()
    {
        if(playerData != null)
            Serializer.SaveData(dataPath, playerData);
    }
}

[Serializable]
public class PlayerData
{

    private float airGaugeValue = -1;
    private float waterGaugeValue = -1;
    private float bioGaugeValue = -1;

    public enum Language
    {
        english = 0,
        french = 1,
        italian = 2,
        romanian = 3
    }
    private Language playerLanguage = Language.french;

    //Dictionary<skill name, whether the skill was acquired>
    private Dictionary<string, bool> skills;

    //Dictionary<enemy type, List<Level name where it was met>>
    private Dictionary<string, List<string>> enemies;

    private LinkedList<KeyValuePair<string, List<Vector2>>> levelsHistory;


    public PlayerData()
    {
        if (airGaugeValue < 0)
            airGaugeValue = UnityEngine.Random.Range(40, 60);
        if (waterGaugeValue < 0)
            waterGaugeValue = UnityEngine.Random.Range(40, 60);
        if (bioGaugeValue < 0)
            bioGaugeValue = UnityEngine.Random.Range(40, 60);
    }

    public float AirGaugeValue
    {
        get
        {
            return airGaugeValue;
        }

        set
        {
            airGaugeValue = value;
            DataManager.SaveData();
        }
    }

    public float WaterGaugeValue
    {
        get
        {
            return waterGaugeValue;
        }

        set
        {
            waterGaugeValue = value;
            DataManager.SaveData();
        }
    }

    public float BioGaugeValue
    {
        get
        {
            return bioGaugeValue;
        }

        set
        {
            bioGaugeValue = value;
            DataManager.SaveData();
        }
    }

    public Language PlayerLanguage
    {
        get
        {
            return playerLanguage;
        }

        set
        {
            playerLanguage = value;
            DataManager.SaveData();
        }
    }

    public List<KeyValuePair<string, List<Vector2>>> LevelsHistoryToList
    {
        get
        {
            if (levelsHistory == null)
                return null;

            List<KeyValuePair<string, List<Vector2>>> list = new List<KeyValuePair<string, List<Vector2>>>();
            LinkedListNode<KeyValuePair<string, List<Vector2>>> node = levelsHistory.First;
            while(node != null)
            {
                list.Add(node.Value);
                node = node.Next;
            }
            return list;
        }
    }

    private string FormatString(string s)
    {
        // format answer
        s = s.Replace('é', 'e');
        s = s.Replace('è', 'e');
        s = s.Replace('à', 'a');
        s = s.ToLower();
        return s;
    }

    public void SetSkill(string skillName, bool skillAcquired)
    {
        skillName = FormatString(skillName);

        if (skills == null)
            skills = new Dictionary<string, bool>();

        if (skills.ContainsKey(skillName))
            skills[skillName] = skillAcquired;
        else
            skills.Add(skillName, skillAcquired);
    }

    public bool? CheckSkill(string skillName)
    {
        skillName = FormatString(skillName);

        if (skills.ContainsKey(skillName))
            return skills[skillName];

        return null;
    }

    public void PlayerMetEnemy(string enemyType, string levelName)
    {
        enemyType = FormatString(enemyType);

        if (enemies == null)
            enemies = new Dictionary<string, List<string>>();

        if (!enemies.ContainsKey(enemyType))
            enemies.Add(enemyType, new List<string>());

        if (enemies[enemyType] == null)
            enemies[enemyType] = new List<string>();

        if (!enemies[enemyType].Contains(levelName))
            enemies[enemyType].Add(levelName);
    }

    public void AddLevelToHistory(string levelName)
    {
        if (levelsHistory == null)
            levelsHistory = new LinkedList<KeyValuePair<string, List<Vector2>>>();

        levelsHistory.AddFirst(new LinkedListNode<KeyValuePair<string, List<Vector2>>>(new KeyValuePair<string, List<Vector2>>(levelName, new List<Vector2>())));

        while (levelsHistory.Count > 10)
            levelsHistory.RemoveLast();
    }

    public void AddPosition(Vector2 position)
    {
        if (levelsHistory == null)
            levelsHistory = new LinkedList<KeyValuePair<string, List<Vector2>>>();

        if(levelsHistory.Count > 0)
        {
            if (levelsHistory.First.Value.Value == null)
                levelsHistory.First.Value = new KeyValuePair<string, List<Vector2>>(levelsHistory.First.Value.Key, new List<Vector2>());

            levelsHistory.First.Value.Value.Add(position);
            levelsHistory.First.Value = new KeyValuePair<string, List<Vector2>>(levelsHistory.First.Value.Key, levelsHistory.First.Value.Value);
        }
    }
}
