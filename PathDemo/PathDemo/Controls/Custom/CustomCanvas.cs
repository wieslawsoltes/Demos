// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using TestPATH;

namespace PathDemo.Controls
{
    public class CustomCanvas : Canvas
    {
        public IList<XPath> Paths { get; set; }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (Paths == null)
                return;

            double thickness = 1.0;
            double half = thickness / 2.0;
            var brush = new SolidColorBrush(Color.FromArgb(0xFF, 0x75, 0x75, 0x75));
            brush.Freeze();
            var stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x75, 0x75, 0x75));
            stroke.Freeze();
            var pen = new Pen(stroke, thickness);
            pen.Freeze();

            var sw = Stopwatch.StartNew();

            foreach (var path in Paths)
            {
                //var sw = Stopwatch.StartNew();

                //var g = Geometry.Parse(path.Source);
                //g.Freeze();

                //var xpg = PathGeometryConverter.ToXPathGeometry(path.Source);
                //path.Geometry = xpg;

                var g = XPathGeometryConverter.ToStreamGeometry(path.Geometry);
                var tgh = new TransformGroupHelper(path.Transform);

                dc.PushTransform(tgh.Group);
                dc.DrawGeometry(path.IsFilled ? brush : null, path.IsStroked ? pen : null, g);
                dc.Pop();

                //sw.Stop();
                //Trace.WriteLine(sw.Elapsed.TotalMilliseconds + "ms");
            }

            sw.Stop();
            Trace.WriteLine(sw.Elapsed.TotalMilliseconds + "ms");
        }
    }
}
