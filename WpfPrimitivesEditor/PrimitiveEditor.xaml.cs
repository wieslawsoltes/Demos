// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfPrimitivesEditor
{
    public class XColor
    {
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public XColor(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }
    }

    public class XPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public XPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    public class XQuadraticBezier
    {
        public XPoint Start { get; set; }
        public XPoint Point1 { get; set; }
        public XPoint Point2 { get; set; }
        public XColor Fill { get; set; }
        public XColor Stroke { get; set; }
        public double StrokeThickness { get; set; }
        public bool IsClosed { get; set; }
    }

    public class WpfQuadraticBezier
    {
        private SolidColorBrush fillBrush;
        private SolidColorBrush strokeBrush;
        private Path path;
        private PathGeometry pg;
        private PathFigure pf;
        private QuadraticBezierSegment qbs;

        public object Native { get { return path; } }

        public WpfQuadraticBezier(XQuadraticBezier qb)
        {
            Create(qb.Start, qb.Point1, qb.Point2, qb.Fill, qb.Stroke, qb.StrokeThickness, qb.IsClosed);
        }

        private void Create(XPoint start, XPoint point1, XPoint point2, XColor fill, XColor stroke, double strokeThickness, bool isClosed)
        {
            fillBrush = new SolidColorBrush(Color.FromArgb(fill.A, fill.R, fill.G, fill.B));
            fillBrush.Freeze();
            strokeBrush = new SolidColorBrush(Color.FromArgb(stroke.A, stroke.R, stroke.G, stroke.B));
            strokeBrush.Freeze();
            path = new Path();
            path.Tag = this;
            path.Fill = fillBrush;
            path.Stroke = strokeBrush;
            path.StrokeThickness = strokeThickness;
            pg = new PathGeometry();
            pf = new PathFigure();
            pf.StartPoint = new Point(start.X, start.Y);
            pf.IsClosed = isClosed;
            qbs = new QuadraticBezierSegment();
            qbs.Point1 = new Point(point1.X, point1.Y);
            qbs.Point2 = new Point(point2.X, point2.Y);
            pf.Segments.Add(qbs);
            pg.Figures.Add(pf);
            path.Data = pg;
        }

        public void SetStart(double x, double y)
        {
            pf.StartPoint = new Point(x, y);
        }

        public void SetPoint1(double x, double y)
        {
            qbs.Point1 = new Point(x, y);
        }

        public void SetPoint2(double x, double y)
        {
            qbs.Point2 = new Point(x, y);
        }

        public double GetStartX()
        {
            return pf.StartPoint.X;
        }

        public double GetStartY()
        {
            return pf.StartPoint.Y;
        }

        public double GetPoint1X()
        {
            return qbs.Point1.X;
        }

        public double GetPoint1Y()
        {
            return qbs.Point1.Y;
        }

        public double GetPoint2X()
        {
            return qbs.Point2.X;
        }

        public double GetPoint2Y()
        {
            return qbs.Point2.Y;
        }
    }

    public enum QuadraticBezierEditorState
    {
        None,
        Start,
        Point1
    }

    public partial class PrimitiveEditor : UserControl
    {
        public ObservableCollection<WpfQuadraticBezier> QuadraticBeziers { get; set; }
        private QuadraticBezierEditorState qbeState = QuadraticBezierEditorState.None;
        private WpfQuadraticBezier wqbEdit;
        private WpfQuadraticBezier wqbSelected;
        private int precision = 1;

        public PrimitiveEditor()
        {
            InitializeComponent();
            QuadraticBeziers = new ObservableCollection<WpfQuadraticBezier>();
            DataContext = this;
            HideAllThumbs();
        }

        private void ShowAllThumbs()
        {
            ts.Visibility = Visibility.Visible;
            tp1.Visibility = Visibility.Visible;
            tp2.Visibility = Visibility.Visible;
        }

        private void HideAllThumbs()
        {
            ts.Visibility = Visibility.Hidden;
            tp1.Visibility = Visibility.Hidden;
            tp2.Visibility = Visibility.Hidden;
        }

        private void SetStartPosition(double x, double y)
        {
            Canvas.SetLeft(ts, x);
            Canvas.SetTop(ts, y);
        }

        private void SetPoint1Position(double x, double y)
        {
            Canvas.SetLeft(tp1, x);
            Canvas.SetTop(tp1, y);
        }

        private void SetPoint2Position(double x, double y)
        {
            Canvas.SetLeft(tp2, x);
            Canvas.SetTop(tp2, y);
        }

        private WpfQuadraticBezier CreateQuadraticBezier(double x, double y)
        {
            var qb = new XQuadraticBezier()
            {
                Start = new XPoint(x, y),
                Point1 = new XPoint(x, y),
                Point2 = new XPoint(x, y),
                Fill = new XColor(0x00, 0xFF, 0xFF, 0xFF),
                Stroke = new XColor(0xFF, 0x00, 0x00, 0x00),
                StrokeThickness = 2.0,
                IsClosed = false
            };
            var wqb = new WpfQuadraticBezier(qb);
            return wqb;
        }

        private void NextQuadraticBezierEditorState(double x, double y)
        {
            switch (qbeState)
            {
                case QuadraticBezierEditorState.None:
                    {
                        wqbEdit = CreateQuadraticBezier(x, y);
                        QuadraticBeziers.Add(wqbEdit);
                        canvasDrawing.Children.Add(wqbEdit.Native as UIElement);
                        SetStartPosition(x, y);
                        SetPoint1Position(x, y);
                        SetPoint2Position(x, y);
                        ts.Visibility = Visibility.Visible;
                        tp1.Visibility = Visibility.Hidden;
                        tp2.Visibility = Visibility.Visible;
                        qbeState = QuadraticBezierEditorState.Start;
                        //canvasEditor.CaptureMouse();
                    }
                    break;
                case QuadraticBezierEditorState.Start:
                    {
                        wqbEdit.SetPoint2(x, y);
                        SetPoint2Position(x, y);
                        tp1.Visibility = Visibility.Visible;
                        qbeState = QuadraticBezierEditorState.Point1;
                    }
                    break;
                case QuadraticBezierEditorState.Point1:
                    {
                        wqbEdit.SetPoint1(x, y);
                        SetPoint1Position(x, y);
                        wqbEdit = null;
                        HideAllThumbs();
                        qbeState = QuadraticBezierEditorState.None;
                        //canvasEditor.ReleaseMouseCapture();
                    }
                    break;
            }
        }

        private void Reset()
        {
            canvasDrawing.Children.Clear();
            QuadraticBeziers.Clear();
            ResetSelected();
            HideAllThumbs();
        }

        private void DragStart(double dx, double dy)
        {
            double x = Math.Round(Canvas.GetLeft(ts) + dx, precision);
            double y = Math.Round(Canvas.GetTop(ts) + dy, precision);
            wqbSelected.SetStart(x, y);
            Canvas.SetLeft(ts, x);
            Canvas.SetTop(ts, y);
        }

        private void DragPoint1(double dx, double dy)
        {
            double x = Math.Round(Canvas.GetLeft(tp1) + dx, precision);
            double y = Math.Round(Canvas.GetTop(tp1) + dy, precision);
            wqbSelected.SetPoint1(x, y);
            Canvas.SetLeft(tp1, x);
            Canvas.SetTop(tp1, y);
        }

        private void DragPoint2(double dx, double dy)
        {
            double x = Math.Round(Canvas.GetLeft(tp2) + dx, precision);
            double y = Math.Round(Canvas.GetTop(tp2) + dy, precision);
            wqbSelected.SetPoint2(x, y);
            Canvas.SetLeft(tp2, x);
            Canvas.SetTop(tp2, y);
        }

        private void DragAll(double dx, double dy)
        {
            DragStart(dx, dy);
            DragPoint1(dx, dy);
            DragPoint2(dx, dy);
        }

        private void ts_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (wqbSelected != null && qbeState == QuadraticBezierEditorState.None)
            {
                double dx = e.HorizontalChange;
                double dy = e.VerticalChange;
                bool onlyCtrl = Keyboard.Modifiers == ModifierKeys.Control;
                if (onlyCtrl)
                {
                    DragAll(dx, dy);
                }
                else
                {
                    DragStart(dx, dy);
                }
            }
        }

        private void tp1_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (wqbSelected != null && qbeState == QuadraticBezierEditorState.None)
            {
                double dx = e.HorizontalChange;
                double dy = e.VerticalChange;
                bool onlyCtrl = Keyboard.Modifiers == ModifierKeys.Control;
                if (onlyCtrl)
                {
                    DragAll(dx, dy);
                }
                else
                {
                    DragPoint1(dx, dy);
                }
            }
        }

        private void tp2_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (wqbSelected != null && qbeState == QuadraticBezierEditorState.None)
            {
                double dx = e.HorizontalChange;
                double dy = e.VerticalChange;
                bool onlyCtrl = Keyboard.Modifiers == ModifierKeys.Control;
                if (onlyCtrl)
                {
                    DragAll(dx, dy);
                }
                else
                {
                    DragPoint2(dx, dy);
                }
            }
        }

        private void SetSelected(WpfQuadraticBezier wqb)
        {
            wqbSelected = wqb;
            SetStartPosition(wqbSelected.GetStartX(), wqbSelected.GetStartY());
            SetPoint1Position(wqbSelected.GetPoint1X(), wqbSelected.GetPoint1Y());
            SetPoint2Position(wqbSelected.GetPoint2X(), wqbSelected.GetPoint2Y());
        }

        private void ResetSelected()
        {
            wqbSelected = null;
        }

        private bool HitTest<T, R>(Point center)
            where T : class
            where R : WpfQuadraticBezier
        {
            var results = new List<DependencyObject>();
            VisualTreeHelper.HitTest(canvasDrawing,
                (d) =>
                {
                    if (d is T)
                    {
                        return HitTestFilterBehavior.Continue;
                    }
                    return HitTestFilterBehavior.ContinueSkipSelf;
                },
                (r) =>
                {
                    results.Add(r.VisualHit);
                    return HitTestResultBehavior.Continue;
                },
                new GeometryHitTestParameters(new EllipseGeometry(center, 6.0, 6.0)));

            if (results.Count > 0)
            {
                var d = results.FirstOrDefault();
                SetSelected((d as FrameworkElement).Tag as R);
                ShowAllThumbs();
                return true;
            }
            return false;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            var wqb = (sender as ListBox).SelectedItem as WpfQuadraticBezier;
            if (wqb != null)
            {
                SetSelected(wqb);
                ShowAllThumbs();
            }
            else
            {
                ResetSelected();
                HideAllThumbs();
            }
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool none = Keyboard.Modifiers == ModifierKeys.None;
            bool onlyCtrl = Keyboard.Modifiers == ModifierKeys.Control;
            bool onlyShift = Keyboard.Modifiers == ModifierKeys.Shift;
            bool ctrlShift = Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift);

            switch (e.Key)
            {
                case Key.Delete:
                    if (onlyCtrl)
                    {
                        Reset();
                    }
                    break;
            };
        }

        private void UserControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (qbeState == QuadraticBezierEditorState.None)
            {
                if (!((e.OriginalSource as FrameworkElement).TemplatedParent is Thumb) && wqbSelected != null)
                {
                    ResetSelected();
                    HideAllThumbs();
                    return;
                }

                var canvas = e.OriginalSource as Canvas;
                if (canvas == null || canvas.Name != "canvasEditor")
                {
                    return;
                }

                var center = e.GetPosition(canvasDrawing);
                if(HitTest<Path, WpfQuadraticBezier>(center))
                {
                    return;
                }
            }

            var p = e.GetPosition(canvasEditor);
            double x = Math.Round(p.X, precision);
            double y = Math.Round(p.Y, precision);
            NextQuadraticBezierEditorState(x, y);
        }

        private void UserControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (qbeState == QuadraticBezierEditorState.Start)
            {
                var p = e.GetPosition(canvasEditor);
                double x = Math.Round(p.X, precision);
                double y = Math.Round(p.Y, precision);
                wqbEdit.SetPoint2(x, y);
                SetPoint2Position(x, y);
            }
            else if (qbeState == QuadraticBezierEditorState.Point1)
            {
                var p = e.GetPosition(canvasEditor);
                double x = Math.Round(p.X, precision);
                double y = Math.Round(p.Y, precision);
                wqbEdit.SetPoint1(x, y);
                SetPoint1Position(x, y);
            }
        }

        private void UserControl_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (qbeState == QuadraticBezierEditorState.None)
            {
                var canvas = e.OriginalSource as Canvas;
                if (canvas == null || canvas.Name != "canvasEditor")
                {
                    return;
                }

                if (wqbSelected != null)
                {
                    ResetSelected();
                    HideAllThumbs();
                    return;
                }
            }
            else
            {
                QuadraticBeziers.Remove(wqbEdit);
                canvasDrawing.Children.Remove(wqbEdit.Native as UIElement);
                wqbEdit = null;
                HideAllThumbs();
                qbeState = QuadraticBezierEditorState.None;
                //canvasEditor.ReleaseMouseCapture();
            }
        }
    }
}
