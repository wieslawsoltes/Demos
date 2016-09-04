// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace BlockDesigner
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Markup;
    using System.Xml;

    #endregion

    #region ExportXaml

    public class XamlExporter
    {
        public static void WriteToFile(string fileName, string text)
        {
            using (var stream = new System.IO.StreamWriter(fileName))
            {
                stream.Write(text);
            }
        }

        public static string GetXaml(object obj, string indent)
        {
            var sb = new StringBuilder();
            var writer = System.Xml.XmlWriter.Create(sb, new System.Xml.XmlWriterSettings
            {
                Indent = true,
                IndentChars = indent,
                ConformanceLevel = ConformanceLevel.Fragment,
                OmitXmlDeclaration = true
            });

            var mgr = new XamlDesignerSerializationManager(writer)
            {
                XamlWriterMode = XamlWriterMode.Expression
            };

            System.Windows.Markup.XamlWriter.Save(obj, mgr);

            return sb.ToString();
        }

        public static string GetControlTemplate(object obj, string key)
        {
            string objXaml = GetXaml(obj, "    ");

            string openTag = string.Concat("<ControlTemplate x:Key=\"",
                                           key,
                                           //"\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">\n");
                                           "\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n");

            string closeTag = "</ControlTemplate>";

            return string.Concat(openTag, objXaml, closeTag); ;
        }

        public static string GetBlockEllipseStyle()
        {
            return
                "<Style x:Key=\"BlockEllipseKey\" TargetType=\"Ellipse\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
                    "<Setter Property=\"Stroke\" Value=\"Red\"/>" +
                    "<Setter Property=\"Fill\" Value=\"Red\"/>" +
                    "<Setter Property=\"StrokeThickness\" Value=\"1.0\"/>" +
                    "<Setter Property=\"Width\" Value=\"8.0\"/>" +
                    "<Setter Property=\"Height\" Value=\"8.0\"/>" +
                    "<Setter Property=\"Margin\" Value=\"-4,-4,0,0\"/>" +
                    "<Setter Property=\"SnapsToDevicePixels\" Value=\"True\"/>" +
                "</Style>";
        }

        public static string GetResourceDictionary(IEnumerable<Tuple<string, object>> blocks)
        {
            var sb = new StringBuilder();

            string openTag = "<ResourceDictionary xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">";
            string closeTag = "</ResourceDictionary>";

            // add resource dictionary open tag
            sb.AppendLine(openTag);

            // create control styles

            string ellipseStyleText = GetBlockEllipseStyle();
            sb.AppendLine(ellipseStyleText);

            // create control templates
            foreach (var tuple in blocks)
            {
                string objText = GetControlTemplate(tuple.Item2, tuple.Item1);

                sb.AppendLine(objText);
            }

            // add resource dictionary close tag
            sb.AppendLine(closeTag);

            // return resource dictionary xaml string
            return sb.ToString();
        }

        public static string FormatXml(string xml)
        {
            XmlDocument doc = new XmlDocument();

            doc.LoadXml(xml);

            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";
            settings.NewLineChars = "\r\n";
            settings.NewLineHandling = NewLineHandling.Replace;
            settings.OmitXmlDeclaration = true;

            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                doc.Save(writer);
            }

            return sb.ToString();
        }
    }

    #endregion
}
