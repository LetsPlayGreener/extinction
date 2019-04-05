using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance;

    public GaugeInfo airGauge;
    public GaugeInfo waterGauge;
    public GaugeInfo bioGauge;

    public GaugeInfo lifeGauge;
    public Damageable player;

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
}
