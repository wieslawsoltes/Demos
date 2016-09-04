
namespace Logic.Model
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion

    #region IClock

    public interface IClock
    {
        long Cycle { get; set; }
        int Resolution { get; set; }
    }

    #endregion
}
