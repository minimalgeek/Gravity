using UnityEngine;
using UnityEditor;
using System.Collections;
using Gamelogic.Extensions;
using System.Collections.Generic;

public class LevelEditorGUI : EditorWindow
{
    #region Fields

    // The Level Root object, into which all other objects are placed
    GameObject root;
    Transform rootTransform;
    //GameObject grid;

    LevelEditorCatalog _catalog;
    public LevelEditorCatalog Catalog
    {
        get { return _catalog; }
        set
        {
            _catalogLoaded = _catalog != null;
            if (_catalog == null || _catalog != value)
            {
                _catalog = value;
                _catalogLoaded = value != null;
                LoadCatalog();
            }
        }
    }
    bool _catalogLoaded = false;
    public bool CatalogLoaded
    {
        get { return _catalogLoaded; }
    }
    int catalogIconSize = 100;
    private class InventoryEntry : LevelEditorCatalog.CatalogEntry
    {
        public bool isArc = false;
    }
    List<InventoryEntry> inventory;

    Vector2 windowScrollPosition;

    Vector2 itemSelectorScrollPosition;
    int selectedItem;

    //GameObject block;
    GameObject ghost;

    bool levelObjectsFoldoutState = false;

    private GameObject player;
    private Camera camera;
    private GameObject forceField;
    private GameObject lights;


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
    float wallFirstRadius;
    LineRenderer wallRay;

    bool arcPlacingStarted = false;
    Vector3 arcFirstPosition;
    float arcRadius;

    bool autoRotateEnabled = true;
    bool radialSnapEnabled = true;
    bool angularSnapEnabled = true;
    int selectedAngularDensity = 0;
    int customAngularDensity = 192;

    private float radialThickness = 0.25f;
    int selectedRadialThickness = 0;
    private int radialGridDensity = 2;

    #endregion Fields





    #region Window related methods

    // Add menu item named "Level Editor" to the Tools menu
    [MenuItem("Tools/Level Editor")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        GetWindow(typeof(LevelEditorGUI));
    }

    void OnEnable()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
        titleContent.text = "Level Editor";

        CreateLevelRoot();
        //CreateGrid();

