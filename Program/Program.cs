using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

using Point = (int X, int Y);
using Range = (int Min, int Max);

namespace Lab3;

class Program
{
    // static void Main(string[] args)
    // {
    //     Point b = GetCoords();
    //     Point c = MinAreaFromPoints(b);

    //     Console.WriteLine($"Found coordinates of C: {c}");
    //     Console.WriteLine($"Area: {TriangleAreaFromCoords(b, c)}");

    //     Point c2 = Heron(b);
    //     Console.WriteLine($"Found coordinates of C2: {c2}");
    //     Console.WriteLine($"Area C2: {TriangleAreaFromCoords(b, c2)}");
    // }

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

    // Метод для пошуку координат точки C з мінімальною ненульовою площею
    static Point MinAreaFromPoints(Point b)
    {
        (var X, var Y) = GetPossibleValuesRange(b);

        double minArea = double.MaxValue;
        Point c = (0, 0);

        for (int x = X.Min; x <= X.Max; ++x)
        {
            for (int y = Y.Min; y <= Y.Max; ++y)
            {
                double area = TriangleAreaFromCoords(b, (x, y));

                if (!area.ApproximatelyEquals(0, 0.1) && area < minArea)
                {
                    minArea = area;
                    c = (x, y);
                }
            }
        }

        return c;
    }

    static Point Heron(Point b)
    {
        Range X = (-10, 10);
        Range Y = (-10, 10);

        Point bestC = (0, 0);
        double minArea = double.PositiveInfinity;

        for (int x = X.Min; x <= X.Max; ++x)
        {
            for (int y = Y.Min; y <= Y.Max; ++y)
            {
                if (x == 0 && y == 0) continue;

                var area = HeronArea((0, 0), b, (x, y));

                if (!area.ApproximatelyEquals(0, 0.1) && area < minArea)
                {
                    minArea = area;
                    bestC = (x, y);
                }
            }
        }

        return bestC;
    }

    static double HeronArea(Point a, Point b, Point c)
    {
        static double SideLength(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
        }

        var AB = SideLength(a, b);
        var BC = SideLength(b, c);
        var CA = SideLength(c, a);

        var s = (AB + BC + CA) * 0.5;
        var area = Math.Sqrt(s * (s - AB) * (s - BC) * (s - CA));
        return area;
    }

    static (Range x, Range y) GetPossibleValuesRange(Point b)
    {
        Range x = (b.X > 0) ? (-1, b.X) : (b.X, 1);
        Range y = (b.Y > 0) ? (-1, b.Y) : (b.Y, 1);
        return (x, y);
    }

    // A = (1/2) * abs(x1*(y2−y3) + x2*(y3−y1) + x3*(y1−y2))
    static double TriangleAreaFromCoords(Point b, Point c)
    {
        // return Math.Abs(a * y + x * -b) * 0.5;
        return Math.Abs(b.X * c.Y - b.Y * c.X) * 0.5;
    }
}

static class DoubleExt
{
    public static bool ApproximatelyEquals(this double lhs, double rhs, double diff = double.Epsilon)
    {
        return Math.Abs(lhs - rhs) <= diff;
    }
}

class MyForm : Form
{
    System.Drawing.Point A = new(0, 0);
    System.Drawing.Point B = new(1, 2);
    System.Drawing.Point C;

    System.Drawing.Point PrevMousePos;

    Pen APen = new Pen(Color.Blue);
    Pen BPen = new Pen(Color.Red);
    Pen CPen = new Pen(Color.Green);

    Label BPosition = new();

    bool IsMouseGrabbed = false;

    public MyForm()
    {
        InitializeComponent();

        Width = 800;
        Height = 600;
        PrevMousePos = B;
    }

    public override void Refresh()
    {
        const int circleSize = 500;

        BPosition.Text = B.ToString();
        BPosition.Refresh();

        Graphics g = CreateGraphics();
        g.DrawEllipse(APen, A.X, A.Y, circleSize, circleSize);
        g.DrawEllipse(BPen, B.X, B.Y, circleSize, circleSize);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            Console.WriteLine("Grabbed");
            IsMouseGrabbed = true;
        }
        base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            Console.WriteLine("End");
            IsMouseGrabbed = false;
        }
        base.OnMouseUp(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (IsMouseGrabbed)
        {
            Console.WriteLine($"({PrevMousePos.X - e.X}, {PrevMousePos.Y - e.Y})");
            B.X += PrevMousePos.X - e.X;
            B.Y += PrevMousePos.Y - e.Y;
            PrevMousePos.X = e.X;
            PrevMousePos.Y = e.Y;
        }
        base.OnMouseMove(e);
    }

    private void InitializeComponent()
    {
        // this.components = new System.ComponentModel.Container();
        // System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
        // System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
        // this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
        // ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
        SuspendLayout();

        // chart1

        // chartArea1.Name = "ChartArea1";
        // this.chart1.ChartAreas.Add(chartArea1);
        // this.chart1.Dock = DockStyle.Fill;
        // legend1.Name = "Legend1";
        // this.chart1.Legends.Add(legend1);
        // this.chart1.Location = new System.Drawing.Point(0, 50);
        // this.chart1.Name = "chart1";
        // // this.chart1.Size = new System.Drawing.Size(284, 212);
        // this.chart1.TabIndex = 0;
        // this.chart1.Text = "chart1";

        // Form1

        // this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        // this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        // this.ClientSize = new System.Drawing.Size(284, 262);
        // this.Controls.Add(this.chart1);
        // this.Name = "Form1";
        // this.Text = "FakeChart";
        // this.Load += new System.EventHandler(this.Form1_Load);
        // ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
        ResumeLayout(false);
    }

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MyForm());
    }
}
