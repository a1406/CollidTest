using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class line_capsule_5_3_7 : MonoBehaviour
{
    public Transform m_linePos1;
    public Transform m_linePos2;
    public Transform m_capsule;
    public Transform m_hitPos;
    public Material[] m_materials;

    const float m_hight = 2.0f;
    const float m_radius = 0.5f;

    MeshRenderer m_capsuleShow;
    LineRenderer line_render;
    // Start is called before the first frame update
    void Start()
    {
        m_capsuleShow = m_capsule.GetComponent<MeshRenderer>();
        line_render = GetComponent<LineRenderer>();
    }

    void DrawLine()
    {
        line_render.positionCount = 2;

        line_render.SetPosition(0, m_linePos1.position);
        line_render.SetPosition(1, m_linePos2.position);
    }

    bool checkSphere(float radius, Vector3 pos, CollidTest.LineSegment lineSeg, out Vector3 hitPos)
    {
        Vector3 D = lineSeg.Start - pos;
        Vector3 V = lineSeg.End - lineSeg.Start;
        //ax*x + bx + c = 0
        float a = Vector3.Dot(V, V);
        float b = 2 * Vector3.Dot(D, V);
        float c = Vector3.Dot(D, D) - radius * radius;
        if (a == 0)
        {
            hitPos = new();
            return false;
        }

        float cc = b * b - 4 * a * c;
        if (cc < 0)
         {
            hitPos = new();
            return false;
        }
        cc = Mathf.Sqrt(cc);

        float x1 = (-b + cc) / 2 / a;
        float x2 = (-b - cc) / 2 / a;
        if (x1 > x2)
            x1 = x2;

        if (x1 >= 0 && x1 <= 1)
        {
            hitPos = lineSeg.Start + (lineSeg.End - lineSeg.Start) * x1;
            return true;
        }

        hitPos = new();
        return false;
    }

    bool checkCylinder(CollidTest.Capsule capsule, CollidTest.LineSegment lineSeg, out Vector3 hitPos)
    {
        Vector3 d = new Vector3(0, 1, 0);
        Vector3 m = lineSeg.Start;
        Vector3 n = lineSeg.End - lineSeg.Start;
        float nd = Vector3.Dot(n, d);
        float md = Vector3.Dot(m, d);
        float a = Vector3.Dot(n, n) - nd * nd;
        float b = Vector3.Dot(m, n) - nd * md;
        float c = Vector3.Dot(m, m) - capsule.HalfLen * capsule.HalfLen - md * md;

        if (Mathf.Abs(a) < float.Epsilon)
        {
            hitPos = new();
            return false;
        }
        float t = b * b - a * c;
        if (t < 0)
        {
            hitPos = new();
            return false;
        }
        t = Mathf.Sqrt(t);

        float hitTime = (-b + t) / a;
        float hitTime2 = (-b - t) / a;
        if (hitTime > hitTime2)
            hitTime = hitTime2;

        if (hitTime < 0 || hitTime > 1)
        {
            hitPos = new();
            return false;
        }

        hitPos = lineSeg.Start + n * hitTime;

        Vector3 B = hitPos;
        float len = Vector3.Dot(B, d);
        if (len < 0 || len > capsule.HalfLen * 2)
        {
            hitPos = new();
            return false;
        }

        /*        m_capsuleShow.material = m_materials[1];
                m_hitPos.position = capsule.Mat.MultiplyPoint3x4(hitPos);
                m_hitPos.gameObject.SetActive(true);*/
        return true;
    }


    void UpdateCapsule()
    {
        CollidTest.Capsule capsule = new(m_capsule.position, m_radius, m_hight, m_capsule.rotation);
        Vector3 pos1 = capsule.InverseMat.MultiplyPoint3x4(m_linePos1.position);
        Vector3 pos2 = capsule.InverseMat.MultiplyPoint3x4(m_linePos2.position);
        CollidTest.LineSegment line = new(pos1, pos2);
        Vector3 hitPos;
        Vector3 spherePos = Vector3.zero;

        if (checkCylinder(capsule, line, out hitPos))
        {
            m_capsuleShow.material = m_materials[1];
            m_hitPos.position = capsule.Mat.MultiplyPoint3x4(hitPos);
            m_hitPos.gameObject.SetActive(true);
            return;
        }

        if (checkSphere(m_radius, spherePos, line, out hitPos) && hitPos.y <= spherePos.y)
        {
            m_capsuleShow.material = m_materials[1];
            m_hitPos.position = capsule.Mat.MultiplyPoint3x4(hitPos);
            m_hitPos.gameObject.SetActive(true);
            return;
        }
        spherePos.y = m_hight / 2;
        if (checkSphere(m_radius, spherePos, line, out hitPos) && hitPos.y >= spherePos.y)
        {
            m_capsuleShow.material = m_materials[1];
            m_hitPos.position = capsule.Mat.MultiplyPoint3x4(hitPos);
            m_hitPos.gameObject.SetActive(true);
            return;
        }

        m_capsuleShow.material = m_materials[0];
        m_hitPos.gameObject.SetActive(false);
        return;


/*        Vector3 d = new Vector3(0, 1, 0);
        Vector3 m = pos1;
        Vector3 n = pos2 - pos1;
        float nd = Vector3.Dot(n, d);
        float md = Vector3.Dot(m, d);
        float a = Vector3.Dot(n, n) - nd * nd;
        float b = Vector3.Dot(m, n) - nd * md;
        float c = Vector3.Dot(m, m) - capsule.HalfLen * capsule.HalfLen - md * md;

        if (Mathf.Abs(a) < float.Epsilon)
        {
            m_capsuleShow.material = m_materials[0];
            m_hitPos.gameObject.SetActive(false);
            return;
        }
        float t = b * b - a * c;
        if (t < 0)
        {
            m_capsuleShow.material = m_materials[0];
            m_hitPos.gameObject.SetActive(false);
            return;
        }
        t = Mathf.Sqrt(t);

        float hitTime = (-b + t) / a;
        float hitTime2 = (-b - t) / a;
        if (hitTime > hitTime2)
            hitTime = hitTime2;

        if (hitTime < 0 || hitTime > 1)
        {
            m_capsuleShow.material = m_materials[0];
            m_hitPos.gameObject.SetActive(false);
            return;
        }

        hitPos = pos1 + n * hitTime;

        Vector3 B = hitPos;
        float len = Vector3.Dot(B, d);
        if (len < 0 || len > capsule.HalfLen * 2)
        {
            m_capsuleShow.material = m_materials[0];
            m_hitPos.gameObject.SetActive(false);
            return;
        }

        m_capsuleShow.material = m_materials[1];
        m_hitPos.position = capsule.Mat.MultiplyPoint3x4(hitPos);
        m_hitPos.gameObject.SetActive(true);*/
    }

    // Update is called once per frame
    void Update()
    {
        DrawLine();

        UpdateCapsule();
    }
}
