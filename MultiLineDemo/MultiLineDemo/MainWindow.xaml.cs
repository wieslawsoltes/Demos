// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Logic.Model;
using Logic.Utilities;
    
namespace MultiLineDemo
{
    public partial class MainWindow : Window
    {
        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            // create default context
            var context = new Context()
            {
                Id = "context0"
            };

            // set window datacontext
            this.DataContext = context;

            // observe elements in context
            context.ObserveElements(1000.0);
        }

        #endregion

        #region Thumb Events

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var location = (sender as FrameworkElement).DataContext as ILocation;
            if (location != null)
            {
                location.X = SnapToGrid.Snap(location.X + e.HorizontalChange, 15);
                location.Y = SnapToGrid.Snap(location.Y + e.VerticalChange, 15);
            }
        }

        #endregion

        #region Button Events

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            var context = this.DataContext as Context;
            if (context.Sync == true)
            {
                System.Diagnostics.Debug.Print("Sync with ButtonOpen");
                return;
            }

            var dlg = new Microsoft.Win32.OpenFileDialog()
            {
                DefaultExt = "jpg",
                Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*",
                FilterIndex = 0,
                FileName = "context"
            };

            if (dlg.ShowDialog() == true)
            {
                if (dlg.FilterIndex == 1)
                {
                    var newContext = ContextSerializer.Open(dlg.FileName);
                    if (newContext != null)
                    {
                        // set window datacontext
                        this.DataContext = newContext;

                        // update connected lines to pins
                        newContext.UpdateConnectedLines();

                        // observe elements in context
                        newContext.ObserveElements(1000.0);
                    }
                }
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var context = this.DataContext as Context;
            if (context.Sync == true)
            {
                System.Diagnostics.Debug.Print("Sync with ButtonOpen");
                return;
            }

            var dlg = new Microsoft.Win32.SaveFileDialog()
            {
                DefaultExt = "jpg",
                Filter = "XML Files (*.xml)|*.xml|JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                FilterIndex = 0,
                FileName = "context"
            };
            
            if (dlg.ShowDialog() == true)
            {
                if (dlg.FilterIndex == 1)
                {
                    // XML Serializer
                    if (context != null)
                    {
                        ContextSerializer.Save(context, dlg.FileName);
                    }
                }
                else if (dlg.FilterIndex == 2)
                {
                    // JSON serializer
                    var s = new JavaScriptSerializer();
                    var text = s.Serialize(context);
                    using (var w = new System.IO.StreamWriter(dlg.FileName))
                    {
                        w.Write(text);
                    }
                }
            }
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            var bw = new BackgroundWorker();
            var context = this.DataContext as Context;

            bw.DoWork += (_s, _e) =>
                {
                    context.ResetContext(1000.0);
                };

            bw.RunWorkerAsync();
        }

        #endregion
    }

    #region Data Template Selectors

    public class ModelDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return (DataTemplate)App.Current.Windows[0].TryFindResource(string.Format("{0}{1}", item.GetType().Name, "DataTemplateKey"));
        }
    }

    #endregion
}

namespace Logic.Controls
{
    #region References

    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Logic.Model;
    using Logic.Utilities;

    #endregion

    #region Context Canvas

    public class ContextCanvas : Canvas
    {
        #region Constructor

        public ContextCanvas()
            : base()
        {
            this.PreviewMouseLeftButtonDown += ContextCanvas_PreviewMouseLeftButtonDown;
            this.MouseLeftButtonDown += ContextCanvas_MouseLeftButtonDown;
            this.PreviewMouseRightButtonDown += ContextCanvas_PreviewMouseRightButtonDown;
            this.MouseRightButtonDown += ContextCanvas_MouseRightButtonDown;
            this.MouseMove += ContextCanvas_MouseMove;
        }

        #endregion

        #region Context Handlers

        private Pin firstPin = null;
        private Pin previousPin = null;
        private Pin tempPin = null;
        private int pinCount = 0;
        private Line previousLine = null;

