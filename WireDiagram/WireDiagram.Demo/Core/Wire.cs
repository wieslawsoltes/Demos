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
    [XmlRoot("Wire"), XmlType("Wire")]
    public class Wire : Item, IWire, ILocation
    {
        #region ILocation Implementation

        private double x;
        private double y;
        private double z;

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

        private Pin startPin = null;
        private Pin endPin = null;
        private Diagram diagram;

        [XmlElement("Start")]
        public Pin StartPin
        {
            get
            {
                return startPin;
            }
            set
            {
                if (value != startPin)
                {
                    startPin = value;
                    Notify("StartPin");
                }
            }
        }

        [XmlElement("End")]
        public Pin EndPin
        {
            get
            {
                return endPin;
            }
            set
            {
                if (value != endPin)
                {
                    endPin = value;
                    Notify("EndPin");
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
            this.Diagram = diagram as Diagram;
            this.FindPins(this.Diagram.Nodes);
        }

        public void SetStartPin(Pin pin)
        {
            if (pin != null)
            {
                this.startPin = pin;
                this.startPin.IsConnected = true;
            }
            else
            {
                // reset pin Id but keep position info
                if (this.startPin != null)
                {
                    this.startPin.ParentId = Guid.Empty;
                    this.startPin.Id = Guid.Empty;
                    this.endPin.IsConnected = false;
                }
            }
        }

        public void SetEndPin(Pin pin)
        {
            if (pin != null)
            {
                this.endPin = pin;
                this.endPin.IsConnected = true;
            }
            else
            {
                // reset pin Id but keep position info
                if (this.endPin != null)
                {
                    this.endPin.ParentId = Guid.Empty;
                    this.endPin.Id = Guid.Empty;
                    this.endPin.IsConnected = false;
                }
            }
        }

        public void FindPins(IEnumerable<Node> nodes)
        {
            if (this.startPin != null)
            {
                if (this.startPin.ParentId != Guid.Empty)
                {
                    this.startPin = nodes.Where(n => n.Id == startPin.ParentId).First().Pins.Where(p => p.Id == startPin.Id).First();
                    this.startPin.IsConnected = true;
                }
            }

            if (this.endPin != null)
            {
                if (this.endPin.ParentId != Guid.Empty)
                {
                    this.endPin = nodes.Where(n => n.Id == endPin.ParentId).First().Pins.Where(p => p.Id == endPin.Id).First();
                    this.endPin.IsConnected = true;
                }
            }
        }

        #endregion
    }
}
