using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

public class GraphicsDemo : Form
{
    private Bitmap canvas;
    private PictureBox pictureBox;
    private Button bezierButton, fractalButton, lsystemButton;
    private string currentDrawing = "";

    public GraphicsDemo()
    {
        this.Text = "L-System, Bézier Curve, Fractals Demo";
        this.Size = new Size(900, 700);
        this.Resize += (s, e) => RefreshCanvas();

        bezierButton = new Button { Text = "Show Bézier Curve", Location = new Point(50, 10) };
        fractalButton = new Button { Text = "Show Fractal Tree", Location = new Point(200, 10) };
        lsystemButton = new Button { Text = "Show L-System", Location = new Point(350, 10) };

        bezierButton.Click += (sender, e) => { currentDrawing = "Bezier"; RefreshCanvas(); };
        fractalButton.Click += (sender, e) => { currentDrawing = "Fractal"; RefreshCanvas(); };
        lsystemButton.Click += (sender, e) => { currentDrawing = "LSystem"; RefreshCanvas(); };

        this.Controls.Add(bezierButton);
        this.Controls.Add(fractalButton);
        this.Controls.Add(lsystemButton);

        canvas = new Bitmap(this.ClientSize.Width, this.ClientSize.Height - 50);
        pictureBox = new PictureBox { Location = new Point(0, 50), Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 50), Image = canvas, Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
        this.Controls.Add(pictureBox);
    }

    private void RefreshCanvas()
    {
        canvas = new Bitmap(this.ClientSize.Width, this.ClientSize.Height - 50);
        pictureBox.Image = canvas;
        pictureBox.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 50);

        using (Graphics g = Graphics.FromImage(canvas))
        {
            g.Clear(Color.White);
        }

        if (currentDrawing == "Bezier") DrawBezierCurve();
        else if (currentDrawing == "Fractal") DrawFractalTree(ClientSize.Width / 2, ClientSize.Height - 50, -90, 12);
        else if (currentDrawing == "LSystem") DrawLSystem("F-F-F-F", new Dictionary<char, string> { { 'F', "F+F-F-F+F" } }, 4, ClientSize.Width / 2, ClientSize.Height / 2, 0);
    }

    private void DrawBezierCurve()
    {
        using (Graphics g = Graphics.FromImage(canvas))
        using (Pen pen = new Pen(Color.Blue, 5))
        {
            PointF p0 = new PointF(ClientSize.Width * 0.25f, ClientSize.Height * 0.75f);
            PointF p1 = new PointF(ClientSize.Width * 0.5f, ClientSize.Height * 0.2f);
            PointF p2 = new PointF(ClientSize.Width * 0.75f, ClientSize.Height * 0.75f);

            g.DrawBezier(pen, p0, p1, p2, p2);
        }
        pictureBox.Invalidate();
    }

    private void DrawFractalTree(float x, float y, float angle, int depth)
    {
        if (depth == 0) return;

        float x1 = x + (float)(Math.Cos(angle * Math.PI / 180) * depth * 10);
        float y1 = y + (float)(Math.Sin(angle * Math.PI / 180) * depth * 10);

        using (Graphics g = Graphics.FromImage(canvas))
        using (Pen pen = new Pen(Color.Brown, depth))
        {
            g.DrawLine(pen, x, y, x1, y1);
        }

        DrawFractalTree(x1, y1, angle - 20, depth - 1);
        DrawFractalTree(x1, y1, angle + 20, depth - 1);
        pictureBox.Invalidate();
    }

    private void DrawLSystem(string axiom, Dictionary<char, string> rules, int iterations, float startX, float startY, float angle)
    {
        string current = axiom;
        for (int i = 0; i < iterations; i++)
        {
            string next = "";
            foreach (char c in current)
            {
                next += rules.ContainsKey(c) ? rules[c] : c.ToString();
            }
            current = next;
        }

        using (Graphics g = Graphics.FromImage(canvas))
        using (Pen pen = new Pen(Color.Green, 1))
        {
            Stack<(float, float, float)> stateStack = new Stack<(float, float, float)>();
            float x = startX, y = startY;
            float dir = angle;

            foreach (char c in current)
            {
                switch (c)
                {
                    case 'F':
                        float newX = x + (float)Math.Cos(dir * Math.PI / 180) * 10;
                        float newY = y + (float)Math.Sin(dir * Math.PI / 180) * 10;
                        g.DrawLine(pen, x, y, newX, newY);
                        x = newX;
                        y = newY;
                        break;
                    case '+':
                        dir += 90;
                        break;
                    case '-':
                        dir -= 90;
                        break;
                    case '[':
                        stateStack.Push((x, y, dir));
                        break;
                    case ']':
                        (x, y, dir) = stateStack.Pop();
                        break;
                }
            }
        }
        pictureBox.Invalidate();
    }

    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.Run(new GraphicsDemo());
    }
}
