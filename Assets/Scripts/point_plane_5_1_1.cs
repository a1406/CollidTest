using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class point_plane_5_1_1 : MonoBehaviour
{
    public Transform point;
    public Transform plane;
    LineRenderer m_lineRenderer;
    Transform m_transform;
    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!m_lineRenderer)
            m_lineRenderer = GetComponent<LineRenderer>();
        if (!m_transform)
            m_transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 start = plane.position;
        Quaternion rotation = plane.rotation;
/*        float angle = Mathf.Acos(rotation.w);*/

        Matrix4x4 rot_mat = Matrix4x4.Rotate(rotation);
        //rot_mat.SetTRS(new Vector3(0, 0, 0), rotation, new Vector3(1, 1, 1));
        Vector4 normal4 = new Vector4(0, 1, 0, 1);
        normal4 = rot_mat * normal4;
        Vector3 normal = new Vector3(normal4.x, normal4.y, normal4.z);
        //maybe not necessary
        //normal.Normalize();

        //for debug, show normal line
        Vector3 end = start + normal * 30;


        start = point.position;
        Vector3 c = start - plane.position;
        float cn = Vector3.Dot(c, normal);
        float nn = Vector3.Dot(normal, normal);
        float t = -cn / nn;

        end = start + normal * t;


        m_lineRenderer.positionCount = 2;

        m_lineRenderer.SetPosition(0, start);
        m_lineRenderer.SetPosition(1, end);

    }
}
