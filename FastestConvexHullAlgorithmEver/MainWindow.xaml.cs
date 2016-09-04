// Visualization for the fastest convex hull algorithm ever
// http://mindthenerd.blogspot.com/2012/05/fastest-convex-hull-algorithm-ever.html

#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ConvexHull;

#endregion

namespace FastestConvexHullAlgorithmEver
{
    #region MainWindow

    public partial class MainWindow : Window
    {
        #region Fields

        private List<Point2> Points;
        private int Steps = 1000;
        private Point2 A, B, C, D;
        private Rect R = new Rect();
        private bool EnableDraw = false;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            Set();

            if (EnableDraw)
                Draw();

            TextSteps.Text = Steps.ToString();
        }

        #endregion

        #region Points

        private static List<Point2> CreatePoints(int n)
        {
            var points = new List<Point2>();
            var rand = new Random();

            for (int i = 0; i < n; i++)
                points.Add(new Point2(rand.Next(50, 650), rand.Next(50, 650)));

            return points;
        }

        private void Set()
        {
            Points = CreatePoints(Steps);
        }

        private static void ResetFlags(List<Point2> points)
        {
            for (int i = 0; i < points.Count; i++)
                points[i].IsInside = false;
        }

        #endregion

        #region Algorithm

        private void FastestConvexHullAlgorithm(List<Point2> points)
        {
            var sw1 = System.Diagnostics.Stopwatch.StartNew();

            var prunned = Prune(this.Points);

            sw1.Stop();

            var sw2 = System.Diagnostics.Stopwatch.StartNew();

            Point2[] hull;
            int k;

            // calculate convex hull after prune optimization
            ConvexHull.MonotoneChain.ConvexHullArray(prunned, out hull, out k, true);

            sw2.Stop();

            System.Diagnostics.Debug.Print("Prune: {0}ms", sw1.Elapsed.TotalMilliseconds);
            System.Diagnostics.Debug.Print("ConvexHull: {0}ms", sw2.Elapsed.TotalMilliseconds);
            System.Diagnostics.Debug.Print("ConvexHull + Prune: {0}ms", sw1.Elapsed.TotalMilliseconds + sw2.Elapsed.TotalMilliseconds);

            this.Title = string.Format("ConvexHull + Prune: {0}ms", sw1.Elapsed.TotalMilliseconds + sw2.Elapsed.TotalMilliseconds);

            // redraw
            if (EnableDraw)
                DrawConvexHull(hull, k);
        }

        private List<Point2> Prune(List<Point2> points)
        {
            // prune optimization - initialize vertices 
            // use first or last point as initializer
            A = B = C = D = points[points.Count - 1];
            UpdateR(ref R, A, B, C, D);

            // redraw
            //DrawStep();

            // prune optimization - 1st pass
            var pass1 = Pass1(points);

            // prune optimization - 2nd pass
            var pass2 = Pass2(pass1);

            return pass2;
        }

        private List<Point2> Pass1(List<Point2> points)
        {
            var pass1 = new List<Point2>();

            for (int i = points.Count - 1; i > -1; i--)
            {
                var pt = points[i];

                if (Contains(ref this.R, pt))
                {
                    pt.IsInside = true;
                }
                else
                {
                    pass1.Add(pt);

                    // expand vertices
                    A = ExpandA(A, pt);
                    B = ExpandB(B, pt);
                    C = ExpandC(C, pt);
                    D = ExpandD(D, pt);

                    // build rectangle
                    UpdateR(ref R, A, B, C, D);
                }

                // redraw
                //DrawStep();
            }

            return pass1;
        }

        private List<Point2> Pass2(List<Point2> pass1)
        {
            var pass2 = new List<Point2>();

            for (int i = pass1.Count - 1; i > -1; i--)
            {
                var pt = pass1[i];

                if (Contains(ref this.R, pt))
                    pt.IsInside = true;
                else
                    pass2.Add(pt);

                // redraw
                //DrawStep();
            }

            return pass2;
        }

        private static bool Contains(ref Rect R, Point2 pt)
        {       
            // IsEmpty      
            if (R.Width <= 0 || R.Height <= 0)
                return false;
            
            // Do not include points on the edge as "contained".
            // To include points on the edge change '<' to '>=' and '<' to '<='.
            return ((pt.X > R.X) && 
                    (pt.X - R.Width < R.X) &&
                    (pt.Y > R.Y) && 
                    (pt.Y - R.Height < R.Y));
        }

        private static Point2 ExpandA(Point2 A, Point2 p)
        {
            return (p.X - p.Y) > (A.X - A.Y) ? p : A;
        }

        private static Point2 ExpandB(Point2 B, Point2 p)
        {
            return (p.X + p.Y) > (B.X + B.Y) ? p : B;
        }

        private static Point2 ExpandC(Point2 C, Point2 p)
        {
            return (p.X - p.Y) < (C.X - C.Y) ? p : C;
        }

        private static Point2 ExpandD(Point2 D, Point2 p)
        {
            return (p.X + p.Y) < (D.X + D.Y) ? p : D;
        }

