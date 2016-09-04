
namespace Logic.Model
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion

    #region SignalSimulation

    public class SignalSimulation : ISimulation
    {
        #region Constructor

        public SignalSimulation()
            : base()
        {
            this.InitialState = false;
        }

        #endregion

        #region Properties

        public Element Element { get; set; }

        #endregion

        #region ISimulation

        public IClock Clock { get; set; }

        public IState State { get; set; }
        public bool? InitialState { get; set; }
        public Tuple<IState, bool>[] StatesCache { get; set; }
        public bool HaveCache { get; set; }

        public Element[] DependsOn { get; set; }

        public void Compile()
        {
            if (HaveCache)
                Reset();

            var signal = Element as Signal;

            if (signal.Input == null || signal.Input.Connections == null || signal.Input.Connections.Length <= 0)
            {
                if (SimulationSettings.EnableDebug)
                {
                    Console.WriteLine("No Valid Input/Connections for Id: {0} | State: {1}", Element.Id, State.State);
                }

                return;
            }

            // get all connected inputs with possible state
            var connections = signal.Input.Connections.Where(x => x.Item1.Type == PinType.Output);

            // set ISimulation dependencies (used for topological sort)
            DependsOn = connections.Where(x => x.Item1 != null).Select(y => y.Item1.Parent).Take(1).ToArray();

            if (SimulationSettings.EnableDebug)
            {
                foreach (var connection in connections)
                {
                    Console.WriteLine("Pin: {0} | Inverted: {1} | Parent: {2} | Type: {3}",
                    connection.Item1.Id,
                    connection.Item2,
                    (connection.Item1.Parent != null) ? connection.Item1.Parent.Id : UInt32.MaxValue,
                    connection.Item1.Type);
                }
            }

            // get all connected inputs with state
            var states = connections.Select(x => new Tuple<IState, bool>((x.Item1.Parent as IStateSimulation).Simulation.State, x.Item2)).ToArray();

            if (states.Length == 1)
            {
                StatesCache = states;
                HaveCache = true;
            }
            else
            {
                // invalidate state
                State = null;

                StatesCache = null;
                HaveCache = false;
            }
        }

        public void Calculate()
        {
            if (HaveCache)
            {
                // calculate new state
                var first = StatesCache[0];
                State.State = first.Item2 ? !(first.Item1.State) : first.Item1.State;

                if (SimulationSettings.EnableDebug)
                {
                    Console.WriteLine("Id: {0} | State: {1}", Element.Id, State.State);
                    Console.WriteLine("");
                }
            }
        }

        public void Reset()
        {
            HaveCache = false;
            StatesCache = null;
            State = null;
            Clock = null;
        }

        #endregion
    }

    #endregion
}
