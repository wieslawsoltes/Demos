
namespace Logic.Model
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Logic.Model;

    #endregion

    #region Command Parser

    public static class TextParser
    {
        #region Properties

        public static Solution CurrentSolution { get; set; }

        #endregion

        #region Commands

        private static void CommandHelp(string[] args, int count)
        {
            // Help
            if (count == 1)
            {
                Help.Show();
            }
        }

        private static void CommandQuit(string[] args, int count)
        {
            // Quit
            if (count == 1)
            {
                Environment.Exit(0);
            }
        }

        private static void CommandClear(string[] args, int count)
        {
            // CLS
            if (count == 1)
            {
                try
                {
                    if (SimulationFactory.IsConsole)
                    {
                        Console.Clear();
                    }
                }
                catch (System.IO.IOException ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }
            }
        }

        private static void CommandList(string[] args, int count)
        {
            // List
            if (count == 1)
            {
                if (CurrentSolution == null)
                    return;

                Print.Solution(CurrentSolution);
            }
        }

        public static void SetState(Solution solution, UInt32 id, string state)
        {
            if (solution == null || solution.CurrentContext == null)
                return;

            var element = solution.CurrentContext.Children.Single(child => child.Id == id);

            if (element != null && element is IStateSimulation)
            {
                var simulation = (element as IStateSimulation).Simulation;
                if (simulation != null && simulation.State != null)
                {
                    if (StringUtils.FastCompare(state, "true") || StringUtils.FastCompare(state, "t"))
                        simulation.State.State = true;
                    else if (StringUtils.FastCompare(state, "false") || StringUtils.FastCompare(state, "f"))
                        simulation.State.State = false;
                    else if (StringUtils.FastCompare(state, "null") || StringUtils.FastCompare(state, "n"))
                        simulation.State.State = null;
                }
            }
        }

        private static void CommandState(string[] args, int count)
        {
            // State
            if (count == 1)
            {
                if (CurrentSolution == null)
                    return;

                Print.States(CurrentSolution);
            }
            // State <Name> <True|False|Null>
            else if (count == 3)
            {
                if (CurrentSolution == null || CurrentSolution.CurrentContext == null)
                    return;

                UInt32 id;
                if (UInt32.TryParse(args[1], out id))
                {
                    var state = args[2];

                    SetState(CurrentSolution, id, state);
                }
            }
        }

        private static void CommandDebug(string[] args, int count)
        {
            // DEBUG <On|Off>
            if (count == 1)
            {
                Console.WriteLine("Debug is {0}", SimulationSettings.EnableDebug == true ? "On" : "Off");
            }
            else if (count == 2)
            {
                var value = args[1];

                if (StringUtils.FastCompare(value, "on"))
                {
                    // enable Console debug output
                    SimulationSettings.EnableDebug = true;

                    Console.WriteLine("Debug is On");
                }
                else if (StringUtils.FastCompare(value, "off"))
                {
                    // disable Console debug output
                    SimulationSettings.EnableDebug = false;

                    Console.WriteLine("Debug is Off");
                }
            }
        }

        private static void CommandLog(string[] args, int count)
        {
            // LOG <On|Off>
            if (count == 1)
            {
                Console.WriteLine("Log is {0}", SimulationSettings.EnableLog == true ? "On" : "Off");
            }
            else if (count == 2)
            {
                var value = args[1];

                if (StringUtils.FastCompare(value, "on"))
                {
                    // enable Log for Run() Console debug output (Debug must be On)
                    SimulationSettings.EnableLog = true;

                    Console.WriteLine("Log is On");
                }
                else if (StringUtils.FastCompare(value, "off"))
                {
                    // disable Log for Run() Console debug output
                    SimulationSettings.EnableLog = false;

                    Console.WriteLine("Log is Off");
                }
            }
        }

        private static void CommandRun(string[] args, int count)
        {
            if (CurrentSolution == null)
                return;

            var projects = CurrentSolution.Children.Cast<Project>();
            if (projects == null)
                return;

            var contexts = projects.SelectMany(x => x.Children).Cast<Context>().ToList();
            if (contexts == null)
                return;

            // Run
            if (count == 1)
            {
                SimulationFactory.Run(contexts);
            }

            // Run <CyclePeriod>
            else if (count == 2)
            {
                int period;
                if (int.TryParse(args[1], out period) == false)
                    return;

                if (period < 0)
                    return;

                SimulationFactory.Run(contexts, period);
            }
        }

        private static void CommandBreak(string[] args, int count)
        {
            // Break
            if (count == 1)
            {
                if (SimulationFactory.CurrentSimulationContext.SimulationTimer != null)
                {
                    SimulationFactory.Stop();

                    Console.WriteLine("Timer Stop");

                    Console.Title = "Logic.Console";
                }
            }
        }

        private static void CommandExecute(string[] args, int count)
        {
            // Execute <FileName>
            if (count == 2)
            {
                try
                {
                    CurrentSolution = null;

                    SimulationFactory.Reset(true);

                    //string path = args[1].EndsWith(".bin", StringComparison.OrdinalIgnoreCase) == true ? args[1] : string.Concat(args[1], ".bin");
                    string path = args[1].EndsWith(".bin.gz", StringComparison.OrdinalIgnoreCase) == true ? args[1] : string.Concat(args[1], ".bin.gz");

                    // check if file exists
                    if (System.IO.File.Exists(path) == false)
                        return;

                    var sw = System.Diagnostics.Stopwatch.StartNew();

                    BinaryParser.OpenCompressed(path);
                    //BinaryParser.OpenUnCompressed(path);

                    CurrentSolution = BinaryParser.CurrentSolution;

                    sw.Stop();
                    Console.WriteLine("Executed '{0}' in: {1}ms", args[1], sw.Elapsed.TotalMilliseconds);

                    /*
                    // get file path
                    string path = args[1].EndsWith(".txt", StringComparison.OrdinalIgnoreCase) == true ? args[1] : string.Concat(args[1], ".txt");

                    // check if file exists
                    if (System.IO.File.Exists(path) == false)
                        return;

                    var sw = System.Diagnostics.Stopwatch.StartNew();

                    // load script
                    using (var stream = new System.IO.StreamReader(path))
                    {
                        string line;
                        while ((line = stream.ReadLine()) != null)
                        {
                            ExecuteCommand(line);
                            totalCommands++;
                        }
                    }

                    sw.Stop();
                    Console.WriteLine("Executed '{0}' in: {1}ms, total commands: {2}", args[1], sw.Elapsed.TotalMilliseconds, totalCommands);
                    */
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: {0}", ex.Message);
                }

                return;
            }
        }

        private static void CommandReset(string[] args, int count)
        {
            // Reset
            if (count == 1)
            {
                BinaryParser.CurrentSolution = null;
                BinaryParser.CurrentProject = null;
                BinaryParser.CurrentContext = null;

                CurrentSolution = null;

                SimulationFactory.Reset(true);
            }
        }

        private static void CommandPack(string[] args, int count)
        {
            // Pack <FileName> <FileName>
            if (count == 3)
            {
                try
                {
                    string sourcePath = args[1].EndsWith(".bin", StringComparison.OrdinalIgnoreCase) == true ? args[1] : string.Concat(args[1], ".bin");
                    string destinationPath = args[2].EndsWith(".bin.gz", StringComparison.OrdinalIgnoreCase) == true ? args[2] : string.Concat(args[2], ".bin.gz");

                    // check if file exists
                    if (System.IO.File.Exists(sourcePath) == false)
                        return;

                    var sw = System.Diagnostics.Stopwatch.StartNew();

                    BinaryParser.CompressFile(sourcePath, destinationPath);

                    sw.Stop();
                    Console.WriteLine("Pack '{0}' in: {1}ms", args[1], sw.Elapsed.TotalMilliseconds);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: {0}", ex.Message);
                }

                return;
            }
        }

        private static void CommandUnPack(string[] args, int count)
        {
            // UnPack <FileName> <FileName>
            if (count == 3)
            {
                try
                {
                    string sourcePath = args[1].EndsWith(".bin.gz", StringComparison.OrdinalIgnoreCase) == true ? args[1] : string.Concat(args[1], ".bin.gz");
                    string destinationPath = args[2].EndsWith(".bin", StringComparison.OrdinalIgnoreCase) == true ? args[2] : string.Concat(args[2], ".bin");

                    // check if file exists
                    if (System.IO.File.Exists(sourcePath) == false)
                        return;

                    var sw = System.Diagnostics.Stopwatch.StartNew();

                    BinaryParser.DeCompressFile(sourcePath, destinationPath);

                    sw.Stop();
                    Console.WriteLine("Pack '{0}' in: {1}ms", args[1], sw.Elapsed.TotalMilliseconds);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: {0}", ex.Message);
                }

                return;
            }
        }

        #endregion

        #region Command Dict

        private delegate void DelegateCommand(string[] p, int count);

        private static Dictionary<string, DelegateCommand> CommandDict =
            new Dictionary<string, DelegateCommand>(StringComparer.OrdinalIgnoreCase)
        {
            // Help
            { "h",        CommandHelp      },
            { "help",     CommandHelp      },
            // Quit
            { "q",        CommandQuit      },
            { "quit",     CommandQuit      },
            { "exit",     CommandQuit      },
            // Clear
            { "cls",      CommandClear     },
            { "clear",    CommandClear     },
            // List
            { "ls",       CommandList      },
            { "list",     CommandList      },
            // State
            { "s",        CommandState     },
            { "state",    CommandState     },
            // Debug
            { "d",        CommandDebug     },
            { "debug",    CommandDebug     },
            // Log
            { "l",        CommandLog       },
            { "log",      CommandLog       },
            // Run
            { "r",        CommandRun       },
            { "run",      CommandRun       },
            // Break
            { "b",        CommandBreak      },
            { "break",    CommandBreak      },
            // Execute
            { "e",        CommandExecute   },
            { "execute",  CommandExecute   },
            // Reset
            { "reset",    CommandReset     },
            // Pack
            { "p",        CommandPack       },
            { "pack",     CommandPack       },
            // UnPack
            { "u",        CommandUnPack     },
            { "unpack",   CommandUnPack     },
        };

        #endregion

        #region Execute

        private static char[] SplitChar = { ' ', '\t' };

        public static void ExecuteCommand(string str)
        {
            if (str == string.Empty || str.Length <= 0)
                return;

            var args = str.Split(SplitChar, StringSplitOptions.RemoveEmptyEntries);
            int count = args.Length;
            if (count <= 0)
                return;

            string command = args[0];

            // ignore comments (allowed only at the start of the line)
            if (command.StartsWith("//", StringComparison.OrdinalIgnoreCase) == true)
                return;

            // execute command if exists
            DelegateCommand delegateCommand = null;

            CommandDict.TryGetValue(command, out delegateCommand);

            if (delegateCommand != null)
            {
                delegateCommand(args, count);
            }
        }

        #endregion
    }

    #endregion
}
