#region References

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

#endregion

namespace Logic.Model.Core
{
    #region DigitalLogic

    public abstract class DigitalLogic : LogicObject, IDigitalLogic
    {
        #region IDigitalLogic Implementation

        private ObservableCollection<DigitalSignal> inputs = new ObservableCollection<DigitalSignal>();
        private ObservableCollection<DigitalSignal> outputs = new ObservableCollection<DigitalSignal>();
        private ObservableCollection<DigitalPin> pins = new ObservableCollection<DigitalPin>();

        public virtual ObservableCollection<DigitalSignal> Inputs
        {
            get { return inputs; }
            set
            {
                if (value != inputs)
                {
                    inputs = value;
                    Notify("Inputs");
                }
            }
        }

        public virtual ObservableCollection<DigitalSignal> Outputs
        {
            get { return outputs; }
            set
            {
                if (value != outputs)
                {
                    outputs = value;
                    Notify("Outputs");
                }
            }
        }

        public virtual ObservableCollection<DigitalPin> Pins
        {
            get { return pins; }
            set
            {
                if (value != pins)
                {
                    pins = value;
                    Notify("Pins");
                }
            }
        }

        #endregion

        #region Abstract Interface

        public abstract void Calculate();

        #endregion
    }

    #endregion
}
