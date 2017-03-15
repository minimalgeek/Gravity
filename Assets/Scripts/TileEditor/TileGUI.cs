using UnityEngine;
using UnityEditor;
using System.Collections;
using Gamelogic.Extensions;
using System.Collections.Generic;

public class TileGUI : EditorWindow
{
    // The Level Root object, into which all objects are placed
    GameObject root;
    Transform rootTransform;

    TileEditorCatalog _catalog;
    public TileEditorCatalog Catalog
    {
        get { return _catalog; }
        set
        {
            _catalogLoaded = _catalog != null;
            if (_catalog == null || _catalog != value)
            {
                _catalog = value;
                _catalogLoaded = value != null;
                PopulateStock();
            }
        }
    }
    bool _catalogLoaded = false;
    public bool CatalogLoaded
    {
        get { return _catalogLoaded; }
    }
    int catalogIconSize = 128;
    private class InventoryEntry : TileEditorCatalog.CatalogEntry
    {
        public bool universal = false;
        public SortedDictionary<int, SortedDictionary<int, GameObject>> stock = new SortedDictionary<int, SortedDictionary<int, GameObject>>();
        public new SortedDictionary<int, GameObject> this[int div]
        {
            get { return stock[div]; }
        }
    }
    List<InventoryEntry> inventory;

    Vector2 itemSelectorScrollPosition;
    int selectedItem;

    //GameObject block;
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
    int selectedAngularDensity = 0;
    int customAngularDensity = 192;

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

