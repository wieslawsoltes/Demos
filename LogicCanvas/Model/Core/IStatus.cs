using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicCanvas.Model.Core
{
    public interface IStatus
    {
        bool IsNew { get; set; }
        bool IsModified { get; set; }
        bool IsDeleted { get; set; }
    }
}
