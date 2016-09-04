#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace Logic.Model.Core
{
    #region DigitalWire

    public class DigitalWire : LogicObject, IDigitalWire
    {
        #region IDigitalWire Implementation

        private DigitalSignal signal;
        private DigitalPin startPin;
        private DigitalPin endPin;

        public DigitalSignal Signal
        {
            get { return signal; }
            set
            {
                if (value != signal)
                {
                    signal = value;
                    Notify("Signal");
                }
            }
        }

        public DigitalPin StartPin
        {
            get { return startPin; }
            set
            {
                if (value != startPin)
                {
                    startPin = value;
                    Notify("StartPin");
                }
            }
        }

        public DigitalPin EndPin
        {
            get { return endPin; }
            set
            {
                if (value != endPin)
                {
                    endPin = value;
                    Notify("EndPin");
                }
            }
        }

        #endregion
    }

    #endregion
}
