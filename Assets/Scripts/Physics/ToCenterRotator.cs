using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;

[ExecuteInEditMode]
public class ToCenterRotator : GLMonoBehaviour
{

    void Update()
    {
        Vector3 vectorToTarget = -transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.Euler(0,0,angle - 90);
    }
}
