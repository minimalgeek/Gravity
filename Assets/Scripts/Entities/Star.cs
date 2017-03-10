using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    private bool collected = false;
    private Vector3 scale;
    private new Light light;
    private float lightRange;
    public float pickUpTime = 0.2f;
    private float scaleMultiplier = 1;

    void Start()
    {
        scale = transform.localScale;
        light = GetComponentInChildren<Light>();
        lightRange = light.range;
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, 0, Time.deltaTime * 100));
        if (collected)
        {
            scaleMultiplier -= Time.deltaTime / pickUpTime;
            transform.localScale = scale * scaleMultiplier;
            light.range = lightRange * scaleMultiplier * (5 - 4 * scaleMultiplier);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!collected && other.tag == "Player")
        {
            collected = true;
            Destroy(gameObject, pickUpTime);
        }
    }
}
