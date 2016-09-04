#region References

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

#endregion

namespace CustomDrawing
{
    #region References

    using CustomDrawing.Core;

    #endregion

    #region WpfEditor

    public partial class WpfEditor : UserControl
    {
        #region Fields

        private Editor editor;
        private Factory factory;
        private WpfElement root;

        #endregion

        #region Constructor

        public WpfEditor()
        {
            InitializeComponent();
            Initialize();
        }

        #endregion

        #region Initialize

        private void Initialize()
        {
            editor = new Editor();
            editor.Factory = new Factory();
            editor.Painter = new Painter();
            editor.Pointer = new WpfPointer(canvas);
            editor.Redraw = () => root.Redraw();

            root = new WpfElement(editor.Painter);
            factory = editor.Factory;

            canvas.Children.Add(root);
            canvas.MouseLeftButtonDown += (sender, e) => editor.LeftDown(GetPosition(e));
            canvas.MouseLeftButtonUp += (sender, e) => editor.LeftUp();
            canvas.MouseMove += (sender, e) => editor.Move(GetPosition(e));

            this.PreviewMouseRightButtonDown += (sender, e) => editor.RightDown();
            this.PreviewKeyDown += (sender, e) => HandleKey(e);

            ThreadPool.QueueUserWorkItem(new WaitCallback((obj) => Tests()));

            UpdateMenu();
        }

        private Point GetPosition(MouseButtonEventArgs e)
        {
            var p = e.GetPosition(canvas);
            p.X = Math.Round(p.X, 0);
            p.Y = Math.Round(p.Y, 0);
            return p;
        }

        private Point GetPosition(MouseEventArgs e)
        {
            var p = e.GetPosition(canvas);
            p.X = Math.Round(p.X, 0);
            p.Y = Math.Round(p.Y, 0);
            return p;
        }

        private void UpdateMenu()
        {
            TextModeMove.Tag = editor.CurrentEditMode == EditMode.HitTest || editor.CurrentEditMode == EditMode.Move ? "True" : "False";
            TextModeCreate.Tag = editor.CurrentEditMode == EditMode.Create ? "True" : "False";
            TextCreateLine.Tag = editor.CurrentCreateMode == CreateMode.Line ? "True" : "False";
            TextCreateRect.Tag = editor.CurrentCreateMode == CreateMode.Rectangle ? "True" : "False";
            TextCreateText.Tag = editor.CurrentCreateMode == CreateMode.Text ? "True" : "False";
            TextSnapOrigin.Tag = IsSnapMode(PinTypes.Origin) ? "True" : "False";
            TextSnapSnap.Tag = IsSnapMode(PinTypes.Snap) ? "True" : "False";
            TextSnapConnector.Tag = IsSnapMode(PinTypes.Connector) ? "True" : "False";
            TextSnapGuide.Tag = IsSnapMode(PinTypes.Guide) ? "True" : "False";
        }

        private bool IsSnapMode(PinTypes mode)
        {
            return (root.Painter.SnapMode & mode) == mode;
        }

        private void HandleKey(KeyEventArgs e)
        {
            // cancel
            if (e.Key == Key.Escape)
            {
                if (editor.CurrentEditMode == EditMode.Move)
                    editor.CancelEdit();
            }

            // open
            else if (e.Key == Key.F7)
            {
                Open();
                e.Handled = true;
            }

            // save
            else if (e.Key == Key.F8)
            {
                Save();
                e.Handled = true;
            }

            // mode
            else if (e.Key == Key.M)
            {
                editor.SetEditModeToHitTest();
                e.Handled = true;
            }
            else if (e.Key == Key.C)
            {
                editor.SetEditModeToCreate();
                e.Handled = true;
            }

            // create
            else if (e.Key == Key.L)
            {
                editor.SetCreateModeToLine();
                e.Handled = true;
            }
            else if (e.Key == Key.R)
            {
                editor.SetCreateModeToRectangle();
                e.Handled = true;
            }
            else if (e.Key == Key.T)
            {
                editor.SetCreateModeToText();
                e.Handled = true;
            }

            // text align
            else if (e.Key == Key.Up)
            {
                editor.MoveTextUp();
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                editor.MoveTextDown();
                e.Handled = true;
            }
            else if (e.Key == Key.Left)
            {
                editor.MoveTextLeft();
                e.Handled = true;
            }
            else if (e.Key == Key.Right)
            {
                editor.MoveTextRight();
                e.Handled = true;
            }

            // snap
            else if (e.Key == Key.O)
            {
                editor.ToggleSnapMode(PinTypes.Origin);
                editor.Move(editor.Pointer.GetPosition());
                e.Handled = true;
            }
            else if (e.Key == Key.S)
            {
                editor.ToggleSnapMode(PinTypes.Snap);
                editor.Move(editor.Pointer.GetPosition());
                e.Handled = true;
            }
            else if (e.Key == Key.N)
            {
                editor.ToggleSnapMode(PinTypes.Connector);
                editor.Move(editor.Pointer.GetPosition());
                e.Handled = true;
            }
            else if (e.Key == Key.G)
            {
                editor.ToggleSnapMode(PinTypes.Guide);
                editor.Move(editor.Pointer.GetPosition());
                e.Handled = true;
            }

            UpdateMenu();
        }