        if (Catalog == null)
        {
            TileEditorCatalog[] catalogs = Resources.FindObjectsOfTypeAll<TileEditorCatalog>();
            if (catalogs.Length > 0) Catalog = catalogs[0];
        }
    }


    // Called when updating this editor window
    void OnGUI()
    {
        Catalog = (TileEditorCatalog)EditorGUILayout.ObjectField("Object Catalog", Catalog, typeof(TileEditorCatalog), false);

        if (!CatalogLoaded)
            EditorGUILayout.HelpBox("Load a catalog (TileEditorCatalog) to access all functionality.\nTo create a catalog, use the Create menu in the Assets folder and look for Tile Editor/Tile Editor Catalog.", MessageType.Warning);

        GUILayout.Label("Object Placement", EditorStyles.boldLabel);

        using (new EditorGUI.DisabledGroupScope(!CatalogLoaded))
        {
            placingMode = GUILayout.SelectionGrid(placingMode, placingModes, placingModes.Length, "button");
            if (PlacingMode != PlacingModes.Off) Tools.current = Tool.None;
            else if (Tools.current == Tool.None) Tools.current = Tool.Move;
        }

        #region Rotation controls
        EditorGUILayout.BeginHorizontal();
        using (new EditorGUI.DisabledGroupScope(!CatalogLoaded))
        {
            autoRotateEnabled = EditorGUILayout.Toggle("Auto rotate", autoRotateEnabled);
        }
        GUILayout.Button(new GUIContent("↺", "Rotate 90° CCW"), GUILayout.Width(20));
        if (GUILayout.Button("Rotate Selection", GUILayout.ExpandWidth(false)))
            RotateSelection();
        GUILayout.Button(new GUIContent("↻", "Rotate 90° CW"), GUILayout.Width(20));
        EditorGUILayout.EndHorizontal();
        #endregion Rotation controls

        #region Radial and angular snapping controls
        using (new EditorGUI.DisabledGroupScope(!CatalogLoaded))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Snapping:", GUILayout.Width(146));
                using (new EditorGUILayout.HorizontalScope())
                {

                    var snapHS = new EditorGUILayout.HorizontalScope(GUILayout.MaxWidth(57));
                    radialSnapEnabled = GUI.Toggle(snapHS.rect, radialSnapEnabled, "", "button");
                    GUILayout.Label("  Radial");
                    snapHS.Dispose();
                    snapHS = new EditorGUILayout.HorizontalScope(GUILayout.MaxWidth(57));
                    angularSnapEnabled = GUI.Toggle(snapHS.rect, angularSnapEnabled, "", "button");
                    GUILayout.Label(" Angular");
                    snapHS.Dispose();

                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Angular Divisions", GUILayout.Width(146));
                using (new EditorGUILayout.HorizontalScope())
                {
                    selectedAngularDensity = EditorGUILayout.IntPopup(selectedAngularDensity, new string[] { "Auto", "6", "12", "24", "48", "96", "192", "384", "768", "Custom..." }, new int[] { 0, 6, 12, 24, 48, 96, 192, 384, 768, -1 });
                    if (selectedAngularDensity == -1)
                        customAngularDensity = EditorGUILayout.IntField(customAngularDensity);
                }
            }
        }
        #endregion Radial and angular snapping controls

        EditorGUILayout.Space();

        //block = (GameObject)EditorGUILayout.ObjectField("Prefab", block, typeof(GameObject), false);

        if (CatalogLoaded && !inventory[selectedItem].universal)
        {
            int[] keys = new int[inventory[selectedItem].stock.Keys.Count];
            inventory[selectedItem].stock.Keys.CopyTo(keys, 0);
            GUILayout.SelectionGrid(0, System.Array.ConvertAll(keys, x => x.ToString()), keys.Length, "button");
        }

        #region Catalog
        if (CatalogLoaded)
        {
            Color defColor = GUI.backgroundColor;
            Color highlight = EditorStyles.label.focused.textColor.Lighter().Lighter().Lighter().Lighter();

            // Prefab selector scroll view
            using (var scrollView = new EditorGUILayout.ScrollViewScope(itemSelectorScrollPosition))
            {
                GUI.backgroundColor = Color.gray;
                using (var v = new EditorGUILayout.VerticalScope())
                {
                    GUI.Box(v.rect, "");


                    // Here we populate the list view ...
                    int n = 0;
                    while (n < Catalog.Count)
                    {
                        // ... row by row ...
                        using (var H = new EditorGUILayout.HorizontalScope())
                        {
                            int w = 0;
                            do
                            {
                                // ... element by element
                                using (var h = new EditorGUILayout.HorizontalScope("button", GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false), GUILayout.MaxWidth(catalogIconSize), GUILayout.MaxHeight(catalogIconSize), GUILayout.Width(catalogIconSize), GUILayout.Height(catalogIconSize)))
                                {
                                    // A checkbox disguised as a button with the functionality of a radio button

                                    Texture2D tex = Catalog[n].GetIcon();
                                    string displayName = Catalog[n].displayName;
                                    GUIContent content = new GUIContent(tex, displayName);

                                    // The toggle with an image
                                    if (n == selectedItem)
                                        GUI.backgroundColor = highlight;
                                    if (GUI.Toggle(h.rect, n == selectedItem ? true : false, content, "button"))
                                        selectedItem = n;
                                    GUI.backgroundColor = Color.gray;

                                    // and some text over it
                                    GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
                                    style.normal.textColor = n == selectedItem ? highlight : Color.white;
                                    style.wordWrap = true;
                                    GUILayout.Label(displayName, style);
                                }
                                w += catalogIconSize;
                                n++;
                            } while (n < Catalog.Count && w < (position.width - catalogIconSize));
                        }
                    }

                    itemSelectorScrollPosition = scrollView.scrollPosition;
                }
                GUI.backgroundColor = defColor;
            }
        }
        #endregion Catalog

        GUILayout.Label("Level Parameters", EditorStyles.boldLabel);


    }



    // Called when updating the Scene view
    void OnSceneGUI(SceneView sceneview)
    {
        if (root == null)
            CreateLevelRoot();

        Event e = Event.current;

        #region KeyDown
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
        #endregion KeyDown

        #region MouseMove
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
                        // TODO:
                        //ghost = Instantiate(block, pos, GetRotator(pos));
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
            else if (ghost != null)
                DestroyImmediate(ghost.gameObject);
        }
        #endregion KeyDown

        #region MouseDown
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
                            GameObject newObject = Instantiate(GetCatalogObject(pos.magnitude, GetAngularGridDensity(pos.magnitude)), pos, GetRotator(pos), rootTransform);
                            Undo.RegisterCreatedObjectUndo(newObject, "Place Segment");
                            break;
                        }
                    case PlacingModes.Arc:
                        break;
                    case PlacingModes.Wall:
                        {
                            if (wallPlacingStarted)
                            {
                                // TODO:
                                //PlaceWall(wallFirstPosition, ScreenToWorldSnap(e.mousePosition));
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
                        // TODO:
                        //PlaceRing(ScreenToWorldSnap(e.mousePosition));
                        DrawGrid(Vector2.zero);
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion MouseDown

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

    float SnapRadial(float r)
    {
        return ((Mathf.Floor(r * radialGridDensity) + 1) / radialGridDensity);
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
        return (int)(12 * Mathf.Pow(2, Mathf.Floor(Mathf.Log(r, 2) + 0.65f)));
    }

    void DrawGrid(Vector2 centre)
    {
        Debug.DrawLine(new Vector2(0, 0), new Vector2(1, 1), Color.cyan);
    }

    // Place a ring from a given point
    void PlaceRing(GameObject go, Vector3 pos)
    {
        float R = pos.magnitude;
        int angularGridDensity = GetAngularGridDensity(R);
        float deltaAngle = 360f / angularGridDensity;
        Quaternion rotator = Quaternion.Euler(0, 0, deltaAngle);

        GameObject ringRoot = new GameObject("Ring " + R);
        ringRoot.transform.parent = rootTransform;
        for (int i = 0; i < angularGridDensity; i++)
        {
            pos = rotator * pos;
            Instantiate(go, pos, GetRotator(pos), ringRoot.transform);
        }
        Undo.RegisterCreatedObjectUndo(ringRoot, "Place Ring " + R);
    }

    // Place a ring from a given point
    void PlaceWall(SortedDictionary<int, GameObject> column, Vector3 from, Vector3 to)
    {
        float R1 = from.magnitude;
        float R2 = to.magnitude;
        float deltaR = (R2 > R1 ? 1f : -1f) / radialGridDensity;
        Debug.Log(Mathf.Abs(R2 - R1) * radialGridDensity);
        int n = Mathf.FloorToInt(Mathf.Abs(R2 - R1) * radialGridDensity + 0.001f) + 1;
        float r = R1;
        Vector3 fromDir = Vector3.Normalize(from);
        Quaternion rotator = GetRotator(from);

        GameObject wallRoot = new GameObject("Wall");
        wallRoot.transform.parent = rootTransform;
        for (int i = 0; i < n; ++i)
        {
            // TODO:
            //Instantiate(block, fromDir * r, rotator, wallRoot.transform);
            r += deltaR;
        }
        Undo.RegisterCreatedObjectUndo(wallRoot, "Place Wall");
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

    GameObject GetCatalogObject(float r, int angularDensity)
    {
        InventoryEntry entry = inventory[selectedItem];
        if (entry.universal)
        {
            return entry[0][0];
        }
        else
        {
            return entry[angularDensity][Mathf.RoundToInt(r)];
        }

        return null;
    }

    void PopulateStock()
    {
        Debug.LogWarning("Populating...");
        if (CatalogLoaded)
        {
            inventory = new List<InventoryEntry>();
            for (int i = 0; i < Catalog.Count; i++)
            {
                InventoryEntry entry = new InventoryEntry();
                TileEditorCatalog.CatalogEntry category = Catalog[i];
                for (int j = 0; j < category.Count; j++)
                {
                    string name = category[j].name;
                    string suffix = name.Substring(category.prefabNamePrefix.Length);
                    string[] parameters = suffix.Split(new char[] { '_' }, System.StringSplitOptions.RemoveEmptyEntries);
                    int radius, angularDensity;
                    if (parameters.Length == 2)
                    {
                        radius = int.Parse(parameters[0]);
                        angularDensity = int.Parse(parameters[1]);
                    }
                    else
                    {
                        entry.universal = true;
                        radius = angularDensity = 0;
                    }
                    SortedDictionary<int, GameObject> column;
                    if (entry.stock.TryGetValue(angularDensity, out column))
                        try
                        {
                            column.Add(radius, category[j]);
                        }
                        catch
                        {
                            Debug.LogWarning("Multiple entries of '" + entry.displayName + "' at {R=" + radius + ", divs=" + angularDensity + "} in '" + Catalog.name + ". Cannot add prefab '" + name + "' to Tile Editor.", Catalog);
                        }
                    else
                    {
                        column = new SortedDictionary<int, GameObject>();
                        column.Add(radius, category[j]);
                        entry.stock.Add(angularDensity, column);
                    }
                    if (entry.universal)
                    {
                        if (category.Count > 1)
                            Debug.LogWarning("Error parsing entry '" + name + "' of '" + entry.displayName + "' in TileEditorCatalog '" + Catalog.name + "'.", Catalog);
                        break;
                    }
                }
                inventory.Add(entry);
            }
        }
    }
}
