using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

[CustomEditor(typeof(LevelLoader))]
public class LevelLoaderEditor : Editor
{

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Collect all scenes"))
        {
            LevelLoader loader = (LevelLoader)target;
            GenerateButtons(loader);
        }
        this.DrawDefaultInspector();
    }

    private void GenerateButtons(LevelLoader loader)
    {
        if (loader.buttonPrefab && loader.panel) {
            UnityEngine.Object[] buttons = FindObjectsOfType(typeof(Button));
            foreach (var btn in buttons) {
                DestroyImmediate(((Button)btn).gameObject);
            }
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
                GameObject newButton = Instantiate(loader.buttonPrefab, loader.panel.transform);
                Button btn = newButton.GetComponent<Button>();
                ButtonController cntrl = newButton.GetComponent<ButtonController>();
                string levelName = Path.GetFileName(scene.path).Split('.')[0];
                Debug.Log(levelName + " is added to the loader menu");
                cntrl.sceneNameToStart = levelName;
                Text txt = btn.GetComponentInChildren<Text>();
                txt.text = levelName;

                newButton.transform.localScale = Vector3.one;
            }
        }
    }
}