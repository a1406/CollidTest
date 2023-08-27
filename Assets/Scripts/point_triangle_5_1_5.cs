using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class point_triangle_5_1_5 : MonoBehaviour
{
    public Transform point;

    public Transform vert1;
    public Transform vert2;
    public Transform vert3;
    LineRenderer m_lineRenderer;
    public Transform triangle;

    // Start is called before the first frame update
    void Start()
    {
        if (!m_lineRenderer)
            m_lineRenderer = GetComponent<LineRenderer>();
    }

    void DrawTriangle()
    {
        Mesh m = new Mesh();
        Vector3[] verts = new Vector3[]
        {
            vert1.position,
            vert2.position,
            vert3.position,
        };
        m.vertices = verts;
        int[] triangles = new int[] { 0, 1, 2 };
        m.triangles = triangles;

        triangle.GetComponent<MeshFilter>().mesh = m;
    }

    // Update is called once per frame
    void Update()
    {
        DrawTriangle();
        CollidTest.triangle TestTriangle = new CollidTest.triangle(vert1.position, vert2.position, vert3.position);
        Vector3 start = point.position;
        Vector3 end = TestTriangle.GetClosestPoint(start);
        m_lineRenderer.positionCount = 2;

        m_lineRenderer.SetPosition(0, start);
        m_lineRenderer.SetPosition(1, end);

    }
}
