#region References

using Logic.Model;
using Logic.Model.Core;
using Logic.Model.Diagrams;
using Logic.Model.Gates;
using Logic.Model.Rx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks; 

#endregion

namespace Logic.Tests
{
    #region Tests: Digital Logic Diagrams

    public static class LogicDiagramTests
    {
        #region Test Diagram #1 (Timers)

        public static DigitalLogicDiagram GetTestDigitalLogicDiagram1(IScheduler scheduler)
        {
            var diagram = new DigitalLogicDiagram() { Id = Guid.NewGuid() };

            var input1 = Factory.NewDigitalSignal("input1", 0, 90, 0);
            var input2 = Factory.NewDigitalSignal("input2", 0, 150, 0);

            var output1 = Factory.NewDigitalSignal("output1", 0, 0, 0);
            var output2 = Factory.NewDigitalSignal("output2", 0, 0, 0);
            var output3 = Factory.NewDigitalSignal("output3", 450, 90, 0);

            var andGate1 = Factory.NewAndGate("andGate1", 180, 90, 0);
            var timerOnDelay1 = Factory.NewTimerOnDelay("timerOnDelay1", 270, 90, 0, 2.0);
            var timerPulse1 = Factory.NewTimerPulse("timerPulse1", 360, 90, 0, 1.0);

            var wire1 = Factory.NewDigitalWire("wire1", input1.OutputPin, andGate1.Pins[3], input1);
            var wire2 = Factory.NewDigitalWire("wire2", input2.OutputPin, andGate1.Pins[2], input2);

            var wire3 = Factory.NewDigitalWire("wire3", andGate1.Pins[1], timerOnDelay1.Pins[3], output1);
            var wire4 = Factory.NewDigitalWire("wire4", timerOnDelay1.Pins[1], timerPulse1.Pins[3], output2);
            var wire5 = Factory.NewDigitalWire("wire5", timerPulse1.Pins[1], output3.InputPin, output3);

            andGate1.Outputs.Add(output1);
            andGate1.Inputs.Add(input1);
            andGate1.Inputs.Add(input2);

            timerOnDelay1.Outputs.Add(output2);
            timerOnDelay1.Inputs.Add(output1);

            timerPulse1.Outputs.Add(output3);
            timerPulse1.Inputs.Add(output2);

            diagram.Elements.Add(wire1);
            diagram.Elements.Add(wire2);
            diagram.Elements.Add(wire3);
            diagram.Elements.Add(wire4);
            diagram.Elements.Add(wire5);
            diagram.Elements.Add(andGate1);
            diagram.Elements.Add(timerOnDelay1);
            diagram.Elements.Add(timerPulse1);
            diagram.Elements.Add(input1);
            diagram.Elements.Add(input2);
            diagram.Elements.Add(output3);

            output1.State = false;
            output2.State = false;
            output3.State = false;
            input1.State = false;
            input2.State = false;

            diagram.ObserveInputs(scheduler, diagram.Disposables);
            diagram.ObserveElements(scheduler, diagram.Disposables);

            return diagram;
        }

        #endregion

        #region Test Diagram #2 (SR NOR latch)

