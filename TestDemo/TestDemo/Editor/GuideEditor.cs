using System;
using System.Collections.Generic;
using System.Linq;

namespace TestDemo
{
    public class GuideEditor
    {
        public Action InvalidateVisual;
        public Action CaptureMouse;
        public Action ReleaseMouseCapture;
        public BasicStyle GuideStyle;
        public BasicStyle SnapGuideStyle;
        public BasicStyle NewLineStyle;
        public BasicStyle LineStyle;
        public GuidePoint GuidePosition;
        public GuidePoint Point0;
        public GuidePoint Point1;
        public bool IsCaptured;
        public IList<GuideLine> Lines;
        public double SnapTreshold;
        public SnapMode SnapMode;
        public double SnapPointRadius;
        public GuidePoint SnapPoint;
        public bool HaveSnapPoint;
        public SnapMode SnapResult;

        public static bool TryToSnapToLine(
            IList<GuideLine> lines, 
            SnapMode mode, 
            double treshold, 
            GuidePoint point, 
            out GuidePoint snap, 
            out SnapMode result)
        {
            snap = default(GuidePoint);
            result = default(SnapMode);

            if (lines.Count == 0 || mode == SnapMode.None)
            {
                return false;
            }

            if (mode.HasFlag(SnapMode.Point))
            {
                foreach (var line in lines) 
                {
                    var distance0 = GuideHelpers.Distance(line.Point0, point);
                    if (distance0 < treshold)
                    {
                        snap = new GuidePoint(line.Point0.X, line.Point0.Y);
                        result = SnapMode.Point;
                        return true;
                    }
                    
                    var distance1 = GuideHelpers.Distance(line.Point1, point);
                    if (distance1 < treshold)
                    {
                        snap = new GuidePoint(line.Point1.X, line.Point1.Y);
                        result = SnapMode.Point;
                        return true;
                    }
                }
            }

            if (mode.HasFlag(SnapMode.Middle))
            {
                foreach (var line in lines) 
                {
                    var middle = GuideHelpers.Middle(line.Point0, line.Point1);
                    var distance = GuideHelpers.Distance(middle, point);
                    if (distance < treshold)
                    {
                        snap = middle;
                        result = SnapMode.Middle;
                        return true;
                    }
                }
            }

            if (mode.HasFlag(SnapMode.Nearest))
            {
                foreach (var line in lines) 
                {
                    var nearest = GuideHelpers.NearestPointOnLine(line.Point0, line.Point1, point);
                    var distance = GuideHelpers.Distance(nearest, point);
                    if (distance < treshold)
                    {
                        snap = nearest;
                        result = SnapMode.Nearest;
                        return true;
                    }
                }
            }
            
            if (mode.HasFlag(SnapMode.Intersection))
            {
                foreach (var line0 in lines) 
                {
                    foreach (var line1 in lines) 
                    {
                        if (line0 == line1)
                            continue;

                        GuidePoint intersection;
                        if (GuideHelpers.LineLineIntersection(line0.Point0, line0.Point1, line1.Point0, line1.Point1, out intersection))
                        {
                            var distance = GuideHelpers.Distance(intersection, point);
                            if (distance < treshold)
                            {
                                snap = intersection;
                                result = SnapMode.Intersection;
                                return true;
                            }
                        }
                    }
                }
            }

            double horizontal = default(double);
            double vertical = default(double);

            if (mode.HasFlag(SnapMode.Horizontal))
            {
                foreach (var line in lines) 
                {
                    if (point.Y >= line.Point0.Y - treshold && point.Y <= line.Point0.Y + treshold)
                    {
                        snap = new GuidePoint(point.X, line.Point0.Y);
                        result |= SnapMode.Horizontal;
                        horizontal = line.Point0.Y;
                        break;
                    }
                    
                    if (point.Y >= line.Point1.Y - treshold && point.Y <= line.Point1.Y + treshold)
                    {
                        snap = new GuidePoint(point.X, line.Point1.Y);
                        result |= SnapMode.Horizontal;
                        horizontal = line.Point1.Y;
                        break;
                    }
                }
            }
            
            if (mode.HasFlag(SnapMode.Vertical))
            {
                foreach (var line in lines) 
                {
                    if (point.X >= line.Point0.X - treshold && point.X <= line.Point0.X + treshold)
                    {
                        snap = new GuidePoint(line.Point0.X, point.Y);
                        result |= SnapMode.Vertical;
                        vertical = line.Point0.X;
                        break;
                    }
                    
                    if (point.X >= line.Point1.X - treshold && point.X <= line.Point1.X + treshold)
                    {
                        snap = new GuidePoint(line.Point1.X, point.Y);
                        result |= SnapMode.Vertical;
                        vertical = line.Point1.X;
                        break;
                    }
                }
            }
            
            if (result.HasFlag(SnapMode.Horizontal) || result.HasFlag(SnapMode.Vertical))
            {
                snap = new GuidePoint(
                    result.HasFlag(SnapMode.Vertical) ? vertical : point.X,
                    result.HasFlag(SnapMode.Horizontal) ? horizontal : point.Y);
                return true;
            }

            return false;
        }

