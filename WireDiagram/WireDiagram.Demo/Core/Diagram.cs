// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.ObjectModel;

using WireDiagram.Core.Model;
using WireDiagram.Core.Lists;

namespace WireDiagram.Core.Items
{
    [XmlRoot("Diagram"), XmlType("Diagram")]
    public class Diagram : Item, IDiagram, ILocation
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

        private ItemList items;
        private ItemList selectedItems;
        private double width;
        private double height;
        private bool snapToGrid;
        private bool showGrid;
        private bool previewSelection;

        [XmlAttribute("Width")]
        public double Width
        {
            get
            {
                return width;
            }
            set
            {
            	if(value != width)
            	{
	                width = value;
	            	Notify("Width");
            	}
            }
        }

        [XmlAttribute("Height")]
        public double Height
        {
            get
            {
                return height;
            }
            set
            {
            	if(value != height)
            	{
	                height = value;
	                Notify("Height");
            	}
            }
        }

        [XmlArray("Items")]
        public ItemList Items
        {
            get
            {
                return items;
            }
            set
            {
                if (value != items)
                {
                    items = value;
                    Notify("Items");
                }
            }
        }

        [XmlAttribute("SnapToGrid")]
        public bool SnapToGrid
        {
            get
            {
                return snapToGrid;
            }
            set
            {
                if (value != snapToGrid)
                {
                    snapToGrid = value;
                    Notify("SnapToGrid");
                }
            }
        }

        [XmlAttribute("ShowGrid")]
        public bool ShowGrid
        {
            get
            {
                return showGrid;
            }
            set
            {
                if (value != showGrid)
                {
                    showGrid = value;
                    Notify("ShowGrid");
                }
            }
        }

        [XmlAttribute("PreviewSelection")]
        public bool PreviewSelection
        {
            get
            {
                return previewSelection;
            }
            set
            {
                if (value != previewSelection)
                {
                    previewSelection = value;
                    Notify("PreviewSelection");
                }
            }
        }

        [XmlIgnore]
        public ItemList SelectedItems
        {
            get
            {
                return selectedItems;
            }
            set
            {
                if (value != selectedItems)
                {
                    selectedItems = value;
                    Notify("SelectedItems");
                }
            }
        }

        [XmlIgnore]
        public IEnumerable<Node> Nodes
        {
            get
            {
                if (this.items != null)
                {
                    return from node in this.items where node is Node select node as Node;
                }
                return null;
            }
        }

        [XmlIgnore]
        public IEnumerable<Wire> Wires
        {
            get
            {
                if (this.items != null)
                {
                    return from wire in this.items where wire is Wire select wire as Wire;
                }
                return null;
            }
        }

        [XmlIgnore]
        public IEnumerable<Pin> Pins
        {
            get
            {
                if (this.items != null)
                {
                    return from pin in this.items where pin is Pin select pin as Pin;
                }
                return null;
            }
        }

        #endregion

        #region Methods

        public override void Update(IDiagram diagram)
        {
            foreach (var item in items)
            {
                item.Update(this);
            }
        }

        public bool RemoveItem(Item item)
        {
            if (item is Node)
            {
                var node = item as Node;

                this.RemoveNodeConnections(node);
                this.Items.Remove(item);

                return true;
            }
            else if (item is Wire)
            {
                var wire = item as Wire;

                if (wire.StartPin != null)
                    wire.StartPin.IsConnected = false;

                if (wire.EndPin != null)
                    wire.EndPin.IsConnected = false;

                this.Items.Remove(wire);

                return true;
            }
            else
            {
                return false;
            }
        }

        public void RemoveSelected()
        {
            if (selectedItems != null)
            {
                foreach (var item in selectedItems)
                {
                    this.RemoveItem(item);
                }

                SelectedItems.Clear();
                SelectedItems = null;
            }
        }

        public void RemoveAllItems()
        {
            this.Items.Clear();
        }

        public void RemoveNodeConnections(Node node)
        {
            foreach(var wire in this.Wires)
            {
                if (wire.StartPin.ParentId == node.Id)
                {
                    wire.SetStartPin(null);
                }

                if (wire.EndPin.ParentId == node.Id)
                {
                    wire.SetEndPin(null);
                }
            }
        }

        #endregion
    }
}
