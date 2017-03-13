using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;

[ExecuteInEditMode]
public class DrawCircle : MonoBehaviour
{

    [SerializeField]
    [Range(0, 1000)]
    private int segments = 60;

    [SerializeField]
    private float radius = 10;

    [SerializeField]
    [Range(0.0f, 360.0f)]
    private float fromAngle = 30;

    [SerializeField]
    [Range(0.0f, 360.0f)]
    private float toAngle = 60;

    [SerializeField]
    private float offset = 0;

    [SerializeField]
    private float lineRendererWidth = 1f;

    [SerializeField]
    [Tooltip("If checked, the circle will be rendered again each time one of the parameters change.")]
    private bool checkValuesChanged = true;

    private int previousSegmentsValue;
    private float previousRadiusValue;
    private float previousOffsetValue;
    private float previousFromAngle;
    private float previousToAngle;
    private float previousLineRendererWidth;

	private PolygonCollider2D polyCollider;
    private LineRenderer line;

    void Start()
    {
		line = GetOrAddComponent<LineRenderer>();
		polyCollider = GetOrAddComponent<PolygonCollider2D>();
		
        line.useWorldSpace = false;

        UpdateValuesChanged();
        CreatePoints();
    }

	private T GetOrAddComponent<T>() where T : Component {
		T comp = this.gameObject.GetComponent<T>();

		if (comp == null) {
			comp = this.gameObject.AddComponent<T>();
		}

		return comp;
	}

    void Update()
    {
#if UNITY_EDITOR
        line = gameObject.GetComponent<LineRenderer>();
#endif
        if (checkValuesChanged)
        {
            if (Changed())
            {
                line.startWidth = lineRendererWidth;
                line.endWidth = lineRendererWidth;
                CreatePoints();
            }

            UpdateValuesChanged();
        }
    }

    bool Changed()
    {
        return previousSegmentsValue != segments ||
                previousRadiusValue != radius ||
                previousOffsetValue != offset ||
                previousFromAngle != fromAngle ||
                previousToAngle != toAngle ||
                previousLineRendererWidth != lineRendererWidth;
    }

    void UpdateValuesChanged()
    {
        previousSegmentsValue = segments;
        previousRadiusValue = radius;
        previousOffsetValue = offset;
        previousFromAngle = fromAngle;
        previousToAngle = toAngle;
        previousLineRendererWidth = lineRendererWidth;
    }

    void CreatePoints()
    {

        int realSegments = (int)(segments * ((toAngle - fromAngle) / 360.0));
        line.numPositions = realSegments + 1;

        float x;
        float y;
        float z = offset;

        float angle = fromAngle;

        List<Vector2> upperPath = new List<Vector2>();
        List<Vector2> lowerPath = new List<Vector2>();
        for (int i = 0; i < realSegments + 1; i++)
        {
            Debug.Log(angle);
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            Vector3 pos = new Vector3(x, y, z);
            line.SetPosition(i, pos);

			Vector2 pos2D = pos.To2DXY();
			Vector2 pos2DNormal = pos.normalized;
            upperPath.Add(pos2D + pos2DNormal*(lineRendererWidth/2.0f));
            lowerPath.Add(pos2D - pos2DNormal*(lineRendererWidth/2.0f));

            angle += (360f / segments);
        }
        lowerPath.Reverse();
        upperPath.AddRange(lowerPath);

		polyCollider.SetPath(0, upperPath.ToArray());
    }
}
