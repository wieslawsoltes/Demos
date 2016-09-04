// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.Model
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion

    #region Defaults

    public static class Defaults
    {
        #region Constants

        public const string DataFormat = "LogicObjectType";

        public const string OptionsFileName = "options.json";
        public const string RecentFileName = "recent.json";
        public const string ColorsFileName = "colors.json";

        public const string DefaultElement = "Signal";

        public const double NewPinZIndex = 1.0;
        public const double PinZIndex = 2.0;
        public const double LineZIndex = 1.0;
        public const double SignalZIndex = 1.0;
        public const double ElementZIndex = 1.0;

        #endregion

        #region Properties

        private static Options options = new Options();

        public static Options Options
        {
            get { return options; }
            set 
            {
                if (value != options)
                {
                    options = value;
                }
            }
        }

        #endregion

        #region Methods

        public static void SetDefaults(Options options)
        {
            options.HidePins = false;
            options.HideHelperLines = true;

            options.ShortenLineStarts = true;
            options.ShortenLineEnds = false;

            options.X = 0.0;
            options.Y = 0.0;

            options.Zoom = 1.0;
            options.ZoomSpeed = 3.5;
            options.Thickness = 1.0;
            options.Snap = 15.0;
            options.OffsetX = 0.0;
            options.OffsetY = 5.0;

            options.IsSnapEnabled = true;
            options.IsAutoFitEnabled = true;

            options.CaptureMouse = false;
            options.Sync = false;
            options.IsPrinting = false;

            options.EnableCharts = false;

            options.SimulationPeriod = 100;

            options.CurrentElement = DefaultElement;

            options.ShowSolutionExplorer = true;
            options.ShowToolbox = true;

            options.EnableColors = true;
            options.EnableRecent = true;
            options.EnableUndoRedo = true;

            options.DisablePrintColors = false;

            options.DefaultPageType = PageType.Logic;
        }

        #endregion
    }

    #endregion
}
