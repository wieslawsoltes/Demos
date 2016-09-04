// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

using WireDiagram.Core.Items;
using WireDiagram.Nodes.Model;

namespace WireDiagram
{
    public class ItemTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return (DataTemplate)App.Current.Windows[0].FindResource(item.GetType().Name + "DataTemplateKey");
        }
    }
}
