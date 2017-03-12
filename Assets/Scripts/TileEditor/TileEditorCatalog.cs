using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileEditorCatalog.asset", menuName = "Tile Editor/Tile Editor Catalog")]
public class TileEditorCatalog : ScriptableObject
{
    [System.Serializable]
    public class CatalogEntry
    {
        [Tooltip("The name displayed in the Tile Editor window")]
        public string displayName;
        [Tooltip("Index of the prefab whose icon is used in the Tile Editor selection list")]
        [Delayed]
        public int displayIconIndex;
        [Tooltip("TODO: make this tooltip helpful")]
        public string prefabNamePrefix;
        [Tooltip("TODO: make this tooltip helpful")]
        public List<GameObject> subobjects;

        public int Count
        {
            get { return subobjects.Count; }
        }

        public GameObject this[int key]
        {
            get { return subobjects[key]; }
        }

        public Texture2D GetIcon()
        {
            return UnityEditor.AssetPreview.GetAssetPreview(subobjects[displayIconIndex]);
        }
    }

    //[Header("Button Settings")]
    [Tooltip("Objects that can be placed in the scene using the Tile Editor window")]
    public List<CatalogEntry> placableObjects;

    public int Count
    {
        get { return placableObjects.Count; }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        for (int i = 0; i < Count; i++)
        {
            // TODO: hibakezelést jobbá tenni. Megpróbálhatja javítani a hibát. Pl. az index korlátozása, ha nagyobb.
            CatalogEntry entry = placableObjects[i];
            if (entry.subobjects.Count == 0)
                Debug.LogWarning("No prefab was given for '" + entry.displayName + "'.", this);
            else {
                if (entry.displayIconIndex >= entry.subobjects.Count)
                    Debug.LogError("Display icon index out of range at '" + entry.displayName + "'.", this);
                if (entry.prefabNamePrefix.Length == 0 && entry.subobjects.Count > 0)
                    (placableObjects[i]).prefabNamePrefix = entry.subobjects[0].name;
            }
        }
    }
#endif

    public CatalogEntry this[int key]
    {
        get { return placableObjects[key]; }
    }
}
