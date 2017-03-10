using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;
using UnityEditor;

[InitializeOnLoad]
[ExecuteInEditMode]
public class TileEngine : GLMonoBehaviour
{
    public List<GameObject> prefabList;
    public GameObject tileRoot;
    private GameObject currentPrefab;
    public GameObject CurrentPrefab
    {
        get
        {
            if (currentPrefab == null)
            {
                currentPrefab = prefabList[0];
            }
            return currentPrefab;
        }
        set
        {
            currentPrefab = value;
        }
    }

    void Start()
    {
        SceneView.onSceneGUIDelegate += OnSceneGooey;
    }

    void Update()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGooey;
        SceneView.onSceneGUIDelegate += OnSceneGooey;
    }

    void OnSceneGooey(SceneView sceneview)
    {
        Event e = Event.current;

        if (e.type == EventType.MouseDown)
        {
            if (e.button == 0)
            {
				Vector2 mousePosNew = e.mousePosition;
				mousePosNew.y = Camera.current.pixelHeight - mousePosNew.y;
                Vector3 posToInstantiate = Camera.current.ScreenToWorldPoint(mousePosNew).WithZ(0);
                GameObject newObject = SceneView.Instantiate(CurrentPrefab, posToInstantiate, Quaternion.identity);
                newObject.transform.parent = tileRoot.transform;
            }
        }

        if (e.type == EventType.KeyUp)
        {
            CurrentPrefab = prefabList[0] == currentPrefab ? prefabList[1] : prefabList[0];
        }
    }

}
