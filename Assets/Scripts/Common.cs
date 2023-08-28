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

        public float GetClosestDistance(Vector3 point)
        {
            Vector3 c = Pos - point;
            float t = Vector3.Dot(c, Normal);

            return -t;
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

        public static bool GetClosestPoint(Vector3 p1, Vector3 q1, Vector3 p2, Vector3 q2, out Vector3 start, out Vector3 end)
        {
            Vector3 v1 = q1 - p1;
            Vector3 v2 = q2 - p2;
            float v1v2 = Vector3.Dot(v1, v2);
            float v1v1 = Vector3.Dot(v1, v1);
            float v2v2 = Vector3.Dot(v2, v2);
            Vector3 t = p2 - p1;
            float c1 = Vector3.Dot(v1, t);
            float c2 = Vector3.Dot(v2, t);

            //v1v1t1 - v1v2t2 = v1p2 - v1p1
            //v1v2t1 - v2v2t2 = v2p2 - v2p1
            float det = v1v1 * (-v2v2) - (-v1v2) * v1v2;
            if (det != 0)
            {
                float t1 = (c1 * (-v2v2) - (-v1v2) * c2) / det;
                float t2 = (c2 * v1v1 - v1v2 * c1) / det;

                if (t1 < 0)
                {
                    LineSegment l3 = new LineSegment(p2, q2);
                    end = l3.GetClosestPoint(p1);
                    start = p1;
                    return true;
                }
                if (t1 > 1)
                {
                    LineSegment l3 = new LineSegment(p2, q2);
                    end = l3.GetClosestPoint(q1);
                    start = q1;
                    return true;
                }
                if (t2 < 0)
                {
                    LineSegment l3 = new LineSegment(p1, q1);
                    end = l3.GetClosestPoint(p2);
                    start = p2;
                    return true;
                }
                if (t2 > 1)
                {
                    LineSegment l3 = new LineSegment(p1, q1);
                    end = l3.GetClosestPoint(q2);
                    start = q2;
                    return true;
                }

                start = p1 + v1 * t1;
                end = p2 + v2 * t2;
                return true;
            }
            start = new();
            end = new();
            return false;
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

        public Matrix4x4 Mat { get; set; }

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

            Vector4 a1 = new(AxisX.x, AxisX.y, AxisX.z, 0);
            Vector4 a2 = new(AxisY.x, AxisY.y, AxisY.z, 0);
            Vector4 a3 = new(AxisZ.x, AxisZ.y, AxisZ.z, 0);
            Vector4 a4 = new(0, 0, 0, 1);
            Matrix4x4 t = new(a1, a2, a3, a4);

            Mat = t.inverse;
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
        private Vector3 CalcRelativePos(Vector3 pos)
        {
            Vector4 p = new(pos.x, pos.y, pos.z, 0);
            Vector4 p2 = Mat * p;
            return new Vector3(p2.x, p2.y, p2.z);
        }
        public Vector3 GetClosestPoint(Vector3 point)
        {
            Vector3 pc = point - C;
            Mat33 mat = new(AxisX, AxisY, AxisZ);
            Vector3 obb_pos = mat.CalcLinearEquation(pc);

            Vector3 obb_pos2 = CalcRelativePos(pc);

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

    public class triangle
    {
        public Vector3 A { get; set; }
        public Vector3 B { get; set; }
        public Vector3 C { get; set; }

        public Vector3 N { get; set; }

        public triangle(Vector3 a, Vector3 b, Vector3 c)
        {
            A = a;
            B = b;
            C = c;
            N = Vector3.Cross(c, a);
            N.Normalize();
        }

        public Vector3 GetClosestPoint(Vector3 point)
        {
            float ab_split1 = Vector3.Dot(point - A, B - A);
            float ab_split2 = Vector3.Dot(point - B, A - B);
            float ca_split1 = Vector3.Dot(point - C, A - C);
            float ca_split2 = Vector3.Dot(point - A, C - A);
            if (ab_split1 < 0 && ca_split2 < 0)
            {
                return A;
            }

            float bc_split1 = Vector3.Dot(point - B, C - B);
            float bc_split2 = Vector3.Dot(point - C, B - C);
            if (ca_split1 < 0 && bc_split2 < 0)
            {
                return C;
            }
            if (ab_split2 < 0 && bc_split1 < 0)
            {
                return B;
            }

            float areaC = Vector3.Dot(N, Vector3.Cross(A - point, B - point));
            if(areaC < 0)
            {
                return A + (B - A) * (ab_split1 / (ab_split1 + ab_split2));
            }
            float areaB = Vector3.Dot(N, Vector3.Cross(C - point, A - point));
            if (areaB < 0)
            {
                return C + (A - C) * (ca_split1 / (ca_split1 + ca_split2));
            }
            float areaA = Vector3.Dot(N, Vector3.Cross(B - point, C - point));
            if (areaA < 0)
            {
                return B + (C - B) * (bc_split1 / (bc_split1 + bc_split2));
            }
            float area_total = areaA + areaB + areaC;
            return A * (areaA / area_total) + B * (areaB / area_total) + C * (areaC / area_total);
        }
    }
}

