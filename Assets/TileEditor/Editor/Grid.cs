using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;
using UnityEditor;

[ExecuteInEditMode]
public class Grid : EditorWindow
{

    [RangeAttribute(0.5f, 30f)]
    public float radius = 2f;
    [RangeAttribute(0.1f, 5f)]
    public float radiusDivisionLength = 0.2f;
    [RangeAttribute(2, 30)]
    public int cakeSliceNumber = 6;
    public Color lineColor = Color.white;

    void OnDrawGizmos()
    {
        Vector3 pos = Camera.current.transform.position;
        Gizmos.color = this.lineColor;

        for (float ri = 0f; ri <= radius; ri += radiusDivisionLength)
        {
            Gizmos.DrawWireSphere(Vector3.zero, ri);
        }

        float sliceDegreeIncrement = 360.0f / (float)cakeSliceNumber;

        for (float sliceDegree = 0f; sliceDegree <= 360f; sliceDegree += sliceDegreeIncrement)
        {
            Vector3 to = Quaternion.Euler(0, 0, sliceDegree) * Vector3.zero.WithY(radius);
            Gizmos.DrawLine(Vector3.zero, to);
        }
    }
}