        public static DigitalLogicDiagram GetTestDigitalLogicDiagram2(IScheduler scheduler)
        {
            // SR NOR latch
            // SR latch operation
            // S	R	Action
            // 0	0	No Change
            // 0	1	Q = 0
            // 1	0	Q = 1
            // 1	1	Restricted combination
            // mofre info: http://en.wikipedia.org/wiki/Flip-flop_(electronics)

            var diagram = new DigitalLogicDiagram() { Id = Guid.NewGuid() };

            var input1 = new DigitalSignal()
            {
                Id = Guid.NewGuid(),
                Name = "R",
                X = 0,
                Y = 90
            };

            var input2 = new DigitalSignal()
            {
                Id = Guid.NewGuid(),
                Name = "S",
                X = 0,
                Y = 150
            };

            var output1 = new DigitalSignal()
            {
                Id = Guid.NewGuid(),
                Name = "Q",
                X = 270,
                Y = 90
            };

            var output2 = new DigitalSignal() // TODO: Q' must be 'true' for the init value for the SR ("set-reset") latch
            {
                Id = Guid.NewGuid(),
                Name = "Q'",
                X = 270,
                Y = 150
            };

            var norGate1 = new NorGate()
            {
                Id = Guid.NewGuid(),
                Name = "NorGate1",
                Inputs = { input1, output2 },
                Outputs = { output1 }, // TODO: add Inputs before Outputs to properly init diagram
                X = 180,
                Y = 90
            };

            var norGate2 = new NorGate()
            {
                Id = Guid.NewGuid(),
                Name = "NorGate2",
                Inputs = { input2, output1 },
                Outputs = { output2 }, // TODO: add Inputs before Outputs to properly init diagram
                X = 180,
                Y = 150
            };

            var wire1 = new DigitalWire()
            {
                Id = Guid.NewGuid(),
                Name = "wire1",
                Signal = input1,
                StartPin = new DigitalPin()
                {
                    Id = Guid.NewGuid(),
                    Name = "startPin",
                    X = 120,
                    Y = 105
                },
                EndPin = new DigitalPin()
                {
                    Id = Guid.NewGuid(),
                    Name = "endPin",
                    X = 180,
                    Y = 105
                }
            };

            var wire2 = new DigitalWire()
            {
                Id = Guid.NewGuid(),
                Name = "wire2",
                Signal = input2,
                StartPin = new DigitalPin()
                {
                    Id = Guid.NewGuid(),
                    Name = "startPin",
                    X = 120,
                    Y = 165
                },
                EndPin = new DigitalPin()
                {
                    Id = Guid.NewGuid(),
                    Name = "endPin",
                    X = 180,
                    Y = 165
                }
            };

            var wire3 = new DigitalWire()
            {
                Id = Guid.NewGuid(),
                Name = "wire3",
                Signal = output1,
                StartPin = new DigitalPin()
                {
                    Id = Guid.NewGuid(),
                    Name = "startPin",
                    X = 218,
                    Y = 105
                },
                EndPin = new DigitalPin()
                {
                    Id = Guid.NewGuid(),
                    Name = "endPin",
                    X = 270,
                    Y = 105
                }
            };

            var wire4 = new DigitalWire()
            {
                Id = Guid.NewGuid(),
                Name = "wire4",
                Signal = output2,
                StartPin = new DigitalPin()
                {
                    Id = Guid.NewGuid(),
                    Name = "startPin",
                    X = 218,
                    Y = 165
                },
                EndPin = new DigitalPin()
                {
                    Id = Guid.NewGuid(),
                    Name = "endPin",
                    X = 270,
                    Y = 165
                }
            };

            var wire5 = new DigitalWire()
            {
                Id = Guid.NewGuid(),
                Name = "wire5",
                Signal = output1,
                StartPin = new DigitalPin()
                {
                    Id = Guid.NewGuid(),
                    Name = "startPin",
                    X = 240,
                    Y = 105
                },
                EndPin = new DigitalPin()
                {
                    Id = Guid.NewGuid(),
                    Name = "endPin",
                    X = 195,
                    Y = 150
                }
            };

            var wire6 = new DigitalWire()
            {
                Id = Guid.NewGuid(),
                Name = "wire6",
                Signal = output2,
                StartPin = new DigitalPin()
                {
                    Id = Guid.NewGuid(),
                    Name = "startPin",
                    X = 240,
                    Y = 165
                },
                EndPin = new DigitalPin()
                {
                    Id = Guid.NewGuid(),
                    Name = "endPin",
                    X = 195,
                    Y = 120
                }
            };

            diagram.Elements.Add(wire1);
            diagram.Elements.Add(wire2);
            diagram.Elements.Add(wire3);
            diagram.Elements.Add(wire4);
            diagram.Elements.Add(wire5);
            diagram.Elements.Add(wire6);
            diagram.Elements.Add(norGate1);
            diagram.Elements.Add(norGate2);
            diagram.Elements.Add(input1);
            diagram.Elements.Add(input2);
            diagram.Elements.Add(output1);
            diagram.Elements.Add(output2);

            // initialize input/output vector
            output1.State = false; // Q
            output2.State = false; // Q'
            input2.State = false; // S
            input1.State = false; // R

            diagram.ObserveInputs(scheduler, diagram.Disposables);
            diagram.ObserveElements(scheduler, diagram.Disposables);

            return diagram;
        }

        #endregion
    }

    #endregion
}
