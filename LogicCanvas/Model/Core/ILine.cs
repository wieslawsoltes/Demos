using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicCanvas.Model.Core
{
    public interface ILine
    {
        double X1 { get; set; }
        double Y1 { get; set; }
        double X2 { get; set; }
        double Y2 { get; set; }
        bool IsStartInverted { get; set; }
        bool IsEndInverted { get; set; }
    }
}