        private void HandleNewLine(Context context, Point point)
        {
            // create new pin
            var pin = context.CreatePin(SnapToGrid.Snap(point.X, 15), SnapToGrid.Snap(point.Y, 15));
            this.pinCount++;

            if (this.firstPin == null)
            {
                this.firstPin = pin;
                pin.DisableHitTest = false;
            }

            if (this.previousLine != null)
            {
                this.previousLine.DisableHitTest = false;
            }

            // create new line
            if (this.previousPin != null)
            {
                this.previousPin.DisableHitTest = false;

                this.previousLine = context.CreateLine(this.previousPin, pin);

                this.previousPin = pin;
                this.tempPin = pin;
            }
            else
            {
                this.tempPin = context.CreatePin(SnapToGrid.Snap(point.X, 15), SnapToGrid.Snap(point.Y, 15));
                this.pinCount++;

                this.previousLine = context.CreateLine(pin, this.tempPin);

                this.previousPin = this.tempPin;
            }

            this.tempPin.DisableHitTest = true;
        }

        private void HandleSplitLine(Context context, Line line, Point point, Pin pin)
        {
            var newPin = (pin == null) ? context.CreatePin(SnapToGrid.Snap(point.X, 15), SnapToGrid.Snap(point.Y, 15)) : pin;
            newPin.DisableHitTest = false;

            var newLine = context.CreateLine(newPin, line.End);
            newLine.DisableHitTest = false;

            line.End = newPin;
        }

        private void HandleMouseRelase()
        {
            var context = this.DataContext as Context;

            context.Elements.Remove(this.previousLine);
            context.Elements.Remove(this.tempPin);
            context.Lines.Remove(this.previousLine);
            context.Pins.Remove(this.tempPin);

            if (this.pinCount == 2)
            {
                context.Elements.Remove(this.firstPin);
                context.Pins.Remove(this.firstPin);
            }

            // reset mouse capture context
            this.firstPin = null;
            this.pinCount = 0;
            this.previousPin = null;
            this.previousLine = null;
            this.tempPin = null;
        }

        private void HandleRemovePin(Context context, Pin pin)
        {
            // find connected lines to pin
            var lines = context.Lines.Where(l => l.Start == pin || l.End == pin);

            System.Diagnostics.Debug.Print("lines.Count={0}", lines.Count());

            if (lines.Count() == 1)
            {
                var first = lines.First();

                // remove second pin from 'first' line if it's not connected to other lines
                if (first.Start == pin)
                {
                    var endPin = first.End;
                    var linesConnectToEndPin = context.Lines.Where(l => l.Start == endPin || l.End == endPin);
                    if (linesConnectToEndPin.Count() == 1)
                    {
                        context.Pins.Remove(endPin);
                        context.Elements.Remove(endPin);
                    }
                }
                else
                {
                    var startPin = first.Start;
                    var linesConnectToStartPin = context.Lines.Where(l => l.Start == startPin || l.End == startPin);
                    if (linesConnectToStartPin.Count() == 1)
                    {
                        context.Pins.Remove(startPin);
                        context.Elements.Remove(startPin);
                    }
                }

                // remove pin
                context.Pins.Remove(pin);
                context.Elements.Remove(pin);

                // remove connected lines
                context.Lines.Remove(first);
                context.Elements.Remove(first);
            }
            else if (lines.Count() == 2)
            {
                var first = lines.First();
                var last = lines.Last();

                if (first.Start == pin && last.Start == pin)
                {
                    first.Start = last.End;

                    System.Diagnostics.Debug.Print("first.Start = last.End");
                }
                else if (first.End == pin && last.End == pin)
                {
                    first.End = last.Start;

                    System.Diagnostics.Debug.Print("first.End = last.Start");
                }
                else if (first.Start == pin && last.End == pin)
                {
                    first.Start = last.Start;

                    System.Diagnostics.Debug.Print("first.Start = last.Start");
                }
                else if (first.End == pin && last.Start == pin)
                {
                    first.End = last.End;

                    System.Diagnostics.Debug.Print("first.End = last.End");
                }

                context.Pins.Remove(pin);
                context.Elements.Remove(pin);

                context.Lines.Remove(last);
                context.Elements.Remove(last);
            }
        }

        private void HandleRemoveLine(Context context, Line lines)
        {
            //context.Lines.Remove(line);
            //context.Elements.Remove(line);
        }

        #endregion

        #region Canvas Events

        void ContextCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var context = this.DataContext as Context;

            if (context.Sync == true)
            {
                System.Diagnostics.Debug.Print("Sync with PreviewMouseLeftButtonDown");
                return;
            }

            var source = e.OriginalSource as FrameworkElement;

