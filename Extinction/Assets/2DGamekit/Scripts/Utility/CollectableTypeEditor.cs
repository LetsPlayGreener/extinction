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

            EditorGUI.BeginChangeCheck();
            int feature = EditorGUILayout.Popup("Feature", type.feature, CollectionManager.instance.listFeatures);
            int typeName = EditorGUILayout.Popup("Species", type.typeName, CollectionManager.instance.listNames);

            if (CollectionManager.instance && EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "CollectableType value changed");
                type.feature = feature;
                type.typeName = typeName;
            }
        }
    }
#endif
}
