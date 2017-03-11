using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
[ExecuteInEditMode]
public class TileEngine : MonoBehaviour
{
    void Start()
    {
        TileGUI.ShowWindow();
    }
}
