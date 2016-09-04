using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace TestDemo
{
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            InitSnapMode();
        }
        
        private void InitSnapMode()
        {
            snapModePoint.Click += (sender, e) => UpdateSnapMode();
            snapModeMiddle.Click += (sender, e) => UpdateSnapMode();
            snapModeNearest.Click += (sender, e) => UpdateSnapMode();
            snapModeIntersection.Click += (sender, e) => UpdateSnapMode();
            snapModeHorizontal.Click += (sender, e) => UpdateSnapMode();
            snapModeVertical.Click += (sender, e) => UpdateSnapMode();
                
            snapModePoint.IsChecked = canvas.Editor.SnapMode.HasFlag(SnapMode.Point);
            snapModeMiddle.IsChecked = canvas.Editor.SnapMode.HasFlag(SnapMode.Middle);
            snapModeNearest.IsChecked = canvas.Editor.SnapMode.HasFlag(SnapMode.Nearest);
            snapModeIntersection.IsChecked = canvas.Editor.SnapMode.HasFlag(SnapMode.Intersection);
            snapModeHorizontal.IsChecked = canvas.Editor.SnapMode.HasFlag(SnapMode.Horizontal);
            snapModeVertical.IsChecked = canvas.Editor.SnapMode.HasFlag(SnapMode.Vertical);
        }

        private void UpdateSnapMode()
        {
            if (snapModePoint.IsChecked == true)
                canvas.Editor.SnapMode |= SnapMode.Point;
            else
                canvas.Editor.SnapMode &= ~SnapMode.Point;
            
            if (snapModeMiddle.IsChecked == true)
                canvas.Editor.SnapMode |= SnapMode.Middle;
            else
                canvas.Editor.SnapMode &= ~SnapMode.Middle;
            
            if (snapModeNearest.IsChecked == true)
                canvas.Editor.SnapMode |= SnapMode.Nearest;
            else
                canvas.Editor.SnapMode &= ~SnapMode.Nearest;
            
            if (snapModeIntersection.IsChecked == true)
                canvas.Editor.SnapMode |= SnapMode.Intersection;
            else
                canvas.Editor.SnapMode &= ~SnapMode.Intersection;

            if (snapModeHorizontal.IsChecked == true)
                canvas.Editor.SnapMode |= SnapMode.Horizontal;
            else
                canvas.Editor.SnapMode &= ~SnapMode.Horizontal;
            
            if (snapModeVertical.IsChecked == true)
                canvas.Editor.SnapMode |= SnapMode.Vertical;
            else
                canvas.Editor.SnapMode &= ~SnapMode.Vertical;
        }
    }
}
