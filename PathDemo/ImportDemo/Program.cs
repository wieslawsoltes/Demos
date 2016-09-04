// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestPATH;

namespace ImportDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // https://github.com/google/material-design-icons/
            var path = @"g:\Dropbox\ICONS\material-design-icons-master";
            var output = "sources.txt";

            using (var tw = File.CreateText(output))
            {
                MaterialDesignIconsImporter.Find(path, "production", tw);
            }

            var sources = MaterialDesignIconsImporter.ToSources(output);

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
