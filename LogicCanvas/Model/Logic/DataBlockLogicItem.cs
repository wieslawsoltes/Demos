using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LogicCanvas.Model.Core;

namespace LogicCanvas.Model.Logic
{
    [XmlRoot("DataBlock"), XmlType("DataBlock"), Serializable]
    public class DataBlockLogicItem : Item, IData, ILogicItem
    {
        private Guid id;

        [XmlAttribute("Id")]
        public Guid Id
        {
            get { return id; }
            set { id = value; Notify("Id"); }
        }

        #region IData Implementation

        private string designation;
        private string signal;
        private string description;
        private string condition;

        [XmlAttribute("Designation")]
        public string Designation
        {
            get { return designation; }
            set { designation = value; Notify("Designation"); }
        }

        [XmlAttribute("Signal")]
        public string Signal
        {
            get { return signal; }
            set { signal = value; Notify("Signal"); }
        }

        [XmlAttribute("Description")]
        public string Description
        {
            get { return description; }
            set { description = value; Notify("Description"); }
        }

        [XmlAttribute("Condition")]
        public string Condition
        {
            get { return condition; }
            set { condition = value; Notify("Condition"); }
        }

        #endregion
    }
}
