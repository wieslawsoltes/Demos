// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    public class XPoint : ObservableObject
    {
        private double _x;
        private double _y;

        public double X
        {
            get { return _x; }
            set { Update(ref _x, value); }
        }

        public double Y
        {
            get { return _y; }
            set { Update(ref _y, value); }
        }
    }
}
