using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using LogicCanvas.Model.Logic;
using LogicCanvas.Model.ControlBlock;

namespace LogicCanvas
{
    public class ItemTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is LineLogicItem)
                return (DataTemplate)App.Current.FindResource("LineLogicItemTemplate");
            else if (item is AndGateLogicItem)
                return (DataTemplate)App.Current.FindResource("AndGateLogicItemTemplate");
            else if (item is OrGateLogicItem)
                return (DataTemplate)App.Current.FindResource("OrGateLogicItemTemplate");
            else if (item is MemoryResetPriorityLogicItem)
                return (DataTemplate)App.Current.FindResource("MemoryResetPriorityLogicItemTemplate");
            else if (item is MemorySetPriorityLogicItem)
                return (DataTemplate)App.Current.FindResource("MemorySetPriorityLogicItemTemplate");
            else if (item is SequenceStepLogicItem)
                return (DataTemplate)App.Current.FindResource("SequenceStepLogicItemTemplate");
            else if (item is DataBlockLogicItem)
                return (DataTemplate)App.Current.FindResource("DataBlockLogicItemTemplate");
            else if (item is OneWayMotorControlBlockItem)
                return (DataTemplate)App.Current.FindResource("OneWayMotorControlBlockItemTemplate");
            else if (item is TwoWayMotorControlBlockItem)
                return (DataTemplate)App.Current.FindResource("TwoWayMotorControlBlockItemTemplate");
            else if (item is SolenoidValveControlBlockItem)
                return (DataTemplate)App.Current.FindResource("SolenoidValveControlBlockItemTemplate");
            else if (item is ThrottlingValveControlBlockItem)
                return (DataTemplate)App.Current.FindResource("ThrottlingValveControlBlockItemTemplate");
            else if (item is ControlValveControlBlockItem)
                return (DataTemplate)App.Current.FindResource("ControlValveControlBlockItemTemplate");
            else if (item is FrequencyConverterControlBlockItem)
                return (DataTemplate)App.Current.FindResource("FrequencyConverterControlBlockItemTemplate");
            else if (item is SequenceControlControlBlockItem)
                return (DataTemplate)App.Current.FindResource("SequenceControlControlBlockItemTemplate");
            else if (item is GroupControlTwoDevicesControlBlockItem)
                return (DataTemplate)App.Current.FindResource("GroupControlTwoDevicesControlBlockItemTemplate");
            else if (item is GroupControlThreeDevicesControlBlockItem)
                return (DataTemplate)App.Current.FindResource("GroupControlThreeDevicesControlBlockItemTemplate");
            else
                return base.SelectTemplate(item, container);
        }
    }
}
