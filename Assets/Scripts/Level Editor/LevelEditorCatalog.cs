using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelEditorCatalog.asset", menuName = "Level Editor/Level Editor Catalog")]
public class LevelEditorCatalog : ScriptableObject
{
    [System.Serializable]
    public class CatalogEntry
    {
        [Tooltip("The name displayed in the Level Editor selection list")]
        public string displayName;
        [Tooltip("The icon used in the Level Editor selection list")]
        public Texture2D displayIcon;
        [Tooltip("The prefab to be placed")]
        public GameObject prefab;

        public Texture2D GetIcon()
        {
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

    [SerializeField]
    [Tooltip("The player prefab to be placed by the Level Editor.")]
    private GameObject player;

    [SerializeField]
    [Tooltip("The camera prefab to be placed by the Level Editor")]
    private GameObject camera;

    [SerializeField]
    [Tooltip("The force field prefab to be placed by the Level Editor")]
    private GameObject forceField;

    [SerializeField]
    [Tooltip("Spotlights. For the fun of it.")]
    private GameObject lights;

    [SerializeField]
    [Tooltip("Objects that can be placed in the scene using the Level Editor window")]
    private List<CatalogEntry> placableObjects;

    public int Count
    {
        get { return placableObjects.Count; }
    }

    public CatalogEntry this[int key]
    {
        get { return placableObjects[key]; }
    }

    public GameObject Player
    {
        get { return player; }
    }

    public GameObject Camera
    {
        get { return camera; }
    }
    public GameObject ForceField
    {
        get { return forceField; }
    }
    public GameObject Lights
    {
        get { return lights; }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (camera != null && camera.GetComponent<Camera>() == null)
            camera = null;
    }
#endif
}
