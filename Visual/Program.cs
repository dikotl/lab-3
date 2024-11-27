namespace Visual;

using Range = (int Min, int Max);

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }

    public static Point Heron(Point A, Point B, Range X, Range Y)
    {
        Point bestC = Point.Empty;
        double minArea = double.PositiveInfinity;

        for (int x = X.Min; x <= X.Max; ++x)
        {
            for (int y = Y.Min; y <= Y.Max; ++y)
            {
                Point C = new(x, y);

                if (C == A) continue;

                var area = HeronArea(A, B, C);

                if (!area.AlmostEquals(0, 0.1) && area < minArea)
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

    public static (Range x, Range y) GetPossibleValueRanges(Point A, Point B)
    {
        Range x = (B.X > 0) ? new(-1, B.X) : new(B.X, 1);
        Range y = (B.Y > 0) ? new(-1, B.Y) : new(B.Y, 1);
        // Range x = new(-10, 10);
        // Range y = new(-10, 10);
        return (x, y);
    }

    // A = (1/2) * abs(x1*(y2−y3) + x2*(y3−y1) + x3*(y1−y2))
    public static double TriangleArea(PointF A, PointF B, PointF C)
    {
        // return Math.Abs(B.X * C.Y - B.Y * C.X) * 0.5;
        return 0.5f * Math.Abs(
            A.X * (B.Y - C.Y) +
            B.X * (C.Y - A.Y) +
            C.X * (A.Y - B.Y)
        );
    }
}

static class DoubleExt
{
    public static bool AlmostEquals(this double lhs, double rhs, double diff = double.Epsilon)
    {
        return Math.Abs(lhs - rhs) <= diff;
    }
}
