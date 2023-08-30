using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCheckLineDir : MonoBehaviour
{
    public Transform posA;
    public Transform posB;
    public Transform posC;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 helpLine = new Vector3(0, 1, 0);
        Vector3 line_p = posB.position - posA.position;
        Vector3 line_q = posC.position - posA.position;
        Vector3 cross1 = Vector3.Cross(line_p, helpLine);
        float t1 = Vector3.Dot(cross1, line_q);

        Debug.Log($"t1 = {t1}");

    }
}
