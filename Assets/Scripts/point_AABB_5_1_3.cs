using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class point_AABB_5_1_3 : MonoBehaviour
{
    public Transform point;
    public Transform AABB;
    LineRenderer m_lineRenderer;
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
        Vector3 scale = AABB.localScale;
        CollidTest.AABB TestAABB = new CollidTest.AABB(AABB.position - scale / 2, AABB.position + scale / 2);
        Vector3 end = TestAABB.GetClosestPoint(start);
        m_lineRenderer.positionCount = 2;

        m_lineRenderer.SetPosition(0, start);
        m_lineRenderer.SetPosition(1, end);

        float dist = TestAABB.GetSqDistance(start);
        dist = Mathf.Sqrt(dist);
        Debug.Log($"dist = {dist}");
    }
}
