using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class line_triangle_5_3_4 : MonoBehaviour
{
    public Transform m_linePos1;
    public Transform m_linePos2;
    public Transform m_trianglePos1;
    public Transform m_trianglePos2;
    public Transform m_trianglePos3;
    public Material[] m_materials;
    public MeshRenderer m_triangleShow1;
    public MeshRenderer m_triangleShow2;

    public Transform m_hitPos;

    LineRenderer line_render;
    //MeshRenderer triangle_render;
    // Start is called before the first frame update
    void Start()
    {
        line_render = GetComponent<LineRenderer>();
    }

    void DrawTriangle()
    {
        Vector3[] verts = new Vector3[3]
        {
            m_trianglePos1.position,
            m_trianglePos2.position,
            m_trianglePos3.position,
        };
        Mesh m1 = new Mesh();
        m1.vertices = verts;
        int[] triangles1 = new int[] { 0, 1, 2 };
        m1.triangles = triangles1;

        Mesh m2 = new Mesh();
        m2.vertices = verts;
        int[] triangles2 = new int[] { 0, 2, 1 };
        m2.triangles = triangles2;

        m_triangleShow1.GetComponent<MeshFilter>().mesh = m1;
        m_triangleShow2.GetComponent<MeshFilter>().mesh = m2;
    }

    void DrawLine()
    {
        line_render.positionCount = 2;

        line_render.SetPosition(0, m_linePos1.position);
        line_render.SetPosition(1, m_linePos2.position);
    }

    void UpdateTriangle()
    {
        Vector3 pq = m_linePos2.position - m_linePos1.position;
        Vector3 ab = m_trianglePos2.position - m_trianglePos1.position;
        Vector3 bc = m_trianglePos3.position - m_trianglePos2.position;
        Vector3 ca = m_trianglePos1.position - m_trianglePos3.position;
        Vector3 pa = m_trianglePos1.position - m_linePos1.position;
        Vector3 pb = m_trianglePos2.position - m_linePos1.position;
        Vector3 pc = m_trianglePos3.position - m_linePos1.position;

        Vector3 cross_paq = Vector3.Cross(pq, pa);
        float pab = Vector3.Dot(cross_paq, pb);

        Vector3 cross_pbq = Vector3.Cross(pq, pb);
        float pbc = Vector3.Dot(cross_pbq, pc);

        float pca = -Vector3.Dot(cross_paq, pc);
        /////
        //Vector3 cross_pcq = Vector3.Cross(pq, pc);
        //float pca2 = Vector3.Dot(cross_pcq, pa);

        if (pab > 0 && pbc > 0 && pca > 0)
        {
            m_triangleShow1.material = m_materials[1];
            m_triangleShow2.material = m_materials[1];

            Vector3 hitPos = new Vector3();
            float total = pab + pbc + pca;
            hitPos = m_trianglePos3.position * (pab / total)
                + m_trianglePos1.position * (pbc / total)
                + m_trianglePos2.position * (pca / total);
            m_hitPos.position = hitPos;
            m_hitPos.gameObject.SetActive(true);
            return;
        }
        if (pab < 0 && pbc < 0 && pca < 0)
        {
            m_triangleShow1.material = m_materials[1];
            m_triangleShow2.material = m_materials[1];

            Vector3 hitPos = new Vector3();
            float total = pab + pbc + pca;
            hitPos = m_trianglePos3.position * (pab / total)
                + m_trianglePos1.position * (pbc / total)
                + m_trianglePos2.position * (pca / total);
            m_hitPos.position = hitPos;
            m_hitPos.gameObject.SetActive(true);
            return;
        }
        m_triangleShow1.material = m_materials[0];
        m_triangleShow2.material = m_materials[0];
        m_hitPos.gameObject.SetActive(false);
    }

    void UpdateTriangle_v2()
    {
        Vector3 ab = m_trianglePos2.position - m_trianglePos1.position;
        Vector3 ac = m_trianglePos3.position - m_trianglePos1.position;
        Vector3 qp = m_linePos1.position - m_linePos2.position;
        Vector3 ap = m_linePos1.position - m_trianglePos1.position;
        Vector3 n = Vector3.Cross(ab, ac);
        float qpn = Vector3.Dot(qp, n);
        if (qpn == 0)
        {
            m_triangleShow1.material = m_materials[0];
            m_triangleShow2.material = m_materials[0];
            m_hitPos.gameObject.SetActive(false);
            return;
        }
        float t = Vector3.Dot(ap, n) / qpn;
        if (t < 0 || t > 1)
        {
            m_triangleShow1.material = m_materials[0];
            m_triangleShow2.material = m_materials[0];
            m_hitPos.gameObject.SetActive(false);
            return;
        }
        Vector3 pqpa = Vector3.Cross(qp, ap);
        float a2 = Vector3.Dot(pqpa, ac) / qpn;
        if (a2 < 0 || a2 > 1)
        {
            m_triangleShow1.material = m_materials[0];
            m_triangleShow2.material = m_materials[0];
            m_hitPos.gameObject.SetActive(false);
            return;
        }
        float a3 = Vector3.Dot(pqpa, ab) / -qpn;
        if (a3 < 0 || a3 > 1)
        {
            m_triangleShow1.material = m_materials[0];
            m_triangleShow2.material = m_materials[0];
            m_hitPos.gameObject.SetActive(false);
            return;
        }
        float a1 = 1 - a2 - a3;

        m_triangleShow1.material = m_materials[1];
        m_triangleShow2.material = m_materials[1];

        Vector3 hitPos = new Vector3();
        /*        float total = a1 + pbc + pca;
                hitPos = m_trianglePos3.position * (pab / total)
                    + m_trianglePos1.position * (pbc / total)
                    + m_trianglePos2.position * (pca / total);*/
        hitPos = m_linePos1.position - t * qp;
        m_hitPos.position = hitPos;
        m_hitPos.gameObject.SetActive(true);
    }

    void UpdateTriangle_v3()
    {
        m_hitPos.gameObject.SetActive(false);
        //todo 分离轴
        // 4条
        // a  三角形面法线
        // bcd 三条边和线段的叉乘

        Vector3 pq = m_linePos2.position - m_linePos1.position;
        Vector3 ab = m_trianglePos2.position - m_trianglePos1.position;
        Vector3 bc = m_trianglePos3.position - m_trianglePos2.position;
        Vector3 ac = m_trianglePos3.position - m_trianglePos1.position;
        Vector3 pa = m_trianglePos1.position - m_linePos1.position;
        Vector3 pb = m_trianglePos2.position - m_linePos1.position;
        Vector3 pc = m_trianglePos3.position - m_linePos1.position;

        Vector3[] axis = new Vector3[4];
        axis[0] = Vector3.Cross(ab, ac);
        axis[1] = Vector3.Cross(pq, ab);
        axis[2] = Vector3.Cross(pq, bc);
        axis[3] = Vector3.Cross(pq, ac);

        //始终用m_linePos1.position做起点，那么他的投影永远是0
        for (int i = 0; i <= 3; i++)
        {
            float t = Vector3.Dot(pq, axis[i]);
            if (t == 0)
            {
                m_triangleShow1.material = m_materials[0];
                m_triangleShow2.material = m_materials[0];
                return;
            }

            float u2 = Vector3.Dot(pq, axis[i]);
            float v1 = Vector3.Dot(pa, axis[i]);
            float v2 = Vector3.Dot(pb, axis[i]);
            float v3 = Vector3.Dot(pc, axis[i]);

            float umin, umax;
            float vmin, vmax;
            if (u2 > 0)
            {
                umin = 0;
                umax = u2;
            }
            else
            {
                umin = u2;
                umax = 0;
            }

            if (v1 < v2)
            {
                vmin = v1;
                vmax = v2;
            }
            else
            {
                vmin = v2;
                vmax = v1;
            }

            if (vmin > v3)
            {
                vmin = v3;
            }
            else if (vmax < v3)
            {
                vmax = v3;
            }

            if (umin > vmax || vmin > umax)
            {
                m_triangleShow1.material = m_materials[0];
                m_triangleShow2.material = m_materials[0];
                return;
            }
        }
        m_triangleShow1.material = m_materials[1];
        m_triangleShow2.material = m_materials[1];
    }

    // Update is called once per frame
    void Update()
    {
        DrawTriangle();
        DrawLine();

        //UpdateTriangle();
        UpdateTriangle_v3();
    }
}
