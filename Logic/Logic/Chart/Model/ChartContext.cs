// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Chart.Model
{
    #region References

    using Logic.Model;
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

        private Signal signal = null;
        private ObservableQueue<SignalState> states;
        private int limit = 100;

        public Signal Signal
        {
            get { return signal; }
            set
            {
                if (value != signal)
                {
                    signal = value;
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

        public ChartContext(Signal signal)
            : this()
        {
            this.Signal = signal;
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
