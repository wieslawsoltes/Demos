// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Model
{
    #region References

    using Logic.Model.Core;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    #endregion

    #region Options

    [DataContract]
    public class Options : NotifyObject
    {
        #region Construtor

        public Options() : base() { }

        #endregion

        #region Properties

        private bool hidePins = false;
        private bool hideHelperLines = true;

        private bool shortenLineStarts = true;
        private bool shortenLineEnds = false;

        private double x = 0.0;
        private double y = 0.0;
        private double zoom = 1.0;
        private double zoomSpeed = 3.5;
        private double thickness = 1.0;
        private double snap = 15.0;
        private double offsetX = 0.0;
        private double offsetY = 5.0;

        private bool isSnapEnabled = true;
        private bool isAutoFitEnabled = true;

        private bool captureMouse = false;

        private bool isPrinting = false;

        private bool enableCharts = false;

        private int simulationPeriod = 100;

        private bool simulationIsRunning = false;

        private string currentElement = "Signal";

        private bool showSolutionExplorer = true;
        private bool showToolbox = true;

        private bool enableColors = true;
        private bool enableRecent = true;
        private bool enableUndoRedo = true;

        private bool disablePrintColors = false;

        private PageType defaultPageType = PageType.Logic;

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
        public bool ShortenLineStarts
        {
            get { return shortenLineStarts; }
            set
            {
                if (value != shortenLineStarts)
                {
                    shortenLineStarts = value;
                    Notify("ShortenLineStarts");
                }
            }
        }

        [DataMember]
        public bool ShortenLineEnds
        {
            get { return shortenLineEnds; }
            set
            {
                if (value != shortenLineEnds)
                {
                    shortenLineEnds = value;
                    Notify("ShortenLineEnds");
                }
            }
        }

        [IgnoreDataMember]
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

        [IgnoreDataMember]
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

        [IgnoreDataMember]
        public double Zoom
        {
            get { return zoom; }
            set
            {
                if (value != zoom)
                {
                    zoom = value;
                    Notify("Zoom");
                    Notify("Thickness");
                }
            }
        }

        [DataMember]
        public double ZoomSpeed
        {
            get { return zoomSpeed; }
            set
            {
                if (value != zoomSpeed)
                {
                    zoomSpeed = value;
                    Notify("ZoomSpeed");
                }
            }
        }

        [IgnoreDataMember]
        public double Thickness
        {
            get { return thickness / zoom; }
            set
            {
                if (value != thickness)
                {
                    thickness = value / zoom;
                    Notify("Thickness");
                }
            }
        }

        [DataMember]
        public double Snap
        {
            get { return snap; }
            set
            {
                if (value != snap)
                {
                    snap = value;
                    Notify("Snap");
                }
            }
        }

        [DataMember]
        public double OffsetX
        {
            get { return offsetX; }
            set
            {
                if (value != offsetX)
                {
                    offsetX = value;
                    Notify("OffsetX");
                }
            }
        }

        [DataMember]
        public double OffsetY
        {
            get { return offsetY; }
            set
            {
                if (value != offsetY)
                {
                    offsetY = value;
                    Notify("OffsetY");
                }
            }
        }

        [DataMember]
        public bool IsSnapEnabled
        {
            get { return isSnapEnabled; }
            set
            {
                if (value != isSnapEnabled)
                {
                    isSnapEnabled = value;
                    Notify("IsSnapEnabled");
                }
            }
        }

        [DataMember]
        public bool IsAutoFitEnabled
        {
            get { return isAutoFitEnabled; }
            set
            {
                if (value != isAutoFitEnabled)
                {
                    isAutoFitEnabled = value;
                    Notify("IsAutoFitEnabled");
                }
            }
        }

        [DataMember]
        public bool EnableCharts
        {
            get { return enableCharts; }
            set
            {
                if (value != enableCharts)
                {
                    enableCharts = value;
                    Notify("EnableCharts");
                }
            }
        }

        [DataMember]
        public int SimulationPeriod
        {
            get { return simulationPeriod; }
            set
            {
                if (value != simulationPeriod)
                {
                    simulationPeriod = value;
                    Notify("SimulationPeriod");
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
        public volatile bool Sync;

        [IgnoreDataMember]
        public bool IsPrinting
        {
            get { return isPrinting; }
            set
            {
                if (value != isPrinting)
                {
                    isPrinting = value;
                    Notify("IsPrinting");
                }
            }
        }

        [IgnoreDataMember]
        public bool SimulationIsRunning
        {
            get { return simulationIsRunning; }
            set
            {
                if (value != simulationIsRunning)
                {
                    simulationIsRunning = value;
                    Notify("SimulationIsRunning");
                }
            }
        }

        [IgnoreDataMember]
        public string CurrentElement
        {
            get { return currentElement; }
            set
            {
                if (value != currentElement)
                {
                    currentElement = value;
                    Notify("CurrentElement");
                }
            }
        }

        [DataMember]
        public bool ShowSolutionExplorer
        {
            get { return showSolutionExplorer; }
            set
            {
                if (value != showSolutionExplorer)
                {
                    showSolutionExplorer = value;
                    Notify("ShowSolutionExplorer");
                }
            }
        }

        [DataMember]
        public bool ShowToolbox
        {
            get { return showToolbox; }
            set
            {
                if (value != showToolbox)
                {
                    showToolbox = value;
                    Notify("ShowToolbox");
                }
            }
        }

        [DataMember]
        public bool EnableColors
        {
            get { return enableColors; }
            set
            {
                if (value != enableColors)
                {
                    enableColors = value;
                    Notify("EnableColors");
                }
            }
        }

        [DataMember]
        public bool EnableRecent
        {
            get { return enableRecent; }
            set
            {
                if (value != enableRecent)
                {
                    enableRecent = value;
                    Notify("EnableRecent");
                }
            }
        }

        [DataMember]
        public bool EnableUndoRedo
        {
            get { return enableUndoRedo; }
            set
            {
                if (value != enableUndoRedo)
                {
                    enableUndoRedo = value;
                    Notify("EnableUndoRedo");
                }
            }
        }

        [DataMember]
        public bool DisablePrintColors
        {
            get { return disablePrintColors; }
            set
            {
                if (value != disablePrintColors)
                {
                    disablePrintColors = value;
                    Notify("DisablePrintColors");
                }
            }
        }

        [DataMember]
        public PageType DefaultPageType
        {
            get { return defaultPageType; }
            set
            {
                if (value != defaultPageType)
                {
                    defaultPageType = value;
                    Notify("DefaultPageType");
                }
            }
        }

        #endregion
    }

    #endregion
}
