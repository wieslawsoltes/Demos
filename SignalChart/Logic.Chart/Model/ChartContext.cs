
namespace Logic.Chart.Model
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion

    #region ChartContext

    public class ChartContext
    {
        #region Properties

        private string name;
        private ObservableQueue<SignalState> states;
        private int limit = 600;

        public string Name
        {
            get { return name; }
            set
            {
                if (value != name)
                {
                    name = value;
                }
            }
        }

        public ObservableQueue<SignalState> States
        {
            get { return states; }
            set
            {
                if (value != states)
                {
                    states = value;
                }
            }
        }

        public int Limit
        {
            get { return limit; }
            set
            {
                if (value != limit)
                {
                    limit = value;
                }
            }
        }

        #endregion

        #region Constructor

        public ChartContext()
        {
            this.states = new ObservableQueue<SignalState>();
        }

        public ChartContext(string name)
            : this()
        {
            this.name = name;
        }

        #endregion

        #region Methods

        public void Low()
        {
            if (states == null)
                return;

            var last = states.LastOrDefault();
            int count = states.Count();

            if (last == null ||
                last is Low ||
                last is TransitionLow ||
                last is Undefined)
            {
                if (count >= limit)
                {
                    states.Dequeue();
                }

                states.Enqueue(new Low());
            }
            else if (last is High || last is TransitionHigh)
            {
                if (count >= limit)
                {
                    states.Dequeue();
                }

                states.Enqueue(new TransitionLow());
            }
        }

        public void High()
        {
            if (states == null)
                return;

            var last = states.LastOrDefault();
            int count = states.Count();

            if (last == null ||
                last is High ||
                last is TransitionHigh ||
                last is Undefined)
            {
                if (count >= limit)
                {
                    states.Dequeue();
                }

                states.Enqueue(new High());
            }
            else if (last is Low || last is TransitionLow)
            {
                if (count >= limit)
                {
                    states.Dequeue();
                }

                states.Enqueue(new TransitionHigh());
            }
        }

        public void Undefined()
        {
            if (states == null)
                return;

            int count = states.Count();

            if (count >= limit)
            {
                states.Dequeue();
            }

            states.Enqueue(new Undefined());
        }

        #endregion
    }

    #endregion
}
