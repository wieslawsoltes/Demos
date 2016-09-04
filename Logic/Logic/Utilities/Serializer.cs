// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Utilities
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;

    #endregion

    #region Serializer

    public static class Serializer
    {
        #region Newtonsoft Json Serializer

        public static void SaveJson<T>(T item, string fileName)
        {
            var settings = new Newtonsoft.Json.JsonSerializerSettings()
            {
                PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.All,
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects
            };

            string data = Newtonsoft.Json.JsonConvert.SerializeObject(item, Newtonsoft.Json.Formatting.Indented, settings);

            using (System.IO.TextWriter w = new System.IO.StreamWriter(fileName, false, Encoding.UTF8))
            {
                w.Write(data);
            }
        }

        public static void SaveJsonNoReference<T>(T item, string fileName)
        {
            var settings = new Newtonsoft.Json.JsonSerializerSettings()
            {
                PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None,
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None
            };

            string data = Newtonsoft.Json.JsonConvert.SerializeObject(item, Newtonsoft.Json.Formatting.Indented, settings);

            using (System.IO.TextWriter w = new System.IO.StreamWriter(fileName, false, Encoding.UTF8))
            {
                w.Write(data);
            }
        }

        public static T OpenJson<T>(string fileName) where T : class
        {
            using (System.IO.TextReader r = new System.IO.StreamReader(fileName, Encoding.UTF8))
            {
                string data = r.ReadToEnd();

                var settings = new Newtonsoft.Json.JsonSerializerSettings()
                {
                    PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.All,
                    TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects
                };

                T item;

                try
                {
                    item = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(data, settings);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(string.Format("Error: {0}", ex.Message));

                    return null;
                }

                return item;
            }
        }

        public static T OpenJsonNoReference<T>(string fileName) where T : class
        {
            using (System.IO.TextReader r = new System.IO.StreamReader(fileName, Encoding.UTF8))
            {
                string data = r.ReadToEnd();

                var settings = new Newtonsoft.Json.JsonSerializerSettings()
                {
                    PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None,
                    TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None
                };

                T item;

                try
                {
                    item = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(data, settings);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(string.Format("Error: {0}", ex.Message));

                    return null;
                }

                return item;
            }
        }

        #endregion

        #region XML DataContractSerializer

        public static Type[] GetTypes()
        {
            var q = from t in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && t.Namespace == "Logic.Model"
                    select t;

            return q.ToArray();
        }

        public static T OpenXml<T>(string fileName)
        {
            var s = new DataContractSerializer(typeof(T), GetTypes(), int.MaxValue, true, true, null);
            using (var reader = XmlReader.Create(fileName))
            {
                return (T)s.ReadObject(reader);
            }
        }

        public static void SaveXml<T>(T item, string fileName)
        {
            if (item != null)
            {
                var s = new DataContractSerializer(item.GetType(), GetTypes(), int.MaxValue, true, true, null);
                using (var writer = XmlWriter.Create(fileName, new XmlWriterSettings() { Indent = true, IndentChars = "    " }))
                {
                    s.WriteObject(writer, item);
                }
            }
        }

        #endregion
    }

    #endregion
}
