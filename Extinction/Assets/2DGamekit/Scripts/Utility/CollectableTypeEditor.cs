using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gamekit2D
{
#if UNITY_EDITOR
    [CustomEditor(typeof(CollectableType))]
    [CanEditMultipleObjects]
    public class CollectableTypeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            CollectableType type = target as CollectableType;
            
            if (CollectionManager.instance)
            {
                type.zone = EditorGUILayout.Popup("Zone", type.zone, CollectionManager.instance.listZones);
                type.classification = EditorGUILayout.Popup("Classification", type.classification, CollectionManager.instance.listClassifications);
                type.feature = EditorGUILayout.Popup("Feature", type.feature, CollectionManager.instance.listFeatures);
                type.species = EditorGUILayout.Popup("Species", type.species, CollectionManager.instance.listSpecies);
            }
        }
    }
#endif
}
