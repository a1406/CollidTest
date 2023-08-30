using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lineSeq_plane_5_3_1 : MonoBehaviour
{
    public Transform m_linePos1;
    public Transform m_linePos2;
//    public Transform m_line;
    public Transform m_plane;
    public Transform m_sphere;
    public Transform m_aabb;
    public Transform m_obb;
    public Material[] m_materials;

    LineRenderer line_render;
    MeshRenderer plane_render;
    MeshRenderer sphere_render;
    MeshRenderer aabb_render;
    MeshRenderer obb_render;

    // Start is called before the first frame update
    void Start()
    {
        sphere_render = m_sphere.GetComponent<MeshRenderer>();
        plane_render = m_plane.GetComponent<MeshRenderer>();
        aabb_render = m_aabb.GetComponent<MeshRenderer>();
        obb_render = m_obb.GetComponent<MeshRenderer>();
        //       line_render = m_line.GetComponent<LineRenderer>();
        line_render = GetComponent<LineRenderer>();
    }

    void DrawLine()
    {
        line_render.positionCount = 2;

        line_render.SetPosition(0, m_linePos1.position);
        line_render.SetPosition(1, m_linePos2.position);
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

    void UpdateAABB(CollidTest.LineSegment lineSeg)
    {
        Vector3 pos = m_aabb.position;
        Vector3 halfLen = m_aabb.localScale / 2;
        if (lineSeg.Dir.x < float.Epsilon)
        {
            if ((lineSeg.Start.x > pos.x + halfLen.x || lineSeg.Start.x < pos.x - halfLen.x)
                && (lineSeg.End.x > pos.x + halfLen.x || lineSeg.End.x < pos.x - halfLen.x))
            {
                aabb_render.material = m_materials[0];
                return;
            }
        }
        if (lineSeg.Dir.y < float.Epsilon)
        {
            if ((lineSeg.Start.y > pos.y + halfLen.y || lineSeg.Start.y < pos.y - halfLen.y)
                && (lineSeg.End.y > pos.y + halfLen.y || lineSeg.End.y < pos.y - halfLen.y))
            {
                aabb_render.material = m_materials[0];
                return;
            }
        }
        if (lineSeg.Dir.z < float.Epsilon)
        {
            if ((lineSeg.Start.z > pos.z + halfLen.z || lineSeg.Start.z < pos.z - halfLen.z)
                && (lineSeg.End.z > pos.z + halfLen.z || lineSeg.End.z < pos.z - halfLen.z))
            {
                aabb_render.material = m_materials[0];
                return;
            }
        }

        float tmin = float.MinValue;
        float tmax = float.MaxValue;
        if (lineSeg.Dir.x != 0)
        {
            float tx1 = (pos.x - halfLen.x - lineSeg.Start.x) / (lineSeg.End.x - lineSeg.Start.x);
            float tx2 = (pos.x + halfLen.x - lineSeg.Start.x) / (lineSeg.End.x - lineSeg.Start.x);
            if (tx1 > tx2) (tx1, tx2) = (tx2, tx1);
            if ((tx1 < 0 && tx2 < 0) || (tx1 > 1 && tx2 > 1))
            {
                aabb_render.material = m_materials[0];
                return;
            }
            tmin = tx1;
            tmax = tx2;
        }

        if (lineSeg.Dir.y != 0)
        {
            float ty1 = (pos.y - halfLen.y - lineSeg.Start.y) / (lineSeg.End.y - lineSeg.Start.y);
            float ty2 = (pos.y + halfLen.y - lineSeg.Start.y) / (lineSeg.End.y - lineSeg.Start.y);
            if (ty1 > ty2) (ty1, ty2) = (ty2, ty1);
            if ((ty1 < 0 && ty2 < 0) || (ty1 > 1 && ty2 > 1))
            {
                aabb_render.material = m_materials[0];
                return;
            }
            if (tmin < ty1) tmin = ty1;
            if (tmax > ty2) tmax = ty2;
            if (tmin > tmax)
            {
                aabb_render.material = m_materials[0];
                return;
            }
        }

        if (lineSeg.Dir.z != 0)
        {
            float tz1 = (pos.z - halfLen.z - lineSeg.Start.z) / (lineSeg.End.z - lineSeg.Start.z);
            float tz2 = (pos.z + halfLen.z - lineSeg.Start.z) / (lineSeg.End.z - lineSeg.Start.z);
            if (tz1 > tz2) (tz1, tz2) = (tz2, tz1);
            if (tmin < tz1) tmin = tz1;
            if (tmax > tz2) tmax = tz2;
            if ((tz1 < 0 && tz2 < 0) || (tz1 > 1 && tz2 > 1))
            {
                aabb_render.material = m_materials[0];
                return;
            }
        }
        if (tmin > tmax)
        {
            aabb_render.material = m_materials[0];
            return;
        }
        aabb_render.material = m_materials[1];
    }

    void UpdateOBB(CollidTest.LineSegment lineSeg)
    {
        CollidTest.OBB obb = new(m_obb.position, m_obb.localScale, m_obb.rotation);
        Vector3 obbposStart = obb.CalcRelativePos(lineSeg.Start - obb.C);
        Vector3 obbposEnd = obb.CalcRelativePos(lineSeg.End - obb.C);
        CollidTest.LineSegment obbLine = new(obbposStart, obbposEnd);

        ///
        Vector3 pos = Vector3.zero;
        Vector3 halfLen = m_obb.localScale / 2;

        if (obbLine.Dir.x < float.Epsilon)
        {
            if ((obbLine.Start.x > pos.x + halfLen.x || obbLine.Start.x < pos.x - halfLen.x)
                && (obbLine.End.x > pos.x + halfLen.x || obbLine.End.x < pos.x - halfLen.x))
            {
                obb_render.material = m_materials[0];
                return;
            }
        }
        if (obbLine.Dir.y < float.Epsilon)
        {
            if ((obbLine.Start.y > pos.y + halfLen.y || obbLine.Start.y < pos.y - halfLen.y)
                && (obbLine.End.y > pos.y + halfLen.y || obbLine.End.y < pos.y - halfLen.y))
            {
                obb_render.material = m_materials[0];
                return;
            }
        }
        if (obbLine.Dir.z < float.Epsilon)
        {
            if ((obbLine.Start.z > pos.z + halfLen.z || obbLine.Start.z < pos.z - halfLen.z)
                && (obbLine.End.z > pos.z + halfLen.z || obbLine.End.z < pos.z - halfLen.z))
            {
                obb_render.material = m_materials[0];
                return;
            }
        }

        float tmin = float.MinValue;
        float tmax = float.MaxValue;
        if (obbLine.Dir.x != 0)
        {
            float tx1 = (pos.x - halfLen.x - obbLine.Start.x) / (obbLine.End.x - obbLine.Start.x);
            float tx2 = (pos.x + halfLen.x - obbLine.Start.x) / (obbLine.End.x - obbLine.Start.x);
            if (tx1 > tx2) (tx1, tx2) = (tx2, tx1);
            if ((tx1 < 0 && tx2 < 0) || (tx1 > 1 && tx2 > 1))
            {
                obb_render.material = m_materials[0];
                return;
            }
            tmin = tx1;
            tmax = tx2;
        }

        if (obbLine.Dir.y != 0)
        {
            float ty1 = (pos.y - halfLen.y - obbLine.Start.y) / (obbLine.End.y - obbLine.Start.y);
            float ty2 = (pos.y + halfLen.y - obbLine.Start.y) / (obbLine.End.y - obbLine.Start.y);
            if (ty1 > ty2) (ty1, ty2) = (ty2, ty1);
            if ((ty1 < 0 && ty2 < 0) || (ty1 > 1 && ty2 > 1))
            {
                obb_render.material = m_materials[0];
                return;
            }
            if (tmin < ty1) tmin = ty1;
            if (tmax > ty2) tmax = ty2;
            if (tmin > tmax)
            {
                obb_render.material = m_materials[0];
                return;
            }
        }

        if (obbLine.Dir.z != 0)
        {
            float tz1 = (pos.z - halfLen.z - obbLine.Start.z) / (obbLine.End.z - obbLine.Start.z);
            float tz2 = (pos.z + halfLen.z - obbLine.Start.z) / (obbLine.End.z - obbLine.Start.z);
            if (tz1 > tz2) (tz1, tz2) = (tz2, tz1);
            if (tmin < tz1) tmin = tz1;
            if (tmax > tz2) tmax = tz2;
            if ((tz1 < 0 && tz2 < 0) || (tz1 > 1 && tz2 > 1))
            {
                obb_render.material = m_materials[0];
                return;
            }
        }
        if (tmin > tmax)
        {
            obb_render.material = m_materials[0];
            return;
        }
        obb_render.material = m_materials[1];
    }

    void UpdateOBBBySeparateAxis(CollidTest.LineSegment lineSeg)
    {
        CollidTest.OBB obb = new(m_obb.position, m_obb.localScale, m_obb.rotation);
        Vector3 obbposStart = obb.CalcRelativePos(lineSeg.Start - obb.C);
        Vector3 obbposEnd = obb.CalcRelativePos(lineSeg.End - obb.C);
        CollidTest.LineSegment obbLine = new(obbposStart, obbposEnd);

        float a, b, c;
        //1£¬0£¬0
        a = obb.HalfLen.x;
        b = obbLine.Start.x;
        c = obbLine.End.x;
        if ((a < b && a < c) || (-a > b && -a > c))
        {
            obb_render.material = m_materials[0];
            return;
        }

        //0£¬1£¬0
        a = obb.HalfLen.y;
        b = obbLine.Start.y;
        c = obbLine.End.y;
        if ((a < b && a < c) || (-a > b && -a > c))
        {
            obb_render.material = m_materials[0];
            return;
        }

        //0£¬0£¬1
        a = obb.HalfLen.z;
        b = obbLine.Start.z;
        c = obbLine.End.z;
        if ((a < b && a < c) || (-a > b && -a > c))
        {
            obb_render.material = m_materials[0];
            return;
        }

        Vector3[] t = new Vector3[3]
            {
                    new Vector3(1, 0, 0),
                    new Vector3(0, 1, 0),
                    new Vector3(0, 0, 1),
            };
        // 100 * line
        Vector3 axis = Vector3.Cross(t[0], obbLine.Dir);
        axis.Normalize();
        //a = Vector3.Dot(axis, obb.HalfLen);
        a = Mathf.Abs(axis.x * obb.HalfLen.x) + Mathf.Abs(axis.y * obb.HalfLen.y) + Mathf.Abs(axis.z * obb.HalfLen.z);
        b = Vector3.Dot(axis, obbLine.Start);
        c = Vector3.Dot(axis, obbLine.End);
        if ((a < b && a < c) || (-a > b && -a > c))
        {
            obb_render.material = m_materials[0];
            return;
        }

        // 010 * line
        axis = Vector3.Cross(t[1], obbLine.Dir);
        axis.Normalize();
        a = Mathf.Abs(axis.x * obb.HalfLen.x) + Mathf.Abs(axis.y * obb.HalfLen.y) + Mathf.Abs(axis.z * obb.HalfLen.z);
        b = Vector3.Dot(axis, obbLine.Start);
        c = Vector3.Dot(axis, obbLine.End);
        if ((a < b && a < c) || (-a > b && -a > c))
        {
            obb_render.material = m_materials[0];
            return;
        }

        // 001 * line
        axis = Vector3.Cross(t[2], obbLine.Dir);
        axis.Normalize();
        a = Mathf.Abs(axis.x * obb.HalfLen.x) + Mathf.Abs(axis.y * obb.HalfLen.y) + Mathf.Abs(axis.z * obb.HalfLen.z);
        b = Vector3.Dot(axis, obbLine.Start);
        c = Vector3.Dot(axis, obbLine.End);
        if ((a < b && a < c) || (-a > b && -a > c))
        {
            obb_render.material = m_materials[0];
            return;
        }
        obb_render.material = m_materials[1];
    }

    // Update is called once per frame
    void Update()
    {
        DrawLine();
        Vector3[] linePos = new Vector3[2];
        linePos[0] = m_linePos1.position;
        linePos[1] = m_linePos2.position;

        CollidTest.LineSegment lineSeg = new(linePos[0], linePos[1]);
        CollidTest.Plane plane = new(m_plane.position, m_plane.rotation);

        UpdatePlane(plane, lineSeg);

        UpdateSphere(lineSeg);

        UpdateAABB(lineSeg);

        //UpdateOBB(lineSeg);
        UpdateOBBBySeparateAxis(lineSeg);
        return;
    }
}
