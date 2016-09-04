// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using ArcDemo.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ArcDemo.Controls
{
    public class ArcCanvas : Canvas
    {
        public ArcCanvas()
        {
            Initialize();
        }

        private void Initialize()
        {
            var ae = new ArcElement()
            {
                Width = this.Width,
                Height = this.Height,
                State = ArcState.Point1,
                Arcs = new ObservableCollection<XArc>()
            };

            this.Children.Add(ae);

            this.PreviewMouseLeftButtonDown += (s, e) => LeftDown(ae, e.GetPosition(ae));
            this.PreviewMouseRightButtonDown += (s, e) => RightDown(ae, e.GetPosition(ae));
            this.PreviewMouseMove += (s, e) => Move(ae, e.GetPosition(ae));
        }

        private void LeftDown(ArcElement ae, Point p)
        {
            switch (ae.State)
            {
                case ArcState.Point1:
                    {
                        ae.WorkingArc = new XArc()
                        {
                            Point1 = new XPoint(),
                            Point2 = new XPoint(),
                            Point3 = new XPoint(),
                            Point4 = new XPoint()
                        };
                        ae.WorkingArc.Point1.X = p.X;
                        ae.WorkingArc.Point1.Y = p.Y;
                        ae.WorkingArc.Point2.X = p.X;
                        ae.WorkingArc.Point2.Y = p.Y;
                        ae.State = ArcState.Point2;
                    }
                    break;
                case ArcState.Point2:
                    {
                        ae.WorkingArc.Point2.X = p.X;
                        ae.WorkingArc.Point2.Y = p.Y;
                        ae.WorkingArc.Point3.X = p.X;
                        ae.WorkingArc.Point3.Y = p.Y;
                        ae.State = ArcState.Point3;
                    }
                    break;
                case ArcState.Point3:
                    {
                        ae.WorkingArc.Point3.X = p.X;
                        ae.WorkingArc.Point3.Y = p.Y;
                        ae.WorkingArc.Point4.X = p.X;
                        ae.WorkingArc.Point4.Y = p.Y;
                        ae.State = ArcState.Point4;
                    }
                    break;
                case ArcState.Point4:
                    {
                        ae.WorkingArc.Point4.X = p.X;
                        ae.WorkingArc.Point4.Y = p.Y;
                        ae.State = ArcState.Point1;
                        ae.Arcs.Add(ae.WorkingArc);
                        ae.WorkingArc = null;
                    }
                    break;
            }

            ae.InvalidateVisual();
        }

        private void RightDown(ArcElement ae, Point p)
        {
            switch (ae.State)
            {
                case ArcState.Point1:
                    {
                    }
                    break;
                case ArcState.Point2:
                case ArcState.Point3:
                case ArcState.Point4:
                    {
                        ae.State = ArcState.Point1;
                        ae.Arcs.Remove(ae.WorkingArc);
                        ae.WorkingArc = null;
                        ae.InvalidateVisual();
                    }
                    break;
            }
        }

        private void Move(ArcElement ae, Point p)
        {
            if (ae.WorkingArc != null)
            {
                switch (ae.State)
                {
                    case ArcState.Point1:
                        {
                            ae.WorkingArc.Point1.X = p.X;
                            ae.WorkingArc.Point1.Y = p.Y;
                        }
                        break;
                    case ArcState.Point2:
                        {
                            ae.WorkingArc.Point2.X = p.X;
                            ae.WorkingArc.Point2.Y = p.Y;
                        }
                        break;
                    case ArcState.Point3:
                        {
                            ae.WorkingArc.Point3.X = p.X;
                            ae.WorkingArc.Point3.Y = p.Y;
                        }
                        break;
                    case ArcState.Point4:
                        {
                            ae.WorkingArc.Point4.X = p.X;
                            ae.WorkingArc.Point4.Y = p.Y;
                        }
                        break;
                }

                ae.InvalidateVisual();
            }
        }
    }
}
