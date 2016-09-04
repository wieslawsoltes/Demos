using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using LogicCanvas.Model.Logic;

namespace LogicCanvas
{
    class ItemContainerStyleSelector : StyleSelector
    {
        public override System.Windows.Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            if(item is LineLogicItem)
            {
                // TODO: Move resources to App.xaml
                //return (System.Windows.Style) App.Current.FindResource("LineItemStyleKey");
                return (System.Windows.Style)App.Current.Windows[0].FindResource("LineItemStyleKey");
                //return null;
            }
            else
            {
                // TODO: Move resources to App.xaml
                //return (System.Windows.Style) App.Current.FindResource("ItemStyleKey");
                return (System.Windows.Style) App.Current.Windows[0].FindResource("ItemStyleKey");
                //return base.SelectStyle(item, container);
            }
        }

    }
}
