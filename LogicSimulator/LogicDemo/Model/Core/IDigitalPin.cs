#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace Logic.Model.Core
{
    #region IDigitalPin

    public interface IDigitalPin
    {
        DigitalSignal Signal { get; set; }
    }

    #endregion
}
