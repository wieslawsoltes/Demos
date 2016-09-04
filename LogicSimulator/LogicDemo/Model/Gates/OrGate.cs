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
    #region OrGate

    public class OrGate : DigitalLogic
    {
        #region Calculate Implementation

        public override void Calculate()
        {
            if (Outputs.Count == 1 && Inputs.All(i => i.State.HasValue))
            {
                Outputs.First().State = Inputs.Count < 2 ? false : !Inputs.All(i => i.State == false);
            }
        }

        #endregion
    }

    #endregion
}
