using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
[ExecuteInEditMode]
public class LevelEditorStarter : MonoBehaviour
{
#if UNITY_EDITOR
    void Start()
    {
        LevelEditorGUI.ShowWindow();
    }
#endif
}
