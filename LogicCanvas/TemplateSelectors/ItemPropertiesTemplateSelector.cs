using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using LogicCanvas.Model.Logic;
using LogicCanvas.Model.ControlBlock;

namespace LogicCanvas
{
    public class ItemPropertiesTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is LineLogicItem)
                return null;
            else if (item is AndGateLogicItem)
                return null;
            else if (item is OrGateLogicItem)
                return null;
            else if (item is MemoryResetPriorityLogicItem)
                return null;
            else if (item is MemorySetPriorityLogicItem)
                return null;
            else if (item is SequenceStepLogicItem)
                return null;
            else if (item is DataBlockLogicItem)
                return (DataTemplate)App.Current.FindResource("DataBlockLogicPropertiesKey");
            else if (item is OneWayMotorControlBlockItem)
                return null;
            else if (item is TwoWayMotorControlBlockItem)
                return null;
            else if (item is SolenoidValveControlBlockItem)
                return null;
            else if (item is ThrottlingValveControlBlockItem)
                return null;
            else if (item is ControlValveControlBlockItem)
                return null;
            else if (item is FrequencyConverterControlBlockItem)
                return null;
            else if (item is SequenceControlControlBlockItem)
                return null;
            else if (item is GroupControlTwoDevicesControlBlockItem)
                return null;
            else if (item is GroupControlThreeDevicesControlBlockItem)
                return null;
            else
                return base.SelectTemplate(item, container);
        }
    }
}
