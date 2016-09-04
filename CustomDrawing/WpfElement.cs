#region References

using CustomDrawing.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media; 

#endregion

namespace CustomDrawing
{
    #region WpfElement

    public class WpfElement : FrameworkElement
    {
        #region Properties

        public Painter Painter { get; protected set; }

        #endregion

        #region Constructor

        public WpfElement(Painter painter)
        {
            Painter = painter;
            painter.Renderer = new WpfRenderer();
        }

        #endregion

        #region OnRender

        protected override void OnRender(DrawingContext drawingContext)
        {
            var sw = Stopwatch.StartNew();

            Render(drawingContext);

            sw.Stop();
            Debug.Print("Render: " + sw.Elapsed.TotalMilliseconds + "ms");
        }

        private void Render(DrawingContext dc)
        {
            (Painter.Renderer as WpfRenderer).Set(dc);
            Painter.DoRender = true;
            Painter.Draw(Painter.Elements, Painter.Origin, 0f, 0f);
            Painter.DoHitTest = false;
            Painter.DoRender = false;
        }

        public void Redraw()
        {
            this.InvalidateVisual();
        }

        #endregion
    }

    #endregion
}
