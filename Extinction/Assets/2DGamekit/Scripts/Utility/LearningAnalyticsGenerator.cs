using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIG.GBLXAPI;

public class LearningAnalyticsGenerator : MonoBehaviour
{
    public static LearningAnalyticsGenerator instance;
    public bool canGenerateLA = true;
    public float frequenceForContinuous = 0.5f;
    private float continuousTimer = float.MaxValue;

    public Transform player;
    private Vector3 oldPosition;

    private void Awake()
    {
        if (Application.isPlaying)
        {
            instance = this;
            if (!GBLXAPI.Instance.IsInit())
                GBLXAPI.Instance.init(GBL_Interface.lrsAddresses, GBL_Interface.standardsConfigDefault, GBL_Interface.standardsConfigUser);

            if (player)
            {
                if(canGenerateLA)
                    GBL_Interface.SendStatement("moved", "avatar", "Alex", new Dictionary<string, List<string>>()
                    {
                        { "position", new List<string>() { player.position.ToString() } }
                    });
                oldPosition = player.position;
                continuousTimer = Time.time;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canGenerateLA)
        {
            if(Time.time - continuousTimer > frequenceForContinuous)
            {
                if(player.position != oldPosition)
                {
                    GBL_Interface.SendStatement("moved", "avatar", "Alex", new Dictionary<string, List<string>>()
                    {
                        { "position", new List<string>() { player.position.ToString() } }
                    });
                    oldPosition = player.position;
                }
                continuousTimer = Time.time;
            }
        }
    }
}
