// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Services
{
    #region References

    using Logic.Model;
    using Logic.Model.Core;
    using Logic.ViewModels.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Printing;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using System.Windows.Media;

    #endregion

    #region PrintService

    public class PrintService : IPrintService
    {
        #region Fields

        public static string RootDictionaryUri = "Dictionaries/RootDictionary.xaml";
        public static string OptionsDataProvider = "OptionsDataProvider";
        public static string PrintContextTemplate = "PrintContextDataTemplateKey";

        #endregion

        #region Methods

        private static void SetPrintColors(ContentControl element)
        {
            if (element == null)
                throw new ArgumentNullException();

            var backgroundColor = element.Resources["BackgroundColorKey"] as SolidColorBrush;
            backgroundColor.Color = Colors.White;

            var gridColor = element.Resources["GridColorKey"] as SolidColorBrush;
            gridColor.Color = Colors.Transparent;

            var pageColor = element.Resources["PageColorKey"] as SolidColorBrush;
            pageColor.Color = Colors.Black;

            var logicColor = element.Resources["LogicColorKey"] as SolidColorBrush;
            logicColor.Color = Colors.Black;

            var logicSelectedColor = element.Resources["LogicSelectedColorKey"] as SolidColorBrush;
            logicSelectedColor.Color = Colors.Black;

            var helperColor = element.Resources["HelperColorKey"] as SolidColorBrush;
            helperColor.Color = Colors.Transparent;

            var logicMouseOverColor = element.Resources["LogicMouseOverColorKey"] as SolidColorBrush;
            logicMouseOverColor.Color = Colors.Transparent;

            var logicTrueStateColor = element.Resources["LogicTrueStateColorKey"] as SolidColorBrush;
            logicTrueStateColor.Color = Colors.Black;

            var logicFalseStateColor = element.Resources["LogicFalseStateColorKey"] as SolidColorBrush;
            logicFalseStateColor.Color = Colors.Black;

            var logicNullStateColor = element.Resources["LogicNullStateColorKey"] as SolidColorBrush;
            logicNullStateColor.Color = Colors.Black;
        }

        public static ContentControl CreateContextElement(Context context, Size areaExtent, Point origin, Rect area)
        {
            ContentControl element = new ContentControl();

            // set print dictionary
            element.Resources.Source = new Uri(RootDictionaryUri, UriKind.Relative);

            // set print colors
            if (Defaults.Options.DisablePrintColors == false)
                SetPrintColors(element);

            // set print options
            var options = new Options();

            Defaults.SetDefaults(options);

            options.HidePins = true;
            options.IsPrinting = true;

            // set options resource instance
            ObjectDataProvider dataProvider = element.Resources[OptionsDataProvider] as ObjectDataProvider;
            if (dataProvider == null)
                throw new Exception("Invalid options ObjectDataProvider.");

            dataProvider.ObjectInstance = options;

            // set element template and content
            element.ContentTemplate = element.Resources[PrintContextTemplate] as DataTemplate;
            element.Content = context;

            return element;
        }

        public static FixedDocument CreateFixedDocument(IEnumerable<Context> contexts, Size areaExtent, Size areaOrigin)
        {
            if (contexts == null)
                throw new ArgumentNullException();

            var origin = new Point(areaOrigin.Width, areaOrigin.Height);
            var area = new Rect(origin, areaExtent);
            var scale = Math.Min(areaExtent.Width / 1260, areaExtent.Height / 891);

            // create fixed document
            var fixedDocument = new FixedDocument();

            //fixedDocument.DocumentPaginator.PageSize = new Size(areaExtent.Width, areaExtent.Height);

            foreach (Context context in contexts)
            {
                var pageContent = new PageContent();
                var fixedPage = new FixedPage();

                //pageContent.Child = fixedPage;
                ((IAddChild)pageContent).AddChild(fixedPage);

                fixedDocument.Pages.Add(pageContent);

                fixedPage.Width = areaExtent.Width;
                fixedPage.Height = areaExtent.Height;

                var element = CreateContextElement(context, areaExtent, origin, area);

                // transform and scale for print
                element.LayoutTransform = new ScaleTransform(scale, scale);

                // set element position
                FixedPage.SetLeft(element, areaOrigin.Width);
                FixedPage.SetTop(element, areaOrigin.Height);

                // add element to page
                fixedPage.Children.Add(element);

                // update fixed page layout
                //fixedPage.Measure(areaExtent);
                //fixedPage.Arrange(area);
            }

            return fixedDocument;
        }

        public static FixedDocumentSequence CreateFixedDocumentSequence(IEnumerable<Project> projects, Size areaExtent, Size areaOrigin)
        {
            if (projects == null)
                throw new ArgumentNullException();

            var fixedDocumentSeq = new FixedDocumentSequence();

            foreach (var project in projects)
            {
                var fixedDocument = CreateFixedDocument(project.Children.Cast<Context>(), areaExtent, areaOrigin);

                var documentRef = new DocumentReference();
                documentRef.BeginInit();
                documentRef.SetDocument(fixedDocument);
                documentRef.EndInit();

                (fixedDocumentSeq as IAddChild).AddChild(documentRef);
            }

            return fixedDocumentSeq;
        }

        private static void SetPrintDialogOptions(PrintDialog dlg)
        {
            if (dlg == null)
                throw new ArgumentNullException();

            dlg.PrintQueue = LocalPrintServer.GetDefaultPrintQueue();

            dlg.PrintTicket = dlg.PrintQueue.DefaultPrintTicket;
            dlg.PrintTicket.PageOrientation = PageOrientation.Landscape;
            dlg.PrintTicket.OutputQuality = OutputQuality.High;
            dlg.PrintTicket.TrueTypeFontMode = TrueTypeFontMode.DownloadAsNativeTrueTypeFont;
        }

        private static bool ShowPrintDialog(PrintDialog dlg)
        {
            if (dlg == null)
                throw new ArgumentNullException();

            // configure printer
            SetPrintDialogOptions(dlg);

            // show print dialog
            if (dlg.ShowDialog() == true)
                return true;
            else
                return false;
        }

        public static void Print(IEnumerable<Context> contexts, string name)
        {
            if (contexts == null)
                throw new ArgumentNullException();

            var dlg = new PrintDialog();

            ShowPrintDialog(dlg);

            // print capabilities
            var caps = dlg.PrintQueue.GetPrintCapabilities(dlg.PrintTicket);
            var areaExtent = new Size(caps.PageImageableArea.ExtentWidth, caps.PageImageableArea.ExtentHeight);
            var areaOrigin = new Size(caps.PageImageableArea.OriginWidth, caps.PageImageableArea.OriginHeight);

            // create document
            var s = System.Diagnostics.Stopwatch.StartNew();

            var fixedDocument = CreateFixedDocument(contexts, areaExtent, areaOrigin);

            s.Stop();
            System.Diagnostics.Debug.Print("CreateFixedDocument in {0}ms", s.Elapsed.TotalMilliseconds);

            // print document
            dlg.PrintDocument(fixedDocument.DocumentPaginator, name);
        }

        public static void Print(IEnumerable<Project> projects, string name)
        {
            if (projects == null)
                throw new ArgumentNullException();

            var dlg = new PrintDialog();

            ShowPrintDialog(dlg);

            // print capabilities
            var caps = dlg.PrintQueue.GetPrintCapabilities(dlg.PrintTicket);
            var areaExtent = new Size(caps.PageImageableArea.ExtentWidth, caps.PageImageableArea.ExtentHeight);
            var areaOrigin = new Size(caps.PageImageableArea.OriginWidth, caps.PageImageableArea.OriginHeight);

            // create document
            var s = System.Diagnostics.Stopwatch.StartNew();

            var fixedDocumentSeq = CreateFixedDocumentSequence(projects, areaExtent, areaOrigin);

            s.Stop();
            System.Diagnostics.Debug.Print("CreateFixedDocumentSequence in {0}ms", s.Elapsed.TotalMilliseconds);

            // print document
            dlg.PrintDocument(fixedDocumentSeq.DocumentPaginator, name);
        }

        public static void Print(Context context)
        {
            var contexts = new List<Context>() { context };

            Print(contexts, context.Name);
        }

        public static void Print(Project project)
        {
            if (project != null && project.Children != null && project.Children.Count > 0)
                Print(project.Children.Cast<Context>(), project.Name);
        }

        public static void Print(Solution solution)
        {
            if (solution != null && solution.Children != null && solution.Children.Count > 0)
                Print(solution.Children.Cast<Project>(), solution.Name);
        }

        #endregion

        #region IPrintService

        public void Print(Element element)
        {
            if (element is Context)
            {
                Print(element as Context);
            }
            else if (element is Project)
            {
                Print(element as Project);
            }
            else if (element is Solution)
            {
                Print(element as Solution);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void Print(IEnumerable<Element> elements)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    #endregion
}
