using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;

public class Shooter : GLMonoBehaviour
{

    public GameObject bulletPrefab;
    public float bulletImpulse = 5f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            GameObject newBullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            Rigidbody2D rb = newBullet.GetComponent<Rigidbody2D>();
            Vector2 direction = transform.position - this.transform.parent.transform.position;
            rb.AddForce(direction * bulletImpulse, ForceMode2D.Impulse);
        }
    }
}
