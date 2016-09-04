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
    #region IDigitalLogic

    public interface IDigitalLogic
    {
        ObservableCollection<DigitalSignal> Inputs { get; set; }
        ObservableCollection<DigitalSignal> Outputs { get; set; }
        ObservableCollection<DigitalPin> Pins { get; set; }
    }

    #endregion
}
