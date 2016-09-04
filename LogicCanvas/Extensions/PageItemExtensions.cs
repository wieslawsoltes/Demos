using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LogicCanvas.Model.Logic;
using LogicCanvas.Model.ControlBlock;
using LogicCanvas.Model.Page;

namespace LogicCanvas.Extensions
{
    public static class PageItemExtensions
    {
        public static Type[] GetTypes(this PageItem page)
        {
            return new Type[] 
                {
                    // item types: logic
                    typeof(LineLogicItem),
                    typeof(AndGateLogicItem),
                    typeof(OrGateLogicItem),
                    typeof(MemoryResetPriorityLogicItem),
                    typeof(MemorySetPriorityLogicItem),
                    typeof(SequenceStepLogicItem),
					typeof(DataBlockLogicItem),
                    // item types: control block
                    typeof(OneWayMotorControlBlockItem),
                    typeof(TwoWayMotorControlBlockItem),
                    typeof(SolenoidValveControlBlockItem),
                    typeof(ThrottlingValveControlBlockItem),
                    typeof(ControlValveControlBlockItem),
                    typeof(FrequencyConverterControlBlockItem),
                    typeof(SequenceControlControlBlockItem),
                    typeof(GroupControlTwoDevicesControlBlockItem),
                    typeof(GroupControlThreeDevicesControlBlockItem)
                };
        }
    }
}
