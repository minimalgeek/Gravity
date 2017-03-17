using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(LineRenderer), typeof(PolygonCollider2D))]
public class ArcTile : MonoBehaviour
{
    [SerializeField]
    [Delayed]
    private int angularResolution = 768;

    [SerializeField]
    [Delayed]
    private float innerRadius = 20;

    [SerializeField]
    [Delayed]
    private float outerRadius = 20.25f;

    [SerializeField]
    [Delayed]
    private int arcNumerator = 1;
    [SerializeField]
    [Delayed]
    private int arcDenominator = 192;

    //[SerializeField]
    //[Delayed]
    //private float zOffset = 0;

    private PolygonCollider2D polyCollider;
    private LineRenderer line;

    void Start()
    {
        line = GetOrAddComponent<LineRenderer>();
        polyCollider = GetOrAddComponent<PolygonCollider2D>();
        if (polyCollider == null) Debug.LogError("poly");

        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        line.receiveShadows = false;
        line.useWorldSpace = false;
        line.startWidth = outerRadius - innerRadius;
        line.endWidth = line.startWidth;

        CreatePoints();
    }

    private T GetOrAddComponent<T>() where T : Component
    {
        T comp = this.gameObject.GetComponent<T>();

        if (comp == null)
        {
            comp = this.gameObject.AddComponent<T>();
        }

        return comp;
    }

    public void SetParams(float innerRadius, float outerRadius, int arcNumerator, int arcDenominator, int angularResolution, float zOffset)
    {
        this.innerRadius = innerRadius;
        this.outerRadius = outerRadius;
        this.arcNumerator = arcNumerator;
        this.arcDenominator = arcDenominator;
        this.angularResolution = angularResolution;
        transform.position = transform.position.WithZ(zOffset);
        CreatePoints();
    }

    public void CreatePoints()
    {
        try
        {
            int realSegments = Mathf.Max(angularResolution * arcNumerator / arcDenominator, 1);
            line.startWidth = outerRadius - innerRadius;
            line.endWidth = line.startWidth;
            line.numPositions = realSegments + 1;

            float centralAngle = 360f * arcNumerator / arcDenominator;
            // Ez lehet, hogy redundáns. TODO: Megvizsgálni, hogy a realSegments int castja miatt nem lenne-e rossz a 360f / angularResolution!
            float stepAngle = centralAngle / realSegments;
            Quaternion stepRotator = Quaternion.Euler(0, 0, stepAngle);

            Vector2 pos = new Vector2(-Mathf.Sin(Mathf.Deg2Rad * centralAngle * 0.5f), -Mathf.Cos(Mathf.Deg2Rad * centralAngle * 0.5f)) * outerRadius;
            Vector2 offset = Vector2.up * outerRadius;

            float innerPerOuter = innerRadius / outerRadius;
            float meanPerOuter = (innerRadius + outerRadius) / (outerRadius * 2f);

            List<Vector2> outerPath = new List<Vector2>();
            List<Vector2> innerPath = new List<Vector2>();
            for (int i = 0; i <= realSegments; i++)
            {
                outerPath.Add(pos + offset);
                innerPath.Add(pos * innerPerOuter + offset);
                try
                {
                    line.SetPosition(i, pos * meanPerOuter + offset);
                }
                catch { }
                pos = stepRotator * pos;
            }
            innerPath.Reverse();
            outerPath.AddRange(innerPath);

            polyCollider.pathCount = 1;
            polyCollider.SetPath(0, outerPath.ToArray());
        }
        catch { }
    }
}
