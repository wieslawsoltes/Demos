using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LogicCanvas.Model.Core;

namespace LogicCanvas.Model.ControlBlock
{
    [XmlRoot("SolenoidValve"), XmlType("SolenoidValve"), Serializable]
    public class SolenoidValveControlBlockItem : Item, IControlBlockItem
    {

    }
}
