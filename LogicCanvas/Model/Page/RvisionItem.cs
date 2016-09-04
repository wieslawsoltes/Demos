using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LogicCanvas.Model.Core;

namespace LogicCanvas.Model.Page
{
    [XmlRoot("Revision"), XmlType("Revision"), Serializable]
    public class RvisionItem : NotifyObject, ICloneable
    {
        private string revision;
        private string date;
        private string author;

        [XmlAttribute("Revision")]
        public string Revision
        {
            get { return revision; }
            set { revision = value; Notify("Revision"); }
        }

        [XmlAttribute("Date")]
        public string Date
        {
            get { return date; }
            set { date = value; Notify("Date"); }
        }

        [XmlAttribute("Author")]
        public string Author
        {
            get { return author; }
            set { author = value; Notify("Author"); }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
