// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WireDiagram.Core.Items;
using WireDiagram.Core.Model;

namespace WireDiagram.Controls
{
    public class PinCanvas : Canvas
    {
        public static T FindVisualParent<T>(DependencyObject child) 
            where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null)
                return null;

            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return FindVisualParent<T>(parentObject);
            }
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!(this.DataContext is Node))
            {
                base.OnPreviewMouseLeftButtonDown(e);
                return;
            }

            var diagram = (this.DataContext as Node).Diagram;
            var element = e.Source as FrameworkElement;
            var visualParent = FindVisualParent<DiagramCanvas>(this);
            var p = e.GetPosition(visualParent);

            if (element.DataContext is Pin)
            {
                var pin = element.DataContext as Pin;
                if (visualParent.IsMouseCaptured)
                {
                    if (visualParent.HaveStart)
                    {
                        visualParent.CurrentWire.EndPin = pin;
                        pin.IsConnected = true;

                        visualParent.CurrentWire = null;
                        visualParent.StartPin = null;
                        visualParent.HaveStart = false;

                        visualParent.ReleaseMouseCapture();

                        e.Handled = true;
                    }
                }
                else
                {
                    Wire wire = new Wire()
                    {
                        Id = Guid.NewGuid(),
                        X = UnitConverter.CmToDip(0.0),
                        Y = UnitConverter.CmToDip(0.0),
                        Z = 0,
                        Diagram = diagram
                    };

                    wire.SetStartPin(pin);

                    Pin endPin = new Pin()
                    {
                        Id = Guid.Empty,
                        ParentId = Guid.Empty,
                        OffsetX = 0.0,
                        OffsetY = 0.0,
                        X = p.X,
                        Y = p.Y,
                        Alignment = PinAlignment.None
                    };

                    wire.SetEndPin(endPin);

                    visualParent.StartPin = pin;
                    diagram.Items.Add(wire);

                    visualParent.CurrentWire = wire;
                    visualParent.HaveStart = true;

                    Mouse.Capture(visualParent, CaptureMode.SubTree);

                    e.Handled = true;
                }
            }
            else
            {
                if(visualParent.IsMouseCaptured == true)
                    e.Handled = true;

                base.OnPreviewMouseLeftButtonDown(e);
            }
        }
    }
}
