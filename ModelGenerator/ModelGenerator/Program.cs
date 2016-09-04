// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace ModelGenerator
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.IO;

    #endregion

    #region Program

    class Program
    {
        #region Main

        static void Main(string[] args)
        {
            try
            {
                string code = CmdParser.Parse("Sample02.txt");

                Console.WriteLine(code);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            Console.ReadLine();
        }

        #endregion
    }

    #endregion

    #region Command Enum

    public enum Cmd
    {
        Namespace,
        AbstractClass,
        Interface,
        Class,
        Property,
        List,
        Function
    }

    #endregion

    #region Command Parser

    public static class CmdParser
    {
        #region Fields

        private static char[] trimChars = new char[] { ' ', '\t' };
        private static char[] splitChars = new char[] { ' ', '\t' };
        private static string indent = "    ";
        private static Stack<Cmd> stack = new Stack<Cmd>();
        private static StringBuilder sb = new StringBuilder();
        private static StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries;

        #endregion

        #region Parser

        public static string Parse(string path)
        {
            using (var stream = new StreamReader(path))
            {
                string line;

                while ((line = stream.ReadLine()) != null)
                {
                    string[] cmd = line.Trim(trimChars).Split(splitChars, splitOptions);

                    if (cmd.Length <= 0 || cmd[0] == string.Empty)
                        continue;

                    switch (cmd[0])
                    {
                        // supported commands
                        case "N":
                            CmdNamespace(cmd);
                            break;
                        case "A":
                            CmdAbstractClass(cmd);
                            break;
                        case "I":
                            CmdInterface(cmd);
                            break;
                        case "C":
                            CmdClass(cmd);
                            break;
                        case "P":
                            CmdProperty(cmd);
                            break;
                        case "L":
                            CmdList(cmd);
                            break;
                        case "F":
                            CmdFunction(cmd);
                            break;
                        // skip begin/end blocks
                        case "{":
                        case "}":
                        case "[":
                        case "]":
                        case "<":
                        case ">":
                            break;
                        // unknown command
                        default:
                            throw new Exception("Unknown command.");
                    }
                }
            }

            CloseLastCmd();

            return sb.ToString();
        }

        private static void ClosePreviousCmd()
        {
            // check for previous command
            if (stack.Count > 0)
            {
                if (stack.Peek() != Cmd.Namespace)
                {
                    var previous = stack.Pop();

                    // close class, interface
                    sb.Append(indent + "}");
                    sb.Append(Environment.NewLine);
                    sb.Append(Environment.NewLine);

                    if (stack.Count > 1 || stack.Peek() != Cmd.Namespace)
                    {
                        // Invalid stack.
                        throw new Exception("Invalid stack.");
                    }
                }
                else
                {
                    // Previous command was namespace.
                }
            }
        }

        private static void CloseLastCmd()
        {
            // check for previous command
            if (stack.Count > 0)
            {
                var last = stack.Pop();
                if (last != Cmd.Namespace)
                {
                    sb.Append(indent + "}");
                    sb.Append(Environment.NewLine);

                    if (stack.Count == 1)
                    {
                        var ns = stack.Pop();

                        if (ns == Cmd.Namespace)
                        {
                            // close namespace
                            sb.Append("}");
                            sb.Append(Environment.NewLine);
                        }
                        else
                        {
                            // Invalid stack.
                            throw new Exception("Invalid stack.");
                        }
                    }
                    else
                    {
                        // Invalid stack.
                        throw new Exception("Invalid stack.");
                    }
                }
                else
                {
                    // close namespace
                    sb.Append("}");
                    sb.Append(Environment.NewLine);
                }
            }
            else
            {
                // First command in script.
            }
        }

        private static void CmdNamespace(string[] cmd)
        {
            CloseLastCmd();

            if (cmd == null || cmd.Length < 2)
            {
                // Invalid namespace command format.
                throw new Exception("Invalid namespace command format.");
            }

            // namespace
            sb.Append(Environment.NewLine + "namespace" + " ");

            // name
            for (int i = 1; i < cmd.Length; i++)
            {
                if (i == 1)
                    sb.Append(cmd[i]);
                else
                    sb.Append("." + cmd[i]);
            }

            sb.Append(Environment.NewLine + "{" + Environment.NewLine);

            stack.Push(Cmd.Namespace);
        }

        private static void CmdAbstractClass(string[] cmd)
        {
            ClosePreviousCmd();

            if (cmd == null || cmd.Length < 2)
            {
                // Invalid abstract class command format.
                throw new Exception("Invalid abstract class command format.");
            }

            sb.Append(indent + "public abstract class" + " ");

            // name
            sb.Append(cmd[1]);

            // params
            for (int i = 2; i < cmd.Length; i++)
            {
                if (i == 2)
                    sb.Append(" : " + cmd[i]);
                else
                    sb.Append(", " + cmd[i]);
            }

            sb.Append(Environment.NewLine + indent + "{" + Environment.NewLine);

            stack.Push(Cmd.AbstractClass);
        }

        private static void CmdInterface(string[] cmd)
        {
            ClosePreviousCmd();

            if (cmd == null || cmd.Length < 2)
            {
                // Invalid interface command format.
                throw new Exception("Invalid interface command format.");
            }

            sb.Append(indent + "public interface" + " ");

            // name
            sb.Append(cmd[1]);

            // params
            for (int i = 2; i < cmd.Length; i++)
            {
                if (i == 2)
                    sb.Append(" : " + cmd[i]);
                else
                    sb.Append(", " + cmd[i]);
            }

            sb.Append(Environment.NewLine + indent + "{" + Environment.NewLine);

            stack.Push(Cmd.Interface);
        }

        private static void CmdClass(string[] cmd)
        {
            ClosePreviousCmd();

            if (cmd == null || cmd.Length < 2)
            {
                // Invalid class command format.
                throw new Exception("Invalid class command format.");
            }

            sb.Append(indent + "public class" + " ");

            // name
            sb.Append(cmd[1]);

            // params
            for (int i = 2; i < cmd.Length; i++)
            {
                if (i == 2)
                    sb.Append(" : " + cmd[i]);
                else
                    sb.Append(", " + cmd[i]);
            }

            sb.Append(Environment.NewLine + indent + "{" + Environment.NewLine);

            stack.Push(Cmd.Class);
        }

        private static void CmdProperty(string[] cmd)
        {
            if (stack.Count <= 0)
            {
                // Invalid stack.
                throw new Exception("Invalid stack.");
            }

            if (cmd == null || cmd.Length != 3)
            {
                // Invalid property command format.
                throw new Exception("Invalid property command format.");
            }

            var peek = stack.Peek();
            if (peek != Cmd.AbstractClass
                && peek != Cmd.Class
                && peek != Cmd.Interface)
            {
                // Unexpected property command.
                throw new Exception("Unexpected property command.");
            }

            // public
            sb.Append(indent + indent + "public" + " ");

            // type
            sb.Append(cmd[2] + " ");

            // name
            sb.Append(cmd[1]);

            sb.Append(" " + "{ get; set; }" + Environment.NewLine);
        }

        private static void CmdList(string[] cmd)
        {
            if (stack.Count <= 0)
            {
                // Invalid stack.
                throw new Exception("Invalid stack.");
            }

            if (cmd == null || cmd.Length != 3)
            {
                // Invalid list command format.
                throw new Exception("Invalid list command format.");
            }

            var peek = stack.Peek();
            if (peek != Cmd.AbstractClass
                && peek != Cmd.Class
                && peek != Cmd.Interface)
            {
                // Unexpected list command.
                throw new Exception("Unexpected list command.");
            }

            // public
            sb.Append(indent + indent + "public ");

            // type
            sb.Append("List<" + cmd[2] + ">" + " ");

            // name
            sb.Append(cmd[1]);

            sb.Append(" " + "{ get; set; }" + Environment.NewLine);
        }

        private static void CmdFunction(string[] cmd)
        {
            if (stack.Count <= 0)
            {
                // Invalid stack.
                throw new Exception("Invalid stack.");
            }

            if (cmd == null || cmd.Length < 3)
            {
                // Invalid list command format.
                throw new Exception("Invalid list command format.");
            }

            var peek = stack.Peek();
            if (peek != Cmd.AbstractClass
                && peek != Cmd.Interface)
            {
                // Unexpected function command.
                throw new Exception("Unexpected function command.");
            }

            // public
            sb.Append(indent + indent + "public" + " ");

            // return
            sb.Append(cmd[2] + " ");

            // name
            sb.Append(cmd[1]);

            // params:types
            sb.Append("(");

            for (int i = 3; i < cmd.Length; i++)
            {
                string[] arg = cmd[i].Split(new char[] { ':' },
                    StringSplitOptions.RemoveEmptyEntries);

                if (arg.Length == 2)
                {
                    if (i == 3)
                        sb.Append(arg[1] + " " + arg[0]);
                    else
                        sb.Append(", " + arg[1] + " " + arg[0]);
                }
            }

            sb.Append(");" + Environment.NewLine);
        }

        #endregion
    }

    #endregion
}
