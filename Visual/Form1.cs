namespace Visual;

public partial class Form1 : Form
{
    PointF A = new(0, 0);
    PointF B = new(1, 2);

    PointF PrevMousePos;
    PointF PrevMousePosScreen;
    SizeF CenterOffset;
    SizeF ScreenOffset;

    readonly Brush APen = new SolidBrush(Color.Blue);
    readonly Brush BPen = new SolidBrush(Color.Red);
    readonly Brush CPen = new SolidBrush(Color.Green);
    readonly Brush ScanAreaBrush = new SolidBrush(Color.LightBlue);
    readonly Pen LinePen = new(Color.DarkGray);
    readonly Size CircleRadius = new(5, 5);

    MouseMoveMode MoveMode = MouseMoveMode.NotMove;

    bool IsDebug = false;
    bool IsSimpleScanRange = false;
    bool DrawScanRange = false;

    float CanvasScaleModifier = 15;

    enum MouseMoveMode
    {
        NotMove,
        MovePoint,
        MoveScreen,
    }

    public Form1()
    {
        FixFlickering();
        InitializeComponent();

        PrevMousePos = B;
        BackColor = Color.White;
        LinePen.Width = 2;
    }

    private void FixFlickering()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.DoubleBuffer,
            true
        );
    }

    protected override void OnResize(EventArgs e)
    {
        CenterOffset = new(Width / 2, Height / 2);

        Invalidate();
        base.OnResize(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var A = new Point((int)this.A.X, (int)this.A.Y);
        var B = new Point((int)this.B.X, (int)this.B.Y);

        (int, int) xRange, yRange;

        if (IsSimpleScanRange)
        {
            const int range = 100;
            xRange = (-range, range);
            yRange = (-range, range);
        }
        else
        {
            xRange = (this.B.X > 0) ? (-1, B.X) : (B.X, 1);
            yRange = (this.B.Y > 0) ? (-1, B.Y) : (B.Y, 1);
        }

        var C = Program.Heron(A, B, xRange, yRange);
        var g = e.Graphics;

        if (DrawScanRange)
        {
            Point topLeft = new(xRange.Item1, yRange.Item1);
            Point bottomRight = new(xRange.Item2, yRange.Item2);
            g.FillRectangle(ScanAreaBrush, CanvasRectangle(topLeft, bottomRight));
        }

        g.DrawLine(LinePen, CanvasPoint(A), CanvasPoint(B));

        if (C != Point.Empty)
        {
            g.DrawLine(LinePen, CanvasPoint(A), CanvasPoint(C));
            g.DrawLine(LinePen, CanvasPoint(B), CanvasPoint(C));

            g.FillEllipse(CPen, CanvasCircle(C, CircleRadius));
        }

        g.FillEllipse(BPen, CanvasCircle(B, CircleRadius));
        g.FillEllipse(APen, CanvasCircle(A, CircleRadius));

        var area = Program.TriangleArea(A, B, C);
        var areaHeron = Program.HeronArea(A, B, C);
        var overlay = IsDebug
            ? $"""
            A = {A}
            B = {B}
            C = {C}
            Area = {area}
            AreaHeron = {areaHeron}
            ScreenOffset = {ScreenOffset}
            CenterOffset = {CenterOffset}
            ScanRange = {xRange} .. {yRange}
            MoveMode = {MoveMode}
            DrawScanRange = {DrawScanRange}
            CanvasScaleModifier = {CanvasScaleModifier}
            """
            : $"""
            A = {A}
            B = {B}
            C = {C}
            Area = {area}
            """;

        g.DrawString(
            overlay,
            new Font(FontFamily.GenericMonospace, 14),
            new SolidBrush(Color.Black),
            new PointF(0, 0)
        );

        base.OnPaint(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (MoveMode == MouseMoveMode.MovePoint && PrevMousePos is PointF prev)
        {
            SizeF delta = new SizeF(prev.X - e.X, prev.Y - e.Y) / 10;

            B += delta;
            PrevMousePos = new(e.X, e.Y);

            Invalidate();
        }

        if (MoveMode == MouseMoveMode.MoveScreen && PrevMousePosScreen is PointF prevScreen)
        {
            SizeF delta = new SizeF(prevScreen.X - e.X, prevScreen.Y - e.Y) / 10;

            ScreenOffset += delta;
            PrevMousePosScreen = new(e.X, e.Y);

            Invalidate();
        }

        base.OnMouseMove(e);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left && MoveMode == MouseMoveMode.NotMove)
        {
            PrevMousePos = B + new Size(e.X, e.Y);
            MoveMode = MouseMoveMode.MovePoint;

            Invalidate();
        }

        if (e.Button == MouseButtons.Right && MoveMode == MouseMoveMode.NotMove)
        {
            PrevMousePosScreen += new Size(e.X, e.Y);
            MoveMode = MouseMoveMode.MoveScreen;

            Invalidate();
        }

        base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            PrevMousePos = Point.Empty;
            MoveMode = MouseMoveMode.NotMove;

            Invalidate();
        }

        if (e.Button == MouseButtons.Right)
        {
            PrevMousePosScreen = Point.Empty;
            MoveMode = MouseMoveMode.NotMove;

            Invalidate();
        }

        base.OnMouseUp(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Up:
                B.Y++;
                break;

            case Keys.Down:
                B.Y--;
                break;

            case Keys.Left:
                // Inverse
                B.X++;
                break;

            case Keys.Right:
                // Inverse
                B.X--;
                break;

            case Keys.F1:
                IsDebug = !IsDebug;
                break;

            case Keys.F2:
                IsSimpleScanRange = !IsSimpleScanRange;
                break;

            case Keys.F3:
                DrawScanRange = !DrawScanRange;
                break;
        }

        Invalidate();
        base.OnKeyDown(e);
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        CanvasScaleModifier += e.Delta / 100;
        if (CanvasScaleModifier < 0) CanvasScaleModifier = 0.1f;

        Invalidate();
        base.OnMouseWheel(e);
    }

    private PointF CanvasPoint(PointF point)
    {
        return (PointF)(ScreenOffset + CenterOffset) -
            new SizeF(point) * CanvasScaleModifier;
    }

    private RectangleF CanvasCircle(PointF center, SizeF radius)
    {
        return new RectangleF(CanvasPoint(center) - radius, 2 * radius);
    }

    private RectangleF CanvasRectangle(PointF topLeft, PointF bottomRight)
    {
        topLeft = CanvasPoint(topLeft);
        bottomRight = CanvasPoint(bottomRight);

        float top = Math.Min(topLeft.Y, bottomRight.Y);
        float bottom = Math.Max(topLeft.Y, bottomRight.Y);
        float left = Math.Min(topLeft.X, bottomRight.X);
        float right = Math.Max(topLeft.X, bottomRight.X);

        return RectangleF.FromLTRB(left, top, right, bottom);
    }
}
