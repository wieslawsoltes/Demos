
namespace Logic.Model
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Logic.Model;

    #endregion

    #region Help

    public static class Help
    {
        #region Helper Methods

        private static void Cmd(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(message);
            Console.ResetColor();
        }

        private static void Arg(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(' ');
            Console.Write(message);
            Console.ResetColor();
        }

        private static void Descr(string message)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(message);
            Console.ResetColor();
        }

        private static void End()
        {
            Console.Write(Environment.NewLine);
        }

        private static void PrintCmd(string command, params string[] args)
        {
            Cmd(command);

            if (args != null)
            {
                for (int i = 0; i < args.Length; i++)
                    Arg(args[i]);
            }

            End();
        }

        private static void PrintDsecr(params string[] description)
        {
            if (description != null)
            {
                for (int i = 0; i < description.Length; i++)
                    Descr(description[i]);

                End();
            }
        }

        #endregion

        #region Show

        public static void Show()
        {
            PrintDsecr("Logic.Console Commands:");
            End();

            PrintCmd("HELP");
            PrintDsecr("Show program help text");

            PrintCmd("QUIT");
            PrintDsecr("Quit program");

            PrintCmd("CLS");
            PrintDsecr("Clear console buffer");

            PrintCmd("LS");
            PrintDsecr("List all elements");

            PrintCmd("STATE", "<Name>", "<True|False|Null>");
            PrintDsecr("Set element state");

            PrintCmd("STATE");
            PrintDsecr("Show elements with state");

            PrintCmd("DEBUG", "<On|Off>");
            PrintDsecr("Enable/disable debug output");

            PrintCmd("LOG", "<On|Off>");
            PrintDsecr("Enable/disable log to file for Run");

            PrintCmd("RUN");
            PrintDsecr("Run simulation for all contexts");

            PrintCmd("RUN", "<Milliseconds>");
            PrintDsecr("Run simulation in periodic time intervals");
            PrintDsecr("Cycle interval is in milliseconds (default 100ms)");

            PrintCmd("BREAK");
            PrintDsecr("Stop periodic simulation");

            PrintCmd("EXECUTE", "<FileName>");
            PrintDsecr("Execute commands from .txt file");

            PrintCmd("RESET");
            PrintDsecr("Reset current solution");

            PrintCmd("PACK", "<FileName>", "<FileName>");
            PrintDsecr("Compress binary file");

            PrintCmd("UNPACK", "<FileName>", "<FileName>");
            PrintDsecr("DeCompress binary file");

            End();
            PrintDsecr("Notes:");
            End();
            PrintDsecr("- All commands and arguments are case insensitive.");
            PrintDsecr("- Use '//' for comments at the beginning of a line.");
            PrintDsecr("- All commands can be executed from .txt script files.");
            PrintDsecr("- It is possible to execute scripts within scripts.");

            End();
            PrintDsecr("Command-line:");
            End();
            PrintCmd("Logic.Console", "<FileName>");
            PrintDsecr("Execute commands from .txt file and run simulation in 100ms intervals");
            PrintCmd("Logic.Console", "<FileName>", "<Milliseconds>");
            PrintDsecr("Execute commands from .txt file and run simulation in ms intervals");
        }

        #endregion
    }

    #endregion
}
