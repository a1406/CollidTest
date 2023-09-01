using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GJK_2d : MonoBehaviour
{
    public LineRenderer lineObj1;
    public LineRenderer lineObj2;
    public LineRenderer Simplex;
    public MeshRenderer result;
    public Material[] m_materials;

    public Transform[] m_objs1;
    public Transform[] m_objs2;
    public bool showCube;

    List<GameObject> m_cubes;

    //   Vector3 startPos1, startPos2;

    Vector3 calcSimplexPos(LineRenderer obj1, LineRenderer obj2, Vector3 v)
    {
        float max_dot1 = float.MinValue;
        int max_index1 = 0;
        float max_dot2 = float.MinValue;
        int max_index2 = 0;
        for (int i = 0; i < obj2.positionCount; ++i)
        {
            Vector3 p = obj2.GetPosition(i);
            float dot = Vector3.Dot(p, v);
            if (dot > max_dot2)
            {
                max_dot2 = dot;
                max_index2 = i;
            }
        }

        v = -v;
        for (int i = 0; i < obj1.positionCount; ++i)
        {
            Vector3 p = obj1.GetPosition(i);
            float dot = Vector3.Dot(p, v);
            if (dot > max_dot1)
            {
                max_dot1 = dot;
                max_index1 = i;
            }
        }
        return lineObj2.GetPosition(max_index2) - lineObj1.GetPosition(max_index1);
    }

    //判断0，0是否在三角形内，先这样凑合用
    bool checkIncludeZero(Vector3[] l)
    {
        Vector3 B = l[1] - l[0];
        Vector3 A = -l[0];
        float t1 = A.x * B.y - A.y * B.x;

        B = l[2] - l[1];
        A = -l[1];
        float t2 = A.x * B.y - A.y * B.x;
        if (t1 * t2 <= 0)
            return false;

        B = l[0] - l[2];
        A = -l[2];
        t2 = A.x * B.y - A.y * B.x;
        if (t1 * t2 <= 0)
            return false;
        return true;
    }

    Vector3 GetClosestPoint(Vector3 start, Vector3 end)
    {
        Vector3 dir = (end - start).normalized;
        Vector3 p1 = -start;
        float len = Vector3.Dot(p1, dir);
        return start + dir * len;
    }

    private Vector3 calcNextV(Vector3[] line1)
    {
        CollidTest.Line []l = new CollidTest.Line[]
        {
            new(line1[0], line1[1]),
            new(line1[1], line1[2]),
            new(line1[2], line1[0])
        };
        Vector3[] hitPos = new Vector3[3];
        hitPos[0] = l[0].GetClosestPoint(Vector3.zero);
        hitPos[1] = l[1].GetClosestPoint(Vector3.zero);
        hitPos[2] = l[2].GetClosestPoint(Vector3.zero);
        float[] dist = new float[]
        {
            Vector3.Dot(hitPos[0], hitPos[0]),
            Vector3.Dot(hitPos[1], hitPos[1]),
            Vector3.Dot(hitPos[2], hitPos[2]),
        };

        int index = 0;
        if (dist[1] < dist[index])
            index = 1;
        if (dist[2] < dist[index])
            index = 2;
        line1[0] = l[index].Start;
        line1[0] = l[index].End;
        return -hitPos[index];
    }

    void initObjs3()
    {
        lineObj1.positionCount = m_objs1.Length;
        lineObj2.positionCount = m_objs2.Length;

        for (int i = 0; i < lineObj1.positionCount; ++i)
        {
            lineObj1.SetPosition(i, m_objs1[i].position);
        }
        for (int i = 0; i < lineObj2.positionCount; ++i)
        {
            lineObj2.SetPosition(i, m_objs2[i].position);
        }
    }

    void checkOverlap()
    {
        result.material = m_materials[0];
        Simplex.positionCount = 3;
        Vector3[] line1 = new Vector3[3];
        //Vector3 v = startPos2 - startPos1;
        Vector3 v = new(1, 0, 0);
        line1[0] = calcSimplexPos(lineObj1, lineObj2, v);
        line1[1] = calcSimplexPos(lineObj1, lineObj2, -v);
        Vector3 t = line1[1] - line1[0];
        if (t.x * line1[0].y - line1[0].x * t.y > 0)
        {
            v = new(t.y, -t.x);
        }
        else
        {
            v = new(-t.y, t.x);
        }

        line1[2] = calcSimplexPos(lineObj1, lineObj2, v);
        Debug.Log($"0: {line1[0]}, {line1[1]}, {line1[2]}, {v}");
        for (int i = 0; i < 10; i++)
        {
            if (checkIncludeZero(line1))
            {
                result.material = m_materials[1];
                break;
            }
            v = calcNextV(line1);
            line1[2] = calcSimplexPos(lineObj1, lineObj2, v);
            Debug.Log($"{i + 1}: {line1[0]}, {line1[1]}, {line1[2]}, {v}, {Vector3.Dot(line1[2], v)}");
            if (Vector3.Dot(line1[2], v) < 0)
            {
                Debug.Log("out, not overlap");
                break;
            }
                
        }
        Simplex.SetPositions(line1);
    }

    void addCube(Vector3 pos)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = pos;
        cube.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        m_cubes.Add(cube);
    }

    void showDebugCube()
    {
        foreach (GameObject cube in m_cubes)
        {
            Destroy(cube);
        }
        m_cubes.Clear();
        if (!showCube)
            return;
        for (int i = 0; i < m_objs1.Length; i++)
        {
            for (int j = 0; j < m_objs2.Length; j++)
            {
                Vector3 pos = m_objs2[j].position - m_objs1[i].position;
                addCube(pos);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //initObjs1();
        //initObjs2();
        initObjs3();
        checkOverlap();

        m_cubes = new();

/*        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new(10, 10, 0);
        Destroy(cube);*/
    }

    // Update is called once per frame
    void Update()
    {
        initObjs3();
        checkOverlap();
        showDebugCube();
    }
}
