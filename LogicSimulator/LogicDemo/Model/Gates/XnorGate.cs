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
    #region XnorGate

    public class XnorGate : DigitalLogic
    {
        #region Calculate Implementation

        public override void Calculate()
        {
            if (Outputs.Count == 1 && Inputs.All(i => i.State.HasValue))
            {
                Outputs.First().State = Inputs.Count < 2 ? false : Inputs.Count(i => i.State == true) % 2 == 0;
            }
        }

        #endregion
    }

    #endregion
}
