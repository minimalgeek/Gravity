using UnityEngine;

public class Trampoline : MonoBehaviour {
    public float throwSpeed = 40f;

    void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.AddForce(transform.up * throwSpeed * other.attachedRigidbody.mass, ForceMode2D.Impulse);
    }
}
