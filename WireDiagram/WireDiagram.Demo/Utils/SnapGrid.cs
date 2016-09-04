// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WireDiagram
{
    public static class SnapGrid
    {
        #region Snap Implementation

        public static double Snap(double original, double snap, double offset)
        {
            return Snap(original - offset, snap) + offset;
        }

        public static double Snap(double original, double snap)
        {
            return original + ((Math.Round(original / snap) - original / snap) * snap);
        }

        #endregion

        #region Point Snap Implementation

        public static Point Snap(Point p, Size snap)
        {
            return new Point(Snap(p.X, snap.Width), Snap(p.Y, snap.Height));
        }

        public static Point Snap(Point p, Size snap, Size offset)
        {
            return new Point(Snap(p.X, snap.Width, offset.Width), Snap(p.Y, snap.Height, offset.Height));
        }

        public static Point Snap(Point p, double snap)
        {
            return new Point(Snap(p.X, snap), Snap(p.Y, snap));
        }

        public static Point Snap(Point p, double snap, double offset)
        {
            return new Point(Snap(p.X, snap, offset), Snap(p.Y, snap, offset));
        }

        public static Point Snap(Point p, double snap, double offsetX, double offsetY)
        {
            return new Point(Snap(p.X - offsetX, snap) + offsetX, Snap(p.Y - offsetY, snap) + offsetY);
        }

        public static Point Snap(double x, double y, double snap, double offsetX, double offsetY)
        {
            return new Point(Snap(x - offsetX, snap) + offsetX, Snap(y - offsetY, snap) + offsetY);
        }

        #endregion
    }
}
