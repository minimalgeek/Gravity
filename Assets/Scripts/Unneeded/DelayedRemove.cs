using UnityEngine;

public class DelayedRemove : MonoBehaviour
{
    public float removeDelay = 5;
    private float lifeTime;

    void Update()
    {
        lifeTime += Time.deltaTime;
        if (lifeTime >= removeDelay)
            Destroy(gameObject);
    }
}
