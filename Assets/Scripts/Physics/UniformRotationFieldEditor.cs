using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UniformRotationField))]
public class UniformRotationFieldEditor : Editor
{
    SerializedProperty omega;
    SerializedProperty frequency;
    SerializedProperty period;

    void OnEnable()
    {
        // Setup the SerializedProperties.
        omega = serializedObject.FindProperty("angularVelocity");
        frequency = serializedObject.FindProperty("angularFrequency");
        period = serializedObject.FindProperty("period");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        UniformRotationField field = ((UniformRotationField)target);

        EditorGUILayout.LabelField("+: CCW, -: CW");
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            EditorGUILayout.DelayedFloatField(omega, new GUIContent("Angular Velocity (rad/s)"));
            if (check.changed)
            {
                field.AngularVelocity = omega.floatValue;
                frequency.floatValue = field.AngularFrequency;
                period.floatValue = field.Period;
                field.Recalculate();
            }
        }
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            EditorGUILayout.DelayedFloatField(frequency, new GUIContent("Angular Frequency (1/s)"));
            if (check.changed)
            {
                field.AngularFrequency = frequency.floatValue;
                omega.floatValue = field.AngularVelocity;
                period.floatValue = field.Period;
                field.Recalculate();
            }
        }
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            EditorGUILayout.DelayedFloatField(period, new GUIContent("Period (s)"));
            if (check.changed)
            {
                field.Period = period.floatValue;
                frequency.floatValue = field.AngularFrequency;
                omega.floatValue = field.AngularVelocity;
                field.Recalculate();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
