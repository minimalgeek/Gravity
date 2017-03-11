using UnityEngine;
using UnityEditor;
using System.Collections;
using Gamelogic.Extensions;

public class TileGUI : EditorWindow
{
    // The Level Root object, into which all objects are placed
    GameObject root;
    Transform rootTransform;

    GameObject block;
    GameObject ghost;

    // Block placing modes
    enum PlacingModes
    {
        Off, Single, Arc, Wall, Ring
    }
    string[] placingModes = { "Off (0)", "Single (1)", "Arc (2)", "Wall (3)", "Ring (4)" };
    int placingMode = 0;
    private PlacingModes PlacingMode
    {
        get
        {
            switch (placingMode)
            {
                case 0:
                    return PlacingModes.Off;
                case 1:
                    return PlacingModes.Single;
                case 2:
                    return PlacingModes.Arc;
                case 3:
                    return PlacingModes.Wall;
                case 4:
                    return PlacingModes.Ring;
                default:
                    return PlacingModes.Off;
            }
        }
        set
        {
            switch (value)
            {
                case PlacingModes.Off:
                    placingMode = 0;
                    break;
                case PlacingModes.Single:
                    placingMode = 1;
                    break;
                case PlacingModes.Arc:
                    placingMode = 2;
                    break;
                case PlacingModes.Wall:
                    placingMode = 3;
                    break;
                case PlacingModes.Ring:
                    placingMode = 4;
                    break;
                default:
                    placingMode = 0;
                    break;
            }
        }
    }

    bool wallPlacingStarted = false;
    Vector3 wallFirstPosition;

    bool autoRotateEnabled = true;
    bool radialSnapEnabled = true;
    bool angularSnapEnabled = true;

    private int radialGridDensity = 2;
    // TEMP, TODO: automatic density
    //private int angularGridDensity = 360;

    // Add menu item named "Tile Editor" to the Tools menu
    [MenuItem("Tools/Tile Editor")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        GetWindow(typeof(TileGUI));
    }

    void OnEnable()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
        titleContent.text = "Tile Editor";

