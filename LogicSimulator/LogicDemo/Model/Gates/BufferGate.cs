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
    #region BufferGate

    public class BufferGate : DigitalLogic
    {
        #region Calculate Implementation

        public override void Calculate()
        {
            if (Inputs.Count > 0 && (Inputs.Count == Outputs.Count) && Inputs.All(i => i.State.HasValue))
            {
                for (int i = 0; i < Inputs.Count; i++)
                {
                    Outputs[i].State = Inputs[i].State;
                }
            }
        }

        #endregion
    }

    #endregion
}
