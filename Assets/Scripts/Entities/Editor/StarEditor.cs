using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Star))]
[CanEditMultipleObjects]
public class StarEditor : Editor
{
    [SerializeField]
    private Color color = new Color(1f, 0.847058824f, 0f, 1f); //FFD800FF

    SerializedProperty pickUpTime;

    void OnEnable()
    {
        //color = serializedObject.FindProperty("color");
        pickUpTime = serializedObject.FindProperty("pickUpTime");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Star star = ((Star)target);

        using (var check = new EditorGUI.ChangeCheckScope())
        {
            color = EditorGUILayout.ColorField("Color", color);
            if (check.changed)
            {
                Light light = star.GetComponentInChildren<Light>();
                if (light != null) light.color = color;
                Renderer renderer = star.GetComponent<Renderer>();
                if (renderer != null) renderer.sharedMaterial.color = color;
            }
        }
        pickUpTime.floatValue = EditorGUILayout.FloatField("Pick-up Time", pickUpTime.floatValue);

        serializedObject.ApplyModifiedProperties();
    }
}
