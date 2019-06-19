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
        if (DataManager.playerData != null)
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
#if UNITY_WEBGL
        try
        {
            Serializer.DeserializeData<PlayerData>(PlayerPrefs.GetString("PlayerWebGLData"), out playerData);
        }
        catch (Exception)
        {
            playerData = null;
        }
#else
        Serializer.LoadData<PlayerData>(dataPath, out playerData);
#endif
        if (playerData == null)
        {
            playerData = new PlayerData();
            SaveData();
        }
    }

    public static void SaveData()
    {
        if (playerData != null)
        {
#if UNITY_WEBGL
            PlayerPrefs.SetString("PlayerWebGLData", Serializer.SerializeData(playerData));
#else
            Serializer.SaveData(dataPath, playerData);
#endif
        }
    }
}

[Serializable]
public class PlayerData
{
    [Serializable]
    public struct SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public void Fill(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }

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

    private LinkedList<KeyValuePair<string, List<SerializableVector3>>> levelsHistory;


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

    public List<KeyValuePair<string, List<Vector3>>> LevelsHistoryToList
    {
        get
        {
            if (levelsHistory == null)
                return null;

            List<Vector3> tmpList;
            List<KeyValuePair<string, List<Vector3>>> list = new List<KeyValuePair<string, List<Vector3>>>();
            LinkedListNode<KeyValuePair<string, List<SerializableVector3>>> node = levelsHistory.First;
            while(node != null)
            {
                tmpList = new List<Vector3>();
                for(int i = 0; i < node.Value.Value.Count; i++)
                {
                    tmpList.Add(node.Value.Value[i].ToVector3());
                }
                list.Add(new KeyValuePair<string, List<Vector3>>(node.Value.Key, tmpList));
                node = node.Next;
            }
            return list;
        }
    }

    public List<KeyValuePair<string, bool>> Skills
    {
        get
        {
            if(skills != null)
            {
                List<KeyValuePair<string, bool>> skillsPairs = new List<KeyValuePair<string, bool>>();
                foreach (string skillName in skills.Keys)
                    skillsPairs.Add(new KeyValuePair<string, bool>(skillName, skills[skillName]));
                return skillsPairs;
            }

            return null;
        }
    }

    public static string FormatString(string s)
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

    public void PlayerMetEnemy(string enemyName, string levelName)
    {
        enemyName = FormatString(enemyName);

        if (enemies == null)
            enemies = new Dictionary<string, List<string>>();

        if (!enemies.ContainsKey(enemyName))
            enemies.Add(enemyName, new List<string>());

        if (enemies[enemyName] == null)
            enemies[enemyName] = new List<string>();

        if (!enemies[enemyName].Contains(levelName))
            enemies[enemyName].Add(levelName);
    }

    public List<string> CheckEnemyMet(string enemyName)
    {
        if (enemies == null)
            enemies = new Dictionary<string, List<string>>();

        if (enemies.ContainsKey(FormatString(enemyName)))
            return new List<string>(enemies[FormatString(enemyName)]);
        return null;
    }

    public void AddLevelToHistory(string levelName)
    {
        if (levelsHistory == null)
            levelsHistory = new LinkedList<KeyValuePair<string, List<SerializableVector3>>>();

        levelsHistory.AddFirst(new LinkedListNode<KeyValuePair<string, List<SerializableVector3>>>(new KeyValuePair<string, List<SerializableVector3>>(levelName, new List<SerializableVector3>())));

        while (levelsHistory.Count > 10)
            levelsHistory.RemoveLast();
    }

    public void AddPosition(Vector3 position)
    {
        if (levelsHistory == null)
            levelsHistory = new LinkedList<KeyValuePair<string, List<SerializableVector3>>>();

        if(levelsHistory.Count > 0)
        {
            if (levelsHistory.First.Value.Value == null)
                levelsHistory.First.Value = new KeyValuePair<string, List<SerializableVector3>>(levelsHistory.First.Value.Key, new List<SerializableVector3>());

            levelsHistory.First.Value.Value.Add(new SerializableVector3(position));
        }
    }
}
