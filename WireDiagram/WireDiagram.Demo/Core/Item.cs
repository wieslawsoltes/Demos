// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace WireDiagram.Core.Model
{
    [XmlRoot("Item"), XmlType("Item")]
    public abstract class Item : NotifyObject, ISelected
    {
        #region Properties

        private Guid id;

        [XmlAttribute("Id")]
        public Guid Id
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

        #region ISelected Implementation

        private bool isSelected;

        [XmlIgnore]
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

        #region Abstract Interface

        public abstract void Update(IDiagram diagram);

        #endregion
    }
}