            System.Diagnostics.Debug.Print("PreviewMouseLeftButtonDown => source type: {0}, Id: {1}", source.DataContext.GetType().Name.ToString(), (source.DataContext as IId).Id);

            if (context.CaptureMouse == false)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    if (source.DataContext is Pin)
                    {
                        var pin = source.DataContext as Pin;

                        this.firstPin = null;

                        this.tempPin = context.CreatePin(pin.X, pin.Y);
                        pinCount++;

                        this.previousLine = context.CreateLine(pin, this.tempPin);

                        this.previousPin = tempPin;

                        // capture mouse
                        context.CaptureMouse = true;

                        e.Handled = true;
                    }
                }
                else
                {
                    if (source.DataContext is Pin)
                    {
                        var pin = source.DataContext as Pin;

                        // TODO:
                        //pin.IsSelected = !pin.IsSelected;
                    }
                    else if (source.DataContext is Line)
                    {
                        var line = source.DataContext as Line;

                        // TODO:

                        if (Keyboard.Modifiers == ModifierKeys.Shift)
                        {
                            // (de)select only one line
                            line.IsSelected = !line.IsSelected;
                        }
                        else
                        {
                            // (de)select all connected lines
                            var q = context.ConnectedLines.Where(cl => cl.Lines.Contains(line));
                            foreach (var cl in q)
                            {
                                foreach (var l in cl.Lines)
                                {
                                    l.IsSelected = !l.IsSelected;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    if (source.DataContext is Pin)
                    {
                        var pin = source.DataContext as Pin;
                        if (pin != tempPin)
                        {
                            // connect to existing pin
                            this.previousLine.End = pin;
                            this.previousLine.DisableHitTest = false;
                            this.firstPin = null;
                            this.previousLine = null;

                            // release mouse capture
                            context.CaptureMouse = false;

                            // handle mouse release
                            HandleMouseRelase();

                            e.Handled = true;
                        }
                    }
                    else if (source.DataContext is Line)
                    {
                        var line = source.DataContext as Line;
                        var point = e.GetPosition(sender as FrameworkElement);

                        // split line (hold Control + click on line)
                        HandleSplitLine(context, line, point, this.tempPin);

                        // connect to existing pin (added after line split)
                        this.previousLine.End = line.End;
                        this.previousLine.DisableHitTest = false;
                        this.firstPin = null;
                        this.previousLine = null;
                        this.tempPin = null;

                        // release mouse capture
                        context.CaptureMouse = false;

                        // handle mouse release
                        HandleMouseRelase();

                        e.Handled = true;
                    }
                }
                else
                {
                    if (source.DataContext is Pin)
                    {
                        var pin = source.DataContext as Pin;
                        var point = new Point(pin.X, pin.Y);

                        if (pin == this.tempPin)
                        {
                            // handle new line
                            HandleNewLine(context, point);

                            e.Handled = true;
                        }
                        else
                        {
                            e.Handled = false;
                        }
                    }
                }
            }
        }

        void ContextCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var context = this.DataContext as Context;

            if (context.Sync == true)
            {
                System.Diagnostics.Debug.Print("Sync with MouseLeftButtonDown");
                return;
            }

            var source = e.OriginalSource as FrameworkElement;
            var point = e.GetPosition(sender as FrameworkElement);

            System.Diagnostics.Debug.Print("MouseLeftButtonDown => source type: {0}, Id: {1}", source.DataContext.GetType().Name.ToString(), (source.DataContext as IId).Id);

            if (context.CaptureMouse == false)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    if (source.DataContext is Line)
                    {
                        var line = source.DataContext as Line;

                        // split line (hold Control + click on line)
                        HandleSplitLine(context, line, point, null);

                        e.Handled = true;
                    }
                }
                else
                {
                    if (source.DataContext is Context)
                    {
                        // handle new line
                        HandleNewLine(context, point);

                        // capture mouse
                        context.CaptureMouse = true;

                        e.Handled = true;
                    }
                }
            }
            else
            {
                if (Keyboard.Modifiers != ModifierKeys.Control)
                {
                    if (source.DataContext is Context)
                    {
                        // handle new line
                        HandleNewLine(context, point);

                        e.Handled = true;
                    }
                }
            }
        }

        void ContextCanvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var context = this.DataContext as Context;

            if (context.Sync == true)
            {
                System.Diagnostics.Debug.Print("Sync with PreviewMouseRightButtonDown");
                return;
            }

            var source = e.OriginalSource as FrameworkElement;

            System.Diagnostics.Debug.Print("PreviewMouseRightButtonDown => source type: {0}, Id: {1}", source.DataContext.GetType().Name.ToString(), (source.DataContext as IId).Id);

            if (context.CaptureMouse == true)
            {
                // release mouse capture
                context.CaptureMouse = false;

                // handle mouse release
                HandleMouseRelase();

                e.Handled = true;
            }
        }

        void ContextCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var context = this.DataContext as Context;

            if (context.Sync == true)
            {
                System.Diagnostics.Debug.Print("Sync with MouseRightButtonDown");
                return;
            }

            var source = e.OriginalSource as FrameworkElement;

            System.Diagnostics.Debug.Print("MouseRightButtonDown => source type: {0}, Id: {1}", source.DataContext.GetType().Name.ToString(), (source.DataContext as IId).Id);

            if (context.CaptureMouse == true)
            {
                // release mouse capture
                context.CaptureMouse = false;

                // handle mouse release
                HandleMouseRelase();
            }
            else
            {
                // TODO: use Ctrl key + mouse Right Click to remove elements from context
                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    // remove pin from context
                    if (source.DataContext is Pin)
                    {
                        var pin = source.DataContext as Pin;

                        // remove pin from context
                        HandleRemovePin(context, pin);
                    }

                    // remove line from context
                    else if (source.DataContext is Line)
                    {
                        var line = source.DataContext as Line;

                        // remove pin from context
                        HandleRemoveLine(context, line);
                    }
                }
            }
        }

        void ContextCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var context = this.DataContext as Context;

            if (context.CaptureMouse == true)
            {
                if (this.tempPin != null)
                {
                    var point = e.GetPosition(sender as FrameworkElement);

                    this.tempPin.X = SnapToGrid.Snap(point.X, 15);
                    this.tempPin.Y = SnapToGrid.Snap(point.Y, 15);

                    /*
                    // use Control key to connect to other pins
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        this.tempPin.X = SnapToGrid.Snap(point.X, 15);
                        this.tempPin.Y = SnapToGrid.Snap(point.Y, 15);
                    }
                    else
                    {
                        double x = Math.Round(SnapToGrid.Snap(point.X, 15), 0);
                        double y = Math.Round(SnapToGrid.Snap(point.Y, 15), 0);

                        //System.Diagnostics.Debug.Print("{0} {1}", x, y);

                        // check if none of existing pins is under new pin location
                        if (!context.Pins.Any(p => Math.Round(p.X, 0) == x && Math.Round(p.Y, 0) == y))
                        {
                            this.tempPin.X = x;
                            this.tempPin.Y = y;
                        }
                    }
                    */
                }
            }
        }

        #endregion
    }

    #endregion
}

