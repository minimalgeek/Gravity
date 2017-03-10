using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

public class LevelEditor : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Serializable]
    private class ArchiveData
    {
        public List<ArchiveEntry> objects = new List<ArchiveEntry>();
    }

    [Serializable]
    private class ArchiveEntry
    {
        public string path;
        public float x, y;
        public float phi;
        public ArchiveEntry(string path, float x, float y, float phi)
        {
            this.path = path;
            this.x = x;
            this.y = y;
            this.phi = phi;
        }
    }

    //private ArrayList objects;
    private ArrayList ghosts;
    private GameObject root;
    //private int layer;

    public bool radialSnap = true;
    public float radialGridDensity = 4;
    public bool angularSnap = true;
    public float angularGridDensity = 90;

    private float ringRadius;
    private float dragStartRadius;
    private float dragStartAngle;

    private enum PlacingState
    {
        None, Single, Arc, Wall, Ring
    }
    private PlacingState placingState = PlacingState.None;

    void Start()
    {
        //objects = new ArrayList();
        ghosts = new ArrayList();
        root = new GameObject("Level Root");
        //layer = gameObject.layer;
    }

    void SpawnGhost()
    {
        SpawnGhost(GetPos());
    }

    void SpawnGhost(Vector3 pos)
    {
        GameObject newGhost = (GameObject)Instantiate(Resources.Load("Editor/Block", typeof(GameObject)), pos, FaceAxis.GetRotator(pos));
        newGhost.AddComponent<LevelEditorObject>().PrefabName = "Editor/Block";
        foreach (Collider2D coll in newGhost.GetComponents<Collider2D>())
        {
            coll.enabled = false;
        }
        ghosts.Add(newGhost);
    }

    void SpawnRing()
    {
        Vector3 pos = GetPos();
        float deltaAngle = 360 / angularGridDensity;
        Quaternion rotator = Quaternion.Euler(0, 0, deltaAngle);
        for (int i = 0; i < angularGridDensity; i++)
        {
            pos = rotator * pos;
            GameObject newGhost = (GameObject)Instantiate(Resources.Load("Editor/Block", typeof(GameObject)), pos, FaceAxis.GetRotator(pos));
            LevelEditorObject leo = newGhost.AddComponent<LevelEditorObject>();
            leo.PrefabName = "Editor/Block";
            leo.IsPlaced = false;
            foreach (Collider2D coll in newGhost.GetComponents<Collider2D>())
            {
                coll.enabled = false;
            }
            ghosts.Add(newGhost);
            //newObject.transform.parent = root.transform;
            //newObject.layer = layer;
            //objects.Add(newObject);
        }
    }

    void PlaceBlocks()
    {
        foreach (GameObject ghost in ghosts)
        {
            foreach (Collider2D coll in ghost.GetComponents<Collider2D>())
            {
                coll.enabled = true;
            }
            ghost.GetComponent<LevelEditorObject>().IsPlaced = true;
            ghost.transform.parent = root.transform;
            //ghost.layer = layer;
            //objects.Add(ghost);
        }
        ghosts.Clear();
    }

    void PlaceRing()
    {
        Vector3 pos = GetPos();
        float deltaAngle = 360 / angularGridDensity;
        Quaternion rotator = Quaternion.Euler(0, 0, deltaAngle);
        for (int i = 0; i < angularGridDensity; i++)
        {
            pos = rotator * pos;
            GameObject newObject = (GameObject)Instantiate(Resources.Load("Editor/Block", typeof(GameObject)), pos, FaceAxis.GetRotator(pos));
            LevelEditorObject leo = newObject.AddComponent<LevelEditorObject>();
            leo.PrefabName = "Editor/Block";
            leo.IsPlaced = true;
            newObject.transform.parent = root.transform;
            //newObject.layer = layer;
            //objects.Add(newObject);
        }
    }

    Vector3 GetPos()
    {
        Vector3 pos;
        if (radialSnap)
            pos = SnapRadial(MouseToWorld());
        else
            pos = MouseToWorld();

        if (angularSnap)
            return SnapAngular(pos);
        else
            return pos;
    }

    static Vector3 MouseToWorld()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        return pos;
    }

    Vector3 SnapRadial(Vector3 pos)
    {
        float r = pos.magnitude;
        return pos * ((Mathf.Floor(r * radialGridDensity) + 1) / (radialGridDensity * r));
    }

    Vector3 SnapAngular(Vector3 pos)
    {
        float phi = (Mathf.Floor(angularGridDensity * Mathf.Atan2(pos.y, pos.x) / Mathf.PI * 0.5f) + 0.5f) * Mathf.PI * 2 / angularGridDensity;
        return new Vector3(Mathf.Cos(phi), Mathf.Sin(phi), 0) * pos.magnitude;
    }


    void Update()
    {
        switch (placingState)
        {
            case PlacingState.None:
                break;
            case PlacingState.Single:
                ((GameObject)ghosts[0]).transform.position = GetPos();
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    Debug.Log("ringing...");
                    placingState = PlacingState.Ring;
                }
                break;
            case PlacingState.Arc:
                break;
            case PlacingState.Wall:
                break;
            case PlacingState.Ring:
                ((GameObject)ghosts[0]).transform.position = GetPos();
                if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    placingState = PlacingState.Single;
                }
                break;
            default:
                break;
        }

        if (Input.GetKeyDown(KeyCode.P))
            Save();
        if (Input.GetKeyDown(KeyCode.O))
            Load();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.LogWarning("Del" + Time.frameCount);
        //Debug.Log("OnPointerClick");
        switch (placingState)
        {
            case PlacingState.None:
                switch (eventData.button)
                {
                    case PointerEventData.InputButton.Left:
                        SpawnGhost();
                        placingState = PlacingState.Single;
                        break;
                    case PointerEventData.InputButton.Right:
                        break;
                    case PointerEventData.InputButton.Middle:
                        break;
                    default:
                        break;
                }
                break;
            case PlacingState.Single:
                switch (eventData.button)
                {
                    case PointerEventData.InputButton.Left:
                        PlaceBlocks();
                        SpawnGhost();
                        break;
                    case PointerEventData.InputButton.Right:
                        Destroy((GameObject)ghosts[0]);
                        ghosts.RemoveAt(0);
                        placingState = PlacingState.None;
                        break;
                    case PointerEventData.InputButton.Middle:
                        break;
                    default:
                        break;
                }
                break;
            case PlacingState.Arc:
                break;
            case PlacingState.Wall:
                break;
            case PlacingState.Ring:
                switch (eventData.button)
                {
                    case PointerEventData.InputButton.Left:
                        PlaceRing();
                        break;
                    case PointerEventData.InputButton.Right:
                        Destroy((GameObject)ghosts[0]);
                        ghosts.RemoveAt(0);
                        placingState = PlacingState.None;
                        break;
                    case PointerEventData.InputButton.Middle:
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("OnPointerDown");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("OnPointerUp");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
    }

    void Save()
    {
        string path = UnityEditor.EditorUtility.SaveFilePanel(
                    "Save current level",
                    "Saves",
                    "save_" + DateTime.UtcNow.ToString("yyyy-MM-dd_HH.mm.ss"),
                    "sav");
        if (path.Length != 0)
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                ArchiveData data = new ArchiveData();
                foreach (Transform child in root.transform)
                {
                    data.objects.Add(new ArchiveEntry(child.gameObject.GetComponent<LevelEditorObject>().PrefabName, child.localPosition.x, child.localPosition.y, child.localRotation.z));
                }
                sw.Write(JsonUtility.ToJson(data));
            }
        }
    }

    void Load()
    {
        string path = UnityEditor.EditorUtility.OpenFilePanel(
                    "Open saved level",
                    "Saves",
                    "sav");
        if (path.Length != 0)
        {
            foreach (Transform child in root.transform)
            {
                Destroy(child.gameObject);
            }
            ArchiveData data = JsonUtility.FromJson<ArchiveData>(File.ReadAllText(path));
            foreach (ArchiveEntry item in data.objects)
            {
                GameObject newObject = (GameObject)Instantiate(Resources.Load(item.path, typeof(GameObject)), new Vector2(item.x, item.y), Quaternion.Euler(0, 0, item.phi));
                LevelEditorObject leo = newObject.AddComponent<LevelEditorObject>();
                leo.PrefabName = item.path;
                leo.IsPlaced = true;
                newObject.transform.parent = root.transform;
                //newObject.layer = layer;
            }
        }
    }
}
