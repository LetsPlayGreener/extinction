using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit2D
{
    public class ZoneTracking : MonoBehaviour
    {
        public static ZoneTracking instance;

        public ZoneTracking()
        {
            if (!instance)
                instance = this;
        }

        public void SetZone(int zone)
        {
            if (CollectionManager.instance && zone < CollectionManager.instance.listFeatures.Length)
            {
                CollectionManager.instance.PlayerCurrentZone = zone;
            }
    }
    }
}
