// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace WpfMatrix
{
    #region MainWindow

    public partial class MainWindow : Window
    {
        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        } 

        #endregion

        #region Windows Events

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

        } 

        #endregion

        #region Transform

        public void Transform(FrameworkElement element)
        {
            // layout transform
            var lm = new Matrix()
            {
                M11 = LayoutTransformMatrixM11.Value,
                M12 = LayoutTransformMatrixM12.Value,
                M21 = LayoutTransformMatrixM21.Value,
                M22 = LayoutTransformMatrixM22.Value,
                OffsetX = LayoutTransformMatrixOffsetX.Value,
                OffsetY = LayoutTransformMatrixOffsetY.Value,
            };

            lm.ScalePrepend(LayoutTransformScaleX.Value, LayoutTransformScaleY.Value);
            lm.SkewPrepend(LayoutTransformSkewX.Value, LayoutTransformSkewY.Value);
            lm.RotatePrepend(LayoutTransformRotateAngle.Value);
            lm.TranslatePrepend(LayoutTransformOffsetX.Value, LayoutTransformOffsetY.Value);

            var lt = new MatrixTransform(lm);

            // render transform
            var rm = new Matrix()
            {
                M11 = RenderTransformMatrixM11.Value,
                M12 = RenderTransformMatrixM12.Value,
                M21 = RenderTransformMatrixM21.Value,
                M22 = RenderTransformMatrixM22.Value,
                OffsetX = RenderTransformMatrixOffsetX.Value,
                OffsetY = RenderTransformMatrixOffsetY.Value,
            };

            rm.ScalePrepend(RenderTransformScaleX.Value, RenderTransformScaleY.Value);
            rm.SkewPrepend(RenderTransformSkewX.Value, RenderTransformSkewY.Value);
            rm.RotatePrepend(RenderTransformRotateAngle.Value);
            rm.TranslatePrepend(RenderTransformOffsetX.Value, RenderTransformOffsetY.Value);

            var rt = new MatrixTransform(rm);

            // render transform origin
            element.RenderTransformOrigin = new Point(RenderTransformOriginX.Value, RenderTransformOriginY.Value);

            // set element transforms
            element.LayoutTransform = lt;
            element.RenderTransform = rt;
        }

        #endregion

        #region Slider Events

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.IsLoaded == false)
                return;

            Transform(this.canvas);
        } 

        #endregion
    } 

    #endregion
}
