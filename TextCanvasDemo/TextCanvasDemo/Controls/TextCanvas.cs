// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TextCanvasDemo.Controls
{
    public class TextCanvas : Canvas
    {
        private StackPanel _stackPanel;
        private TextBox _textBox;

        public TextCanvas()
        {
            _stackPanel = new StackPanel();
            _stackPanel.Visibility = Visibility.Hidden;
            Panel.SetZIndex(_stackPanel, 10);
            Canvas.SetLeft(_stackPanel, 0);
            Canvas.SetTop(_stackPanel, 0);
            Children.Add(_stackPanel);

            _textBox = new TextBox();
            _textBox.Text = "";
            _textBox.Width = 100;
            _textBox.FontSize = 12;
            _textBox.Background = Brushes.LightYellow;
            _textBox.BorderBrush = null;
            _textBox.BorderThickness = new Thickness(0);
            _textBox.FocusVisualStyle = null;
            _stackPanel.Children.Add(_textBox);

            _textBox.KeyDown += (s, e) => KeyDownHandler(e);

            MouseRightButtonDown += (s, e) => RightDownHandler();
            MouseLeftButtonDown += (s, e) => LeftDownHandler(e);
        }

        private void RightDownHandler()
        {
            if (_stackPanel.Visibility == Visibility.Visible)
            {
                _stackPanel.Visibility = Visibility.Hidden;
            }
        }

        private void LeftDownHandler(MouseButtonEventArgs e)
        {
            var p = e.GetPosition(this);
            if (_stackPanel.Visibility == Visibility.Hidden)
            {
                if (e.ClickCount == 2)
                {
                    Canvas.SetLeft(_stackPanel, p.X - _textBox.ActualWidth / 2);
                    Canvas.SetTop(_stackPanel, p.Y - _textBox.ActualHeight / 2);
                    _stackPanel.Visibility = Visibility.Visible;
                    _textBox.Focus();
                }
            }
            else
            {
                _stackPanel.Visibility = Visibility.Hidden;
                AddBlock();
            }
        }

        private void KeyDownHandler(KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                _stackPanel.Visibility = Visibility.Hidden;
                AddBlock();
            }

            if (e.Key == Key.Escape)
            {
                _stackPanel.Visibility = Visibility.Hidden;
            }
        }

        private void AddBlock()
        {
            var text = _textBox.Text;

            var block = new TextBlock()
            {
                Text = text,
                Width = _textBox.ActualWidth,
                Height = _textBox.ActualHeight,
                FontSize = _textBox.FontSize,
                Background = Brushes.White,
                Foreground = Brushes.Blue
            };

            var x = Canvas.GetLeft(_stackPanel);
            var y = Canvas.GetTop(_stackPanel);
            Canvas.SetLeft(block, x);
            Canvas.SetTop(block, y);

            Children.Add(block);
            _textBox.Text = "";
        }
    }
}
