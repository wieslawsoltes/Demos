using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LogicCanvas.Model.Core
{
    [XmlRoot("Item"), XmlType("Item"), Serializable]
    public abstract class Item : NotifyObject, IItem, IStatus, ILocation, ICloneable
    {
        #region ILocation Implementation

        private double x;
        private double y;
        private double z;

        [XmlAttribute("X")]
        public double X
        {
            get { return x; }
            set { x = value; Notify("X"); }
        }

        [XmlAttribute("Y")]
        public double Y
        {
            get { return y; }
            set { y = value; Notify("Y"); }
        }

        [XmlAttribute("Z")]
        public double Z
        {
            get { return z; }
            set { z = value; Notify("Z"); }
        }

        #endregion

        #region IStatus Implementation

        private bool isNew;
        private bool isModified;
        private bool isDeleted;

        [XmlAttribute("IsNew")]
        public bool IsNew
        {
            get { return isNew; }
            set { isNew = value; Notify("IsNew"); }
        }

        [XmlAttribute("IsModified")]
        public bool IsModified
        {
            get { return isModified; }
            set { isModified = value; Notify("IsModified"); }
        }

        [XmlAttribute("IsDeleted")]
        public bool IsDeleted
        {
            get { return isDeleted; }
            set { isDeleted = value; Notify("IsDeleted"); }
        }

        #endregion

        #region ICloneable Implementation

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}
