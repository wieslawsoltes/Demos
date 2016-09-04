
namespace Logic.Model
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion

    #region Simulation

    public static class Simulation
    {
        #region Connections by Id

        private static void FindPinConnections(Pin root,
            Pin pin,
            Dictionary<UInt32, Tuple<Pin, bool>> connections,
            Dictionary<UInt32, List<Tuple<Pin, bool>>> pinToWireDict,
            int level)
        {
            var connectedPins = pinToWireDict[pin.Id].Where(x => x.Item1 != pin && x.Item1 != root && connections.ContainsKey(x.Item1.Id) == false);

            foreach (var p in connectedPins)
            {
                connections.Add(p.Item1.Id, p);

                if (SimulationSettings.EnableDebug)
                {
                    Console.WriteLine("{0}    Pin: {1} | Inverted: {2} | Parent: {3} | Type: {4}",
                    new string(' ', level),
                    p.Item1.Id,
                    p.Item2,
                    p.Item1.Parent.Id,
                    p.Item1.Type);
                }

                if (p.Item1.Type == PinType.Undefined && pinToWireDict.ContainsKey(pin.Id) == true)
                {
                    FindPinConnections(root, p.Item1, connections, pinToWireDict, level + 4);
                }
            }
        }

        private static Dictionary<UInt32, List<Tuple<Pin, bool>>> PinToWireConections(this Element[] elements)
        {
            int lenght = elements.Length;

            var dict = new Dictionary<UInt32, List<Tuple<Pin, bool>>>();

            for (int i = 0; i < lenght; i++)
            {
                var element = elements[i];
                if (element is Wire)
                {
                    var wire = element as Wire;
                    var start = wire.Start;
                    var end = wire.End;
                    bool inverted = wire.InvertStart | wire.InvertEnd;

                    var startId = start.Id;
                    var endId = end.Id;

                    if (!dict.ContainsKey(startId))
                        dict.Add(startId, new List<Tuple<Pin, bool>>());

                    dict[startId].Add(new Tuple<Pin, bool>(start, inverted));
                    dict[startId].Add(new Tuple<Pin, bool>(end, inverted));

                    if (!dict.ContainsKey(endId))
                        dict.Add(endId, new List<Tuple<Pin, bool>>());

                    dict[endId].Add(new Tuple<Pin, bool>(start, inverted));
                    dict[endId].Add(new Tuple<Pin, bool>(end, inverted));
                }
            }

            return dict;
        }

        private static void FindConnections(Element[] elements)
        {
            if (SimulationSettings.EnableDebug)
            {
                Console.WriteLine("");
                Console.WriteLine("--- FindConnections(), elements.Count: {0}", elements.Count());
                Console.WriteLine("");
            }

            var pinToWireDict = elements.PinToWireConections();

            var pins = elements.Where(x => x is IStateSimulation && x.Children != null)
                               .SelectMany(x => x.Children)
                               .Cast<Pin>()
                               .Where(p => (p.Type == PinType.Undefined || p.Type == PinType.Input) && pinToWireDict.ContainsKey(p.Id))
                               .ToArray();

            var lenght = pins.Length;

            for (int i = 0; i < lenght; i++)
            {
                var pin = pins[i];

                if (SimulationSettings.EnableDebug)
                {
                    Console.WriteLine("Pin  {0} | Parent: {1} | Type: {2}",
                        pin.Id,
                        (pin.Parent != null) ? pin.Parent.Id : UInt32.MaxValue,
                        pin.Type);
                }

                var connections = new Dictionary<UInt32, Tuple<Pin, bool>>();

                FindPinConnections(pin, pin, connections, pinToWireDict, 0);

                if (connections.Count > 0)
                    pin.Connections = connections.Values.ToArray();
                else
                    pin.Connections = null;
            }

            if (SimulationSettings.EnableDebug)
            {
                Console.WriteLine("");
            }

            pinToWireDict = null;
            pins = null;
        }

        public static void ResetConnections(IEnumerable<Pin> pins)
        {
            foreach (var pin in pins)
            {
                if (pin.IsPinTypeUndefined)
                {
                    pin.Connections = null;
                    pin.Type = PinType.Undefined;
                }
                else
                {
                    pin.Connections = null;
                }
            }
        }

        #endregion

        #region State Simulation Dictionary

        public static Dictionary<Type, Func<Element, ISimulation>> StateSimulationDict =
            new Dictionary<Type, Func<Element, ISimulation>>()
        {
            // Signal
            { 
                typeof(Signal), 
                (element) =>
                {
                    var stateSimulation = element as IStateSimulation;
                    stateSimulation.Simulation = new SignalSimulation();
                    stateSimulation.Simulation.Element = element;
                    return stateSimulation.Simulation;
                }
            },
            // AndGate
            { 
                typeof(AndGate), 
                (element) =>                    
                {
                    var stateSimulation = element as IStateSimulation;
                    stateSimulation.Simulation = new AndGateSimulation();
                    stateSimulation.Simulation.Element = element;
                    return stateSimulation.Simulation;
                }
            },
            // OrGate
            { 
                typeof(OrGate), 
                (element) =>                     
                {
                    var stateSimulation = element as IStateSimulation;
                    stateSimulation.Simulation = new OrGateSimulation();
                    stateSimulation.Simulation.Element = element;
                    return stateSimulation.Simulation;
                } 
            },
            // TimerOn
            { 
                typeof(TimerOn), 
                (element) =>                    
                {
                    var stateSimulation = element as IStateSimulation;
                    stateSimulation.Simulation = new TimerOnSimulation();
                    stateSimulation.Simulation.Element = element;
                    return stateSimulation.Simulation;
                } 
            },
            // TimerOff
            { 
                typeof(TimerOff), 
                (element) =>                     
                {
                    var stateSimulation = element as IStateSimulation;
                    stateSimulation.Simulation = new TimerOffSimulation();
                    stateSimulation.Simulation.Element = element;
                    return stateSimulation.Simulation;
                } 
            },
            // TimerPulse
            { 
                typeof(TimerPulse), 
                (element) =>                     
                {
                    var stateSimulation = element as IStateSimulation;
                    stateSimulation.Simulation = new TimerPulseSimulation();
                    stateSimulation.Simulation.Element = element;
                    return stateSimulation.Simulation;
                } 
            }
        };

        #endregion

        #region Simulation

        private static void ProcessInput(Pin input, string level)
        {
            var connections = input.Connections;
            var lenght = connections.Length;

            for (int i = 0; i < lenght; i++)
            {
                var connection = connections[i];

                bool isUndefined = connection.Item1.Type == PinType.Undefined;

                if (!(connection.Item1.Parent is Context) && isUndefined)
                {
                    if (!(connection.Item1.Parent is Signal))
                    {
                        var simulation = connection.Item1.Parent as IStateSimulation;

                        connection.Item1.Type = PinType.Output;

                        if (SimulationSettings.EnableDebug)
                        {
                            Console.WriteLine("{0}{1} -> {2}", level, connection.Item1.Id, connection.Item1.Type);
                        }
                    }
                    else
                    {
                        if (SimulationSettings.EnableDebug)
                        {
                            Console.WriteLine("{0}(*) {1} -> {2}", level, connection.Item1.Id, connection.Item1.Type);
                        }
                    }

                    if (connection.Item1.Parent != null && isUndefined)
                    {
                        ProcessOutput(connection.Item1, string.Concat(level, "    "));
                    }
                }
            }
        }

        private static void ProcessOutput(Pin output, string level)
        {
            var pins = output.Parent.Children.Where(p => p != output).Cast<Pin>();

            foreach (var pin in pins)
            {
                bool isUndefined = pin.Type == PinType.Undefined;

                if (!(pin.Parent is Context) && !(pin.Parent is Signal) && isUndefined)
                {
                    var simulation = pin.Parent as IStateSimulation;

                    pin.Type = PinType.Input;

                    if (SimulationSettings.EnableDebug)
                    {
                        Console.WriteLine("{0}{1} -> {2}", level, pin.Id, pin.Type);
                    }
                }

                if (pin.Connections != null && pin.Connections.Length > 0 && isUndefined)
                {
                    ProcessInput(pin, level);
                }
            }
        }

        private static void FindPinTypes(IEnumerable<Element> elements)
        {
            // find input connections
            var connections = elements.Where(x => x.Children != null)
                                      .SelectMany(x => x.Children)
                                      .Cast<Pin>()
                                      .Where(p => p.Connections != null && p.Type == PinType.Input && p.Connections.Length > 0 && p.Connections.Any(i => i.Item1.Type == PinType.Undefined))
                                      .ToArray();

            var lenght = connections.Length;

            if (lenght == 0)
                return;

            // process all input connections
            for (int i = 0; i < lenght; i++)
            {
                var connection = connections[i];
                var simulation = connection.Parent as IStateSimulation;

                if (SimulationSettings.EnableDebug)
                {
                    Console.WriteLine("+ {0} -> {1}", connection.Id, connection.Type);
                }

                ProcessInput(connection, "  ");

                if (SimulationSettings.EnableDebug)
                {
                    Console.WriteLine("");
                }
            }
        }

        private static void InitializeStates(List<ISimulation> simulations)
        {
            var lenght = simulations.Count;

            for (int i = 0; i < lenght; i++)
            {
                var state = new BoolState();
                var simulation = simulations[i];

                state.State = simulation.InitialState;

                simulation.State = state;
            }
        }

        private static void GenerateCompileCache(List<ISimulation> simulations, IClock clock)
        {
            if (simulations == null)
            {
                if (SimulationSettings.EnableDebug)
                {
                    Console.WriteLine("--- warning: no ISimulation elements ---");
                    Console.WriteLine("");
                }

                return;
            }

            var lenght = simulations.Count;

            for (int i = 0; i < lenght; i++)
            {
                if (SimulationSettings.EnableDebug)
                {
                    Console.WriteLine("--- compilation: {0} | Type: {1} ---", simulations[i].Element.Id, simulations[i].GetType());
                    Console.WriteLine("");
                }

                simulations[i].Compile();

                simulations[i].Clock = clock;

                if (SimulationSettings.EnableDebug)
                {
                    Console.WriteLine("");
                }
            }
        }

        private static void Calculate(ISimulation[] simulations)
        {
            if (SimulationSettings.EnableDebug)
            {
                Console.WriteLine("");
            }

            if (simulations == null)
            {
                if (SimulationSettings.EnableDebug)
                {
                    Console.WriteLine("--- warning: no ISimulation elements ---");
                    Console.WriteLine("");
                }

                return;
            }

            var lenght = simulations.Length;

            if (SimulationSettings.EnableDebug)
            {
                Console.WriteLine("");
            }

            var sw = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < lenght; i++)
            {
                if (SimulationSettings.EnableDebug)
                {
                    Console.WriteLine("--- simulation: {0} | Type: {1} ---", simulations[i].Element.Id, simulations[i].GetType());
                    Console.WriteLine("");
                }

                simulations[i].Calculate();
            }

            sw.Stop();
            Console.WriteLine("Calculate() done in: {0}ms | {1} elements", sw.Elapsed.TotalMilliseconds, lenght);
        }

        private static SimulationCache Compile(Element[] elements, IClock clock)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            var cache = new SimulationCache();

            // -- step 1: reset pin connections ---
            var pins = elements.Where(x => x is Pin).Cast<Pin>();

            Simulation.ResetConnections(pins);

            // -- step 2: initialize IStateSimulation simulation
            var simulations = new List<ISimulation>();
            
            var lenght = elements.Length;
            for (int i = 0; i < lenght; i++)
            {
                var element = elements[i];
                if (element is IStateSimulation)
                {
                    var simulation = StateSimulationDict[element.GetType()](element);
                    simulations.Add(simulation);
                }
            }

            // -- step 3: update pin connections ---
            Simulation.FindConnections(elements);

            if (SimulationSettings.EnableDebug)
            {
                Console.WriteLine("--- elements with input connected ---");
                Console.WriteLine("");
            }

            // -- step 4: get ordered elements for simulation ---
            Simulation.FindPinTypes(elements);

            // -- step 5: initialize ISimulation states
            Simulation.InitializeStates(simulations);

            // -- step 6: complile each simulation ---
            Simulation.GenerateCompileCache(simulations, clock);

            // -- step 7: sort simulations using dependencies ---

            if (SimulationSettings.EnableDebug)
            {
                Console.WriteLine("-- dependencies ---");
                Console.WriteLine("");
            }

            var sortedSimulations = simulations.TopologicalSort(x =>
            {
                if (x.DependsOn == null)
                    return null;
                else
                    return x.DependsOn.Cast<IStateSimulation>().Select(y => y.Simulation);
            });

            if (SimulationSettings.EnableDebug)
            {
                Console.WriteLine("-- sorted dependencies ---");
                Console.WriteLine("");

                foreach (var simulation in sortedSimulations)
                {
                    Console.WriteLine("{0}", simulation.Element.Id);
                }

                Console.WriteLine("");
            }

            // -- step 8: cache sorted elements
            if (sortedSimulations != null)
            {
                cache.Simulations = sortedSimulations.ToArray();
                cache.HaveCache = true;
            }

            // Connections are not used after compilation is done
            foreach (var pin in pins)
                pin.Connections = null;

            // DependsOn are not used after compilation is done
            foreach (var simulation in simulations)
                simulation.DependsOn = null;

            pins = null;
            simulations = null;
            sortedSimulations = null;

            sw.Stop();

            Console.WriteLine("Compile() done in: {0}ms", sw.Elapsed.TotalMilliseconds);

            return cache;
        }

        #endregion

        #region Run

        public static SimulationCache Compile(IEnumerable<Context> contexts, IClock clock)
        {
            var elements = contexts.SelectMany(x => x.Children).ToArray();

            // compile elements
            var cache = Simulation.Compile(elements, clock);

            // collect unused memory
            System.GC.Collect();

            return cache;
        }

        public static SimulationCache Compile(Context context, IClock clock)
        {
            var elements = context.Children.ToArray();

            // compile elements
            var cache = Simulation.Compile(elements, clock);

            // collect unused memory
            System.GC.Collect();

            return cache;
        }

        public static void Run(SimulationCache cache)
        {
            if (cache == null || cache.HaveCache == false)
                return;

            Simulation.Calculate(cache.Simulations);
        }

        #endregion

        #region Topological Sort

        private static IEnumerable<T> TopologicalSort<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> dependencies)
        {
            var sorted = new List<T>();
            var visited = new HashSet<T>();

            foreach (var item in source)
            {
                if (SimulationSettings.EnableDebug)
                {
                    Console.WriteLine("+ {0} depends on:", (item as ISimulation).Element.Id);
                }

                Visit(item, visited, sorted, dependencies);

                if (SimulationSettings.EnableDebug)
                {
                    Console.WriteLine("");
                }
            }

            return sorted;
        }

        private static void Visit<T>(T item, HashSet<T> visited, List<T> sorted, Func<T, IEnumerable<T>> dependencies)
        {
            if (!visited.Contains(item))
            {
                visited.Add(item);

                var dependsOn = dependencies(item);

                if (dependsOn != null)
                {
                    foreach (var dep in dependsOn)
                    {
                        if (SimulationSettings.EnableDebug)
                        {
                            Console.WriteLine("|     {0}", (dep as ISimulation).Element.Id);
                        }

                        Visit(dep, visited, sorted, dependencies);
                    }

                    // add only items with dependencies
                    sorted.Add(item);
                }

                // add all items
                //sorted.Add(item);
            }
            //else if (!sorted.Contains(item))
            //{
            //    Console.WriteLine("Invalid dependency cycle: {0}", (item as Element).Name);
            //}
        }

        #endregion
    }

    #endregion
}
