using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GraphDemo
{
    public class CanvasEditor
    {
        private IList<XLine> _lines;
        private XLine _line;
        private XPoint _point;
        private double _pointRadius;
        private XPoint _hoverPoint;
        public int _mode; // 0: Selection, 1: Line
        public int _state;
        public bool _polyMode;
        private double _hitRadius;      
        private double _snap;
        public Action Invalidate;
        
        public CanvasEditor()
        {
            _lines = new List<XLine>();
            _line = null;
            _point = null;
            _pointRadius = 3;
            _hoverPoint = null;
            _mode = 1;
            _state = 0;
            _polyMode = true;
            _hitRadius = 4;
            _snap = 15.0;
        }

        private XLine CreateLine(Point p)
        {
            var line = new XLine();
            line.Start = new XPoint();
            line.End = new XPoint();
            line.Start.X = p.X;
            line.Start.Y = p.Y;
            line.End.X = p.X;
            line.End.Y = p.Y;
            return line;
        }
        
        private XPoint HitTest(Point p)
        {
            double size = _hitRadius + _hitRadius;
            foreach (var line in _lines)
            {
                if ((new Rect(line.Start.X - _hitRadius, line.Start.Y - _hitRadius, size, size)).Contains(p))
                {
                    return line.Start;
                }

                if ((new Rect(line.End.X - _hitRadius, line.End.Y - _hitRadius, size, size)).Contains(p))
                {
                    return line.End;
                }
            }
            return null;
        }

        public static double Snap(double value, double snap)
        {
            double r = value % snap;
            return r >= snap / 2.0 ? value + snap - r : value - r;
        }
        
        public static Point Snap(Point point, double snap)
        {
            return new Point(Snap(point.X, snap), Snap(point.Y, snap));
        }
        
        public void LeftDown(Point p)
        {
            p = Snap(p, _snap);
            
            switch (_mode) 
            {
                case 0:
                    {
                        switch (_state) 
                        {
                            case 0:
                                {
                                    var result = HitTest(p);
                                    if (result != null)
                                    {
                                        _point = result;
                                        this.Invalidate();
                                        _state = 1;
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case 1:
                    {
                        switch (_state) 
                        {
                            case 0:
                                {
                                     _line = CreateLine(p);
                                    var result = HitTest(p);
                                    if (result != null)
                                    {
                                        _line.Start = result;
                                    }
                                    _lines.Add(_line);
                                    this.Invalidate();
                                    _state = 1;
                                }
                                break;
                            case 1:
                                {
                                    if (_line != null)
                                    {
                                        _line.End.X = p.X;
                                        _line.End.Y = p.Y;
                                        var result = HitTest(p);
                                        if (result != null && result != _line.End)
                                        {
                                            _line.End = result;
                                        }
                                        
                                        if (_polyMode)
                                        {
                                            var end = _line.End;
                                            _line = CreateLine(p);
                                            _line.Start = end;
                                            _lines.Add(_line);
                                        }
                                        else
                                        {
                                            _state = 0;
                                            _line = null;
                                        }
                                        this.Invalidate();
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }
        }
 
        public void LeftUp(Point p)
        {
            p = Snap(p, _snap);
            
            switch (_mode) 
            {
                case 0:
                    {
                        switch (_state) 
                        {
                            case 1:
                                if (_point != null)
                                {
                                    _state = 0;
                                    _point = null;
                                }
                                break;
                        }
                    }
                    break;
            }
        }
        
        public void RightDown(Point p)
        {
            p = Snap(p, _snap);
            
            switch (_mode) 
            {
                case 1:
                    {
                        switch (_state) 
                        {
                            case 1:
                                if (_line != null)
                                {
                                    _lines.Remove(_line);
                                    this.Invalidate();
                                    _state = 0;
                                    _line = null;
                                }
                                break;
                        }
                    }
                    break;
            }
        }
        
        public void Move(Point p)
        {
            p = Snap(p, _snap);
            
            bool invalidate = false;

            var result = HitTest(p);
            if (result != null)
            {
                if (_line != null)
                {
                    if (result != _line.Start && result != _line.End)
                    {
                        _hoverPoint = result;
                        invalidate = true;
                    }
                    else
                    {
                        if (_hoverPoint != null)
                        {
                            _hoverPoint = null;
                            invalidate = true;
                        }
                    }
                }
                else
                {
                    _hoverPoint = result;
                    invalidate = true;
                }
            }
            else
            {
                if (_hoverPoint != null)
                {
                    _hoverPoint = null;
                    invalidate = true;
                }
            }

            switch (_mode) 
            {
                case 0:
                    {
                        switch (_state) 
                        {
                            case 1:
                                if (_point != null)
                                {
                                    _point.X = p.X;
                                    _point.Y = p.Y;
                                }
                                invalidate = true;
                                break;
                        }
                    }
                    break;
                case 1:
                    {
                        switch (_state) 
                        {
                            case 1:
                                if (_line != null)
                                {
                                    _line.End.X = p.X;
                                    _line.End.Y = p.Y;
                                }
                                invalidate = true;
                                break;
                        }
                    }
                    break;
            }
            
            if (invalidate)
            {
                this.Invalidate();
            }
        }

        private static void DrawLine(DrawingContext dc, Pen normalPen, double half, Point p0, Point p1)
        {
            var gs = new GuidelineSet(
                new double[] { p0.X + half, p1.X + half }, 
                new double[] { p0.Y + half, p1.Y + half });
            gs.Freeze();
            dc.PushGuidelineSet(gs);
            dc.DrawLine(normalPen, p0, p1);
            dc.Pop();
        }
        
        public void Render(DrawingContext dc)
        {
            var points = new HashSet<XPoint>();
            
            double thickness = 1.0;
            double half = thickness / 2.0;
            
            var hoverBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0x00));
            hoverBrush.Freeze();
            
            var hoverPen = new Pen(hoverBrush, thickness);
            hoverPen.Freeze();
            
            var normalBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00));
            normalBrush.Freeze();
            
            var normalPen = new Pen(normalBrush, thickness);
            normalPen.Freeze();
            
            foreach (var line in _lines)
            {
                var p0 = new Point(line.Start.X, line.Start.Y);
                var p1 = new Point(line.End.X, line.End.Y);
                
                DrawLine(dc, normalPen, half, p0, p1);

                if (!points.Contains(line.Start))
                {
                    points.Add(line.Start);
                    if (_hoverPoint != null && line.Start == _hoverPoint)
                    {
                        //dc.DrawEllipse(hoverBrush, hoverPen, p0, _pointRadius, _pointRadius);
                    }
                    else
                    {
                        dc.DrawEllipse(normalBrush, normalPen, p0, _pointRadius, _pointRadius);
                    }
                }
                if (!points.Contains(line.End))
                {
                    points.Add(line.End);
                    if (_hoverPoint != null && line.End == _hoverPoint)
                    {
                        //dc.DrawEllipse(hoverBrush, hoverPen, p1, _pointRadius, _pointRadius);
                    }
                    else
                    {
                        dc.DrawEllipse(normalBrush, normalPen, p1, _pointRadius, _pointRadius);
                    }
                }
            }
            
            if (_hoverPoint != null)
            {
                dc.DrawEllipse(hoverBrush, hoverPen, new Point(_hoverPoint.X, _hoverPoint.Y), _pointRadius, _pointRadius);
            }
            
            normalPen = null;
            normalBrush = null;
            hoverPen = null;
            hoverBrush = null;
            points = null;
        }
    }
    
    public class GraphCanvas : Canvas
    {
        private CanvasEditor _editor;
        
        public GraphCanvas()
        {
            _editor = new CanvasEditor();
            _editor.Invalidate = () => this.InvalidateVisual();
            
            PreviewKeyDown += KeyDownHandler;
            
            PreviewMouseLeftButtonDown += LeftDownHandler;
            PreviewMouseLeftButtonUp += LefUpHandler;
            PreviewMouseRightButtonDown += RightDownHandler;
            PreviewMouseMove += MoveHandler;
        }

        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            switch (e.Key) 
            {
                case Key.O:
                    {
                        if (_editor._state == 0)
                        {
                            _editor._polyMode = !_editor._polyMode;
                        }
                    }
                    break;
                case Key.S:
                    {
                        if (_editor._state == 0)
                        {
                            _editor._mode = 0;
                        }
                    }
                    break;
                case Key.L:
                    {
                        if (_editor._state == 0)
                        {
                            _editor._mode = 1;
                        }
                    }
                    break;
            }
        }
        
        private void LeftDownHandler(object sender, MouseButtonEventArgs e)
        {
            _editor.LeftDown(e.GetPosition(this));
        }

        private void LefUpHandler(object sender, MouseButtonEventArgs e)
        {
            _editor.LeftUp(e.GetPosition(this));
        }
        
        private void RightDownHandler(object sender, MouseButtonEventArgs e)
        {
            _editor.RightDown(e.GetPosition(this));
        }

        private void MoveHandler(object sender, MouseEventArgs e)
        {
            _editor.Move(e.GetPosition(this));
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            
            _editor.Render(dc);   
        }
    }
}
