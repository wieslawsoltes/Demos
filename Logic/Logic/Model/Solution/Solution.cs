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

    #region Solution

    [DataContract]
    public class Solution : Element
    {
        #region Construtor

        public Solution() : base() { }

        #endregion

        #region Properties

        private ObservableCollection<Tag> tags = new ObservableCollection<Tag>();
        private Tag defaultTag = null;

        [DataMember]
        public ObservableCollection<Tag> Tags
        {
            get { return tags; }
            set
            {
                if (value != tags)
                {
                    tags = value;
                    Notify("Tags");
                }
            }
        }

        [IgnoreDataMember]
        public Tag DefaultTag
        {
            get { return defaultTag; }
            set
            {
                if (value != defaultTag)
                {
                    defaultTag = value;
                    Notify("DefaultTag");
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
