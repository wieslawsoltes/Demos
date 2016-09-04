using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LogicCanvas.Model.Core;

namespace LogicCanvas.Model.ControlBlock
{
    [XmlRoot("SequenceControl"), XmlType("SequenceControl"), Serializable]
    public class SequenceControlControlBlockItem : Item, IControlBlockItem
    {

    }
}
