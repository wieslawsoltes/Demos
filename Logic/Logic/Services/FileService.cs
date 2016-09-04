// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Services
{
    #region References

    using Logic.Model;
    using Logic.Model.Core;
    using Logic.Utilities;
    using Logic.ViewModels.Core;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;

    #endregion

    #region FileService

    public class FileService : NotifyObject, IFileService
    {
        #region Properties

        private enum FileFormat { JSON, XML };

        #endregion

        #region IFileService Properties

        private bool haveFileName = false;
        private string fileName = string.Empty;

        public bool HaveFileName
        {
            get { return haveFileName; }
            set
            {
                if (value != haveFileName)
                {
                    haveFileName = value;
                    Notify("HaveFileName");
                }
            }
        }

        public string FileName
        {
            get { return fileName; }
            set
            {
                if (value != fileName)
                {
                    fileName = value;
                    Notify("FileName");
                }
            }
        }

        #endregion

        #region IFileService Methods

        public Task<Tuple<Solution,string>> Open()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog()
            {
                DefaultExt = "json",
                Filter = "Solution Files (*.json;*.xml)|*.json;*.xml|JSON Files (*.json)|*.json|XML Files (*.xml)|*.xml|All Files (*.*)|*.*",
                FilterIndex = 1,
                FileName = ""
            };

            if (dlg.ShowDialog() == true)
            {
                return Open(dlg.FileName);
            }

            return null;
        }

        public Task<Tuple<Solution,string>> Open(string fileName)
        {
            try
            {
                Func<Tuple<Solution, string>> loadSolutionAction = () =>
                {
                    var s = System.Diagnostics.Stopwatch.StartNew();

                    Solution solution = null;

                    string ext = System.IO.Path.GetExtension(fileName).ToLower();

                    if (ext == ".json")
                    {
                        // JSON (default)
                        solution = Serializer.OpenJson<Solution>(fileName);
                    }
                    else if (ext == ".xml")
                    {
                        // XML
                        solution = Serializer.OpenXml<Solution>(fileName);
                    }

                    s.Stop();

                    if (solution == null)
                    {
                        //throw new Exception("Failed to load solution.");
                        System.Diagnostics.Debug.Print("Failed to load solution.");

                        this.HaveFileName = false;
                    }
                    else
                    {
                        this.HaveFileName = true;
                        this.FileName = fileName;

                        System.Diagnostics.Debug.Print("Load Solution in {0}ms, Name: {1}", s.Elapsed.TotalMilliseconds, solution.Name);
                    }

                    return new Tuple<Solution,string>(solution, fileName);
                };

                return Task.Factory.StartNew(loadSolutionAction);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return null;
        }

        public Task<string> Save(Solution solution)
        {
            if (this.haveFileName)
                return this.Save(this.fileName, solution);
            else
                return this.SaveAs(solution);
        }

        private Task<string> Save(string fileName, Solution solution)
        {
            string ext = System.IO.Path.GetExtension(fileName).ToLower();

            if (ext == ".json")
                return this.Save(this.fileName, FileFormat.JSON, solution);
            else if (ext == ".xml")
                return this.Save(this.fileName, FileFormat.XML, solution);
            else
                throw new InvalidEnumArgumentException("Invalid file extension.");
        }

        private Task<string> Save(string fileName, FileFormat format, Solution solution)
        {
            try
            {
                Func<string> saveSolutionAction = () =>
                {
                    var s = System.Diagnostics.Stopwatch.StartNew();

                    if (format == FileFormat.JSON)
                    {
                        Serializer.SaveJson<Solution>(solution, fileName);
                    }
                    else if (format == FileFormat.XML)
                    {
                        Serializer.SaveXml<Solution>(solution, fileName);
                    }

                    s.Stop();
                    System.Diagnostics.Debug.Print("Save Solution in {0}ms, Name: {1}", s.Elapsed.TotalMilliseconds, solution.Name);

                    this.HaveFileName = true;
                    this.FileName = fileName;

                    return fileName;
                };

                return Task.Factory.StartNew(saveSolutionAction);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return null;
        }

        public Task<string> SaveAs(Solution solution)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog()
            {
                DefaultExt = "json",
                Filter = "JSON Files (*.json)|*.json|XML Files (*.xml)|*.xml|All Files (*.*)|*.*",
                FilterIndex = 1,
                FileName = solution.Name
            };

            if (dlg.ShowDialog() == true)
            {
                switch (dlg.FilterIndex)
                {
                    case 1:
                    default:
                        {
                            // JSON (default)
                            return this.Save(dlg.FileName, FileFormat.JSON, solution);
                        }
                    case 2:
                        {
                            // XML
                            return this.Save(dlg.FileName, FileFormat.XML, solution);
                        }
                };
            }

            return null;
        }

        public void Reset()
        {
            this.HaveFileName = false;
            this.FileName = string.Empty;
        }

        #endregion
    }

    #endregion
}
