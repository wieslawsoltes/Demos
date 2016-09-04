using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LogicCanvas.Model.Core;

namespace LogicCanvas.Model.ControlBlock
{
    [XmlRoot("OneWayMotor"), XmlType("OneWayMotor"), Serializable]
    public class OneWayMotorControlBlockItem : Item, IControlBlockItem
    {

    }
}
