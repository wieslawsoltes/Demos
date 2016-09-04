#region References

using Logic.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace Logic.Model.Gates
{
    #region AndGate

    public class AndGate : DigitalLogic
    {
        #region Calculate Implementation

        public override void Calculate()
        {
            if (Outputs.Count == 1 && Inputs.All(i => i.State.HasValue))
            {
                Outputs.First().State = (Inputs.Count < 2) ? false : Inputs.All(i => (bool)i.State == true);
            }
        }

        #endregion
    }

    #endregion
}