        public void LeftDown(double x, double y)
        {
            if (IsCaptured)
            {
                Point1 = new GuidePoint(x, y);

                HaveSnapPoint = TryToSnapToLine(Lines, SnapMode, SnapTreshold, Point1, out SnapPoint, out SnapResult);
                if (HaveSnapPoint)
                {
                    GuidePosition = new GuidePoint(SnapPoint.X, SnapPoint.Y);
                    Point1 = new GuidePoint(SnapPoint.X, SnapPoint.Y);
                }

                IsCaptured = false;
                ReleaseMouseCapture();
                Lines.Add(new GuideLine(Point0, Point1));
                InvalidateVisual();
            }
            else
            {
                Point0 = new GuidePoint(x, y);
                Point1 = new GuidePoint(x, y);

                HaveSnapPoint = TryToSnapToLine(Lines, SnapMode, SnapTreshold, Point1, out SnapPoint, out SnapResult);
                if (HaveSnapPoint)
                {
                    GuidePosition = new GuidePoint(SnapPoint.X, SnapPoint.Y);
                    Point0 = new GuidePoint(SnapPoint.X, SnapPoint.Y);
                    Point1 = new GuidePoint(SnapPoint.X, SnapPoint.Y);
                }

                IsCaptured = true;
                CaptureMouse();
                InvalidateVisual();
            }   
        }
        
        public void RightDown(double x, double y)
        {
            IsCaptured = false;
            ReleaseMouseCapture();
            InvalidateVisual();
        }
        
        public void Move(double x, double y)
        {
            GuidePosition = new GuidePoint(x, y);
            Point1 = new GuidePoint(x, y);
            
            HaveSnapPoint = TryToSnapToLine(Lines, SnapMode, SnapTreshold, Point1, out SnapPoint, out SnapResult);
            if (HaveSnapPoint)
            {
                GuidePosition = new GuidePoint(SnapPoint.X, SnapPoint.Y);
                Point1 = new GuidePoint(SnapPoint.X, SnapPoint.Y);
            }
#if DEBUG
            double distance = GuideHelpers.Distance(Point0, Point1);
            double angle = GuideHelpers.LineSegmentAngle(Point0, Point1);
            System.Diagnostics.Debug.Print("X: {0} Y: {1} D: {2} A: {3}, S: {4}", Point1.X, Point1.Y, distance, angle, SnapResult);
#endif
            InvalidateVisual();
        }

        public static GuideEditor Create()
        {
            var editor = new GuideEditor();
            
            editor.GuideStyle = new BasicStyle(
                new Argb(128, 0, 255, 255), 
                new Argb(128, 0, 255, 255), 
                1.0);
            editor.SnapGuideStyle = new BasicStyle(
                new Argb(128, 0, 255, 255), 
                new Argb(128, 0, 255, 255), 
                1.0);
            editor.NewLineStyle = new BasicStyle(
                new Argb(255, 255, 255, 0), 
                new Argb(255, 255, 255, 0), 
                2.0);
            editor.LineStyle = new BasicStyle(
                new Argb(255, 0, 255, 0), 
                new Argb(255, 0, 255, 0), 
                2.0);

            editor.IsCaptured = false;
            
            editor.Lines = new List<GuideLine>();
            
            editor.SnapTreshold = 10.0;
            editor.SnapMode = SnapMode.Point | SnapMode.Middle | SnapMode.Intersection | SnapMode.Horizontal | SnapMode.Vertical;
            editor.SnapPointRadius = 4.0;
            
            editor.SnapPoint = default(GuidePoint);
            editor.HaveSnapPoint = false;
            
            return editor;
        }
    }
}
