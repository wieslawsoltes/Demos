#region References

using Logic.Model.Core;
using Logic.Model.Diagrams;
using Logic.Model.Gates;
using Logic.Model.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

#endregion

namespace Logic.Model
{
    #region Serializer

    public static class Serializer
    {
        public static Type[] GetDiagramTypes()
        {
            return new Type[]
            { 
                typeof(DigitalPin),
                typeof(DigitalSignal),
                typeof(DigitalWire),
                typeof(AndGate),
                typeof(OrGate),
                typeof(NotGate),
                typeof(BufferGate),
                typeof(NandGate),
                typeof(NorGate),
                typeof(XorGate),
                typeof(XnorGate),
                typeof(TimerPulse),
                typeof(TimerOnDelay),
                typeof(DigitalLogicDiagram)
            };
        }

        public static DigitalLogicDiagram OpenDiagram()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog()
            {
                DefaultExt = "xml",
                Filter = "Xml Files (*.xml)|*.xml|All Files (*.*)|*.*",
                FilterIndex = 0
            };

            if (dlg.ShowDialog() == true)
            {
                var s = new DataContractSerializer(typeof(DigitalLogicDiagram), GetDiagramTypes(), int.MaxValue, true, true, null);
                using (var reader = XmlReader.Create(dlg.FileName))
                {
                    return (DigitalLogicDiagram)s.ReadObject(reader);
                }
            }
            return null;
        }

        public static void SaveDiagram(DigitalLogicDiagram diagram)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog()
            {
                DefaultExt = "xml",
                Filter = "Xml Files (*.xml)|*.xml|All Files (*.*)|*.*",
                FilterIndex = 0,
                FileName = "diagram"
            };

            if (dlg.ShowDialog() == true)
            {
                if (diagram != null)
                {
                    var s = new DataContractSerializer(diagram.GetType(), GetDiagramTypes(), int.MaxValue, true, true, null);
                    using (var writer = XmlWriter.Create(dlg.FileName, new XmlWriterSettings() { Indent = true, IndentChars = "    " }))
                    {
                        s.WriteObject(writer, diagram);
                    }
                }
            }
        }
    }

    #endregion
}
