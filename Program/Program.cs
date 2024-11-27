using System;

using Range = (int Min, int Max);
using Point = (int X, int Y);
using PointF = (float X, float Y);

namespace Lab3;

class Program
{
    static void Main(string[] args)
    {
        Point A = (0, 0);
        Point B = GetCoords();
        Point C = FindMinAreaFromPoints(A, B);

        Console.WriteLine($"Found coordinates of C: {C}");
        Console.WriteLine($"Area: {TriangleArea(A, B, C)}");

        (var rangeX, var rangeY) = GetValuesRange(B);
        Point C2 = Heron(A, B, rangeX, rangeY);
        Console.WriteLine($"Found coordinates of C2: {C2}");
        Console.WriteLine($"Area C2: {TriangleArea(A, B, C2)}");
    }

    static Point GetCoords()
    {
        Point b;

        do
        {
            Console.Write("Input coordinates of B (a b)\n> ");
            int[] input = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
            b = (input[0], input[1]);
        }
        while (b.X == 0 && b.Y == 0);

        return b;
    }

    // Метод для пошуку координат точки C з мінімальною ненульовою площею.
    static Point FindMinAreaFromPoints(Point A, Point B)
    {
        (var X, var Y) = GetValuesRange(B);

        double minArea = double.MaxValue;
        Point c = (0, 0);

        for (int x = X.Min; x <= X.Max; ++x)
        {
            for (int y = Y.Min; y <= Y.Max; ++y)
            {
                double area = TriangleArea(A, B, (x, y));

                if (!area.ApproximatelyEquals(0, 0.1) && area < minArea)
                {
                    minArea = area;
                    c = (x, y);
                }
            }
        }

        return c;
    }

    public static Point Heron(Point A, Point B, Range X, Range Y)
    {
        Point bestC = (0, 0);
        double minArea = double.PositiveInfinity;

        for (int x = X.Min; x <= X.Max; ++x)
        {
            for (int y = Y.Min; y <= Y.Max; ++y)
            {
                Point C = new(x, y);

                if (C == A) continue;

                var area = HeronArea(A, B, C);

                if (!area.ApproximatelyEquals(0, 0.1) && area < minArea)
                {
                    minArea = area;
                    bestC = C;
                }
            }
        }

        return bestC;
    }

    public static double HeronArea(Point A, Point B, Point C)
    {
        static double SideLength(Point A, Point B)
        {
            return Math.Sqrt(Math.Pow(B.X - A.X, 2) + Math.Pow(B.Y - A.Y, 2));
        }

        var AB = SideLength(A, B);
        var BC = SideLength(B, C);
        var CA = SideLength(C, A);

        var s = (AB + BC + CA) * 0.5;
        var area = Math.Sqrt(s * (s - AB) * (s - BC) * (s - CA));
        return area;
    }

    public static (Range x, Range y) GetValuesRange(Point B)
    {
        Range x = (B.X > 0) ? new(-1, B.X) : new(B.X, 1);
        Range y = (B.Y > 0) ? new(-1, B.Y) : new(B.Y, 1);
        return (x, y);
    }

    public static double TriangleArea(PointF A, PointF B, PointF C)
    {
        return 0.5f * Math.Abs(
            A.X * (B.Y - C.Y) +
            B.X * (C.Y - A.Y) +
            C.X * (A.Y - B.Y)
        );
    }
}

static class DoubleExt
{
    public static bool ApproximatelyEquals(this double lhs, double rhs, double diff = double.Epsilon)
    {
        return Math.Abs(lhs - rhs) <= diff;
    }
}
