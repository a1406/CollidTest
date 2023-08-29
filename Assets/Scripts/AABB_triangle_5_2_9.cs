using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABB_triangle_5_2_9 : MonoBehaviour
{
    public Transform vert1;
    public Transform vert2;
    public Transform vert3;
    public Transform triangle;
    // Start is called before the first frame update
    void Start()
    {
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
    }
}
