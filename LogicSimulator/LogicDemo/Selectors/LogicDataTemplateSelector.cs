#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace Logic.Selectors
{
    #region LogicDataTemplateSelector

    public class LogicDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            try
            {
                var key = item.GetType().Name + "DataTemplateKey";
                return (DataTemplate)Application.Current.FindResource(key);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print("Exception: {0}", ex.Message);
            }
            return null;
        }
    } 

    #endregion
}
