using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LogicCanvas.Model.Core;
using System.Windows;

namespace LogicCanvas.Model.Page
{
    [XmlRoot("Page"), XmlType("Page"), Serializable]
    public class PageItem : NotifyObject, ICloneable
    {
        private TableItem table;
        private ItemList items;

        [XmlElement("Table")]
        public TableItem Table
        {
            get { return table; }
            set { table = value; Notify("Table"); }
        }

        [XmlArray("Items")]
        public ItemList Items
        {
            get { return items; }
            set { items = value; Notify("Items"); }
        }

        #region Xml Ignore

        private bool isPrinting;
        private bool isGridVisible;
        private bool isDeletedVisible;
        private Item selectedItem;
        private DataItemList signals;

        [XmlIgnore]
        public bool IsPrinting
        {
            get { return isPrinting; }
            set { isPrinting = value; Notify("IsPrinting"); }
        }

        [XmlIgnore]
        public bool IsGridVisible
        {
            get { return isGridVisible; }
            set { isGridVisible = value; Notify("IsGridVisible"); }
        }

        [XmlIgnore]
        public bool IsDeletedVisible
        {
            get { return isDeletedVisible; }
            set { isDeletedVisible = value; Notify("IsDeletedVisible"); }
        }

        [XmlIgnore]
        public Item SelectedItem
        {
            get { return selectedItem; }
            set { selectedItem = value; Notify("SelectedItem"); }
        }

        [XmlIgnore]
        public DataItemList Signals
        {
            get { return signals; }
            set { signals = value; Notify("Signals"); }
        }

        #endregion

        public object Clone()
        {
            PageItem item = (PageItem)this.MemberwiseClone();
            item.Table = (TableItem)this.Table.Clone();
            item.Items = (ItemList)this.Items.Clone();
            return item;
        }
    }
}
