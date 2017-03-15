using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ArcTile))]
[CanEditMultipleObjects]
public class ArcTileEditor : Editor
{
    SerializedProperty innerRadiusProp;
    SerializedProperty outerRadiusProp;
    SerializedProperty arcNumeratorProp;
    SerializedProperty arcDenominatorProp;
    SerializedProperty angularResolutionProp;
    SerializedProperty zOffsetProp;

    void OnEnable()
    {
        // Setup the SerializedProperties.
        angularResolutionProp = serializedObject.FindProperty("angularResolution");
        innerRadiusProp = serializedObject.FindProperty("innerRadius");
        outerRadiusProp = serializedObject.FindProperty("outerRadius");
        arcNumeratorProp = serializedObject.FindProperty("arcNumerator");
        arcDenominatorProp = serializedObject.FindProperty("arcDenominator");
        zOffsetProp = serializedObject.FindProperty("zOffset");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.DelayedFloatField(innerRadiusProp, new GUIContent("Inner Radius"));
        EditorGUILayout.DelayedFloatField(outerRadiusProp, new GUIContent("Outer Radius"));
        EditorGUILayout.DelayedIntField(arcNumeratorProp, new GUIContent("Arc Length Numerator"));
        EditorGUILayout.DelayedIntField(arcDenominatorProp, new GUIContent("Arc Length Denominator"));
        EditorGUILayout.DelayedIntField(angularResolutionProp, new GUIContent("Angular Resolution"));
        EditorGUILayout.DelayedFloatField(zOffsetProp, new GUIContent("Z Offset"));

        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Update Arc"))
        {
            foreach (ArcTile arcTile in targets)
            {
                arcTile.CreatePoints();
            }
        }
    }
}