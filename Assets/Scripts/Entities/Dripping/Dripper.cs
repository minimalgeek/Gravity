using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dripper : MonoBehaviour
{

    public GameObject drop;
    public float lifeTime = 10;
    public float rate = 2;
    public float initialSpeed = 1;

    // Update is called once per frame
    void Update()
    {
        if (Random.value < rate * Time.deltaTime)
        {
            GameObject newDrop = Instantiate(drop, transform.position, Quaternion.Euler(0, 0, Random.value * 360));
            newDrop.AddComponent<DelayedRemove>().removeDelay = lifeTime;
            Rigidbody2D rb = newDrop.GetComponent<Rigidbody2D>();
            if (rb)
            {
                rb.velocity += (Vector2)transform.up * initialSpeed;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector2 end = transform.position + transform.up * initialSpeed;
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, end);
        Gizmos.DrawLine(end, end - (Vector2)transform.up * 0.2f - (Vector2)transform.right * 0.1f);
        Gizmos.DrawLine(end, end - (Vector2)transform.up * 0.2f + (Vector2)transform.right * 0.1f);
        Gizmos.DrawIcon(transform.position, "HintCS45.png");
    }
}