// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Model
{
    #region References

    using Logic.Model.Core;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    #endregion

    #region Context

    [DataContract]
    public class Context : Element
    {
        #region Construtor

        public Context() : base() { }

        #endregion

        #region Properties

        private int number;
        private PageType pageType = PageType.Logic;
        private ObservableCollection<Element> selectedElements = null;

        [DataMember]
        public int Number
        {
            get { return number; }
            set
            {
                if (value != number)
                {
                    number = value;
                    Notify("Number");
                }
            }
        }

        [DataMember]
        public PageType PageType
        {
            get { return pageType; }
            set
            {
                if (value != pageType)
                {
                    pageType = value;
                    Notify("PageType");
                }
            }
        }

        [IgnoreDataMember]
        public ObservableCollection<Element> SelectedElements
        {
            get { return selectedElements; }
            set
            {
                if (value != selectedElements)
                {
                    selectedElements = value;
                    Notify("SelectedElements");
                }
            }
        }

        #endregion

        #region Clone

        public override object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }

    #endregion
}
