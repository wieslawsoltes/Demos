// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Simulation
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Logic.Model;
    using Logic.Simulation.Core;

    #endregion

    #region SimulationFactory

    public static class SimulationFactory
    {
        #region Properties

        public static SimulationContext CurrentSimulationContext { get; set; }
        public static bool IsConsole { get; set; }

        #endregion

        #region Reset

        public static void Reset(bool collect)
        {
            // reset simulation cache
            if (CurrentSimulationContext.Cache != null)
            {
                SimulationCache.Reset(CurrentSimulationContext.Cache);

                CurrentSimulationContext.Cache = null;
            }

            // collect memory
            if (collect)
            {
                System.GC.Collect();
            }
        }

        #endregion

        #region Simulation

        private static void Run(IEnumerable<Context> contexts, IEnumerable<Tag> tags, bool showInfo)
        {
            // print simulation info
            if (showInfo)
            {
                // get total number of elements for simulation
                var elements = contexts.SelectMany(x => x.Children).Concat(tags);

                System.Diagnostics.Debug.Print("Simulation for {0} contexts, elements: {1}", contexts.Count(), elements.Count());
                System.Diagnostics.Debug.Print("Debug Simulation Enabled: {0}", SimulationSettings.EnableDebug);
                System.Diagnostics.Debug.Print("Have Cache: {0}", CurrentSimulationContext.Cache == null ? false : CurrentSimulationContext.Cache.HaveCache);
            }

            if (CurrentSimulationContext.Cache == null || CurrentSimulationContext.Cache.HaveCache == false)
            {
                // compile simulation for contexts
                CurrentSimulationContext.Cache = Simulation.Compile(contexts, tags, CurrentSimulationContext.SimulationClock);
            }

            if (CurrentSimulationContext.Cache != null || CurrentSimulationContext.Cache.HaveCache == true)
            {
                // run simulation for contexts
                Simulation.Run(CurrentSimulationContext.Cache);
            }
        }

        private static void Run(Action<object> action, object contexts, object tags, TimeSpan period)
        {
            CurrentSimulationContext.SimulationClock.Cycle = 0;
            CurrentSimulationContext.SimulationClock.Resolution = (int)period.TotalMilliseconds;

            CurrentSimulationContext.SimulationTimerSync = new object();

            var virtualTime = new TimeSpan(0);
            var realTime = System.Diagnostics.Stopwatch.StartNew();
            var dt = DateTime.Now;

            CurrentSimulationContext.SimulationTimer = new System.Threading.Timer(
                (s) =>
                {
                    lock (CurrentSimulationContext.SimulationTimerSync)
                    {
                        CurrentSimulationContext.SimulationClock.Cycle++;
                        virtualTime = virtualTime.Add(period);

                        var sw = System.Diagnostics.Stopwatch.StartNew();
                        action(s);
                        sw.Stop();

                        if (IsConsole)
                        {
                            Console.Title = string.Format("Cycle {0} | {1}ms | vt:{2} rt:{3} dt:{4} id:{5}",
                                CurrentSimulationContext.SimulationClock.Cycle,
                                sw.Elapsed.TotalMilliseconds,
                                virtualTime.TotalMilliseconds,
                                realTime.Elapsed.TotalMilliseconds,
                                DateTime.Now - dt,
                                System.Threading.Thread.CurrentThread.ManagedThreadId);
                        }

                        /*
                        if (Settings.EnableDebug)
                        {
                            System.Diagnostics.Debug.Print("Cycle {0} | {1}ms | vt:{2} rt:{3} dt:{4} id:{5}",
                                SimulationClock.Cycle,
                                sw.Elapsed.TotalMilliseconds,
                                virtualTime.TotalMilliseconds,
                                realTime.Elapsed.TotalMilliseconds,
                                DateTime.Now - dt,
                                System.Threading.Thread.CurrentThread.ManagedThreadId);
                        }
                        */

                        System.Diagnostics.Debug.Print("Cycle {0} | {1}ms | vt:{2} rt:{3} dt:{4} id:{5}",
                            CurrentSimulationContext.SimulationClock.Cycle,
                            sw.Elapsed.TotalMilliseconds,
                            virtualTime.TotalMilliseconds,
                            realTime.Elapsed.TotalMilliseconds,
                            DateTime.Now - dt,
                            System.Threading.Thread.CurrentThread.ManagedThreadId);
                    }
                },
                contexts,
                TimeSpan.FromMilliseconds(0),
                period);
        }

        private static void LogRun(Action run, string message)
        {
            string logPath = string.Format("run-{0:yyyy-MM-dd_HH-mm-ss-fff}.log", DateTime.Now);
            var consoleOut = Console.Out;

            System.Diagnostics.Debug.Print("{1}: {0}", message, logPath);
            try
            {
                using (var writer = new System.IO.StreamWriter(logPath))
                {
                    Console.SetOut(writer);
                    run();
                }
            }
            finally
            {
                Console.SetOut(consoleOut);
            }
            System.Diagnostics.Debug.Print("Done {0}.", message);
        }

        public static void Run(List<Context> contexts, IEnumerable<Tag> tags, int period, Action update)
        {
            ResetTimerAndClock();

            var action = new Action(() =>
            {
                Run(contexts, tags, false);
                Run((state) =>  
                {
                    Run(state as List<Context>, tags, false);
                    update();
                }, contexts, tags, TimeSpan.FromMilliseconds(period));
            });

            if (SimulationSettings.EnableLog)
                LogRun(action, "Run");
            else
                action();
        }

        public static void Run(List<Context> contexts, IEnumerable<Tag> tags)
        {
            ResetTimerAndClock();

            if (SimulationSettings.EnableLog)
                LogRun(() => Run(contexts, tags, true), "Run");
            else
                Run(contexts, tags, true);
        }

        public static void Stop()
        {
            if (CurrentSimulationContext != null &&
            CurrentSimulationContext.SimulationTimer != null)
            {
                CurrentSimulationContext.SimulationTimer.Dispose();
            }
        }

        public static void ResetTimerAndClock()
        {
            // stop simulation timer
            if (CurrentSimulationContext.SimulationTimer != null)
            {
                CurrentSimulationContext.SimulationTimer.Dispose();
            }

            // reset simulation clock
            CurrentSimulationContext.SimulationClock.Cycle = 0;
            CurrentSimulationContext.SimulationClock.Resolution = 0;
        }

        #endregion
    }

    #endregion
}