        private static void UpdateR(ref Rect R, Point2 A, Point2 B, Point2 C, Point2 D)
        {
            double x1 = Math.Max(C.X, D.X);
            double x2 = Math.Min(A.X, B.X);
            double y1 = Math.Max(A.Y, D.Y);
            double y2 = Math.Min(B.Y, C.Y);

            // create rectangle R with vertices (x2, y1), (x2, y2), (x1, y2), (x2, y1)

            //R = new Rect(new Point(x1, y1), new Point(x2, y2));
            double _x = Math.Min(x1, x2);
            double _y = Math.Min(y1, y2);
            double _width = Math.Max(Math.Max(x1, x2) - _x, 0.0);
            double _height = Math.Max(Math.Max(y1, y2) - _y, 0.0);
            R.X = _x;
            R.Y = _y;
            R.Width = _width;
            R.Height = _height;
        }

        #endregion

        #region Events

        private void ButtonConvexHull_Click(object sender, RoutedEventArgs e)
        {
            // reset prune flag
            ResetFlags(this.Points);

            var sw = System.Diagnostics.Stopwatch.StartNew();

            Point2[] hull;
            int k;

            // calculate convex hull without prune optimization
            ConvexHull.MonotoneChain.ConvexHullArray(this.Points, out hull, out k, true);
            //ConvexHull.MonotoneChain.ConvexHull(this.points, out hull, out k);

            sw.Stop();
            System.Diagnostics.Debug.Print("ConvexHull: {0}ms", sw.Elapsed.TotalMilliseconds);

            this.Title = string.Format("ConvexHull: {0}ms", sw.Elapsed.TotalMilliseconds);

            // redraw
            if (EnableDraw)
                DrawConvexHull(hull, k);
        }

        private void ButtonConvexHullPrune_Click(object sender, RoutedEventArgs e)
        {
            FastestConvexHullAlgorithm(this.Points);
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            Set();

            if (EnableDraw)
                Draw();
        }

        private void CheckBoxDraw_Click(object sender, RoutedEventArgs e)
        {
            this.EnableDraw = CheckBoxDraw.IsChecked.Value;
        }

        private void ButtonSet_Click(object sender, RoutedEventArgs e)
        {
            int n;
            if (int.TryParse(TextSteps.Text, out n))
            {
                this.Steps = n;

                Set();

                if (EnableDraw)
                    Draw();
            }
        }

        #endregion

        #region Drawing

        private void DrawConvexHull(Point2[] hull, int k)
        {
            this.canvas.Children.Clear();
            AddPoints(this.canvas, Points);

            for (int i = 0; i < k - 1; i++)
                AddLine(canvas, hull[i], hull[i + 1], true);
        }

        private void Draw()
        {
            this.canvas.Children.Clear();
            AddPoints(this.canvas, Points);
        }

        private void DrawStep()
        {
            this.canvas.Children.Clear();
            AddRectangle(this.canvas, ref this.R);
            AddQuadrilateral(this.canvas, this.A, this.B, this.C, this.D);
            AddPoints(this.canvas, Points);
        }

        private static void AddRectangle(Canvas canvas, ref Rect R)
        {
            var r = CreateRectangle();
            SetRectanglePos(r, ref R);
            canvas.Children.Add(r);
        }

        private static void AddQuadrilateral(Canvas canvas, Point2 A, Point2 B, Point2 C, Point2 D)
        {
            AddLine(canvas, A, B, false);
            AddLine(canvas, B, C, false);
            AddLine(canvas, C, D, false);
            AddLine(canvas, D, A, false);
        }

        private static void AddPoints(Canvas canvas, List<Point2> p)
        {
            int n = p.Count;
            for (int i = 0; i < n; i++)
            {
                var pt = p[i];
                AddEllipse(canvas, pt);
            }
        }

        private static void AddEllipse(Canvas canvas, Point2 pt)
        {
            var e = CreateEllipse(pt.IsInside);
            SetEllipsePos(e, pt);
            canvas.Children.Add(e);
        }

        private static void AddLine(Canvas canvas, Point2 p1, Point2 p2, bool isHull)
        {
            var l = CreateLine(isHull);
            SetLinePos(l, p1, p2);
            canvas.Children.Add(l);
        }

        private static Ellipse CreateEllipse(bool isInside)
        {
            return new Ellipse()
            {
                Width = 5,
                Height = 5,
                Fill = isInside ? Brushes.Red : Brushes.Black,
                Stroke = isInside ? Brushes.Red : Brushes.Black,
                StrokeThickness = 1
            };
        }

        private static Line CreateLine(bool isHull)
        {
            return new Line()
            {
                Stroke = isHull ? Brushes.Red : Brushes.LightGray,
                StrokeThickness = 1
            };
        }

        private static Rectangle CreateRectangle()
        {
            return new Rectangle()
            {
                Stroke = Brushes.LightGreen,
                Fill = Brushes.LightGreen,
                StrokeThickness = 1
            };
        }

        private static void SetEllipsePos(Ellipse e, Point2 pt)
        {
            Canvas.SetLeft(e, pt.X - e.Width / 2.0);
            Canvas.SetTop(e, pt.Y - e.Height / 2.0);
        }

        private static void SetRectanglePos(Rectangle r, ref Rect rl)
        {
            r.Width = rl.Width;
            r.Height = rl.Height;
            Canvas.SetLeft(r, rl.Left);
            Canvas.SetTop(r, rl.Top);
        }

        private static void SetLinePos(Line l, Point2 p1, Point2 p2)
        {
            l.X1 = p1.X;
            l.Y1 = p1.Y;
            l.X2 = p2.X;
            l.Y2 = p2.Y;
        }

        #endregion
    } 

    #endregion
}