        CreateLevelRoot();

    }

    // Called when updating this editor window
    void OnGUI()
    {
        GUILayout.Label("Object Placement", EditorStyles.boldLabel);

        placingMode = GUILayout.SelectionGrid(placingMode, placingModes, placingModes.Length, "button");
        if (PlacingMode != PlacingModes.Off) Tools.current = Tool.None;
        else if (Tools.current == Tool.None) Tools.current = Tool.View;

        EditorGUILayout.BeginHorizontal();
        autoRotateEnabled = EditorGUILayout.Toggle("Auto rotate", autoRotateEnabled);
        if (GUILayout.Button("Rotate Selection"))
            RotateSelection();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Snapping:");
        EditorGUILayout.BeginVertical();
        radialSnapEnabled = EditorGUILayout.ToggleLeft("Radial", radialSnapEnabled);
        angularSnapEnabled = EditorGUILayout.ToggleLeft("Angular", angularSnapEnabled);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        block = (GameObject)EditorGUILayout.ObjectField("Prefab", block, typeof(GameObject), false);


        GUILayout.Label("Level Parameters", EditorStyles.boldLabel);


    }

    // Called when updating the Scene view
    void OnSceneGUI(SceneView sceneview)
    {
        if (root == null)
            CreateLevelRoot();

        Event e = Event.current;

        if (e.type == EventType.KeyDown)
        {
            switch (e.keyCode)
            {
                case KeyCode.Alpha0:
                    PlacingMode = PlacingModes.Off;
                    break;
                case KeyCode.Alpha1:
                    PlacingMode = PlacingModes.Single;
                    break;
                case KeyCode.Alpha2:
                    PlacingMode = PlacingModes.Arc;
                    break;
                case KeyCode.Alpha3:
                    PlacingMode = PlacingModes.Wall;
                    break;
                case KeyCode.Alpha4:
                    PlacingMode = PlacingModes.Ring;
                    break;
                default:
                    break;
            }
        }

        if (e.type == EventType.MouseMove)
        {
            if (PlacingMode != PlacingModes.Off)
            {
                try
                {
                    // Place or move ghost
                    if (ghost == null)
                    {
                        // Remove stuck ghosts from a previous session
                        GhostPlaceholder[] formerGhosts = FindObjectsOfType<GhostPlaceholder>();
                        foreach (GhostPlaceholder formerGhost in formerGhosts)
                        {
                            DestroyImmediate(formerGhost.gameObject);
                        }

                        // Place new ghost
                        Vector3 pos = ScreenToWorldSnap(e.mousePosition);
                        ghost = Instantiate(block, pos, GetRotator(pos));
                        ghost.hideFlags = HideFlags.HideInHierarchy;
                        ghost.AddComponent<GhostPlaceholder>();
                    }
                    else
                    {
                        // Update ghost attitude
                        ghost.transform.position = ScreenToWorldSnap(e.mousePosition);
                        ghost.transform.rotation = GetRotator(ghost.transform.position);
                    }
                }
                catch { }
            }
            else
                DestroyImmediate(ghost.gameObject);
        }

        if (e.type == EventType.MouseDown)
        {
            if (e.button == 0)
            {
                switch (PlacingMode)
                {
                    case PlacingModes.Off:
                        break;
                    case PlacingModes.Single:
                        {
                            Vector3 pos = ScreenToWorldSnap(e.mousePosition);
                            GameObject newObject = Instantiate(block, pos, GetRotator(pos), rootTransform);
                            break;
                        }
                    case PlacingModes.Arc:
                        break;
                    case PlacingModes.Wall:
                        {
                            if (wallPlacingStarted)
                            {
                                PlaceWall(wallFirstPosition, ScreenToWorldSnap(e.mousePosition));
                                wallPlacingStarted = false;
                            }
                            else
                            {
                                wallFirstPosition = ScreenToWorldSnap(e.mousePosition);
                                wallPlacingStarted = true;
                            }
                            break;
                        }
                    case PlacingModes.Ring:
                        PlaceRing(ScreenToWorldSnap(e.mousePosition));
                        DrawGrid(Vector2.zero);
                        break;
                    default:
                        break;
                }
            }
        }

        switch (PlacingMode)
        {
            case PlacingModes.Off:
                break;
            case PlacingModes.Single:
                break;
            case PlacingModes.Arc:
                break;
            case PlacingModes.Wall:
                if (wallPlacingStarted)
                {
                    Vector3 fromDir = Vector3.Normalize(wallFirstPosition);
                    Debug.DrawLine(Vector2.zero, fromDir * 50, Color.cyan);
                    Vector3 to = fromDir * ScreenToWorldSnap(e.mousePosition).magnitude;
                    Debug.DrawLine(wallFirstPosition, to, Color.red);
                }
                else
                    Debug.DrawLine(Vector2.zero, Vector3.Normalize(ScreenToWorldSnap(e.mousePosition)) * 50, Color.cyan);
                break;
            case PlacingModes.Ring:
                break;
            default:
                break;
        }
    }

    Quaternion GetRotator(Vector2 pos)
    {
        return GetRotator(pos, false);
    }

    Quaternion GetRotator(Vector2 pos, bool forcePolar)
    {
        if (forcePolar || autoRotateEnabled)
        {
            return Quaternion.AngleAxis(Mathf.Atan2(pos.x, -pos.y) * Mathf.Rad2Deg, Vector3.forward);
        }

        return Quaternion.identity;
    }

    Vector3 ScreenToWorld(Vector2 mousePosition)
    {
        mousePosition.y = Camera.current.pixelHeight - mousePosition.y;
        return Camera.current.ScreenToWorldPoint(mousePosition).WithZ(0);
    }

    Vector3 ScreenToWorldSnap(Vector2 mousePosition)
    {
        return GetPos(ScreenToWorld(mousePosition));
    }

    // Applies the enabled snappings to a vector
    Vector3 GetPos(Vector3 pos)
    {
        if (radialSnapEnabled)
            pos = SnapRadial(pos);

        if (angularSnapEnabled)
            pos = SnapAngular(pos);

        return pos;
    }

    // Applies radial snapping to a vector
    Vector3 SnapRadial(Vector3 pos)
    {
        float r = pos.magnitude;
        return pos * ((Mathf.Floor(r * radialGridDensity) + 1) / (radialGridDensity * r));
    }

    // Applies angular snapping to a vector
    Vector3 SnapAngular(Vector3 pos)
    {
        int angularGridDensity = GetAngularGridDensity(pos.magnitude);
        float phi = (Mathf.Floor(angularGridDensity * Mathf.Atan2(pos.y, pos.x) / Mathf.PI * 0.5f) + 0.5f) * Mathf.PI * 2 / angularGridDensity;
        return new Vector3(Mathf.Cos(phi), Mathf.Sin(phi), 0) * pos.magnitude;
    }

    int GetAngularGridDensity(float r)
    {
        return (int)(12 * Mathf.Pow(2, Mathf.Floor(Mathf.Log(r, 2) + 0.5f)));
    }

    void DrawGrid(Vector2 centre)
    {
        Debug.DrawLine(new Vector2(0, 0), new Vector2(1, 1), Color.cyan);
    }

    // Place a ring from a given point
    void PlaceRing(Vector3 pos)
    {
        int angularGridDensity = GetAngularGridDensity(pos.magnitude);
        float deltaAngle = 360f / angularGridDensity;
        Quaternion rotator = Quaternion.Euler(0, 0, deltaAngle);
        for (int i = 0; i < angularGridDensity; i++)
        {
            pos = rotator * pos;
            GameObject newObject = Instantiate(block, pos, GetRotator(pos), rootTransform);
        }
    }

    // Place a ring from a given point
    void PlaceWall(Vector3 from, Vector3 to)
    {
        float R1 = from.magnitude;
        float R2 = to.magnitude;
        float deltaR = (R2 > R1 ? 1f : -1f) / radialGridDensity;
        int n = Mathf.FloorToInt(Mathf.Abs(R2 - R1) * radialGridDensity);
        float r = R1;
        Vector3 fromDir = Vector3.Normalize(from);
        Quaternion rotator = GetRotator(from);

        for (int i = 0; i < n; ++i)
        {
            GameObject newObject = Instantiate(block, fromDir * r, rotator, rootTransform);
            r += deltaR;
        }
    }

    void CreateLevelRoot()
    {
        root = GameObject.Find("Level Root");
        if (root == null)
            root = new GameObject("Level Root");
        rootTransform = root.transform;
    }

    void RotateSelection()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            if (obj.transform.IsChildOf(rootTransform))
                obj.transform.rotation = GetRotator(obj.transform.position, true);
        }
    }
}
