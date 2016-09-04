#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Logic.Model.Core;
using System.Windows.Controls.Primitives;

#endregion

namespace Logic
{
    #region DiagramView

    public partial class DiagramView : UserControl
    {
        #region Contructor

        public DiagramView()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var element = (sender as FrameworkElement).DataContext as DigitalLogic;
            if (element != null)
                MovelElement(e.HorizontalChange, e.VerticalChange, element);

            e.Handled = true;
        }

        private static void MovelElement(double dX, double dY, DigitalLogic element)
        {
            element.X += dX;
            element.Y += dY;

            foreach (var pin in element.Pins)
            {
                pin.X += dX;
                pin.Y += dY;
            }
        }

        #endregion
    }

    #endregion
}
