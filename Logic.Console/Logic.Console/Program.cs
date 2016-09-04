
namespace Logic.Console
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Logic.Model;

    #endregion

    #region Program

    static class Program
    {
        #region Main

        static void Main(string[] args)
        {
            Initialize();

            ParseArgs(args);

            CommandLoop();
        }

        private static void Initialize()
        {
            Console.Title = "Logic.Console";

            // create simulation context
            SimulationFactory.CurrentSimulationContext = new SimulationContext()
            {
                Cache = null,
                SimulationClock = new Clock()
            };

            SimulationFactory.IsConsole = true;

            // disable Console debug output
            SimulationSettings.EnableDebug = false;

            // disable Log for Run()
            SimulationSettings.EnableLog = false;
        }

        private static void ParseArgs(string[] args)
        {
            if (args.Length == 1)
            {
                // execute script file from command line
                TextParser.ExecuteCommand(string.Concat("execute ", args[0]));

                // start simulation loop in 100ms cycles
                TextParser.ExecuteCommand("run 100");
            }
            else if (args.Length == 2)
            {
                // execute script file from command line
                TextParser.ExecuteCommand(string.Concat("execute ", args[0]));

                // start simulation loop in user defined cycles
                TextParser.ExecuteCommand(string.Concat("run ", args[1]));
            }
        }

        private static void CommandLoop()
        {
            while (true)
            {
                string line = Console.ReadLine();

                TextParser.ExecuteCommand(line);
            }
        }

        #endregion
    }

    #endregion
}
