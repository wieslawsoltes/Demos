#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace Logic.Model.Core
{
    #region IDigitalSignal

    public interface IDigitalSignal
    {
        bool? State { get; set; }
        DigitalPin InputPin { get; set; }
        DigitalPin OutputPin { get; set; }
    }

    #endregion
}
