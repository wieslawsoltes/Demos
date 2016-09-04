// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Model.Core
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    #endregion

    #region Element

    [DataContract]
    public abstract class Element : NotifyObject, IId, ILocation, ISelected
    {
        #region Construtor

        public Element() { }

        #endregion

        #region IId Implementation

        private string id;

        [DataMember]
        public string Id
        {
            get { return id; }
            set
            {
                if (value != id)
                {
                    id = value;
                    Notify("Id");
                }
            }
        }

        #endregion

        #region ILocation Implementation

        private double x;
        private double y;
        private double z;
        private bool isLocked = false;

        [DataMember]
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

        [DataMember]
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

        [DataMember]
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

        [DataMember]
        public bool IsLocked
        {
            get { return isLocked; }
            set
            {
                if (value != isLocked)
                {
                    isLocked = value;
                    Notify("IsLocked");
                }
            }
        }

        #endregion

        #region ISelected Implementation

        private bool isSelected = false;

        [IgnoreDataMember]
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (value != isSelected)
                {
                    isSelected = value;
                    Notify("IsSelected");
                }
            }
        }

        #endregion

        #region Properties

        private UInt32 elementId;
        private string name = string.Empty;
        private string factoryName = string.Empty;
        private bool isEditable = true;
        private bool selectChildren = true;
        private Element parent = null;
        private ObservableCollection<Element> children = new ObservableCollection<Element>();

        [IgnoreDataMember]
        public UInt32 ElementId
        {
            get { return elementId; }
            set
            {
                if (value != elementId)
                {
                    elementId = value;
                    Notify("ElementId");
                }
            }
        }

        [DataMember]
        public string Name
        {
            get { return name; }
            set
            {
                if (value != name)
                {
                    name = value;
                    Notify("Name");
                }
            }
        }

        [DataMember]
        public string FactoryName
        {
            get { return factoryName; }
            set
            {
                if (value != factoryName)
                {
                    factoryName = value;
                    Notify("FactoryName");
                }
            }
        }

        [DataMember]
        public bool IsEditable
        {
            get { return isEditable; }
            set
            {
                if (value != isEditable)
                {
                    isEditable = value;
                    Notify("IsEditable");
                }
            }
        }

        [DataMember]
        public bool SelectChildren
        {
            get { return selectChildren; }
            set
            {
                if (value != selectChildren)
                {
                    selectChildren = value;
                    Notify("SelectChildren");
                }
            }
        }

        [DataMember]
        public Element Parent
        {
            get { return parent; }
            set
            {
                if (value != parent)
                {
                    parent = value;
                    Notify("Parent");
                }
            }
        }

        [DataMember]
        public ObservableCollection<Element> Children
        {
            get { return children; }
            set
            {
                if (value != children)
                {
                    children = value;
                    Notify("Children");
                }
            }
        }

        [IgnoreDataMember]
        public Element SimulationParent { get; set; }

        #endregion

        #region Clone

        public static ObservableCollection<T> CopyObservableCollection<T>(IEnumerable<T> list) where T : Element
        {
            return new ObservableCollection<T>(list.Select(x => x.Clone()).Cast<T>());
        }

        public abstract object Clone();

        #endregion
    }

    #endregion
}
