using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicCanvas.Model.Core
{
    public interface ILocation
    {
        double X { get; set; }
        double Y { get; set; }
        double Z { get; set; }
    }
}
