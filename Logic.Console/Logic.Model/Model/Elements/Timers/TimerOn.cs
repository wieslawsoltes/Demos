
namespace Logic.Model
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion

    #region TimerOn

    public class TimerOn : Element, IStateSimulation, ITimer
    {
        public TimerOn()
            : base()
        {
            this.Children = new List<Element>();
        }

        public TimerOn(float delay)
            : this()
        {
            this.Delay = delay;
        }

        public float Delay { get; set; }

        public ISimulation Simulation { get; set; }
    }

    #endregion
}
