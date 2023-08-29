using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lineSeq_plane_5_3_1 : MonoBehaviour
{
    public Transform m_line;
    public Transform m_plane;
    public Transform m_sphere;
    public Transform m_cube;
    public Material[] m_materials;

    LineRenderer line_render;
    MeshRenderer plane_render;
    MeshRenderer sphere_render;
    MeshRenderer cube_render;

    // Start is called before the first frame update
    void Start()
    {
        sphere_render = m_sphere.GetComponent<MeshRenderer>();
        plane_render = m_plane.GetComponent<MeshRenderer>();
        cube_render = m_cube.GetComponent<MeshRenderer>();
        line_render = m_line.GetComponent<LineRenderer>();
    }

    void UpdatePlane(CollidTest.Plane plane, CollidTest.LineSegment lineSeg)
    {
        float b = Vector3.Dot(plane.Normal, lineSeg.End - lineSeg.Start);
        if (b == 0)
        {
            plane_render.material = m_materials[0];
            return;
        }
        float a = Vector3.Dot(plane.Pos, plane.Normal) - Vector3.Dot(plane.Normal, lineSeg.Start);
        float t = a / b;
        if (t < 0 || t > 1)
        {
            plane_render.material = m_materials[0];
            return;
        }
        else
        {
            plane_render.material = m_materials[1];
            return;
        }
    }

    void UpdateSphere(CollidTest.LineSegment lineSeg)
    {
        Vector3 D = lineSeg.Start - m_sphere.position;
        Vector3 V = lineSeg.End - lineSeg.Start;
        //ax*x + bx + c = 0
        float a = Vector3.Dot(V, V);
        float b = 2 * Vector3.Dot(D, V);
        float c = Vector3.Dot(D, D) - m_sphere.localScale.x * m_sphere.localScale.x / 4;
        if (a == 0)
        {
            sphere_render.material = m_materials[0];
            return;
        }

        float cc = b * b - 4 * a * c;
        if (cc < 0)
        {
            sphere_render.material = m_materials[0];
            return;
        }
        cc = Mathf.Sqrt(cc);

        float x1 = (-b + cc) / 2 / a;
        float x2 = (-b - cc) / 2 / a;

        if ((x1 >= 0 && x1 <= 1) || (x2 >= 0 && x2 <= 1))
        {
            sphere_render.material = m_materials[1];
            return;
        }


        sphere_render.material = m_materials[0];
        return;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3[] linePos = new Vector3[2];
        line_render.GetPositions(linePos);
        linePos[0] += m_line.position;
        linePos[1] += m_line.position;

        CollidTest.LineSegment lineSeg = new(linePos[0], linePos[1]);
        CollidTest.Plane plane = new(m_plane.position, m_plane.rotation);

        UpdatePlane(plane, lineSeg);

        UpdateSphere(lineSeg);

        //////
        Vector3 pos = m_cube.position;
        Vector3 halfLen = m_cube.localScale / 2;
        if (lineSeg.Dir.x < float.Epsilon)
        {
            if ((lineSeg.Start.x > pos.x + halfLen.x || lineSeg.Start.x < pos.x - halfLen.x)
                && (lineSeg.End.x > pos.x + halfLen.x || lineSeg.End.x < pos.x - halfLen.x))
            {
                cube_render.material = m_materials[0];
                return;
            }
        }
        if (lineSeg.Dir.y < float.Epsilon)
        {
            if ((lineSeg.Start.y > pos.y + halfLen.y || lineSeg.Start.y < pos.y - halfLen.y)
                && (lineSeg.End.y > pos.y + halfLen.y || lineSeg.End.y < pos.y - halfLen.y))
            {
                cube_render.material = m_materials[0];
                return;
            }
        }
        if (lineSeg.Dir.z < float.Epsilon)
        {
            if ((lineSeg.Start.z > pos.z + halfLen.z || lineSeg.Start.z < pos.z - halfLen.z)
                && (lineSeg.End.z > pos.z + halfLen.z || lineSeg.End.z < pos.z - halfLen.z))
            {
                cube_render.material = m_materials[0];
                return;
            }
        }

        float tx1 = (pos.x - halfLen.x - lineSeg.Start.x) / (lineSeg.End.x - lineSeg.Start.x);
        float tx2 = (pos.x + halfLen.x - lineSeg.Start.x) / (lineSeg.End.x - lineSeg.Start.x);
        if (tx1 > tx2) (tx1, tx2) = (tx2, tx1);
        float tmin = tx1;
        float tmax = tx2;

        float ty1 = (pos.y - halfLen.y - lineSeg.Start.y) / (lineSeg.End.y - lineSeg.Start.y);
        float ty2 = (pos.y + halfLen.y - lineSeg.Start.y) / (lineSeg.End.y - lineSeg.Start.y);
        if (ty1 > ty2) (ty1, ty2) = (ty2, ty1);
        if (tmin < ty1) tmin = ty1;
        if (tmax > ty2) tmax = ty2;
        if (tmin > tmax)
        {
            cube_render.material = m_materials[0];
            return;
        }

        float tz1 = (pos.z - halfLen.z - lineSeg.Start.z) / (lineSeg.End.z - lineSeg.Start.z);
        float tz2 = (pos.z + halfLen.z - lineSeg.Start.z) / (lineSeg.End.z - lineSeg.Start.z);
        if (tz1 > tz2) (tz1, tz2) = (tz2, tz1);
        if (tmin < tz1) tmin = tz1;
        if (tmax > tz2) tmax = tz2;
        if (tmin > tmax)
        {
            cube_render.material = m_materials[0];
            return;
        }
        cube_render.material = m_materials[1];
        return;
    }
}
