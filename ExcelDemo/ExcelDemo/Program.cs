using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;

namespace ExcelDemo
{
    public class ExcelReader
    {
        public IList<object[,]> Open(string path)
        {
            Application app = null;
            Workbook wb = null;
            try
            {
                app = new Application();
                wb = app.Workbooks.Open(path);
                
                var values = new List<object[,]>();
                Scan(wb, values);
                
                wb.Close(false, path, null);
                app.Quit();
                
                Marshal.ReleaseComObject(wb);
                Marshal.ReleaseComObject(app);
                
                wb = null;
                app = null;
                
                return values;
            }
            catch (Exception ex)
            {
                if (wb != null)
                    Marshal.ReleaseComObject(wb);
                
                if (app != null)
                    Marshal.ReleaseComObject(app);
                
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
            return null;
        }

        private void Scan(Workbook workbook, IList<object[,]> values)
        {
            int numSheets = workbook.Sheets.Count;

            for (int sheetNum = 1; sheetNum < numSheets + 1; sheetNum++)
            {
            	var sheet = (Worksheet)workbook.Sheets[sheetNum];
            	
            	Range range = sheet.UsedRange;
            	var value = (object[,])
            	    range.get_Value(XlRangeValueDataType.xlRangeValueDefault);
            	
            	Marshal.ReleaseComObject(sheet);
                Marshal.ReleaseComObject(range);
                
            	values.Add(value);
            }
        }
    }
    
    class Program
    {
        public static void Main(string[] args)
        {
            //Console.WriteLine(System.IO.Directory.GetCurrentDirectory());
      
            var xlsPath = @"t.xls";
            var csvPath = @"t.csv";

            var r = new ExcelReader();
            var values = r.Open(xlsPath);
            
            var value = values[0];

            try
            {
                WriteCsv(csvPath, value, separator: ';');
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
 
            Console.Write("Done . . .");
            Console.ReadKey(true);
        }

        public static void WriteCsv(string path, object[,] value, char separator)
        {
            int rows = value.GetLength(0);
            int columns = value.GetLength(1);
            using (var sw = System.IO.File.CreateText(path))
            {
                for (int row = 1; row <= rows; row++)
                {
                    for (int column = 1; column <= columns; column++)
                    {
                        var cell = value[row, column];
                        sw.Write(cell);
                        if (column < columns)
                            sw.Write(separator);
                    }
                    sw.Write(Environment.NewLine);
                }
            }
        }
    }
}
