using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit2D
{
    public class ZoneTracking : MonoBehaviour
    {
        public void SetZone(int zone)
        {
            if (CollectionManager.instance && zone < CollectionManager.instance.listZones.Length)
            {
                CollectionManager.instance.PlayerCurrentZone = zone;
            }
    }
    }
}
