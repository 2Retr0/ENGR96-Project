using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionConeMeshRenderer : MonoBehaviour
{
    private const int IgnoreLayerMask = ~(1 << 2);
    private const float ArcLengthDelta = 0.005f;

    [SerializeField] private float range = 5f;
    [SerializeField] private float arcAngle = 45f;

    private Mesh mesh;
    private float rayCount;
    private float arcAngleDelta;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh = new Mesh();
        rayCount = (int) (range * (arcAngle * Mathf.Deg2Rad) / ArcLengthDelta);
        arcAngleDelta = arcAngle / rayCount;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DrawMesh(transform);
    }

    void DrawMesh(Transform t)
    {
        var vertices = new List<Vector3> { t.position };
        var castDirection = Quaternion.Euler(0, -arcAngle * 0.5f, 0);

        for (var i = 0; i < rayCount; ++i)
        {
            castDirection *= Quaternion.Euler(0, arcAngleDelta, 0);

            if (Physics.Raycast(t.position, castDirection * t.forward, out var hit, range, IgnoreLayerMask))
            {
                // Debug.DrawRay(t.position, castDirection * t.forward * hit.distance, Color.white);
                vertices.Add(castDirection * Vector3.forward * hit.distance);
            }
            else
            {
                // Debug.DrawRay(t.position, castDirection * t.forward * range, Color.white);
                vertices.Add(castDirection * Vector3.forward * range);
            }
        }

        var triangles = getTris(vertices.ToArray());

        mesh.ClearBlendShapes();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles;
    }


    int[] getTris(Vector3[] vertices)
    {
        var triangles = new List<int>();
        for(var i = 0; i < vertices.Length - 2; ++i)
        {
            triangles.Add(0);
            triangles.Add(i + 1);
            triangles.Add(i + 2);
        }

        return triangles.ToArray();
    }
}
