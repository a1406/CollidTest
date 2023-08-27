using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollidTest
{
    public class Plane
    {
        public Vector3 Pos { get; set; }
        public Vector3 Normal { get; set; }
        public Plane(Vector3 pos, Quaternion rotation)
        {
            Pos = pos;
            Matrix4x4 rot_mat = Matrix4x4.Rotate(rotation);
            Vector4 normal4 = new(0, 1, 0, 1);
            normal4 = rot_mat * normal4;
            Normal = new(normal4.x, normal4.y, normal4.z);
            Normal.Normalize();
        }

        public Vector3 GetClosestPoint(Vector3 point)
        {
            Vector3 c = Pos - point;
            float t = Vector3.Dot(c, Normal);

            return point + Normal * t;
        }
    }

    public class LineSegment
    {
        public Vector3 Start { get; set; }
        public Vector3 End { get; set; }
        public Vector3 Dir { get; set; }

        public LineSegment(Vector3 pos1, Vector3 pos2)
        {
            Start = pos1;
            End = pos2;
            Dir = (End - Start).normalized;
        }

        public Vector3 GetClosestPoint(Vector3 point)
        {
            Vector3 p1 = point - Start;
            float len = Vector3.Dot(p1, Dir);
            if (len < 0)
                return Start;

            if (len * len > Vector3.Dot(End - Start, End - Start))
                return End;
            return Start + Dir * len;
        }


    }

    public class AABB
    {
        public Vector3 Min { get; set; }
        public Vector3 Max { get; set; }

        public AABB(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }
        public Vector3 GetClosestPoint(Vector3 point)
        {
            Vector3 ret = point;
            if (point.x < Min.x) ret.x = Min.x;
            else if (point.x > Max.x) ret.x = Max.x;
            if (point.y < Min.y) ret.y = Min.y;
            else if (point.y > Max.y) ret.y = Max.y;
            if (point.z < Min.z) ret.z = Min.z;
            else if (point.z > Max.z) ret.z = Max.z;
            return ret;
        }
        public float GetSqDistance(Vector3 point)
        {
            Vector3 t = GetClosestPoint(point) - point;
            return Vector3.Dot(t, t);
        }
    }
}

