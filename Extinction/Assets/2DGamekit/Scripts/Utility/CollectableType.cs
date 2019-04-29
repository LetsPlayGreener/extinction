using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit2D
{
    public class CollectableType : MonoBehaviour
    {
        public CollectableEvent OnInteract;
        public CollectableEvent OnInteractWithNoUI;
        public CollectableEvent OnFirstCollection;
        public CollectableEvent OnCollected;
        public CollectableEvent OnFailCollection;
        public CollectableEvent OnGenerated;

        [HideInInspector]
        public int feature;
        [HideInInspector]
        public int typeName;
        public Sprite sprite;

        private float height;
        private Vector3 scale;

        public bool collectableByPlayer = true;

        private string id;

        private bool collectedAtLeastOnce = false;

        #region Getter/Setter
        public float Height
        {
            get
            {
                return height;
            }

            set
            {
                height = value;
            }
        }

        public Vector3 Scale
        {
            get
            {
                return scale;
            }

            set
            {
                scale = value;
            }
        }

        public string Id
        {
            get
            {
                return id;
            }
        }

        public bool WasCollectedAtLeastOnce
        {
            get
            {
                return collectedAtLeastOnce;
            }
        }
        #endregion

        private void Awake()
        {
            SetID();
        }

        public void SetID()
        {
            id = string.Concat(feature, ".", typeName);
        }

        public void SetAsCollectedAtLeastOnce()
        {
            collectedAtLeastOnce = true;
        }

        public override string ToString()
        {
            if (CollectionManager.instance)
                return string.Concat(CollectionManager.instance.listFeatures[feature], ".", CollectionManager.instance.listNames[typeName]);
            return "";
        }
    }
}
