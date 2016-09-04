using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using LogicCanvas.Model.Core;

namespace LogicCanvas.Model.Page
{
    [XmlRoot("Table"), XmlType("Table"), Serializable]
    public class TableItem : NotifyObject, ICloneable
    {
        private string drawnBy;
        private string checkedBy;
        private string date;
        private string type;
        private string description1;
        private string description2;
        private string description3;
        private string revision;
        private string format;
        private string page;
        private string pages;
        private string status;
        private string project;
        private string orderNo;
        private string documentNo1;
        private string documentNo2;
        private BitmapImage logo1;
        private BitmapImage logo2;
        private RevisionList revisions;

        [XmlElement("DrawnBy")]
        public string DrawnBy
        {
            get { return drawnBy; }
            set { drawnBy = value; Notify("DrawnBy"); }
        }

        [XmlElement("CheckedBy")]
        public string CheckedBy
        {
            get { return checkedBy; }
            set { checkedBy = value; Notify("CheckedBy"); }
        }

        [XmlElement("Date")]
        public string Date
        {
            get { return date; }
            set { date = value; Notify("Date"); }
        }

        [XmlElement("Type")]
        public string Type
        {
            get { return type; }
            set { type = value; Notify("Type"); }
        }

        [XmlElement("Description1")]
        public string Description1
        {
            get { return description1; }
            set { description1 = value; Notify("Description1"); }
        }

        [XmlElement("Description2")]
        public string Description2
        {
            get { return description2; }
            set { description2 = value; Notify("Description2"); }
        }

        [XmlElement("Description3")]
        public string Description3
        {
            get { return description3; }
            set { description3 = value; Notify("Description3"); }
        }

        [XmlElement("Revision")]
        public string Revision
        {
            get { return revision; }
            set { revision = value; Notify("Revision"); }
        }

        [XmlElement("Format")]
        public string Format
        {
            get { return format; }
            set { format = value; Notify("Format"); }
        }

        [XmlElement("Page")]
        public string Page
        {
            get { return page; }
            set { page = value; Notify("Page"); }
        }

        [XmlElement("Pages")]
        public string Pages
        {
            get { return pages; }
            set { pages = value; Notify("Pages"); }
        }

        [XmlElement("Status")]
        public string Status
        {
            get { return status; }
            set { status = value; Notify("Status"); }
        }

        [XmlElement("Project")]
        public string Project
        {
            get { return project; }
            set { project = value; Notify("Project"); }
        }

        [XmlElement("OrderNo")]
        public string OrderNo
        {
            get { return orderNo; }
            set { orderNo = value; Notify("OrderNo"); }
        }

        [XmlElement("DocumentNo1")]
        public string DocumentNo1
        {
            get { return documentNo1; }
            set { documentNo1 = value; Notify("DocumentNo1"); }
        }

        [XmlElement("DocumentNo2")]
        public string DocumentNo2
        {
            get { return documentNo2; }
            set { documentNo2 = value; Notify("DocumentNo2"); }
        }

        [XmlArray("Revisions")]
        public RevisionList Revisions
        {
            get { return revisions; }
            set { revisions = value; Notify("Revisions"); }
        }

        #region Xml Ignore

        [XmlIgnore]
        public BitmapImage Logo1
        {
            get { return logo1; }
            set { logo1 = value; Notify("Logo1"); }
        }

        [XmlIgnore]
        public BitmapImage Logo2
        {
            get { return logo2; }
            set { logo2 = value; Notify("Logo2"); }
        }

        [XmlIgnore]
        public RvisionItem Revision1
        {
            get
            {
                if (revisions.Count >= 4)
                {
                    return revisions[revisions.Count - 4];
                }
                else if (revisions.Count == 3)
                {
                    return revisions.First();
                }
                else if (revisions.Count == 2)
                {
                    return revisions.First();
                }
                else if (revisions.Count == 1)
                {
                    return revisions.First();
                }
                else
                {
                    return null;
                }
            }
        }

        [XmlIgnore]
        public RvisionItem Revision2
        {
            get
            {
                if (revisions.Count >= 4)
                {
                    return revisions[revisions.Count - 3];
                }
                else if (revisions.Count == 3)
                {
                    return revisions[1];
                }
                else if (revisions.Count == 2)
                {
                    return revisions.Last();
                }
                else if (revisions.Count == 1)
                {
                    return null;
                }
                else
                {
                    return null;
                }
            }
        }

        [XmlIgnore]
        public RvisionItem Revision3
        {
            get
            {
                if (revisions.Count >= 4)
                {
                    return revisions[revisions.Count - 2];
                }
                else if (revisions.Count == 3)
                {
                    return revisions[2];
                }
                else if (revisions.Count == 2)
                {
                    return null;
                }
                else if (revisions.Count == 1)
                {
                    return null;
                }
                else
                {
                    return null;
                }
            }
        }

        [XmlIgnore]
        public RvisionItem Revision4
        {
            get
            {
                if (revisions.Count >= 4)
                {
                    return revisions.Last();
                }
                else if (revisions.Count == 3)
                {
                    return null;
                }
                else if (revisions.Count == 2)
                {
                    return null;
                }
                else if (revisions.Count == 1)
                {
                    return null;
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        public object Clone()
        {
            TableItem item = (TableItem)this.MemberwiseClone();
            item.Logo1 = this.Logo1.Clone();
            item.Logo2 = this.Logo2.Clone();
            item.Revisions = (RevisionList)this.Revisions.Clone();
            return item;
        }
    }
}