namespace Logic.Utilities
{
    #region References

    using System;

    #endregion

    #region Snap To Grid

    public static class SnapToGrid
    {
        public static double Snap(double original, double snap, double offset)
        {
            return Snap(original - offset, snap) + offset;
        }

        public static double Snap(double original, double snap)
        {
            return original + ((Math.Round(original / snap) - original / snap) * snap);
        }
    }

    #endregion
}

namespace Logic.Model
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Runtime.Serialization;
    using System.Xml;

    #endregion

    #region Data Model

    public interface IId
    {
        string Id { get; set; }
    }

    public interface ILocation
    {
        double X { get; set; }
        double Y { get; set; }
        double Z { get; set; }
    }

    public interface ISelection
    {
        bool IsSelected { get; set; }
    }

    [DataContract]
    public abstract class NotifyObject : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Implementation

        public virtual void Notify(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    [DataContract]
    public class Location : NotifyObject, ILocation
    {
        #region Construtor

        public Location() { }

        #endregion

        #region ILocation Implementation

        private double x;
        private double y;
        private double z;

        [DataMember]
        public double X
        {
            get { return x; }
            set
            {
                if (value != x)
                {
                    x = value;
                    Notify("X");
                }
            }
        }

        [DataMember]
        public double Y
        {
            get { return y; }
            set
            {
                if (value != y)
                {
                    y = value;
                    Notify("Y");
                }
            }
        }

        [DataMember]
        public double Z
        {
            get { return z; }
            set
            {
                if (value != z)
                {
                    z = value;
                    Notify("Z");
                }
            }
        }

        #endregion
    }

    [DataContract]
    public abstract class Element : NotifyObject, IId, ILocation, ISelection
    {
        #region Construtor

        public Element() { }

        #endregion

        #region IId Implementation

        private string id;

        [DataMember]
        public string Id
        {
            get { return id; }
            set
            {
                if (value != id)
                {
                    id = value;
                    Notify("Id");
                }
            }
        }

        #endregion

        #region ILocation Implementation

        private double x;
        private double y;
        private double z;

        [DataMember]
        public double X
        {
            get { return x; }
            set
            {
                if (value != x)
                {
                    x = value;
                    Notify("X");
                }
            }
        }

        [DataMember]
        public double Y
        {
            get { return y; }
            set
            {
                if (value != y)
                {
                    y = value;
                    Notify("Y");
                }
            }
        }

        [DataMember]
        public double Z
        {
            get { return z; }
            set
            {
                if (value != z)
                {
                    z = value;
                    Notify("Z");
                }
            }
        }

        #endregion

        #region ISelection Implementation

        private bool isSelected = false;

        [IgnoreDataMember]
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (value != isSelected)
                {
                    isSelected = value;
                    Notify("IsSelected");
                }
            }
        }

        #endregion

        #region Properties

        private bool disableHitTest = false;

        [IgnoreDataMember]
        public bool DisableHitTest
        {
            get { return disableHitTest; }
            set
            {
                if (value != disableHitTest)
                {
                    disableHitTest = value;
                    Notify("DisableHitTest");
                }
            }
        }

        #endregion
    }

    [DataContract]
    public class Pin : Element
    {
        #region Construtor

        public Pin() : base() { }

        #endregion

        #region Properties

        private bool isConnected = false;
        private bool isUpdated = false;
        private ConnectedLine connectedLine = new ConnectedLine();

        [DataMember]
        public bool IsConnected
        {
            get { return isConnected; }
            set
            {
                if (value != isConnected)
                {
                    isConnected = value;
                    Notify("IsConnected");
                }
            }
        }

        [IgnoreDataMember]
        public bool IsUpdated
        {
            get { return isUpdated; }
            set
            {
                if (value != isUpdated)
                {
                    isUpdated = value;
                    Notify("IsUpdated");
                }
            }
        }

        [IgnoreDataMember]
        public ConnectedLine ConnectedLine
        {
            get { return connectedLine; }
            set
            {
                if (value != connectedLine)
                {
                    connectedLine = value;
                    Notify("ConnectedLine");
                }
            }
        }

        #endregion
    }

    [DataContract]
    public class Line : Element
    {
        #region Construtor

        public Line()
            : base()
        {
            this.Initialize();
        }

        #endregion

        #region Initialize

        private void Initialize()
        {
            this.startPoint = new Location();
            this.endPoint = new Location();
            this.startCenter = new Location();
            this.endCenter = new Location();

            this.invertedThickness = 10.0 / 2.0;
            this.thickness = 1.0 / 2.0;

            this.CalculateLocation();
        }

        [OnDeserialized]
        private void Initialize(StreamingContext s)
        {
            this.Initialize();
        }

        #endregion

        #region Events

        private void PinPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "X" || e.PropertyName == "Y")
            {
                this.CalculateLocation();
            }
        }

        #endregion

        #region Properties

        private double invertedThickness;
        private double thickness;

        private Pin start;
        private Pin end;

        private bool invertStart = false;
        private bool invertEnd = false;

        private Location startPoint;
        private Location endPoint = new Location();
        private Location startCenter = new Location();
        private Location endCenter = new Location();

        [DataMember]
        public Pin Start
        {
            get { return start; }
            set
            {
                if (value != start)
                {
                    // remove listener for start pin change notifications
                    if (start != null)
                    {
                        start.PropertyChanged -= PinPropertyChanged;
                    }

                    // add listener for start pin change notifications
                    if (value != null)
                    {
                        value.PropertyChanged += PinPropertyChanged;
                    }

                    start = value;

                    // update inveted start/end location
                    this.CalculateLocation();

                    Notify("Start");
                }
            }
        }

        [DataMember]
        public Pin End
        {
            get { return end; }
            set
            {
                if (value != end)
                {
                    // remove listener for end pin change notifications
                    if (end != null)
                    {
                        end.PropertyChanged -= PinPropertyChanged;
                    }

                    // add listener for end pin change notifications
                    if (value != null)
                    {
                        value.PropertyChanged += PinPropertyChanged;
                    }

                    end = value;

                    // update inveted start/end location
                    this.CalculateLocation();

                    Notify("End");
                }
            }
        }

        [DataMember]
        public bool InvertStart
        {
            get { return invertStart; }
            set
            {
                if (value != invertStart)
                {
                    invertStart = value;

                    // update inveted start/end location
                    this.CalculateLocation();

                    Notify("InvertStart");
                }
            }
        }

        [DataMember]
        public bool InvertEnd
        {
            get { return invertEnd; }
            set
            {
                if (value != invertEnd)
                {
                    invertEnd = value;

                    // update inveted start/end location
                    this.CalculateLocation();

                    Notify("InvertEnd");
                }
            }
        }

        [IgnoreDataMember]
        public Location StartPoint
        {
            get { return startPoint; }
            set
            {
                if (value != startPoint)
                {
                    startPoint = value;
                    Notify("StartPoint");
                }
            }
        }

        [IgnoreDataMember]
        public Location EndPoint
        {
            get { return endPoint; }
            set
            {
                if (value != endPoint)
                {
                    endPoint = value;
                    Notify("EndPoint");
                }
            }
        }

        [IgnoreDataMember]
        public Location StartCenter
        {
            get { return startCenter; }
            set
            {
                if (value != startCenter)
                {
                    startCenter = value;
                    Notify("StartCenter");
                }
            }
        }

        [IgnoreDataMember]
        public Location EndCenter
        {
            get { return endCenter; }
            set
            {
                if (value != endCenter)
                {
                    endCenter = value;
                    Notify("EndCenter");
                }
            }
        }

        private void CalculateLocation()
        {
            // check for valid stan/end pin
            if (this.start == null || this.end == null)
            {
                return;
            }

            // check if obejct was initialized
            if (this.startPoint == null || this.endPoint == null || this.startCenter == null || this.endCenter == null)
            {
                return;
            }

            double startX = this.start.X;
            double startY = this.start.Y;
            double endX = this.end.X;
            double endY = this.end.Y;

            // calculate new inverted start/end position
            double alpha = Math.Atan2(startY - endY, endX - startX);
            double theta = Math.PI - alpha;
            double zet = theta - Math.PI / 2;
            double sizeX = Math.Sin(zet) * (invertedThickness - thickness);
            double sizeY = Math.Cos(zet) * (invertedThickness - thickness);

            // set line start location
            if (this.invertStart)
            {
                startCenter.X = startX + sizeX - invertedThickness;
                startCenter.Y = startY - sizeY - invertedThickness;

                startPoint.X = startX + (2 * sizeX);
                startPoint.Y = startY - (2 * sizeY);
            }
            else
            {
                startCenter.X = startX;
                startCenter.Y = startY;

                startPoint.X = startX;
                startPoint.Y = startY;
            }

            // set line end location
            if (this.invertEnd)
            {
                endCenter.X = endX - sizeX - invertedThickness;
                endCenter.Y = endY + sizeY - invertedThickness;

                endPoint.X = endX - (2 * sizeX);
                endPoint.Y = endY + (2 * sizeY);
            }
            else
            {
                endCenter.X = endX;
                endCenter.Y = endY;

                endPoint.X = endX;
                endPoint.Y = endY;
            }
        }

        #endregion
    }

    [DataContract]
    public class ConnectedLine : Element
    {
        #region Construtor

        public ConnectedLine() : base() { }

        #endregion

        #region Properties

        private ObservableCollection<Pin> pins = new ObservableCollection<Pin>();
        private ObservableCollection<Line> lines = new ObservableCollection<Line>();

        [DataMember]
        public ObservableCollection<Pin> Pins
        {
            get { return pins; }
            set
            {
                if (value != pins)
                {
                    pins = value;
                    Notify("Pins");
                }
            }
        }

        [DataMember]
        public ObservableCollection<Line> Lines
        {
            get { return lines; }
            set
            {
                if (value != lines)
                {
                    lines = value;
                    Notify("Lines");
                }
            }
        }

        #endregion
    }

    [DataContract]
    public class Context : Element
    {
        #region Construtor

        public Context() : base() { }

        #endregion

        #region Properties

        private bool hidePins = false;
        private bool hideHelperLines = true;
        private ObservableCollection<Pin> pins = new ObservableCollection<Pin>();
        private ObservableCollection<Line> lines = new ObservableCollection<Line>();
        private ObservableCollection<Element> elements = new ObservableCollection<Element>();
        private bool captureMouse = false;
        private ObservableCollection<ConnectedLine> connectedLines = new ObservableCollection<ConnectedLine>();
        private bool sync = false;

        [DataMember]
        public bool HidePins
        {
            get { return hidePins; }
            set
            {
                if (value != hidePins)
                {
                    hidePins = value;
                    Notify("HidePins");
                }
            }
        }

        [DataMember]
        public bool HideHelperLines
        {
            get { return hideHelperLines; }
            set
            {
                if (value != hideHelperLines)
                {
                    hideHelperLines = value;
                    Notify("HideHelperLines");
                }
            }
        }

        [DataMember]
        public ObservableCollection<Pin> Pins
        {
            get { return pins; }
            set
            {
                if (value != pins)
                {
                    pins = value;
                    Notify("Pins");
                }
            }
        }

        [DataMember]
        public ObservableCollection<Line> Lines
        {
            get { return lines; }
            set
            {
                if (value != lines)
                {
                    lines = value;
                    Notify("Lines");
                }
            }
        }

        [DataMember]
        public ObservableCollection<Element> Elements
        {
            get { return elements; }
            set
            {
                if (value != elements)
                {
                    elements = value;
                    Notify("Elements");
                }
            }
        }

        [IgnoreDataMember]
        public bool CaptureMouse
        {
            get { return captureMouse; }
            set
            {
                if (value != captureMouse)
                {
                    captureMouse = value;
                    Notify("CaptureMouse");
                }
            }
        }

        [IgnoreDataMember]
        public ObservableCollection<ConnectedLine> ConnectedLines
        {
            get { return connectedLines; }
            set
            {
                if (value != connectedLines)
                {
                    connectedLines = value;
                    Notify("ConnectedLines");
                }
            }
        }

        [IgnoreDataMember]
        public bool Sync
        {
            get { return sync; }
            set
            {
                if (value != sync)
                {
                    sync = value;
                    Notify("Sync");
                }
            }
        }

        #endregion
    }

    #endregion

    #region Data Model Extensions

    public static class ContextExtensions
    {
        #region Context Helpers

        public static void ResetContext(this Context context, double milliseconds)
        {
            if (context == null)
            {
                throw new ArgumentNullException();
            }

            // reset context
            context.Elements = new ObservableCollection<Element>();
            context.Lines = new ObservableCollection<Line>();
            context.ConnectedLines = new ObservableCollection<ConnectedLine>();
            context.Pins = new ObservableCollection<Pin>();
            context.CaptureMouse = false;

            // observe elements in context
            context.ObserveElements(1000.0);
        }

        #endregion

        #region Create Elements

        public static Line CreateLine(this Context context, Pin start, Pin end)
        {
            var line = new Line()
            {
                Start = start,
                End = end,
                X = 0,
                Y = 0,
                Z = 1,
                Id = context.Lines.Count().ToString(),
                DisableHitTest = true
            };

            context.Lines.Add(line);
            context.Elements.Add(line);

            return line;
        }

        public static Pin CreatePin(this Context context, double x, double y)
        {
            var pin = new Pin()
            {
                X = x,
                Y = y,
                Z = 2,
                Id = context.Pins.Count().ToString(),
                DisableHitTest = true
            };

            context.Pins.Add(pin);
            context.Elements.Add(pin);

            return pin;
        }

        #endregion

        #region Connected Lines

        public static void FindLinesAndPins(this Context context, Pin pin, List<Pin> pins, List<Line> lines)
        {
            if (context == null || pin == null || lines == null || pins == null)
            {
                throw new ArgumentNullException();
            }

            var q = context.Lines.Where(l => (l.Start == pin || l.End == pin));
            if (q == null)
            {
                return;
            }

            foreach (var line in q)
            {
                if (lines.Contains(line) == false)
                {
                    lines.Add(line);
                }

                if (line.Start != pin && pins.Contains(line.Start) == false)
                {
                    pins.Add(line.Start);

                    context.FindLinesAndPins(line.Start, pins, lines);
                }

                if (line.End != pin && pins.Contains(line.End) == false)
                {
                    pins.Add(line.End);

                    context.FindLinesAndPins(line.End, pins, lines);
                }
            }
        }

        public static ConnectedLine FindConnectedLine(this Context context, Pin pin)
        {
            if (context == null || pin == null)
            {
                throw new ArgumentNullException();
            }

            // create temporary lists
            var pins = new List<Pin>();
            var lines = new List<Line>();

            // find all conntected lines & pins to pin
            context.FindLinesAndPins(pin, pins, lines);

            // create new connected line (unique line & pin set)
            var connectedLine = new ConnectedLine()
            {
                Pins = new ObservableCollection<Pin>(pins),
                Lines = new ObservableCollection<Line>(lines)
            };

            // update all connected pins (they have same line set)
            foreach (var p in pins)
            {
                p.ConnectedLine = connectedLine;
                p.IsUpdated = true;
            }

            return connectedLine;
        }

        public static void UpdateConnectedLines(this Context context)
        {
            if (context == null)
            {
                throw new ArgumentNullException();
            }

            var s = System.Diagnostics.Stopwatch.StartNew();

            // reset IsUpdated flag for all pins  
            foreach (var pin in context.Pins)
            {
                pin.IsUpdated = false;
            }

            var connectedLines = new List<ConnectedLine>();

            // update all pins in context
            foreach (var pin in context.Pins)
            {
                if (pin.IsUpdated == false)
                {
                    var connectedLine = context.FindConnectedLine(pin);

                    connectedLines.Add(connectedLine);
                }
            }

            // update context connected lines list
            context.ConnectedLines = new ObservableCollection<ConnectedLine>(connectedLines);

            s.Stop();

            System.Diagnostics.Debug.Print("All UpdateConnectedLinesToPin in {0}ms, thread Id: {1}, connectedLines: {2}",
                s.Elapsed.TotalMilliseconds,
                System.Threading.Thread.CurrentThread.ManagedThreadId,
                connectedLines.Count);
        }

        public static IDisposable ObserveElements(this Context context, double milliseconds)
        {
            var observable = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => (s, e) => h(s, e),
                h => context.Elements.CollectionChanged += h,
                h => context.Elements.CollectionChanged -= h);

            var disposable = observable.ObserveOn(Scheduler.Default)
                .Throttle(TimeSpan.FromMilliseconds(milliseconds))
                .Subscribe(e =>
                {
                    // lock sync flag
                    context.Sync = true;

                    System.Diagnostics.Debug.Print("ObserveElements on context: {0}, action: {1}", context.Id, e.EventArgs.Action.ToString());

                    // update connected lines
                    UpdateConnectedLines(context);

                    // unlock sync flag
                    context.Sync = false;
                });

            return disposable;
        }

        #endregion
    }

    #endregion

    #region Context Serializer

    public static class ContextSerializer
    {
        public static Type[] GetTypes()
        {
            return new Type[]
            { 
                typeof(Location),
                typeof(Element),
                typeof(Pin),
                typeof(Line),
                typeof(Context)
            };
        }

        public static Context Open(string fileName)
        {
            var s = new DataContractSerializer(typeof(Context), GetTypes(), int.MaxValue, true, true, null);
            using (var reader = XmlReader.Create(fileName))
            {
                return (Context)s.ReadObject(reader);
            }
        }

        public static Context Open()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog()
            {
                DefaultExt = "xml",
                Filter = "Xml Files (*.xml)|*.xml|All Files (*.*)|*.*",
                FilterIndex = 0
            };

            if (dlg.ShowDialog() == true)
            {
                return Open(dlg.FileName);
            }
            return null;
        }

        public static void Save(Context context, string fileName)
        {
            if (context != null)
            {
                var s = new DataContractSerializer(context.GetType(), GetTypes(), int.MaxValue, true, true, null);
                using (var writer = XmlWriter.Create(fileName, new XmlWriterSettings() { Indent = true, IndentChars = "    " }))
                {
                    s.WriteObject(writer, context);
                }
            }
        }

        public static void Save(Context context)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog()
            {
                DefaultExt = "xml",
                Filter = "Xml Files (*.xml)|*.xml|All Files (*.*)|*.*",
                FilterIndex = 0,
                FileName = "context"
            };

            if (dlg.ShowDialog() == true)
            {
                Save(context, dlg.FileName);
            }
        }
    }

    #endregion
}
