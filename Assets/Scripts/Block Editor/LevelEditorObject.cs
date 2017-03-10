using UnityEngine;
using UnityEngine.EventSystems;

public class LevelEditorObject : MonoBehaviour//, IPointerClickHandler
{
    public string PrefabName { get; set; }

    private bool isPlaced = true;
    public bool IsPlaced
    {
        get { return isPlaced; }
        set { isPlaced = value; }
    }

    public void OnMouseOver()
    {
        if (IsPlaced)
        {
            if (Input.GetMouseButtonDown(1))
                Destroy(gameObject);
        }
    }
}
