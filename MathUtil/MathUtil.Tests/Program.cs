// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathUtil.Tests
{
    public interface IShape
    {
        Vector2[] Vertices { get; set; }
    }

    public class Polygon : IShape
    { 
        public Vector2[] Vertices { get; set; }
    }

    class Program
    {
        static int BenchSize = 100 * 1000 * 1000;

        static void Main(string[] args)
        {
            //TestVectors();
            TestSat();
            Console.ReadLine();
        }

        static void TestSat()
        {
            var sat = new SeparatingAxisTheorem();

            // shape1
            var shape1 = new Polygon();
            shape1.Vertices = new Vector2[4];
            shape1.Vertices[0] = new Vector2(0.0, 0.0);
            shape1.Vertices[1] = new Vector2(10.0, 0.0);
            shape1.Vertices[2] = new Vector2(10.0, 10.0);
            shape1.Vertices[3] = new Vector2(0.0, 10.0);

            // shape2
            var shape2 = new Polygon();
            shape2.Vertices = new Vector2[4];

            // overlaps
            //shape2.Vertices[0] = new Vector2(5.0, 5.0);
            //shape2.Vertices[1] = new Vector2(20.0, 5.0);
            //shape2.Vertices[2] = new Vector2(20.0, 20.0);
            //shape2.Vertices[3] = new Vector2(5.0, 20.0);

            // do not overlaps
            //shape2.Vertices[0] = new Vector2(15.0, 5.0);
            //shape2.Vertices[1] = new Vector2(20.0, 5.0);
            //shape2.Vertices[2] = new Vector2(20.0, 20.0);
            //shape2.Vertices[3] = new Vector2(15.0, 20.0);

            // shape1 contains shape2
            shape2.Vertices[0] = new Vector2(2.0, 2.0);
            shape2.Vertices[1] = new Vector2(8.0, 2.0);
            shape2.Vertices[2] = new Vector2(8.0, 8.0);
            shape2.Vertices[3] = new Vector2(2.0, 8.0);

            // sat get axes
            //var axes1 = sat.GetAxes(shape1.Vertices);
            //var axes2 = sat.GetAxes(shape2.Vertices);
            //Console.WriteLine("axes1:");
            //foreach(var axis in axes1) Console.WriteLine(axis);
            //Console.WriteLine("axes2:");
            //foreach(var axis in axes2) Console.WriteLine(axis);

            // sat overlap
            bool overlap = sat.Overlap(shape1.Vertices, shape2.Vertices);
            Console.WriteLine("overlap: " + overlap);

            //TestSatProjectionGetOverlap();

            // sat mtv
            MinimumTranslationVector? mtv;
            bool overlapMTV = sat.MinimumTranslationVector(
                shape1.Vertices, 
                shape2.Vertices, 
                out mtv);
            Console.WriteLine("overlapMTV: " + overlapMTV);
            if (mtv.HasValue)
            {
                Console.WriteLine("mtv.overlap: " + mtv.Value.Overlap);
                Console.WriteLine("mtv.smallest: " + mtv.Value.Smallest);
            }

            // sat mtv with containment
            MinimumTranslationVector? mtvContainment;
            bool overlapMTVContainment = sat.MinimumTranslationVectorWithContainment(
                shape1.Vertices, 
                shape2.Vertices, 
                out mtvContainment);
            Console.WriteLine("overlapMTVContainment: " + overlapMTVContainment);
            if (mtv.HasValue)
            {
                Console.WriteLine("mtvContainment.overlap: " + mtvContainment.Value.Overlap);
                Console.WriteLine("mtvContainment.smallest: " + mtvContainment.Value.Smallest);
            }
        }

        static void TestSatProjectionGetOverlap()
        {
            var ps = new Projection[10];
            ps[0] = new Projection(0, 3);
            ps[1] = new Projection(2, 30);
            ps[2] = new Projection(4, 20);
            ps[3] = new Projection(-1, 7);
            ps[4] = new Projection(1, 10);
            ps[5] = new Projection(2, 4);
            ps[6] = new Projection(0, 3);
            ps[7] = new Projection(3, 5);
            ps[8] = new Projection(-2, 6);
            ps[9] = new Projection(-1, 10);

            Console.WriteLine(ps[0].GetOverlap(ps[1]) == 1);
            Console.WriteLine(ps[2].GetOverlap(ps[3]) == 3);
            Console.WriteLine(ps[4].GetOverlap(ps[5]) == 2);
            Console.WriteLine(ps[6].GetOverlap(ps[7]) == 0);
            Console.WriteLine(ps[8].GetOverlap(ps[9]) == 7);
        }

        static void TestVectors()
        {
            TestSubtract();
            TestAdd();
            TestMultiply();
            BenchMultiply();
            TestDivide();
            TestDot();
            BenchDot();
            TestCross();
            TestLength();
            TestLengthSquared();
            TestNormalize();
            TestProject();
            BenchProject();
            TestReflect();
            TestAngle();
            TestLerp();
            TestSlerp();
            BenchSlerp();
            TestNlerp();
            BenchNlerp();
            TestDistance();
            TestMiddle();
            TestNearestPointOnLine();
        }

        static void TestSubtract()
        {
            {
                var v1 = new Vector2(1.0, 1.0);
                var v2 = new Vector2(1.0, 2.0);
                var subtract = v1.Subtract(v2);
                Console.WriteLine("Subtract: " + subtract);
            }

            {
                var v1 = new Vector2(1.0, 1.0);
                var v2 = new Vector2(1.0, 2.0);
                var subtract = v1 - v2;
                Console.WriteLine("Subtract: " + subtract);
            }
        }

        static void TestAdd()
        {
            {
                var v1 = new Vector2(1.0, 1.0);
                var v2 = new Vector2(1.0, 2.0);
                var add = v1.Add(v2);
                Console.WriteLine("Add: " + add);
            }

            {
                var v1 = new Vector2(1.0, 1.0);
                var v2 = new Vector2(1.0, 2.0);
                var add = v1 + v2;
                Console.WriteLine("Add: " + add);
            }
        }

        static void TestMultiply()
        {
            {
                var v1 = new Vector2(1.0, 1.0);
                var v2 = v1.Multiply(2.0);
                Console.WriteLine("Multiply: " + v2);
            }

            {
                var v1 = new Vector2(1.0, 1.0);
                var v2 = v1 * 2.0;
                Console.WriteLine("Multiply: " + v2);
            }
        }

        static void BenchMultiply()
        {
            var a = new Vector2[BenchSize];
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < BenchSize; i++)
            {
                var v1 = new Vector2(1.0, 1.0);
                var v2 = v1.Multiply(2.0);
                a[i] = v2;
            }
            sw.Stop();
            Console.WriteLine("Multiply: " + sw.Elapsed.TotalMilliseconds + "ms");
        }

        static void TestDivide()
        {
            {
                var v1 = new Vector2(1.0, 1.0);
                var v2 = v1.Divide(2.0);
                Console.WriteLine("Divide: " + v2);
            }

            {
                var v1 = new Vector2(1.0, 1.0);
                var v2 = v1 / 2.0;
                Console.WriteLine("Divide: " + v2);
            }
        }

        static void TestDot()
        {
            var v1 = new Vector2(1.0, 1.0);
            var v2 = new Vector2(1.0, 2.0);
            double dot = v1.Dot(v2);
            Console.WriteLine("Dot: " + dot);
        }

        static void BenchDot()
        {
            var a = new double[BenchSize];
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < BenchSize; i++)
            {
                var v1 = new Vector2(1.0, 1.0);
                var v2 = new Vector2(1.0, 2.0);
                double dot = v1.Dot(v2);
                a[i] = dot;
            }
            sw.Stop();
            Console.WriteLine("Dot: " + sw.Elapsed.TotalMilliseconds + "ms");
        }

        static void TestCross()
        {
            var v1 = new Vector2(1.0, 1.0);
            var v2 = new Vector2(1.0, 2.0);
            double dot = v1.Cross(v2);
            Console.WriteLine("Cross: " + dot);
        }

        static void TestLength()
        {
            var v = new Vector2(10.0, 0.0);
            var length = v.Length();
            Console.WriteLine("Length: " + length);
        }

        static void TestLengthSquared()
        {
            var v = new Vector2(10.0, 0.0);
            var lengthSquared = v.LengthSquared();
            Console.WriteLine("LengthSquared: " + lengthSquared);
        }

        static void TestNormalize()
        {
            var v = new Vector2(10.0, 0.0);
            var normalize = v.Normalize();
            Console.WriteLine("Normalize: " + normalize);
        }

        static void TestProject()
        {
            var v1 = new Vector2(1.0, 1.0);
            var v2 = new Vector2(1.0, 2.0);
            var project = v1.Project(v2);
            Console.WriteLine("Project: " + project);
        }

        static void BenchProject()
        {
            var a = new Vector2[BenchSize];
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < BenchSize; i++)
            {
                var v1 = new Vector2(1.0, 1.0);
                var v2 = new Vector2(1.0, 2.0);
                var project = v1.Project(v2);
                a[i] = project;
            }
            sw.Stop();
            Console.WriteLine("Project: " + sw.Elapsed.TotalMilliseconds + "ms");
        }

        static void TestReflect()
        {
            var v1 = new Vector2(1.0, 0.0);
            var v2 = new Vector2(1.0, 0.0);
            var reflect = v1.Reflect(v2);
            Console.WriteLine("Reflect: " + reflect);
        }

        private static void TestAngle()
        {
            var v1 = new Vector2(1.0, 0.0);
            var v2 = new Vector2(0.0, 1.0);
            Console.WriteLine("Acos: " + v1.Angle(v2) * Vector2.RadiansToDegrees);
        }

        static void TestLerp()
        {
            var v1 = new Vector2(1.0, 1.0);
            var v2 = new Vector2(1.0, 2.0);
            var lerp = v1.Lerp(v2, 0.2);
            Console.WriteLine("Lerp: " + lerp);
        }

        static void TestSlerp()
        {
            var v1 = new Vector2(1.0, 0.0);
            var v2 = new Vector2(0.0, 1.0);
            var slerp = v1.Slerp(v2, 0.5);
            Console.WriteLine("Slerp: " + slerp);
        }

        static void BenchSlerp()
        {
            var a = new Vector2[BenchSize];
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < BenchSize; i++)
            {
                var v1 = new Vector2(1.0, 1.0);
                var v2 = new Vector2(1.0, 2.0);
                var slerp = v1.Slerp(v2, 0.5);
                a[i] = slerp;
            }
            sw.Stop();
            Console.WriteLine("Slerp: " + sw.Elapsed.TotalMilliseconds + "ms");
        }

        static void TestNlerp()
        {
            var v1 = new Vector2(1.0, 0.0);
            var v2 = new Vector2(0.0, 1.0);
            var nlerp = v1.Nlerp(v2, 0.5);
            Console.WriteLine("Nlerp: " + nlerp);
        }

        static void BenchNlerp()
        {
            var a = new Vector2[BenchSize];
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < BenchSize; i++)
            {
                var v1 = new Vector2(1.0, 1.0);
                var v2 = new Vector2(1.0, 2.0);
                var nlerp = v1.Nlerp(v2, 0.5);
                a[i] = nlerp;
            }
            sw.Stop();
            Console.WriteLine("Nlerp: " + sw.Elapsed.TotalMilliseconds + "ms");
        }

        static void TestDistance()
        {
            var a = new Vector2(100.0, 100.0);
            var b = new Vector2(100.0, 200.0);
            var distance = a.Distance(b);
            Console.WriteLine("Distance: " + distance);
        }

        static void TestMiddle()
        {
            var a = new Vector2(100.0, 100.0);
            var b = new Vector2(100.0, 200.0);
            var middle = a.Middle(b);
            Console.WriteLine("Middle: " + middle);
        }

        static void TestNearestPointOnLine()
        {
            var a = new Vector2(100.0, 100.0);
            var b = new Vector2(100.0, 200.0);
            var p = new Vector2(50.0, 150.0);
            var nearest = p.NearestPointOnLine(a, b);
            Console.WriteLine("NearestPointOnLine: " + nearest);
        }
    }
}
