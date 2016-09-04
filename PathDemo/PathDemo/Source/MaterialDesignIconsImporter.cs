// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace TestPATH
{
    public static class MaterialDesignIconsImporter
    {
        private static IEnumerable<XmlNode> GetAllChildNodes(XmlNodeList nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                yield return nodes[i];
                if (nodes[i].HasChildNodes)
                {
                    foreach (var node in GetAllChildNodes(nodes[i].ChildNodes))
                    {
                        yield return node;
                    }
                }
            }
        }

        public static void Find(string path, string sub, TextWriter tw)
        {
            var dirs = Directory.EnumerateDirectories(path, "*.*", SearchOption.TopDirectoryOnly);
            if (dirs != null)
            {
                foreach (var dir in dirs)
                {
                    Find(dir, sub, tw);
                    if (Path.GetFileNameWithoutExtension(dir) == sub)
                    {
                        var files = Directory.EnumerateFiles(dir, "*.svg", SearchOption.TopDirectoryOnly);
                        foreach (var file in files)
                        {
                            try
                            {
                                using (var xml = File.OpenRead(file))
                                {
                                    var doc = new XmlDocument();
                                    doc.Load(xml);
                                    if (doc.HasChildNodes)
                                    {
                                        var nodes = GetAllChildNodes(doc.ChildNodes);
                                        var p = nodes.Where(n => n.Name == "path").LastOrDefault();
                                        if (p != null)
                                        {
                                            var value = p.Attributes["d"].Value;
                                            tw.WriteLine(Path.GetFileNameWithoutExtension(file) + "=" + value);
                                        }
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("[Error reading file: " + Path.GetFileName(file) + "]");
                            }
                        }
                    }
                }
            }
        }

        public static IList<Source> ToSources(string output)
        {
            var sources = new List<Source>();

            using (var tr = File.OpenText(output))
            {
                while (true)
                {
                    var line = tr.ReadLine();
                    if (line == null)
                        break;

                    var split = line.Split('=');
                    if (split == null || split.Length != 2)
                        continue;

                    var source = new Source()
                    {
                        Name = split[0],
                        Value = split[1]
                    };
                    sources.Add(source);
                }
            }

            return sources;
        }
    }
}
