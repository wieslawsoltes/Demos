
namespace Logic.Model
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion

    #region IStateSimulation

    public interface IStateSimulation
    {
        ISimulation Simulation { get; set; }
    }

    #endregion
}
