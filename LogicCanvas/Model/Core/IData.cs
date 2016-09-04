using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicCanvas.Model.Core
{
    public interface IData
    {
        string Designation { get; set; }
        string Signal { get; set; }
        string Description { get; set; }
        string Condition { get; set; }
    }
}
