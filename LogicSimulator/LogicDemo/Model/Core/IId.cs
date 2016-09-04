#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace Logic.Model.Core
{
    #region IId

    public interface IId
    {
        Guid Id { get; set; }
        string Name { get; set; }
    }

    #endregion
}
