// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

using WireDiagram.Core.Model;
using WireDiagram.Core.Lists;

namespace WireDiagram.Core.Items
{
    [XmlRoot("Node"), XmlType("Node")]
    public class Node : Item, INode, ILocation
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

                    if (pins != null)
                    {
                        foreach (var pin in pins)
                        {
                            pin.X = x + pin.OffsetX;
                        }
                    }
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

                    if (pins != null)
                    {
                        foreach (var pin in pins)
                        {
                            pin.Y = y + pin.OffsetY;
                        }
                    }
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

        private PinList pins;
        private Diagram diagram;

        [XmlArray("Pins")]
        public PinList Pins
        {
            get
            {
                return pins;
            }
            set
            {
                if (value != pins)
                {
                    pins = value;
                    Notify("Pins");
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
            	if(value != diagram)
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
            if (this.pins != null)
            {
                this.diagram = diagram as Diagram;

                foreach (var pin in this.pins)
                {
                    pin.ParentId = this.Id;
                    pin.Update(diagram);

                    pin.X = x + pin.OffsetX;
                    pin.Y = y + pin.OffsetY;
                }
            }
        }

        public virtual void CreateDefaultPins() { }

        public void CreatePins(PinPointList points)
        {
            if(points != null)
            {
                pins = new PinList();
                foreach (var p in points)
                {
                    pins.Add(new Pin()
                    {
                        Id = Guid.NewGuid(),
                        OffsetX = p.X * 96.0 / 2.54,
                        OffsetY = p.Y * 96.0 / 2.54,
                        Alignment = p.Alignment
                    });
                }
            }
        }

        #endregion
    }
}
