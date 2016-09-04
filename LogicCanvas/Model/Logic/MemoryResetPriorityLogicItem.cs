using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LogicCanvas.Model.Core;

namespace LogicCanvas.Model.Logic
{
    [XmlRoot("MemoryResetPriority"), XmlType("MemoryResetPriority"), Serializable]
    public class MemoryResetPriorityLogicItem : Item, ILogicItem
    {

    }
}
