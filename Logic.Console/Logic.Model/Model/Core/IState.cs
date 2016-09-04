
namespace Logic.Model
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion

    #region IState

    public interface IState
    {
        bool? State { get; set; }
    }

    #endregion
}
