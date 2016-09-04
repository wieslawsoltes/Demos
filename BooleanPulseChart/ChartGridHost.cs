// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace BooleanPulseChart
{
    public class ChartGridHost : FrameworkElement
    {
        private ChartGrid _child;

        protected override int VisualChildrenCount
        {
            get { return _child == null ? 0 : 1; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (_child == null)
            {
                throw new ArgumentOutOfRangeException();
            }
            return _child;
        }

        public ChartGridHost(ChartGrid child)
        {
            _child = child;
        }
    }
}
