using UnityEngine;
using System.Collections;
using System;

public class Turret : MonoBehaviour
{
    public GameObject projectile;
    public float despawnTime = 60;
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
        Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();
        rb.simulated = true;
        rb.velocity = reticle.transform.position - instance.transform.position;
        foreach (Collider2D coll in instance.GetComponents<Collider2D>())
        {
            coll.enabled = true;
        }
        instance.AddComponent<DelayedRemove>().removeDelay = despawnTime;
    }

    public void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
            Spawn();
        }
    }
}
