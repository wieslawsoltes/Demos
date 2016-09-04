using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace LogicCanvas.Helpers
{
    public static class CsvFile
    {
        public static IEnumerable<Row> ParseToIEnumerable(string path, params char[] separator)
        {
            using (System.IO.TextReader r = new StreamReader(path))
            {
                while (true)
                {
                    var s = r.ReadLine();
                    if (s == null)
                    {
                        break;
                    }

                    var col = s.Split(separator);
                    Row row = new Row();
                    foreach (var c in col)
                    {
                        row.Add(new Column() 
                        { 
                            Value = c 
                        });
                    }

                    yield return row;
                }
            }
        }

        public static RowList ParseToRowList(string path, params char[] separator)
        {
            RowList rows = new RowList();
            try
            {
                using (System.IO.TextReader r = new StreamReader(path))
                {
                    while (true)
                    {
                        var s = r.ReadLine();
                        if (s == null)
                        {
                            break;
                        }

                        var col = s.Split(separator);
                        Row row = new Row();
                        foreach (var c in col)
                        {
                            row.Add(new Column() 
                            { 
                                Value = c 
                            });
                        }

                        rows.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                return null;
            }

            return rows;
        }

        public static void SaveToXml(string path, RowList rows)
        {
            try
            {
                using (System.IO.TextWriter writer = new System.IO.StreamWriter(path))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(RowList));
                    serializer.Serialize(writer, rows);
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
            }
        }

        #region Data Types

        [XmlType("Rows")]
        public class RowList : List<Row>
        {

        }

        [XmlType("Row")]
        public class Row : List<Column>
        {

        }

        [XmlType("Column")]
        public class Column
        {
            [XmlAttribute("Value")]
            public string Value { get; set; }
        }

        #endregion
    }
}
