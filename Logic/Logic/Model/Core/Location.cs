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

    #region Location

    [DataContract]
    public class Location : NotifyObject, ILocation
    {
        #region Construtor

        public Location() { }

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
    }

    #endregion
}
