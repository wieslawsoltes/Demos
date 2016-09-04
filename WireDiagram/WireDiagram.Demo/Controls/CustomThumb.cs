// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using WireDiagram.Core.Items;
using WireDiagram.Core.Lists;
using WireDiagram.Core.Model;

namespace WireDiagram.Controls
{
    public class CustomThumb : Thumb
    {
        public CustomThumb()
        {
            this.PreviewMouseLeftButtonDown += (sender, e) =>
            {
                if (this.DataContext is Node && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    var node = this.DataContext as Node;

                    if (node.IsSelected)
                    {
                        if (node.Diagram.SelectedItems != null)
                        {
                            if (node.Diagram.SelectedItems.Contains(node))
                            {
                                node.Diagram.SelectedItems.Remove(node);
                                if (node.Diagram.SelectedItems.Count == 0)
                                {
                                    node.Diagram.SelectedItems = null;
                                }
                            }
                        }

                        node.IsSelected = false;
                    }
                    else
                    {
                        if (node.Diagram.SelectedItems == null)
                        {
                            node.Diagram.SelectedItems = new ItemList();
                        }

                        if (!node.Diagram.SelectedItems.Contains(node))
                        {
                            node.IsSelected = true;
                            node.Diagram.SelectedItems.Add(node);
                        }
                    }
                }
            };
        }
    }
}
