// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace BooleanPulseChart
{
    public class BooleanPulseChartElement : FrameworkElement
    {
        public Queue<bool> States { get; set; }
        public int DiplayedStates { get; set; }
        public bool LimitStatesBufferSize { get; set; }
        public double OffsetX { get; set; }
        public double OffsetY { get; set; }
        public double PulseWidth { get; set; }
        public double PulseHeight { get; set; }

        private double _penHalfThickness = 0.5;
        private Pen _pen = new Pen(Brushes.Black, 1.0);

        public Tuple<Point, IList<Point>> GetPoints()
        {
            IList<Point> points = new List<Point>();
            double yfalse = OffsetY + PulseHeight;
            double ytrue = OffsetY + 0;
            double position = OffsetX + ((double)DiplayedStates) * PulseWidth;

            var statesToDisplay = States.Reverse().Take(DiplayedStates);
            bool first = statesToDisplay.First();
            bool? prev = first;

            Point start = new Point(position, first ? ytrue : yfalse);
            position -= PulseWidth;
            points.Add(new Point(position, first ? ytrue : yfalse));

            foreach (var state in statesToDisplay.Skip(1))
            {
                switch (prev)
                {
                    case true:
                        if (state)
                        {
                            // true -> true
                            position -= PulseWidth;
                            points.Add(new Point(position, ytrue));
                        }
                        else
                        {
                            // true -> false
                            points.Add(new Point(position, yfalse));
                            position -= PulseWidth;
                            points.Add(new Point(position, yfalse));
                        }
                        break;
                    case false:
                        if (state)
                        {
                            // false -> true
                            points.Add(new Point(position, ytrue));
                            position -= PulseWidth;
                            points.Add(new Point(position, ytrue));
                        }
                        else
                        {
                            // false -> false
                            position -= PulseWidth;
                            points.Add(new Point(position, yfalse));
                        }
                        break;
                    default:
                        // skip
                        break;
                }
                prev = state;
            }

            return Tuple.Create<Point, IList<Point>>(start, points);
        }

        public StreamGeometry GetGeometry(Tuple<Point, IList<Point>> points)
        {
            var sg = new StreamGeometry();
            using (var ctx = sg.Open())
            {
                ctx.BeginFigure(points.Item1, false, false);
                ctx.PolyLineTo(points.Item2, true, false);
            }
            sg.Freeze();
            return sg;
        }

        public void Add(bool state)
        {
            var states = States;
            var limit = DiplayedStates;

            if (LimitStatesBufferSize)
            {
                if (states.Count > limit - 1)
                {
                    while (states.Count > limit - 1)
                        states.Dequeue();
                }
            }

            states.Enqueue(state);
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            if (States == null || States.Count < 1)
                return;

            var points = GetPoints();
            var sg = GetGeometry(points);
            var gs = new GuidelineSet(
                new[] { _penHalfThickness, _penHalfThickness },
                new[] { _penHalfThickness, _penHalfThickness });
            dc.PushGuidelineSet(gs);
            dc.DrawGeometry(null, _pen, sg);
            dc.Pop();
        }
    }
}
