using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sphere_plane_5_2_2 : MonoBehaviour
{
    public Transform cube;
    public Transform sphere;
    public Transform plane;
    public Material[] sphere_materials;
    MeshRenderer sphere_render;
    MeshRenderer cube_render;

    // Start is called before the first frame update
    void Start()
    {
        sphere_render = sphere.GetComponent<MeshRenderer>();
        cube_render = cube.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        CollidTest.Plane Testplane = new CollidTest.Plane(plane.position, plane.rotation);
        float distance = Testplane.GetClosestDistance(sphere.position);
        if (distance > 0.5)
        {
            sphere_render.material = sphere_materials[0];
        }
        else
        {
            sphere_render.material = sphere_materials[1];
        }

        Vector3 scale = cube.localScale;
        CollidTest.OBB TestOBB = new CollidTest.OBB(cube.position, scale, cube.rotation);
        set_cube_material_ver2(Testplane, TestOBB);
    }

    void set_cube_material_ver1(CollidTest.Plane Testplane, CollidTest.OBB TestOBB)
    {
        Vector3 hit_point = Testplane.GetClosestPoint(cube.position);
        Vector3 separate_axis = Testplane.Normal;

        Vector3[] points;
        TestOBB.GetPoints(out points);

        float max_distance = Vector3.Dot(cube.position - hit_point, cube.position - hit_point);
        foreach (Vector3 point in points)
        {
            Vector3 v = point - cube.position;
            float dist = Mathf.Abs(Vector3.Dot(v, separate_axis));
            if (dist * dist >= max_distance)
            {
                cube_render.material = sphere_materials[1];
                return;
            }
        }
        cube_render.material = sphere_materials[0];
    }

    void set_cube_material_ver2(CollidTest.Plane Testplane, CollidTest.OBB TestOBB)
    {
        float distance = Testplane.GetClosestDistance(cube.position);
        float r = Mathf.Abs(Vector3.Dot(TestOBB.AxisX, Testplane.Normal)) * TestOBB.HalfLen.x
            + Mathf.Abs(Vector3.Dot(TestOBB.AxisY, Testplane.Normal)) * TestOBB.HalfLen.y
            + Mathf.Abs(Vector3.Dot(TestOBB.AxisZ, Testplane.Normal)) * TestOBB.HalfLen.z;
        if (r >= distance)
        {
            cube_render.material = sphere_materials[1];
        }
        else
        {
            cube_render.material = sphere_materials[0];
        }
    }
}
