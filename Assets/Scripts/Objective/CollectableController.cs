using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CollectableController : GLMonoBehaviour
{

    public int orderValue;
    public string label;
    private Text labelText;

    void Start()
    {
        labelText = GetComponentInChildren<Text>();
		labelText.text = label;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(TagsAndLayers.PLAYER))
        {
            Destroy(this.gameObject);
			ObjectiveCollector.Instance.AddCollectedItem(this);
        }
    }
}
