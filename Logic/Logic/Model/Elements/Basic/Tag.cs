// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Logic.Model.Core;
using Logic.Simulation.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Model
{
    [DataContract]
    public class Tag : Element, IStateSimulation
    {
        private IDictionary<string, Property> _properties = null;
        public ISimulation _simulation;

        [DataMember]
        public IDictionary<string, Property> Properties
        {
            get { return _properties; }
            set
            {
                if (value != _properties)
                {
                    _properties = value;
                    Notify("Properties");
                }
            }
        }

        [IgnoreDataMember]
        public ISimulation Simulation
        {
            get { return _simulation; }
            set
            {
                if (value != _simulation)
                {
                    _simulation = value;

                    Notify("Simulation");
                }
            }
        }

        public Tag()
            : base()
        {
            this._properties = new Dictionary<string, Property>();
        }

        public override object Clone()
        {
            Tag tag = (Tag)this.MemberwiseClone();
            return tag;
        }
    }
}
