using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBuoyancyEffectorWaterValue : MonoBehaviour
{
    public float density = 25;
    public float surfaceLevel = 1;
    private BuoyancyEffector2D buoyancyEffector2D;

    // Start is called before the first frame update
    void Start()
    {
        buoyancyEffector2D = GetComponent<BuoyancyEffector2D>();

        if (buoyancyEffector2D)
        {
            buoyancyEffector2D.density = density;
            buoyancyEffector2D.surfaceLevel = surfaceLevel;
        }
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
