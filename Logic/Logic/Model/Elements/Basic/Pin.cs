// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Logic.Model.Core;
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
    public class Pin : Element
    {
        [DataMember]
        public Alignment Alignment { get; set; }

        [DataMember]
        public bool IsPinTypeUndefined { get; set; }

        [DataMember]
        public PinType Type { get; set; }

        // connection format: Tuple<Pin,bool> where bool is flag Inverted==True|False
        public Tuple<Pin, bool>[] Connections { get; set; }

        public Pin() 
            : base() 
        { 
        }

        public override object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
