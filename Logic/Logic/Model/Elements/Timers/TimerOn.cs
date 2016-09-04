// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Model
{
    #region References

    using Logic.Model.Core;
    using Logic.Simulation.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    #endregion

    #region TimerOn

    [DataContract]
    public class TimerOn : Element, ITimer, IStateSimulation
    {
        #region Construtor

        public TimerOn() : base() { }

        #endregion

        #region ITimer Implementation

        private float delay = 1.0F;
        private string unit = "s";

        [DataMember]
        public float Delay
        {
            get { return delay; }
            set
            {
                if (value != delay)
                {
                    delay = value;
                    Notify("Delay");
                }
            }
        }

        [DataMember]
        public string Unit
        {
            get { return unit; }
            set
            {
                if (value != unit)
                {
                    unit = value;
                    Notify("Unit");
                }
            }
        }

        #endregion

        #region IStateSimulation

        public ISimulation simulation;

        [IgnoreDataMember]
        public ISimulation Simulation
        {
            get { return simulation; }
            set
            {
                if (value != simulation)
                {
                    simulation = value;

                    Notify("Simulation");
                }
            }
        }

        #endregion

        #region Properties

        private LabelPosition labelPosition = LabelPosition.Top;

        [DataMember]
        public LabelPosition LabelPosition
        {
            get { return labelPosition; }
            set
            {
                if (value != labelPosition)
                {
                    labelPosition = value;
                    Notify("LabelPosition");
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
