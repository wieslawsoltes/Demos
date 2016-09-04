
namespace Logic.Model
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Logic.Model;

    #endregion

    #region Print

    public static class Print
    {
        #region States

        public static void States(Solution solution)
        {
            var projects = solution.Children.Cast<Project>();
            if (projects == null)
                return;

            var contexts = projects.SelectMany(x => x.Children).Cast<Context>();
            if (contexts == null)
                return;

            var elements = contexts.SelectMany(x => x.Children).Where(y => y is IStateSimulation);
            if (elements == null)
                return;

            foreach (var element in elements)
            {
                var simulation = (element as IStateSimulation).Simulation;
                var state = simulation == null ? null : simulation.State;

                Console.WriteLine("Id: {0} | State: {1}", element.Id, state != null ? state.State.ToString() : "<Null>");
            }
        }

        #endregion

        #region Solution

        public static void Solution(Solution solution)
        {
            Print.Element(solution, false);

            var projects = solution.Children.Cast<Project>();

            foreach (var project in projects)
            {
                Print.Element(project, false);

                var contexts = project.Children.Cast<Context>();

                foreach (var context in contexts)
                {
                    Print.Element(context, false);

                    var elements = context.Children;

                    foreach (var element in elements)
                    {
                        if (element.Parent is Context == true)
                            Print.Element(element, true);
                    }
                }
            }
        }

        #endregion

        #region Element

        public static void Element(Element element, bool children)
        {
            Console.WriteLine("{0}, Id: {1}, Parent: {2}",
            element.GetType().ToString().Split('.').Last(),
            element.Id,
            element.Parent != null ? element.Parent.Id : UInt32.MaxValue);

            // print children
            if (children == true && element.Children != null)
            {
                foreach (var child in element.Children)
                {
                    Console.WriteLine("    {0}, Id: {1}, Parent: {2}",
                    child.GetType().ToString().Split('.').Last(),
                    child.Id,
                    child.Parent != null ? child.Parent.Id : UInt32.MaxValue);
                }
            }
        }

        #endregion

        #region Pin

        public static void Pin(Pin pin)
        {
            Console.WriteLine("Pin: {0} | Parent: {1} | Type: {2}",
            pin.Id,
            (pin.Parent != null) ? pin.Parent.Id : UInt32.MaxValue,
            pin.Type);

            if (pin.Connections != null)
            {
                foreach (var connection in pin.Connections)
                {
                    Console.WriteLine("    Connection: {0} | Inverted: {1} | Parent: {2} | Type: {3}",
                    connection.Item1.Id,
                    connection.Item2,
                    (connection.Item1.Parent != null) ? connection.Item1.Parent.Id : UInt32.MaxValue,
                    connection.Item1.Type);
                }
            }
            else
                Console.WriteLine("    <No Connections>");

            Console.WriteLine("");
        }

        #endregion
    }

    #endregion
}
