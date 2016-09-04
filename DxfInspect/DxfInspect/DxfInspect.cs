﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dxf
{
    #region DxfRawTag

    public class DxfRawTag
    {
        public bool IsEnabled { get; set; }
        public int GroupCode { get; set; }
        public string DataElement { get; set; }
        public DxfRawTag Parent { get; set; }
        public List<DxfRawTag> Children { get; set; }
        public string Dxf
        {
            get
            {
                var sb = new StringBuilder();
                ToDxf(this, sb);
                return sb.ToString();
            }
        }

        private static void ToDxf(DxfRawTag tag, StringBuilder sb)
        {
            if (tag.IsEnabled && sb != null)
            {
                sb.Append(tag.GroupCode);
                sb.Append(Environment.NewLine);
                sb.Append(tag.DataElement);
                sb.Append(Environment.NewLine);
                if (tag.Children != null)
                {
                    for (int i = 0; i < tag.Children.Count; i++)
                    {
                        var child = tag.Children[i];
                        if (child.IsEnabled)
                        {
                            ToDxf(child, sb);
                        }
                    }
                }
            }
        }
        public DxfRawTag()
        {
            IsEnabled = true;
        }
        public override string ToString()
        {
            if (Children != null)
            {
                var tagName = Children.FirstOrDefault(t => t.GroupCode == 2);
                if (tagName != null)
                {
                    return string.Concat(DataElement, ':', tagName.DataElement);
                }
            }
            return (GroupCode == 0 || GroupCode == 2) ? DataElement : string.Concat(GroupCode.ToString(), ',', DataElement);
        }
    }

    #endregion

    #region DxfInspect

    public static class DxfInspect
    {
        #region Constants

        public const int DxfCodeForType = 0;
        public const int DxfCodeForName = 2;
        public const string DxfCodeNameSection = "SECTION";
        public const string DxfCodeNameEndsec = "ENDSEC";

        #endregion

        #region Inspect

        public async static Task<string> ToHtml(string path)
        {
            try
            {
                var dxf = await Read(path);
                var sections = await Task.Run(() => Parse(dxf));
                var fileName = System.IO.Path.GetFileName(path);
                return await Task.Run(() => ToHtml(sections, fileName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            return null;
        }

        public async static Task<string> Read(string path)
        {
            using (var reader = new System.IO.StreamReader(path))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public static List<DxfRawTag> Parse(string text)
        {
            var lines = text.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();

            if (lines.Length % 2 != 0)
            {
                throw new Exception("Invalid number of lines.");
            }

            var sections = new List<DxfRawTag>();

            DxfRawTag section = null;
            DxfRawTag other = null;

            for (int i = 0; i < lines.Length; i += 2)
            {
                var tag = new DxfRawTag();
                tag.GroupCode = int.Parse(lines[i]);
                tag.DataElement = lines[i + 1];

                bool isEntityWithType = tag.GroupCode == DxfCodeForType;
                bool isSectionStart = (isEntityWithType) && tag.DataElement == DxfCodeNameSection;
                bool isSectionEnd = (isEntityWithType) && tag.DataElement == DxfCodeNameEndsec;

                if (isSectionStart)
                {
                    section = tag;
                    section.Children = new List<DxfRawTag>();
                    sections.Add(section);
                    other = null;
                }
                else if (isSectionEnd)
                {
                    tag.Parent = section;
                    section.Children.Add(tag);
                    section = null;
                    other = null;
                }
                else
                {
                    if (section != null)
                    {
                        if (isEntityWithType && other == null)
                        {
                            other = tag;
                            other.Parent = section;
                            other.Children = new List<DxfRawTag>();
                            section.Children.Add(other);
                        }
                        else if (isEntityWithType && other != null)
                        {
                            other = tag;
                            other.Parent = section;
                            other.Children = new List<DxfRawTag>();
                            section.Children.Add(other);
                        }
                        else if (!isEntityWithType && other != null)
                        {
                            tag.Parent = other;
                            other.Children.Add(tag);
                        }
                        else
                        {
                            tag.Parent = section;
                            section.Children.Add(tag);
                        }
                    }
                    else
                    {
                        tag.Parent = null;
                        sections.Add(tag);
                    }
                }
            }

            return sections;
        }

        public static string ToHtml(List<DxfRawTag> sections, string fileName)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendFormat("<title>{0}</title>{1}", fileName, Environment.NewLine);
            sb.AppendLine("<meta charset=\"utf-8\"/>");
            sb.AppendLine("<style type=\"text/css\">");
            sb.AppendLine("body     { background-color:rgb(221,221,221); }");
            sb.AppendLine(".Table   { display:none; font-family:\"Courier New\"; font-size:10pt; border-collapse:collapse; margin:10px; float:none; }");
            sb.AppendLine(".Header  { display:table-row; font-weight:normal; text-align:left; background-color:rgb(255,30,102); }");
            sb.AppendLine(".Section { display:table-row; font-weight:normal; text-align:left; background-color:rgb(255,242,102); }");
            sb.AppendLine(".Other   { display:table-row; font-weight:normal; text-align:left; background-color:rgb(191,191,191); }");
            sb.AppendLine(".Row     { display:table-row; background-color:rgb(221,221,221); }");
            sb.AppendLine(".Cell    { display:table-cell; padding-left:5px; padding-right:5px; border:none; }");
            sb.AppendLine(".Line    { overflow:hidden; white-space:nowrap; text-overflow:ellipsis; width:60px; color:rgb(84,84,84); }");
            sb.AppendLine(".Code    { overflow:hidden; white-space:nowrap; text-overflow:ellipsis; width:50px; color:rgb(116,116,116); }");
            sb.AppendLine(".Data    { overflow:hidden; white-space:nowrap; text-overflow:ellipsis; width:300px; color:rgb(0,0,0); }");
            sb.AppendLine(".Toggle  { display:table; cursor:hand; font-family:\"Courier New\"; font-size:14pt; font-weight:normal; text-align:left; border-collapse:collapse; margin-left:10px; margin-top:10px; float:none; width:440px; background-color:rgb(191,191,191); }");
            sb.AppendLine("</style>");
            sb.AppendLine("<script type=\"text/javascript\"> function toggle_visibility(id) { var e = document.getElementById(id); if(e.style.display == 'table') e.style.display = 'none'; else e.style.display = 'table'; } </script>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            int lineNumber = 0;
            for (int i = 0; i < sections.Count; i++)
            {
                var section = sections[i];

                if (section.IsEnabled)
                {
                    // section
                    sb.AppendFormat("{3}<div class=\"Toggle\" onclick=\"toggle_visibility('{0}');\"><p>{1} {2}</p></div>{3}", i, section.DataElement, (section.Children != null) && (section.Children.Count > 0) && (section.Children[0].GroupCode == DxfCodeForName) ? section.Children[0].DataElement : "<Unknown>", Environment.NewLine);
                    //sb.AppendFormat("<!-- SECTION i={0} -->{1}", i, Environment.NewLine);
                    sb.AppendFormat("<div class=\"Table\" id=\"{0}\">{1}", i, Environment.NewLine);
                    sb.AppendFormat("    <div class=\"Header\"><div class=\"Cell\"><p>LINE</p></div><div class=\"Cell\"><p>CODE</p></div><div class=\"Cell\"><p>DATA</p></div></div>{0}", Environment.NewLine);
                    sb.AppendFormat("    <div class=\"{0}\"><div class=\"Cell\"><p class=\"Line\">{1}</p></div><div class=\"Cell\"><p class=\"Code\">{2}:</p></div><div class=\"Cell\"><p class=\"Data\">{3}</p></div></div>{4}", "Section", lineNumber += 2, section.GroupCode, section.DataElement, Environment.NewLine);

                    if (section.Children != null)
                    {
                        for (int j = 0; j < section.Children.Count; j++)
                        {
                            var child = section.Children[j];
                            if (child.IsEnabled)
                            {
                                bool isEntityWithType = child.GroupCode == DxfCodeForType;
                                if (isEntityWithType)
                                {
                                    var other = child;

                                    // entity with children (type)
                                    //sb.AppendFormat("    <!-- OTHER j={0} -->{1}", j, Environment.NewLine);
                                    sb.AppendFormat("    <div class=\"{0}\"><div class=\"Cell\"><p class=\"Line\">{1}</p></div><div class=\"Cell\"><p class=\"Code\">{2}:</p></div><div class=\"Cell\"><p class=\"Data\">{3}</p></div></div>{4}", "Other", lineNumber += 2, other.GroupCode, other.DataElement, Environment.NewLine);

                                    if (other.Children != null)
                                    {
                                        for (int k = 0; k < other.Children.Count; k++)
                                        {
                                            var entity = other.Children[k];
                                            if (entity.IsEnabled)
                                            {
                                                // entity without type
                                                //sb.AppendFormat("        <!-- ENTITY k={0} -->{1}", k, Environment.NewLine);
                                                sb.AppendFormat("        <div class=\"Row\"><div class=\"Cell\"><p class=\"Line\">{0}</p></div><div class=\"Cell\"><p class=\"Code\">{1}:</p></div><div class=\"Cell\"><p class=\"Data\">{2}</p></div></div>{3}", lineNumber += 2, entity.GroupCode, entity.DataElement, Environment.NewLine);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    var entity = child;

                                    // entity without children (type)
                                    //sb.AppendFormat("    <!-- ENTITY j={0} -->{1}", j, Environment.NewLine);
                                    sb.AppendFormat("    <div class=\"Row\"><div class=\"Cell\"><p class=\"Line\">{0}</p></div><div class=\"Cell\"><p class=\"Code\">{1}:</p></div><div class=\"Cell\"><p class=\"Data\">{2}</p></div></div>{3}", lineNumber += 2, entity.GroupCode, entity.DataElement, Environment.NewLine);
                                }
                            }
                        }
                    }

                    sb.AppendLine("</div>");
                }
            }

            sb.AppendFormat("{0}</body></html>", Environment.NewLine);

            return sb.ToString();
        }

        public async static void Convert(string dxfPath, string htmlPath)
        {
            try
            {
                var html = await DxfInspect.ToHtml(dxfPath);
                Save(htmlPath, html);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public async static void Save(string path, string text)
        {
            try
            {
                if (text != null)
                {
                    using (var stream = System.IO.File.CreateText(path))
                    {
                        await stream.WriteAsync(text);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        #endregion
    }

    #endregion
}
