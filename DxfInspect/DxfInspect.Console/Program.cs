using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace DxfInspect.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            // Convert all .dxf files format to HTML format in path.
            // Demo.exe <Path>
            if (args.Length == 1)
            {
                string[] paths = System.IO.Directory.GetFiles(args[0], "*.dxf");
                foreach (string path in paths)
                {
                    Dxf.DxfInspect.Convert(path, path + ".html");
                }
            }
            // Convert .dxf file to HTML format.
            // Demo.exe <file.dxf> <file.html>
            else if (args.Length == 2)
            {
                Dxf.DxfInspect.Convert(args[0], args[1]);
            }
        }
    }
}
