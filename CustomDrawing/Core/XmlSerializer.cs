#region References

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;

#endregion

namespace CustomDrawing.Core
{
    #region XmlSerializer

    public static class XmlSerializer
    {
        #region Fields

        private static DataContractSerializer Serializer =
            new DataContractSerializer(typeof(List<Element>),
                new Type[] 
                { 
                    typeof(Pin), 
                    typeof(Line), 
                    typeof(Rectangle), 
                    typeof(Text), 
                    typeof(Custom), 
                    typeof(Reference) 
                },
                int.MaxValue, false, true, null);

        #endregion

        #region Serializer

        public static string Serialize(List<Element> elements)
        {
            using (var output = new StringWriter())
            {
                using (var writer = new XmlTextWriter(output) { Formatting = Formatting.Indented })
                {
                    Serializer.WriteObject(writer, elements);
                    return output.GetStringBuilder().ToString();
                }
            }
        }

        public static List<Element> DeSerialize(string xml)
        {
            using (var input = new StringReader(xml))
            {
                using (var reader = XmlTextReader.Create(input))
                {
                    return (List<Element>)Serializer.ReadObject(reader);
                }
            }
        }

        #endregion
    }

    #endregion
}
