using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StasisChamber : MonoBehaviour
{

    private bool animalLocked = false;
    public string animalTypeTag;

    void Start()
    {

    }

    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!animalLocked)
        {
            if (other.gameObject.CompareTag(TagsAndLayers.PLAYER))
            {
                Transform itemHoldingPoint = CombinedController.Instance.ItemHoldingTransform;
                bool hasChild = itemHoldingPoint.childCount > 0;
                if (hasChild) {
                    ItemPickupAction action = itemHoldingPoint.GetChild(0).GetComponent<ItemPickupAction>();
                    if (action) {
                        action.ReleaseAndTurnOffPickedUpBehaviour();
                    }
                }
            }
            if (other.gameObject.CompareTag(animalTypeTag))
            {
                GameObject linkedAnimal = other.gameObject;
                animalLocked = true;
                linkedAnimal.transform.SetParent(this.transform);
                linkedAnimal.transform.DOMove(this.transform.position, 1f);
                linkedAnimal.GetComponent<Rigidbody2D>().simulated = false;
            }
        }
    }
}
