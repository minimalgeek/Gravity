using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class Turret : MonoBehaviour, IPointerClickHandler
{
    public GameObject projectile;
    private GameObject instance;
    private GameObject reticle;

    Vector3 spawnPoint;

    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.name.Equals("Reticle"))
                reticle = child.gameObject;
        }
        spawnPoint = transform.position + transform.up * 0.75f;
        GetComponent<LineRenderer>().SetPositions(new Vector3[] { spawnPoint, reticle.transform.position });
        Spawn();
    }

    void Spawn()
    {
        instance = Instantiate(projectile, spawnPoint, transform.rotation) as GameObject;
        foreach (Collider2D coll in instance.GetComponents<Collider2D>())
        {
            coll.enabled = false;
        }
    }

    void Shoot()
    {
        instance.GetComponent<Rigidbody2D>().velocity = reticle.transform.position - instance.transform.position;
        foreach (Collider2D coll in instance.GetComponents<Collider2D>())
        {
            coll.enabled = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Shoot();
        Spawn();
    }


    //private float since = 0f;
    //void Update()
    //{
    //    since += Time.deltaTime;
    //    if (since > 0.001)
    //    {
    //        OnPointerClick(null);
    //        since = 0f;
    //    }
    //}
}
