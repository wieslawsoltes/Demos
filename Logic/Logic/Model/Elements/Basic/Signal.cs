// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Logic.Model.Core;
using Logic.Simulation.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Model
{
    [DataContract]
    public class Signal : Element
    {
        private Tag _tag;

        [DataMember]
        public Tag Tag
        {
            get { return _tag; }
            set
            {
                if (value != _tag)
                {
                    _tag = value;

                    Notify("Tag");
                }
            }
        }

        public Signal()
            : base()
        {
        }

        public override object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
