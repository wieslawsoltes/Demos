using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BoundsTest
{
    public static class FrameworkElementCanvasExtensions
    {
        public static double GetX(this FrameworkElement element)
        {
            return Canvas.GetLeft(element);
        }

        public static double GetY(this FrameworkElement element)
        {
            return Canvas.GetTop(element);
        }

        public static double GetWidth(this FrameworkElement element)
        {
            return element.Width;
        }

        public static double GetHeight(this FrameworkElement element)
        {
            return element.Height;
        }

        public static Point GetLocation(this FrameworkElement element)
        {
            return new Point(Canvas.GetLeft(element), Canvas.GetTop(element));
        }

        public static Size GetSize(this FrameworkElement element)
        {
            return new Size(element.Width, element.Height);
        }

        public static Rect GetRect(this FrameworkElement element)
        {
            return new Rect(Canvas.GetLeft(element), Canvas.GetTop(element), element.Width, element.Height);
        }

        public static void SetX(this FrameworkElement element, double x)
        {
            Canvas.SetLeft(element, x);
        }

        public static void SetY(this FrameworkElement element, double y)
        {
            Canvas.SetTop(element, y);
        }

        public static void SetWidth(this FrameworkElement element, double width)
        {
            element.Width = width;
        }

        public static void SetHeight(this FrameworkElement element, double height)
        {
            element.Height = height;
        }

        public static void Move(this FrameworkElement element, Point position)
        {
            Canvas.SetLeft(element, position.X);
            Canvas.SetTop(element, position.Y);
        }

        public static void Move(this FrameworkElement element, double x, double y)
        {
            Canvas.SetLeft(element, x);
            Canvas.SetTop(element, y);
        }

        public static void MoveDelta(this FrameworkElement element, Point delta)
        {
            Canvas.SetLeft(element, Canvas.GetLeft(element) - delta.X);
            Canvas.SetTop(element, Canvas.GetTop(element) - delta.Y);
        }

        public static void MoveDelta(this FrameworkElement element, double dx, double dy)
        {
            Canvas.SetLeft(element, Canvas.GetLeft(element) - dx);
            Canvas.SetTop(element, Canvas.GetTop(element) - dy);
        }

        public static void Resize(this FrameworkElement element, Size size)
        {
            element.Width = size.Width;
            element.Height = size.Height;
        }

        public static void Resize(this FrameworkElement element, double width, double height)
        {
            element.Width = width;
            element.Height = height;
        }

        public static void ResizeDelta(this FrameworkElement element, Size delta)
        {
            element.Width += delta.Width;
            element.Height += delta.Height;
        }

        public static void ResizeDelta(this FrameworkElement element, double deltaWidth, double deltaHeight)
        {
            element.Width += deltaWidth;
            element.Height += deltaHeight;
        }

        public static void MoveAndResize(this FrameworkElement element, Rect rect)
        {
            Canvas.SetLeft(element, rect.X);
            Canvas.SetTop(element, rect.Y);
            element.Width = rect.Width;
            element.Height = rect.Height;
        }

        public static void MoveAndResize(this FrameworkElement element, double x, double y, double width, double height)
        {
            Canvas.SetLeft(element, x);
            Canvas.SetTop(element, y);
            element.Width = width;
            element.Height = height;
        }
    }
}
