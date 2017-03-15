using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileEditorCatalog.asset", menuName = "Tile Editor/Tile Editor Catalog")]
public class TileEditorCatalog : ScriptableObject
{
    [System.Serializable]
    public class CatalogEntry
    {
        [Tooltip("The name displayed in the Tile Editor selection list")]
        public string displayName;
        [Tooltip("The icon used in the Tile Editor selection list")]
        public Texture2D displayIcon;
        [Tooltip("The prefab to be placed")]
        public GameObject prefab;

        public Texture2D GetIcon()
        {
            Debug.Log(displayName + "  " + displayIcon);
            if (displayIcon == null)
            {
                Texture2D preview = UnityEditor.AssetPreview.GetAssetPreview(prefab);
                if (preview == null)
                    return UnityEditor.AssetPreview.GetMiniThumbnail(prefab);
                else
                    return preview;
            }
            else
                return displayIcon;
            }
        }

        //[Header("Button Settings")]
        [SerializeField]
        [Tooltip("Objects that can be placed in the scene using the Tile Editor window")]
        private List<CatalogEntry> placableObjects;

        public int Count
        {
            get { return placableObjects.Count; }
        }

        public CatalogEntry this[int key]
        {
            get { return placableObjects[key]; }
        }
    }
