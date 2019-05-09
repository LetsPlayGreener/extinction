using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[CustomEditor(typeof(GaugeInfo))]
[CanEditMultipleObjects]
public class GaugeInfoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GaugeInfo gauge = target as GaugeInfo;
        gauge.Value = gauge.value;
    }
}
#endif
