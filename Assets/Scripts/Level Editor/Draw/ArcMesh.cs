using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class ArcMesh : MonoBehaviour
{
    [SerializeField]
    [Delayed]
    private int angularResolution = 768;
    public int AngularResolution { get { return angularResolution; } }

    [SerializeField]
    [Delayed]
    private float innerRadius = 20;
    public float InnerRadius { get { return innerRadius; } }

    [SerializeField]
    [Delayed]
    private float outerRadius = 20.25f;
    public float OuterRadius { get { return outerRadius; } }

    [SerializeField]
    [Delayed]
    private int arcNumerator = 1;
    public int ArcNumerator { get { return arcNumerator; } }
    [SerializeField]
    [Delayed]
    private int arcDenominator = 192;
    public int ArcDenominator { get { return arcDenominator; } }

    [SerializeField]
    [Delayed]
    private float zOffset = 0;
    public float ZOffset { get { return zOffset; } }

    private PolygonCollider2D polyCollider;
    private MeshFilter meshFilter;
    private Mesh mesh;

    // Use this for initialization
    void Start()
    {
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

    public void SetParams(float innerRadius, float outerRadius, int arcNumerator, int arcDenominator, int angularResolution, float zOffset, bool doUndo = false)
    {
        this.innerRadius = innerRadius;
        this.outerRadius = outerRadius;
        this.arcNumerator = arcNumerator;
        this.arcDenominator = arcDenominator;
        this.angularResolution = angularResolution;
        this.zOffset = zOffset;
        CreatePoints(doUndo);
    }

    public void CreatePoints(bool doUndo = false)
    {
        polyCollider = GetOrAddComponent<PolygonCollider2D>();
        meshFilter = GetOrAddComponent<MeshFilter>();
        meshFilter.hideFlags |= HideFlags.NotEditable;
        if (mesh == null)
            meshFilter.mesh = mesh = new Mesh();


        int realSegments = Mathf.Max(angularResolution * arcNumerator / arcDenominator, 1);

        float centralAngle = 360f * arcNumerator / arcDenominator;
        // Ez lehet, hogy redundáns. TODO: Megvizsgálni, hogy a realSegments int castja miatt nem lenne-e rossz a 360f / angularResolution!
        float stepAngle = centralAngle / realSegments;
        Quaternion stepRotator = Quaternion.Euler(0, 0, stepAngle);

        Vector2 pos = new Vector2(-Mathf.Sin(Mathf.Deg2Rad * centralAngle * 0.5f), -Mathf.Cos(Mathf.Deg2Rad * centralAngle * 0.5f)) * outerRadius;
        Vector2 offset = Vector2.up * outerRadius;

        float innerPerOuter = innerRadius / outerRadius;

        List<Vector2> outerPath = new List<Vector2>();
        List<Vector2> innerPath = new List<Vector2>();
        Vector3[] vertices = new Vector3[2 * (realSegments + 1)];
        Vector3[] normals = new Vector3[2 * (realSegments + 1)];
        Vector2[] uvs = new Vector2[2 * (realSegments + 1)];
        for (int i = 0; i <= realSegments; i++)
        {
            outerPath.Add(pos + offset);
            innerPath.Add(pos * innerPerOuter + offset);
            vertices[i * 2] = (Vector3)outerPath[i] + Vector3.forward * zOffset;
            vertices[i * 2 + 1] = (Vector3)innerPath[i] + Vector3.forward * zOffset;
            normals[i * 2] = Vector3.back;
            normals[i * 2 + 1] = Vector3.back;
            uvs[i * 2] = Vector2.right * ((float)i / realSegments);
            uvs[i * 2 + 1] = Vector2.right * ((float)i / realSegments) + Vector2.up;
            pos = stepRotator * pos;
        }

#if UNITY_EDITOR
        //string undoName = UnityEditor.Undo.GetCurrentGroupName();
        //if (undoName == "") undoName = "Arc";
        //Debug.Log(undoName);
        //UnityEditor.Undo.RegisterCompleteObjectUndo(this, undoName);
        //UnityEditor.Undo.RecordObject(meshFilter, undoName);
        //UnityEditor.Undo.RecordObject(mesh, undoName);
#endif

        mesh.Clear();
#if UNITY_EDITOR
        //UnityEditor.Undo.RecordObject(mesh, undoName);
#endif
        mesh.vertices = vertices;
#if UNITY_EDITOR
        //UnityEditor.Undo.RecordObject(mesh, undoName);
#endif
        mesh.normals = normals;
#if UNITY_EDITOR
        //UnityEditor.Undo.RecordObject(mesh, undoName);
#endif
        mesh.uv = uvs;
#if UNITY_EDITOR
        //UnityEditor.Undo.RecordObject(mesh, undoName);
#endif
        mesh.uv2 = uvs;
        int[] triangles = new int[(vertices.Length - 2) * 3];
        for (int i = 0; i < realSegments; i++)
        {
            triangles[6 * i] = 2 * i;
            triangles[6 * i + 1] = 2 * i + 1;
            triangles[6 * i + 2] = 2 * i + 2;

            triangles[6 * i + 3] = 2 * i + 2;
            triangles[6 * i + 4] = 2 * i + 1;
            triangles[6 * i + 5] = 2 * i + 3;
        }
#if UNITY_EDITOR
        //UnityEditor.Undo.RecordObject(mesh, undoName);
#endif
        mesh.triangles = triangles;

        innerPath.Reverse();
        outerPath.AddRange(innerPath);

#if UNITY_EDITOR
        //UnityEditor.Undo.RecordObject(polyCollider, undoName);
#endif
        polyCollider.pathCount = 1;
#if UNITY_EDITOR
        //UnityEditor.Undo.RecordObject(polyCollider, undoName);
#endif
        polyCollider.SetPath(0, outerPath.ToArray());
    }
}
