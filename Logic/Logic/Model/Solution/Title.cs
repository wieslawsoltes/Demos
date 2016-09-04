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

    #region Title

    [DataContract]
    public class Title : NotifyObject
    {
        #region Construtor

        public Title() : base() { }

        #endregion

        #region Properties

        private string documentId = string.Empty;
        private string documentTitle = string.Empty;
        private string documentNumber = string.Empty;

        [DataMember]
        public string DocumentId
        {
            get { return documentId; }
            set
            {
                if (value != documentId)
                {
                    documentId = value;
                    Notify("DocumentId");
                }
            }
        }

        [DataMember]
        public string DocumentTitle
        {
            get { return documentTitle; }
            set
            {
                if (value != documentTitle)
                {
                    documentTitle = value;
                    Notify("DocumentTitle");
                }
            }
        }

        [DataMember]
        public string DocumentNumber
        {
            get { return documentNumber; }
            set
            {
                if (value != documentNumber)
                {
                    documentNumber = value;
                    Notify("DocumentNumber");
                }
            }
        }

        #endregion
    }

    #endregion
}
