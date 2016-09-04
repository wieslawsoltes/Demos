// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using WireDiagram.Core.Model;

namespace WireDiagram.Core.Items
{
    [XmlRoot("Pin"), XmlType("Pin")]
    public class Pin : Item, IPin, ILocation
    {
        #region IPin Implementation

        private double offsetX;
        private double offsetY;

        [XmlAttribute("OffsetX")]
        public double OffsetX
        {
            get
            {
                return offsetX;
            }
            set
            {
                if (value != offsetX)
                {
                    offsetX = value;
                    Notify("OffsetX");
                }
            }
        }

        [XmlAttribute("OffsetY")]
        public double OffsetY
        {
            get
            {
                return offsetY;
            }
            set
            {
                if (value != offsetY)
                {
                    offsetY = value;
                    Notify("OffsetY");
                }
            }
        }

        #endregion

        #region ILocation Implementation

        private double x;
        private double y;
        private double z;

        [XmlAttribute("X")]
        public double X
        {
            get
            {
                return x;
            }
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
            get
            {
                return y;
            }
            set
            {
                if (value != y)
                {
                    y = value;
                    Notify("Y");
                }
            }

        }

        [XmlAttribute("Z")]
        public double Z
        {
            get { return z; }
            set
            {
                if (value != z)
                {
                    z = value;
                    Notify("Z");
                }
            }
        }

        #endregion

        #region Properties

        private PinAlignment alignment;
        private Guid parentId;
        private bool isConnected;
        private Diagram diagram;

        [XmlAttribute("Alignment")]
        public PinAlignment Alignment
        {
            get
            {
                return alignment;
            }
            set
            {
                if (value != alignment)
                {
                    alignment = value;
                    Notify("Alignment");
                }
            }
        }

        [XmlAttribute("ParentId")]
        public Guid ParentId
        {
            get
            {
                return parentId;
            }
            set
            {
            	if(value != parentId)
            	{
	                parentId = value;
                    Notify("ParentId");
            	}
            }
        }

        [XmlIgnore]
        public bool IsConnected
        {
            get
            {
                return isConnected;
            }
            set
            {
                if (value != isConnected)
                {
                    isConnected = value;
                    Notify("IsConnected");
                }
            }
        }

        [XmlIgnore]
        public Diagram Diagram
        {
            get
            {
                return diagram;
            }
            set
            {
                if (value != diagram)
                {
                    diagram = value;
                    Notify("Diagram");
                }
            }
        }

        #endregion

        #region Methods

        public override void Update(IDiagram diagram)
        {
            this.diagram = diagram as Diagram;
        }

        #endregion
    }
}
