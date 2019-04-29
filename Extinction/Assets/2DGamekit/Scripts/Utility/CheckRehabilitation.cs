using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit2D
{
    public class CheckRehabilitation : MonoBehaviour
    {
        public CollectableType type;

        // Start is called before the first frame update
        void Start()
        {
            if (!type)
            {
                Debug.LogError("The CheckRehabilitation component has no CollectableType assigned and thus cannot work");
                enabled = false;
            }
        }

        private void OnEnable()
        {
            if (CollectionManager.instance)
                CollectionManager.instance.rehabCheckers.Add(this);
        }

        private void OnDisable()
        {
            if (CollectionManager.instance && CollectionManager.instance.rehabCheckers.Contains(this))
                CollectionManager.instance.rehabCheckers.Remove(this);
        }

        public bool CheckType(CollectableType typeToCheck)
        {
            return typeToCheck && type.feature == typeToCheck.feature && type.typeName == typeToCheck.typeName;
        }
    }
}
