using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class point_linesegment_5_1_2 : MonoBehaviour
{
    public Transform point;
    public LineRenderer line;
    LineRenderer m_lineRenderer;
//    Transform m_transform;
    // Start is called before the first frame update
    void Start()
    {
        if (!m_lineRenderer)
            m_lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 start = point.position;
        CollidTest.LineSegment TestLine = new CollidTest.LineSegment(line.GetPosition(0), line.GetPosition(1));
        Vector3 end = TestLine.GetClosestPoint(point.position);

        m_lineRenderer.positionCount = 2;

        m_lineRenderer.SetPosition(0, start);
        m_lineRenderer.SetPosition(1, end);
    }
}
