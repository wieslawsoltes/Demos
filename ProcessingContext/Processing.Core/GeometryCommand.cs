using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processing.Core
{
    public class GeometryCommandAttribute : Attribute
    {
        public string Command { get; set; }

        public GeometryCommandAttribute(string command)
        {
            Command = command;
        }
    }
}
