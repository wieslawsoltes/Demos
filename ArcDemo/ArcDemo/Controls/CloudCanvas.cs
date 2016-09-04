// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApplication1
{
    public class CloudCanvas : Canvas
    {
        private bool closeCloud = true;
        private double R = 0;
        private double cX = 0.0;
        private double cY = 0.0;

        public CloudCanvas()
            : base()
        {
            this.MouseLeftButtonDown +=
                (s, e) =>
                {
                    var p = e.GetPosition(s as Canvas);

                    if (this.IsMouseCaptured)
                    {
                        var context = this.DataContext as Context;
                        var cloud = context.CurrentCloud;
                        var count = cloud.Points.Count;

                        if (count == 0)
                        {
                            cloud.Points.Add(p);
                            cloud.Points.Add(new Point(p.X, p.Y));
                        }
                        else if (count >= 2)
                        {
                            Point first = cloud.Points.First();
                            Point last = cloud.Points.Last();

                            if (first == last)
                            {
                                this.ReleaseMouseCapture();
                            }
                            else
                            {
                                if (count != 2)
                                {
                                    p = MathUtil.PointOnCircle(R, cX, cY, p.X, p.Y);
                                }

                                cloud.Points[count - 1] = p;
                                cloud.Points.Add(new Point(p.X, p.Y));

                                cX = p.X;
                                cY = p.Y;

                                double dX = cloud.Points[count - 1].X - cloud.Points[count - 2].X;
                                double dY = cloud.Points[count - 1].Y - cloud.Points[count - 2].Y;

                                R = Math.Sqrt(dX * dX + dY * dY);
                            }
                        }
                    }
                    else
                    {
                        var context = this.DataContext as Context;
                        var cloud = new Cloud();

                        context.Clouds.Add(cloud);
                        context.CurrentCloud = cloud;

                        cloud.Points.Add(new Point(p.X, p.Y));
                        cloud.Points.Add(new Point(p.X, p.Y));

                        this.CaptureMouse();
                    }
                };

            this.MouseRightButtonDown +=
                (s, e) =>
                {
                    if (this.IsMouseCaptured)
                    {
                        this.ReleaseMouseCapture();

                        var context = this.DataContext as Context;
                        var cloud = context.CurrentCloud;

                        context.CurrentCloud = null;

                        if (cloud.Points.Count == 2)
                        {
                            cloud.Points.Clear();
                            context.Clouds.Remove(cloud);
                        }
                        else if (cloud.Points.Count > 2)
                        {
                            if (closeCloud == true)
                            {
                                Point first = cloud.Points.First();
                                Point last = cloud.Points.Last();

                                if (first != last)
                                {
                                    cloud.Points[cloud.Points.Count - 1] = new Point(first.X, first.Y);
                                }
                            }
                        }
                    }
                };

            this.MouseMove +=
                (s, e) =>
                {
                    if (this.IsMouseCaptured)
                    {
                        var p = e.GetPosition(s as Canvas);

                        var context = this.DataContext as Context;
                        var cloud = context.CurrentCloud;
                        var count = cloud.Points.Count;

                        if (count == 2)
                        {
                            cloud.Points[count - 1] = p;
                        }
                        else if (count > 2)
                        {
                            cloud.Points[count - 1] = MathUtil.PointOnCircle(R, cX, cY, p.X, p.Y);
                        }
                    }
                };
        }
    }
}
