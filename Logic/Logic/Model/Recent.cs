// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Model
{
    #region References

    using Logic.Model;
    using Logic.Model.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    #endregion

    #region Recent

    [DataContract]
    public class Recent : NotifyObject
    {
        #region Properties

        private string name;
        private string path;

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
        public string Path
        {
            get { return path; }
            set
            {
                if (value != path)
                {
                    path = value;
                    Notify("Path");
                }
            }
        }

        #endregion
    }

    #endregion
}
