using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;

public class ToCenterRotator : GLMonoBehaviour
{

    private Gravity worldCenter;
    void Start()
    {
        worldCenter = FindObjectOfType(typeof(Gravity)) as Gravity;
    }

    void Update()
    {
        Vector3 vectorToTarget = worldCenter.transform.position - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.Euler(0,0,angle - 90);
    }
}
