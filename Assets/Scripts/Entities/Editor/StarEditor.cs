using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Star))]
[CanEditMultipleObjects]
public class StarEditor : Editor
{
    SerializedProperty color;
    SerializedProperty pickUpTime;

    void OnEnable()
    {
        color = serializedObject.FindProperty("color");
        pickUpTime = serializedObject.FindProperty("pickUpTime");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Star star = ((Star)target);

        using (var check = new EditorGUI.ChangeCheckScope())
        {
            color.colorValue = EditorGUILayout.ColorField("Color", color.colorValue);
            if (check.changed)
            {
                Light light = star.GetComponentInChildren<Light>();
                if (light != null) light.color = color.colorValue;
                Renderer renderer = star.GetComponent<Renderer>();
                if (renderer != null) renderer.sharedMaterial.color = color.colorValue;
            }
        }
        pickUpTime.floatValue = EditorGUILayout.FloatField("Pick-up Time", pickUpTime.floatValue);

        serializedObject.ApplyModifiedProperties();
    }
}
