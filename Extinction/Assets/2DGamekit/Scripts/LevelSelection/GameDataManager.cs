using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit2D
{
    public class GameDataManager : MonoBehaviour
    {
        public static float airGaugeValue = -1;
        public static float waterGaugeValue = -1;
        public static float bioGaugeValue = -1;

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

                if (airGauge)
                {
                    if (airGaugeValue < 0)
                        airGauge.Value = Random.Range(40, 60);
                    else
                        airGauge.Value = airGaugeValue;
                }
                if (waterGauge)
                {
                    if (waterGaugeValue < 0)
                        waterGauge.Value = Random.Range(40, 60);
                    else
                        waterGauge.Value = waterGaugeValue;
                }
                if (bioGauge)
                {
                    if (bioGaugeValue < 0)
                        bioGauge.Value = Random.Range(40, 60);
                    else
                        bioGauge.Value = bioGaugeValue;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (airGauge)
                airGaugeValue = airGauge.Value;
            if (waterGauge)
                waterGaugeValue = waterGauge.Value;
            if (bioGauge)
                bioGaugeValue = bioGauge.Value;
            if (lifeGauge && player)
                lifeGauge.Value = (float)player.CurrentHealth / player.startingHealth * 100;
        }
    }
}
