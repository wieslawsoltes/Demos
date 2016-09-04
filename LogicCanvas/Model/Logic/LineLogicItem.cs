using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LogicCanvas.Model.Core;

namespace LogicCanvas.Model.Logic
{
    [XmlRoot("Line"), XmlType("Line"), Serializable]
    public class LineLogicItem : Item, ILine, ILogicItem
    {
        #region ILine Implementation

        private double x1;
        private double y1;
        private double x2;
        private double y2;
        private bool isStartInverted;
        private bool isEndInverted;

        [XmlAttribute("X1")]
        public double X1
        {
            get { return x1; }
            set { x1 = value; Notify("X1"); }
        }

        [XmlAttribute("Y1")]
        public double Y1
        {
            get { return y1; }
            set { y1 = value; Notify("Y1"); }
        }

        [XmlAttribute("X2")]
        public double X2
        {
            get { return x2; }
            set { x2 = value; Notify("X2"); }
        }

        [XmlAttribute("Y2")]
        public double Y2
        {
            get { return y2; }
            set { y2 = value; Notify("Y2"); }
        }

        [XmlAttribute("IsStartInverted")]
        public bool IsStartInverted
        {
            get { return isStartInverted; }
            set { isStartInverted = value; Notify("IsStartInverted"); }
        }

        [XmlAttribute("IsEndInverted")]
        public bool IsEndInverted
        {
            get { return isEndInverted; }
            set { isEndInverted = value; Notify("IsEndInverted"); }
        }

        #endregion
    }
}
