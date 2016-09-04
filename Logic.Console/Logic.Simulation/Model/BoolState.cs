
namespace Logic.Model
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion

    #region BoolState

    public class BoolState : IState
    {
        public bool? State { get; set; }
    }

    #endregion
}
