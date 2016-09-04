
namespace Logic.Model
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion

    #region Signal

    public class Signal : Element, IStateSimulation
    {
        public Signal()
            : base()
        {
            this.Children = new List<Element>();
        }

        public Pin Input { get; set; }
        public Pin Output { get; set; }

        public ISimulation Simulation { get; set; }
    }

    #endregion
}
