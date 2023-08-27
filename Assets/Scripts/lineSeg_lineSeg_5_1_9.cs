using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lineSeg_lineSeg_5_1_9 : MonoBehaviour
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

        Vector3 start, end;
        if (CollidTest.LineSegment.GetClosestPoint(p1.position, q1.position, p2.position, q2.position, out start, out end))
        {
            m_lineRenderer.positionCount = 2;
            m_lineRenderer.SetPosition(0, start);
            m_lineRenderer.SetPosition(1, end);
        }
        else
        {
            m_lineRenderer.positionCount = 0;
        }
    }
}
