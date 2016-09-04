#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace Logic.Model.Core
{
    #region DigitalPin

    public class DigitalPin : LogicObject, IDigitalPin
    {
        #region IDigitalPin Implementation

        private DigitalSignal signal;

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

        #endregion
    }

    #endregion
}
