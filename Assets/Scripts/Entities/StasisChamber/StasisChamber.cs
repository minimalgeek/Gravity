using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StasisChamber : MonoBehaviour
{

    private bool animalLocked = false;
    public GameObject linkedAnimal;

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
            if (other.gameObject == linkedAnimal)
            {
                animalLocked = true;
				linkedAnimal.transform.DOMove(this.transform.position, 1f);
				linkedAnimal.GetComponent<Rigidbody2D>().simulated = false;
				foreach (var c in linkedAnimal.GetComponents<Collider2D>()) {
					c.enabled = false;
				}
            }
        }
    }
}
