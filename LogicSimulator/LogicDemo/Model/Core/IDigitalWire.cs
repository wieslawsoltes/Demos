#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace Logic.Model.Core
{
    #region IDigitalWire

    public interface IDigitalWire
    {
        DigitalSignal Signal { get; set; }
        DigitalPin StartPin { get; set; }
        DigitalPin EndPin { get; set; }
    }

    #endregion
}
