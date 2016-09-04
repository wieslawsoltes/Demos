using Processing.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Processing.WPF
{
    public class GeometryProperties
    {
        public static string GetData(DependencyObject obj)
        {
            return (string)obj.GetValue(DataProperty);
        }

        public static void SetData(DependencyObject obj, string value)
        {
            obj.SetValue(DataProperty, value);
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.RegisterAttached(
            "Data",
            typeof(string),
            typeof(GeometryProperties),
            new PropertyMetadata(null, OnDataChanged));

        private static void OnDataChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            var path = source as Path;
            var pathGeometry = source as GeometryGroup;

            if (path != null)
            {
                pathGeometry = new GeometryGroup();
                path.Data = pathGeometry;
            }

            if (pathGeometry != null)
            {
                Compiler.Execute(
                    new GeometryCommands(
                        new GeometryContext(pathGeometry)), 
                    args.NewValue as string);
            }
        }
    }
}