        CheckCatalog();
        LookForBasicObjects();
    }

    void OnDisable()
    {
        RemoveGhosts();
    }

    void OnDestroy()
    {
        RemoveGhosts();
    }

    // Called when updating this editor window
    void OnGUI()
    {
        windowScrollPosition = EditorGUILayout.BeginScrollView(windowScrollPosition);

        Catalog = (LevelEditorCatalog)EditorGUILayout.ObjectField("Object Catalog", Catalog, typeof(LevelEditorCatalog), false);

        if (!CatalogLoaded)
            EditorGUILayout.HelpBox("Load a catalog (TileEditorCatalog) to access all functionality.\nTo create a catalog, use the Create menu in the Assets folder and look for Level Editor/Level Editor Catalog.", MessageType.Warning);

        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label("Object Placement", EditorStyles.boldLabel, GUILayout.Width(position.width - 65));
            if (GUILayout.Button("Reload"))
                CheckCatalog();
        }

        using (new EditorGUI.DisabledGroupScope(!CatalogLoaded))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                float buttonWidth = (position.width - 3f) * 0.2f - 4f;
                if (GUILayout.Toggle(PlacingMode == PlacingModes.Off, placingModes[0], "button", GUILayout.Width(buttonWidth)))
                {
                    PlacingMode = PlacingModes.Off;
                    wallPlacingStarted = false;
                }
                if (GUILayout.Toggle(PlacingMode == PlacingModes.Single, placingModes[1], "button", GUILayout.Width(buttonWidth)))
                {
                    PlacingMode = PlacingModes.Single;
                    wallPlacingStarted = false;
                }
                using (new EditorGUI.DisabledGroupScope(!CatalogLoaded || !inventory[selectedItem].isArc))
                {
                    if (GUILayout.Toggle(PlacingMode == PlacingModes.Arc, placingModes[2], "button", GUILayout.Width(buttonWidth)))
                    {
                        PlacingMode = PlacingModes.Arc;
                        wallPlacingStarted = false;
                    }
                    if (GUILayout.Toggle(PlacingMode == PlacingModes.Wall, placingModes[3], "button", GUILayout.Width(buttonWidth)))
                        PlacingMode = PlacingModes.Wall;
                    if (GUILayout.Toggle(PlacingMode == PlacingModes.Ring, placingModes[4], "button", GUILayout.Width(buttonWidth)))
                    {
                        PlacingMode = PlacingModes.Ring;
                        wallPlacingStarted = false;
                    }
                }
            }
            //placingMode = GUILayout.SelectionGrid(placingMode, placingModes, placingModes.Length, "button");
            if (PlacingMode != PlacingModes.Off) Tools.current = Tool.None;
            else if (Tools.current == Tool.None) Tools.current = Tool.Move;
        }

        #region Rotation controls
        EditorGUILayout.BeginHorizontal();
        using (new EditorGUI.DisabledGroupScope(!CatalogLoaded))
        {
            autoRotateEnabled = EditorGUILayout.Toggle("Auto rotate", autoRotateEnabled);
        }
        using (new EditorGUI.DisabledGroupScope(true))
            GUILayout.Button(new GUIContent("↺", "Rotate 90° CCW"), GUILayout.Width(20));
        if (GUILayout.Button("Rotate Selection", GUILayout.ExpandWidth(false)))
            RotateSelection();
        using (new EditorGUI.DisabledGroupScope(true))
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

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Radial Thickness", GUILayout.Width(146));
                using (new EditorGUILayout.HorizontalScope())
                {
                    selectedRadialThickness = EditorGUILayout.Popup(selectedRadialThickness, new string[] { "0.25 m", "50 m", "0.75 m", "1 m", "2 m", "Custom..." });
                    switch (selectedRadialThickness)
                    {
                        case 0:
                            radialThickness = 0.25f;
                            break;
                        case 1:
                            radialThickness = 0.5f;
                            break;
                        case 2:
                            radialThickness = 0.75f;
                            break;
                        case 3:
                            radialThickness = 1f;
                            break;
                        case 4:
                            radialThickness = 2f;
                            break;
                        case 5:
                            radialThickness = EditorGUILayout.Slider(radialThickness, 0.1f, 10);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        #endregion Radial and angular snapping controls


        EditorGUILayout.Space();


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
                    while (n < inventory.Count)
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
                                    {
                                        if (selectedItem != n)
                                        {
                                            if (ghost != null)
                                                DestroyImmediate(ghost.gameObject);
                                            selectedItem = n;
                                            if (!inventory[selectedItem].isArc && PlacingMode != PlacingModes.Off)
                                                PlacingMode = PlacingModes.Single;
                                        }
                                    }
                                    GUI.backgroundColor = Color.gray;

                                    // and some text over it
                                    GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
                                    style.normal.textColor = n == selectedItem ? highlight : Color.white;
                                    style.wordWrap = true;
                                    GUILayout.Label(displayName, style);
                                }
                                w += catalogIconSize;
                                n++;
                            } while (n < inventory.Count && w < (position.width - catalogIconSize));
                        }
                    }

                    itemSelectorScrollPosition = scrollView.scrollPosition;
                }
                GUI.backgroundColor = defColor;
            }
        }
        #endregion Catalog

        GUILayout.Label("Level Parameters", EditorStyles.boldLabel);

        using (new EditorGUI.DisabledGroupScope(!CatalogLoaded))
        {
            LookForBasicObjects();

            using (new EditorGUILayout.HorizontalScope())
            {
                levelObjectsFoldoutState = EditorGUILayout.Foldout(levelObjectsFoldoutState, "Basic objects", true);
                if (GUILayout.Button("Prepare Level", GUILayout.Width(100)))
                    PrepareLevel();
            }
            if (levelObjectsFoldoutState)
            {
                player = (GameObject)EditorGUILayout.ObjectField("Player", player, typeof(GameObject), true);
                if (player != null)
                    if (PrefabUtility.GetPrefabType(player) == PrefabType.Prefab)
                        player = null;
                camera = (Camera)EditorGUILayout.ObjectField("Camera", camera, typeof(Camera), true);
                if (camera != null)
                {
                    if (PrefabUtility.GetPrefabType(camera) == PrefabType.Prefab)
                        camera = null;
                }
                forceField = (GameObject)EditorGUILayout.ObjectField("Force Field", forceField, typeof(GameObject), true);
                if (forceField != null)
                    if (PrefabUtility.GetPrefabType(forceField) == PrefabType.Prefab)
                        forceField = null;
                lights = (GameObject)EditorGUILayout.ObjectField("Spotlights", lights, typeof(GameObject), true);
                if (lights != null)
                    if (PrefabUtility.GetPrefabType(lights) == PrefabType.Prefab)
                        lights = null;
            }
        }

        EditorGUILayout.EndScrollView();
    }

    #endregion Window related methods



    #region Scene update

    // Called when updating the Scene view
    void OnSceneGUI(SceneView sceneview)
    {
        if (root == null)
            CreateLevelRoot();

        Event e = Event.current;

        #region KeyDown
        //if (e.type == EventType.KeyDown)
        //{
        //    switch (e.keyCode)
        //    {
        //        case KeyCode.Alpha0:
        //            PlacingMode = PlacingModes.Off;
        //            break;
        //        case KeyCode.Alpha1:
        //            PlacingMode = PlacingModes.Single;
        //            break;
        //        case KeyCode.Alpha2:
        //            PlacingMode = PlacingModes.Arc;
        //            break;
        //        case KeyCode.Alpha3:
        //            PlacingMode = PlacingModes.Wall;
        //            break;
        //        case KeyCode.Alpha4:
        //            PlacingMode = PlacingModes.Ring;
        //            break;
        //        default:
        //            break;
        //    }
        //}
        #endregion KeyDown


        #region MouseMove
        if (e.type == EventType.MouseMove)
        {
            switch (PlacingMode)
            {
                case PlacingModes.Off:
                    if (ghost != null)
                        RemoveGhosts();
                    break;
                case PlacingModes.Single:
                    // Place or move ghost
                    if (ghost == null)
                    {
                        RemoveGhosts();
                        PlaceSingleGhost(ScreenToWorldSnap(e.mousePosition));
                    }
                    else
                        UpdateSingleGhost(ScreenToWorldSnap(e.mousePosition));
                    break;
                case PlacingModes.Arc:
                    if (arcPlacingStarted)
                        if (ghost == null)
                            arcPlacingStarted = false;
                        else
                            UpdateArcGhost(ScreenToWorldSnap(e.mousePosition));
                    else
                    {
                        if (ghost == null)
                        {
                            RemoveGhosts();
                            PlaceSingleGhost(ScreenToWorldSnap(e.mousePosition));
                        }
                        else
                            UpdateSingleGhost(ScreenToWorldSnap(e.mousePosition));
                    }
                    break;
                case PlacingModes.Wall:
                    if (wallPlacingStarted)
                        if (ghost == null)
                            wallPlacingStarted = false;
                        else
                            UpdateWallGhost(ScreenToWorldSnap(e.mousePosition));
                    else {
                        if (ghost == null)
                        {
                            RemoveGhosts();
                            PlaceSingleGhost(ScreenToWorldSnap(e.mousePosition));
                        }
                        else
                            UpdateSingleGhost(ScreenToWorldSnap(e.mousePosition));
                    }
                    break;
                case PlacingModes.Ring:
                    if (ghost == null)
                    {
                        RemoveGhosts();
                        PlaceSingleGhost(ScreenToWorldSnap(e.mousePosition));
                    }
                    else
                        UpdateSingleGhost(ScreenToWorldSnap(e.mousePosition));
                    break;
                default:
                    break;
            }
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
                            InventoryEntry entry = inventory[selectedItem];
                            Vector3 pos = ScreenToWorldSnap(e.mousePosition);
                            GameObject newObject = Instantiate(entry.prefab, pos, GetRotator(pos), rootTransform);
                            if (entry.isArc)
                            {
                                float rad = pos.magnitude;
                                try
                                {
                                    newObject.GetComponent<ArcMesh>().SetParams(rad - radialThickness, rad, 1, GetAngularGridDensity(rad), 768, 0);
                                }
                                catch { }
                                Undo.RegisterCreatedObjectUndo(newObject, "Place Segment");
                            }
                            else
                            {
                                Undo.RegisterCreatedObjectUndo(newObject, "Place " + entry.displayName);
                            }
                            break;
                        }
                    case PlacingModes.Arc:
                        {
                            if (arcPlacingStarted)
                            {
                                arcPlacingStarted = false;
                                DestroyImmediate(ghost.GetComponent<GhostPlaceholder>());
                                ghost.hideFlags &= ~HideFlags.HideInHierarchy;
                                ghost.transform.parent = rootTransform;
                                Undo.RegisterCreatedObjectUndo(ghost, "Place Arc");
                                ghost = null;
                            }
                            else
                            {
                                DestroyImmediate(ghost);
                                RemoveGhosts();

                                arcFirstPosition = ScreenToWorldSnap(e.mousePosition);
                                arcPlacingStarted = true;
                                PlaceArcGhost();
                            }
                            break;
                        }
                    case PlacingModes.Wall:
                        {
                            if (wallPlacingStarted)
                            {
                                // TODO:
                                //PlaceWall(wallFirstPosition, ScreenToWorldSnap(e.mousePosition));
                                wallPlacingStarted = false;
                                DestroyImmediate(ghost.GetComponent<GhostPlaceholder>());
                                ghost.hideFlags &= ~HideFlags.HideInHierarchy;
                                ghost.transform.parent = rootTransform;
                                Undo.RegisterCreatedObjectUndo(ghost, "Place Wall");
                                ghost = null;
                            }
                            else
                            {
                                DestroyImmediate(ghost);
                                RemoveGhosts();

                                wallFirstPosition = ScreenToWorldSnap(e.mousePosition);
                                wallFirstRadius = wallFirstPosition.magnitude;
                                wallPlacingStarted = true;
                                PlaceWallGhost();
                            }
                            break;
                        }
                    case PlacingModes.Ring:
                        PlaceRing(ScreenToWorldSnap(e.mousePosition));
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion MouseDown
    }

    #endregion Scene update



    #region Object administration

    void CheckCatalog()
    {
        if (Catalog == null)
        {
            LevelEditorCatalog[] catalogs = Resources.LoadAll<LevelEditorCatalog>("");
            if (catalogs.Length > 0) Catalog = catalogs[0];
        }
        else
        {
            if (inventory == null)
                LoadCatalog();
        }
    }

    void LoadCatalog()
    {
        if (CatalogLoaded)
        {
            inventory = new List<InventoryEntry>();
            for (int i = 0; i < Catalog.Count; i++)
            {
                InventoryEntry entry = new InventoryEntry();
                entry.displayName = Catalog[i].displayName;
                entry.displayIcon = Catalog[i].displayIcon;
                entry.prefab = Catalog[i].prefab;
                entry.isArc = entry.prefab.GetComponent<ArcMesh>() != null;
                inventory.Add(entry);
            }
        }
    }

    void LookForBasicObjects()
    {
        if (player == null) try { player = GameObject.FindGameObjectWithTag("Player"); } catch { }
        if (camera == null) try { camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>(); } catch { }
        if (forceField == null) try { forceField = GameObject.FindGameObjectWithTag("MainForceField"); } catch { }
    }

    void PrepareLevel()
    {
        try
        {
            if (player == null)
                player = Instantiate(Catalog.Player, Vector2.down * 5f, Quaternion.identity);
        }
        catch (System.Exception e) { Debug.LogException(e); }
        try
        {
            DestroyImmediate(Camera.main);
            if (camera == null)
                camera = Instantiate(Catalog.Camera).GetComponent<Camera>();
        }
        catch (System.Exception e) { Debug.LogException(e); }
        try
        {
            if (forceField == null)
                forceField = Instantiate(Catalog.ForceField);
        }
        catch (System.Exception e) { Debug.LogException(e); }
        try
        {
            if (lights == null)
                lights = Instantiate(Catalog.Lights);
        }
        catch (System.Exception e) { Debug.LogException(e); }
    }

    void CreateLevelRoot()
    {
        root = GameObject.Find("Level Root");
        if (root == null)
            root = new GameObject("Level Root");
        rootTransform = root.transform;
    }

    //void CreateGrid()
    //{
    //    grid = GameObject.Find("Editor Grid");
    //    if (grid == null)
    //    {
    //        grid = new GameObject("Editor Grid");
    //        wallRay = grid.AddComponent<LineRenderer>();
    //    }
    //    else
    //    {
    //        wallRay = grid.GetComponent<LineRenderer>();
    //    }
    //}

    void DrawGrid(Vector2 centre)
    {
        Debug.DrawLine(new Vector2(0, 0), new Vector2(1, 1), Color.cyan);
    }

    #endregion Object administration



    #region Geometry

    void RotateSelection()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            if (obj.transform.IsChildOf(rootTransform))
                obj.transform.rotation = GetRotator(obj.transform.position, true);
        }
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
        return ((Mathf.Floor(r / radialThickness) + 1) * radialThickness);
    }

    // Applies radial snapping to a vector
    Vector3 SnapRadial(Vector3 pos)
    {
        float r = pos.magnitude;
        return pos * ((Mathf.Floor(r / radialThickness) + 1) * radialThickness / r);
    }

    // Applies angular snapping to a vector
    Vector3 SnapAngular(Vector3 pos)
    {
        int angularGridDensity = GetAngularGridDensity(pos.magnitude);
        float phi = (Mathf.Floor(angularGridDensity * Mathf.Atan2(pos.y, pos.x) / Mathf.PI * 0.5f) + 0.5f) * Mathf.PI * 2 / angularGridDensity;
        return new Vector3(Mathf.Cos(phi), Mathf.Sin(phi), 0) * pos.magnitude;
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

    int GetAngularGridDensity(float r, bool forceAuto = false)
    {
        if (selectedAngularDensity == 0)
            return (int)(12 * Mathf.Pow(2, Mathf.Floor(Mathf.Log(r, 2) + 0.65f)));
        else if (selectedAngularDensity == -1)
            return customAngularDensity;
        else
            return selectedAngularDensity;
    }

    #endregion Geometry



    #region Ghosting and construction

    void RemoveGhosts()
    {
        // Remove stuck ghosts from a previous session
        GhostPlaceholder[] formerGhosts = FindObjectsOfType<GhostPlaceholder>();
        foreach (GhostPlaceholder formerGhost in formerGhosts)
        {
            DestroyImmediate(formerGhost.gameObject);
        }
    }

    // Place new ghost
    void PlaceSingleGhost(Vector3 pos)
    {
        ghost = Instantiate(inventory[selectedItem].prefab, pos, GetRotator(pos));
        if (inventory[selectedItem].isArc)
        {
            float rad = pos.magnitude;
            ghost.GetComponent<ArcMesh>().SetParams(rad - radialThickness, rad, 1, GetAngularGridDensity(rad), 768, 0);
        }
        ghost.hideFlags |= HideFlags.HideInHierarchy;
        ghost.AddComponent<GhostPlaceholder>();
    }

    // Update ghost attitude
    void UpdateSingleGhost(Vector3 pos)
    {
        ghost.transform.position = pos;
        ghost.transform.rotation = GetRotator(ghost.transform.position);
        if (inventory[selectedItem].isArc)
        {
            float rad = pos.magnitude;
            ghost.GetComponent<ArcMesh>().SetParams(rad - radialThickness, rad, 1, GetAngularGridDensity(rad), 768, 0);
        }
    }

    void PlaceWallGhost()
    {
        if (inventory[selectedItem].isArc)
        {
            ghost = Instantiate(inventory[selectedItem].prefab, wallFirstPosition, GetRotator(wallFirstPosition));
            ghost.hideFlags |= HideFlags.HideInHierarchy;
            float rad = wallFirstPosition.magnitude;
            ghost.GetComponent<ArcMesh>().SetParams(rad - radialThickness, rad, 1, GetAngularGridDensity(rad), 768, 0);
            ghost.AddComponent<GhostPlaceholder>();
        }
    }

    void UpdateWallGhost(Vector3 pos)
    {
        if (inventory[selectedItem].isArc)
        {
            float rad2 = pos.magnitude;
            if (rad2 < wallFirstRadius)
            {
                ghost.transform.position = wallFirstPosition;
                ghost.GetComponent<ArcMesh>().SetParams(rad2 - radialThickness, wallFirstRadius, 1, GetAngularGridDensity(wallFirstRadius), 768, 0);
            }
            else if (rad2 > wallFirstRadius)
            {
                ghost.transform.position = Vector3.Normalize(wallFirstPosition) * rad2;
                ghost.GetComponent<ArcMesh>().SetParams(wallFirstRadius - radialThickness, rad2, 1, GetAngularGridDensity(wallFirstRadius), 768, 0);
            }
        }
    }

    //// Place a wall from a given point
    //void PlaceWall(SortedDictionary<int, GameObject> column, Vector3 from, Vector3 to)
    //{
    //    float R1 = from.magnitude;
    //    float R2 = to.magnitude;
    //    float deltaR = (R2 > R1 ? 1f : -1f) * radialThickness;
    //    Debug.Log(Mathf.Abs(R2 - R1) / radialThickness);
    //    int n = Mathf.FloorToInt(Mathf.Abs(R2 - R1) / radialThickness + 0.001f) + 1;
    //    float r = R1;
    //    Vector3 fromDir = Vector3.Normalize(from);
    //    Quaternion rotator = GetRotator(from);

    //    GameObject wallRoot = new GameObject("Wall");
    //    wallRoot.transform.parent = rootTransform;
    //    for (int i = 0; i < n; ++i)
    //    {
    //        Instantiate(inventory[selectedItem].prefab, fromDir * r, rotator, wallRoot.transform);
    //        r += deltaR;
    //    }
    //    Undo.RegisterCreatedObjectUndo(wallRoot, "Place Wall");
    //}

    void PlaceArcGhost()
    {
        if (inventory[selectedItem].isArc)
        {
            ghost = Instantiate(inventory[selectedItem].prefab, arcFirstPosition, GetRotator(arcFirstPosition));
            ghost.hideFlags |= HideFlags.HideInHierarchy;
            arcRadius = arcFirstPosition.magnitude;
            ghost.GetComponent<ArcMesh>().SetParams(arcRadius - radialThickness, arcRadius, 1, GetAngularGridDensity(arcRadius), 768, 0);
            ghost.AddComponent<GhostPlaceholder>();
        }
    }

    void UpdateArcGhost(Vector3 pos)
    {
        if (inventory[selectedItem].isArc)
        {
            int angularGridDensity = GetAngularGridDensity(arcRadius);
            int nominator = Mathf.RoundToInt(Vector2.Angle(arcFirstPosition, pos) * angularGridDensity / 360f) + 1;
            ghost.transform.position = Vector3.Normalize(arcFirstPosition / arcRadius + Vector3.Normalize(pos)) * arcRadius;
            ghost.transform.rotation = GetRotator(ghost.transform.position);
            ghost.GetComponent<ArcMesh>().SetParams(arcRadius - radialThickness, arcRadius, nominator, angularGridDensity, 768, 0);
        }
    }

    // Place a ring from a given point
    void PlaceRing(Vector3 pos)
    {
        float R = Mathf.Round(pos.magnitude / radialThickness) * radialThickness;
        int angularGridDensity = GetAngularGridDensity(R);
        float deltaAngle = 360f / angularGridDensity;
        Quaternion rotator = Quaternion.Euler(0, 0, deltaAngle);

        GameObject ringRoot = new GameObject("Ring " + R);
        GameObject go = inventory[selectedItem].prefab;
        bool arc = inventory[selectedItem].isArc;
        ringRoot.transform.parent = rootTransform;
        for (int i = 0; i < angularGridDensity; i++)
        {
            pos = rotator * pos;
            GameObject newObject = Instantiate(go, pos, GetRotator(pos), ringRoot.transform);
            if (arc)
                newObject.GetComponent<ArcMesh>().SetParams(R - radialThickness, R, 1, angularGridDensity, 768, 0f);
        }
        Undo.RegisterCreatedObjectUndo(ringRoot, "Place Ring " + R);
    }

    #endregion Ghosting and construction
}
