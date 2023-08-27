using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class line_line_5_1_8 : MonoBehaviour
{
    public Transform p1;
    public Transform q1;
    public Transform p2;
    public Transform q2;
    LineRenderer m_lineRenderer;
    LineRenderer line1;
    LineRenderer line2;
    // Start is called before the first frame update
    void Start()
    {
        if (!m_lineRenderer)
            m_lineRenderer = GetComponent<LineRenderer>();
        if (!line1)
            line1 = p1.GetComponent<LineRenderer>();
        if (!line2)
            line2 = p2.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        line1.positionCount = 2;
        line1.SetPosition(0, p1.position);
        line1.SetPosition(1, q1.position);

        line2.positionCount = 2;
        line2.SetPosition(0, p2.position);
        line2.SetPosition(1, q2.position);

        Vector3 v1 = q1.position - p1.position;
        Vector3 v2 = q2.position - p2.position;
        float v1v2 = Vector3.Dot(v1, v2);
        float v1v1 = Vector3.Dot(v1, v1);
        float v2v2 = Vector3.Dot(v2, v2);
        Vector3 t = p2.position - p1.position;
        float c1 = Vector3.Dot(v1, t);
        float c2 = Vector3.Dot(v2, t);

        //v1v1t1 - v1v2t2 = v1p2 - v1p1
        //v1v2t1 - v2v2t2 = v2p2 - v2p1
        float det = v1v1 * (-v2v2) - (-v1v2) * v1v2;
        if (det != 0)
        {
            float t1 = (c1 * (-v2v2) - (-v1v2) * c2) / det;
            float t2 = (c2 * v1v1 - v1v2 * c1) / det;

            Vector3 start = p1.position + v1 * t1;
            Vector3 end = p2.position + v2 * t2;

            m_lineRenderer.positionCount = 2;
            m_lineRenderer.SetPosition(0, start);
            m_lineRenderer.SetPosition(1, end);
        }
    }
}
