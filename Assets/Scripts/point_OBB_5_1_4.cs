using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class point_OBB_5_1_4 : MonoBehaviour
{
    public Transform point;
    public Transform OBB;
    LineRenderer m_lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        if (!m_lineRenderer)
            m_lineRenderer = GetComponent<LineRenderer>();
        CollidTest.Mat33 test = new(new Vector3(4, 2, 1), new Vector3(5, 3, 7), new Vector3(1, 3, 2));
        float det = test.Det();
        Debug.Log($"det = {det}");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 start = point.position;
        Vector3 scale = OBB.localScale;
        CollidTest.OBB TestOBB = new CollidTest.OBB(OBB.position, scale, OBB.rotation);
        /*        Vector3[] points;
                TestOBB.GetPoints(out points);
                m_lineRenderer.positionCount = 10;

                m_lineRenderer.SetPosition(0, points[0]);
                m_lineRenderer.SetPosition(1, points[1]);
                m_lineRenderer.SetPosition(2, points[2]);
                m_lineRenderer.SetPosition(3, points[3]);
                m_lineRenderer.SetPosition(4, points[0]);

                m_lineRenderer.SetPosition(5, points[4]);

                m_lineRenderer.SetPosition(6, points[5]);
                m_lineRenderer.SetPosition(7, points[6]);
                m_lineRenderer.SetPosition(8, points[7]);
                m_lineRenderer.SetPosition(9, points[4]);*/

        Vector3 end = TestOBB.GetClosestPoint(start);
        m_lineRenderer.positionCount = 2;

        m_lineRenderer.SetPosition(0, start);
        m_lineRenderer.SetPosition(1, end);

        float length = (end - start).sqrMagnitude;
        Debug.Log($"len = {length}");


    }
}