        #endregion

        #region Xml

        private string Open(string path)
        {
            using (var reader = new System.IO.StreamReader(path))
                return reader.ReadToEnd();
        }

        private void Save(string path, string xml)
        {
            using (var writer = new System.IO.StreamWriter(path))
                writer.Write(xml);
        }

        private void Open()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "Xml Files (*.xml)|*.xml"
            };

            if (dlg.ShowDialog().Value == true)
            {
                var sw = Stopwatch.StartNew();

                string xml = Open(dlg.FileName);

                sw.Stop();
                Debug.Print("Open: " + sw.Elapsed.TotalMilliseconds + "ms");

                sw.Reset();
                sw.Start();

                Load(xml);

                sw.Stop();
                Debug.Print("Load: " + sw.Elapsed.TotalMilliseconds + "ms");
            }
        }

        private void Load(string xml)
        {
            editor.Painter.Elements.Clear();

            var elements = XmlSerializer.DeSerialize(xml);
            foreach (var item in elements)
                editor.Painter.Elements.Add(item);

            editor.Redraw();
        }

        private void Save()
        {
            var dlg = new Microsoft.Win32.SaveFileDialog()
            {
                Filter = "Xml Files (*.xml)|*.xml",
                FileName = "elements"
            };

            if (dlg.ShowDialog().Value == true)
            {
                var sw = Stopwatch.StartNew();

                string xml = XmlSerializer.Serialize(editor.Painter.Elements);

                sw.Stop();
                Debug.Print("Serialize: " + sw.Elapsed.TotalMilliseconds + "ms");

                sw.Reset();
                sw.Start();

                Save(dlg.FileName, xml);

                sw.Stop();
                Debug.Print("Save: " + sw.Elapsed.TotalMilliseconds + "ms");
            }
        }

        #endregion

        #region Tests

        private void Tests()
        {
            CustomGateTest();
            CustomSRTest();
        }

        private void CustomSRTest()
        {
            var sr = CreateCustomSR();

            InsertSR(sr, 30f, 90f, 15f);
            InsertSR(sr, 150f, 90f, 0f);
            InsertSR(sr, 30f, 150f, 0f);
            InsertSR(sr, 150f, 150f, 0f);
        }

        private void CustomGateTest()
        {
            var g = CreateCustomGate();

            InsertGate(g, "&", 30f, 30f, 0f);
            InsertGate(g, "≥1", 90f, 30f, 0f);
            InsertGate(g, "=1", 150f, 30f, 0f);
        }

        private void InsertSR(Custom sr, float x, float y, float angle)
        {
            var rsr = factory.CreateReference(null, sr, x, y);
            rsr.Variables.Add((sr.Variables[0] as Text).Id, "S");
            rsr.Variables.Add((sr.Variables[1] as Text).Id, "R");
            rsr.Angle = angle;
            CreateConnectorsSR(rsr);
            root.Painter.Elements.Add(rsr);
        }

        private void InsertGate(Custom g, string text, float x, float y, float angle)
        {
            var rg = factory.CreateReference(null, g, x, y);
            rg.Variables.Add((g.Variables[0] as Text).Id, text);
            rg.Angle = angle;
            CreateConnectorsGate(rg);
            root.Painter.Elements.Add(rg);
        }

        private Custom CreateCustomSR()
        {
            var custom = factory.CreateCustom(null, 0f, 0f);

            var p0 = factory.CreatePin(custom, 0f, 0f, PinTypes.Snap);
            var p1 = factory.CreatePin(custom, 30f, 0f, PinTypes.Snap);
            var p2 = factory.CreatePin(custom, 60f, 0f, PinTypes.Snap);
            var p3 = factory.CreatePin(custom, 60f, 30f, PinTypes.Snap);
            var p4 = factory.CreatePin(custom, 30f, 30f, PinTypes.Snap);
            var p5 = factory.CreatePin(custom, 0f, 30f, PinTypes.Snap);
            var p6 = factory.CreatePin(custom, 30f, 20f, PinTypes.Snap);

            var l0 = factory.CreateLine(custom, p0, p1);
            var l1 = factory.CreateLine(custom, p1, p2);
            var l2 = factory.CreateLine(custom, p2, p3);
            var l3 = factory.CreateLine(custom, p3, p4);
            var l4 = factory.CreateLine(custom, p4, p5);
            var l5 = factory.CreateLine(custom, p5, p0);
            var l6 = factory.CreateLine(custom, p1, p6);
            var l7 = factory.CreateLine(custom, p6, p4);

            custom.Children.Add(l0);
            custom.Children.Add(l1);
            custom.Children.Add(l2);
            custom.Children.Add(l3);
            custom.Children.Add(l4);
            custom.Children.Add(l5);
            custom.Children.Add(l6);
            custom.Children.Add(l7);

            var ts = factory.CreateText(custom, 15f, 10f, "S", HAlign.Center, VAlign.Center);
            var tr = factory.CreateText(custom, 45f, 10f, "R", HAlign.Center, VAlign.Center);

            custom.Children.Add(ts);
            custom.Children.Add(tr);

            custom.Variables.Add(ts);
            custom.Variables.Add(tr);

            var r = factory.CreateRectangle(custom, p6, p3);

            custom.Children.Add(r);

            return custom;
        }

        private void CreateConnectorsSR(Reference reference)
        {
            reference.Connectors.Add(factory.CreatePin(reference, 15f, 0f, PinTypes.Connector));
            reference.Connectors.Add(factory.CreatePin(reference, 15f, 30f, PinTypes.Connector));
            reference.Connectors.Add(factory.CreatePin(reference, 45f, 0f, PinTypes.Connector));
            reference.Connectors.Add(factory.CreatePin(reference, 45f, 30f, PinTypes.Connector));
        }

        private Custom CreateCustomGate()
        {
            var custom = factory.CreateCustom(null, 0f, 0f);

            var p0 = factory.CreatePin(custom, 0f, 0f, PinTypes.Snap);
            var p1 = factory.CreatePin(custom, 30f, 0f, PinTypes.Snap);
            var p2 = factory.CreatePin(custom, 30f, 30f, PinTypes.Snap);
            var p3 = factory.CreatePin(custom, 0f, 30f, PinTypes.Snap);

            var l0 = factory.CreateLine(custom, p0, p1);
            var l1 = factory.CreateLine(custom, p1, p2);
            var l2 = factory.CreateLine(custom, p2, p3);
            var l3 = factory.CreateLine(custom, p3, p0);

            custom.Children.Add(l0);
            custom.Children.Add(l1);
            custom.Children.Add(l2);
            custom.Children.Add(l3);

            var text = factory.CreateText(custom, 15f, 15f, "", HAlign.Center, VAlign.Center);

            custom.Children.Add(text);
            custom.Variables.Add(text);

            return custom;
        }

        private void CreateConnectorsGate(Reference reference)
        {
            reference.Connectors.Add(factory.CreatePin(reference, 0f, 15f, PinTypes.Connector));
            reference.Connectors.Add(factory.CreatePin(reference, 30f, 15f, PinTypes.Connector));
            reference.Connectors.Add(factory.CreatePin(reference, 15f, 0f, PinTypes.Connector));
            reference.Connectors.Add(factory.CreatePin(reference, 15f, 30f, PinTypes.Connector));
        }

        #endregion
    }

    #endregion
}
