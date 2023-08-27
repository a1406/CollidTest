using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollidTest
{
    public class Mat33
    {
        public Vector3[] Data { get; set; }
        public Mat33(Vector3 a, Vector3 b, Vector3 c)
        {
            Data = new Vector3[3];
            Data[0] = a;
            Data[1] = b;
            Data[2] = c;
        }
        public float Det()
        {
            //a1，b2，c3
            //+b1，c2，a3
            //+c1，a2，b3
            //-a3，b2，c1
            //-b3，c2，a1
            //-c3，a2，b1
            return Data[0].x * Data[1].y * Data[2].z
                + Data[1].x * Data[2].y * Data[0].z
                + Data[2].x * Data[0].y * Data[1].z
                - Data[0].z * Data[1].y * Data[2].x
                - Data[1].z * Data[2].y * Data[0].x
                - Data[2].z * Data[0].y * Data[1].x;
        }

        public Vector3 CalcLinearEquation(Vector3 p)
        {
            float det = Det();
            Mat33 a = new(p, Data[1], Data[2]);
            Mat33 b = new(Data[0], p, Data[2]);
            Mat33 c = new(Data[0], Data[1], p);
            return new Vector3(a.Det() / det, b.Det() / det, c.Det() / det);
        }

    }
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

    public class OBB
    {
        public Vector3 C { get; set; }
        public Vector3 AxisX { get; set; }
        public Vector3 AxisY { get; set; }
        public Vector3 AxisZ { get; set; }

        public Vector3 HalfLen { get; set; }

        public OBB(Vector3 pos, Vector3 scale, Quaternion rotation)
        {
            C = pos;
            Matrix4x4 rot_mat = Matrix4x4.Rotate(rotation);
            Vector4 x = new(1, 0, 0, 1);
            Vector4 y = new(0, 1, 0, 1);
            Vector4 z = new(0, 0, 1, 1);
            x = rot_mat * x;
            y = rot_mat * y;
            z = rot_mat * z;

            AxisX = new(x.x, x.y, x.z);
            AxisX.Normalize();
            AxisY = new(y.x, y.y, y.z);
            AxisY.Normalize();
            AxisZ = new(z.x, z.y, z.z);
            AxisZ.Normalize();

            HalfLen = scale / 2;
        }
        public bool GetPoints(out Vector3[] points)
        {
            points = new Vector3[8];
            points[0] = C - AxisX * HalfLen.x + AxisY * HalfLen.y + AxisZ * HalfLen.z;
            points[1] = C - AxisX * HalfLen.x + AxisY * HalfLen.y - AxisZ * HalfLen.z;
            points[2] = C + AxisX * HalfLen.x + AxisY * HalfLen.y - AxisZ * HalfLen.z;
            points[3] = C + AxisX * HalfLen.x + AxisY * HalfLen.y + AxisZ * HalfLen.z;

            points[4] = C - AxisX * HalfLen.x - AxisY * HalfLen.y + AxisZ * HalfLen.z;
            points[5] = C - AxisX * HalfLen.x - AxisY * HalfLen.y - AxisZ * HalfLen.z;
            points[6] = C + AxisX * HalfLen.x - AxisY * HalfLen.y - AxisZ * HalfLen.z;
            points[7] = C + AxisX * HalfLen.x - AxisY * HalfLen.y + AxisZ * HalfLen.z;

            return true;
        }
        public Vector3 GetClosestPoint(Vector3 point)
        {
            Vector3 pc = point - C;
            Mat33 mat = new(AxisX, AxisY, AxisZ);
            Vector3 obb_pos = mat.CalcLinearEquation(pc);

            Vector3 Min, Max;
            Min = (HalfLen * -1);
            Max = HalfLen;

            Vector3 ret = obb_pos;
            if (ret.x < Min.x) ret.x = Min.x;
            else if (ret.x > Max.x) ret.x = Max.x;
            if (ret.y < Min.y) ret.y = Min.y;
            else if (ret.y > Max.y) ret.y = Max.y;
            if (ret.z < Min.z) ret.z = Min.z;
            else if (ret.z > Max.z) ret.z = Max.z;

            return (ret.x * AxisX + ret.y * AxisY + ret.z * AxisZ + C);
        }

    }
}

