// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WireDiagram.Core.Model
{
    [XmlRoot("Point"), XmlType("Point")]
    public class PinPoint : NotifyObject, IPinPoint
    {
        #region Constructor

        public PinPoint() { }

        public PinPoint(double x, double y, PinAlignment alignment)
        {
            this.x = x;
            this.y = y;
            this.alignment = alignment;
        }

        #endregion

        #region IPinPoint Implementation

        private double x;
        private double y;
        private PinAlignment alignment;

        [XmlAttribute("X")]
        public double X
        {
            get { return x; }
            set
            {
                if (value != x)
                {
                    x = value;
                    Notify("X");
                }
            }
        }

        [XmlAttribute("Y")]
        public double Y
        {
            get { return y; }
            set
            {
                if (value != y)
                {
                    y = value;
                    Notify("Y");
                }
            }
        }

        [XmlAttribute("Alignment")]
        public PinAlignment Alignment
        {
            get { return alignment; }
            set
            {
                if (value != alignment)
                {
                    alignment = value;
                    Notify("Alignment");
                }
            }
        }

        #endregion
    }
}
