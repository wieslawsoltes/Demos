#region References

using Logic.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace Logic.Model.Timers
{
    #region TimerOnDelay

    public class TimerOnDelay : DigitalLogic
    {
        #region Constructor

        public TimerOnDelay()
            : base()
        {
        }

        public TimerOnDelay(double delay)
            : this()
        {
            this.delay = delay;
        }

        #endregion

        #region Properties

        private double delay = double.NaN;

        public virtual double Delay
        {
            get { return delay; }
            set
            {
                if (value != delay)
                {
                    delay = value;

                    Notify("Delay");
                }
            }
        }

        #endregion

        #region Calculate Implementation

        private IDisposable disposable = null;
        private IScheduler scheduler = null;

        public override void Calculate()
        {
            if (Inputs.Count == 1 && Outputs.Count == 1)
            {
                if (Inputs.First().State == true)
                {
                    if (disposable == null)
                    {
                        // create timer
                        var observable = Observable.Timer(DateTimeOffset.Now.AddSeconds(delay), scheduler == null ? Scheduler.Default : scheduler);

                        // subcribe to timer
                        disposable = observable.Subscribe(x =>
                        {
                            // update output
                            if (Outputs.Count == 1)
                            {
                                Outputs.First().State = Inputs.Count != 1 ? false : Inputs.First().State;
                            }

                            // dispose timer
                            disposable.Dispose();
                            disposable = null;
                        });
                    }
                }
                else
                {
                    // dispose timer
                    if (disposable != null)
                    {
                        disposable.Dispose();
                        disposable = null;
                    }

                    // update output
                    if (Outputs.Count == 1)
                    {
                        Outputs.First().State = Inputs.Count != 1 ? false : Inputs.First().State;
                    }
                }
            }
        }

        #endregion
    }

    #endregion
}
