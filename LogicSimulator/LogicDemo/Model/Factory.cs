#region References

using Logic.Model.Core;
using Logic.Model.Gates;
using Logic.Model.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

#endregion

namespace Logic.Model
{
    #region Factory

    public static class Factory
    {
        public static DigitalPin NewDigitalPin(string name, double x, double y, double z)
        {
            return new DigitalPin()
            {
                Id = Guid.NewGuid(),
                Name = name,
                X = x,
                Y = y,
                Z = z
            };
        }

        public static DigitalSignal NewDigitalSignal(string name, double x, double y, double z)
        {
            return new DigitalSignal()
            {
                Id = Guid.NewGuid(),
                Name = name,
                X = x,
                Y = y,
                Z = z,
                InputPin = NewDigitalPin("left", x, y + 15, 0), // left
                OutputPin = NewDigitalPin("right", x + 120, y + 15, 0) // right
            };
        }

        public static DigitalWire NewDigitalWire(string name, DigitalPin start, DigitalPin end, DigitalSignal signal)
        {
            start.Signal = signal;
            end.Signal = signal;

            return new DigitalWire()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Signal = signal,
                StartPin = start,
                EndPin = end
            };
        }

        public static AndGate NewAndGate(string name, double x, double y, double z)
        {
            return new AndGate()
            {
                Id = Guid.NewGuid(),
                Name = name,
                X = x,
                Y = y,
                Z = z,
                Pins =
                {
                    NewDigitalPin("top", x + 15, y, 0), // top
                    NewDigitalPin("right", x + 30, y + 15, 0), // right
                    NewDigitalPin("bottom", x + 15, y + 30, 0), // bottom
                    NewDigitalPin("left", x, y + 15, 0) // left
                }
            };
        }

        public static OrGate NewOrGate(string name, double x, double y, double z)
        {
            return new OrGate()
            {
                Id = Guid.NewGuid(),
                Name = name,
                X = x,
                Y = y,
                Z = z,
                Pins =
                {
                    NewDigitalPin("top", x + 15, y, 0), // top
                    NewDigitalPin("right", x + 30, y + 15, 0), // right
                    NewDigitalPin("bottom", x + 15, y + 30, 0), // bottom
                    NewDigitalPin("left", x, y + 15, 0) // left
                }
            };
        }

        public static NotGate NewNotGate(string name, double x, double y, double z)
        {
            return new NotGate()
            {
                Id = Guid.NewGuid(),
                Name = name,
                X = x,
                Y = y,
                Z = z,
                Pins =
                {
                    NewDigitalPin("top", x + 15, y, 0), // top
                    NewDigitalPin("right", x + 30 + 10, y + 15, 0), // right
                    NewDigitalPin("bottom", x + 15, y + 30, 0), // bottom
                    NewDigitalPin("left", x, y + 15, 0) // left
                }
            };
        }

        public static BufferGate NewBufferGate(string name, double x, double y, double z)
        {
            return new BufferGate()
            {
                Id = Guid.NewGuid(),
                Name = name,
                X = x,
                Y = y,
                Z = z,
                Pins =
                {
                    NewDigitalPin("top", x + 15, y, 0), // top
                    NewDigitalPin("right", x + 30, y + 15, 0), // right
                    NewDigitalPin("bottom", x + 15, y + 30, 0), // bottom
                    NewDigitalPin("left", x, y + 15, 0) // left
                }
            };
        }

        public static NandGate NewNandGate(string name, double x, double y, double z)
        {
            return new NandGate()
            {
                Id = Guid.NewGuid(),
                Name = name,
                X = x,
                Y = y,
                Z = z,
                Pins =
                {
                    NewDigitalPin("top", x + 15, y, 0), // top
                    NewDigitalPin("right", x + 30 + 10, y + 15, 0), // right
                    NewDigitalPin("bottom", x + 15, y + 30, 0), // bottom
                    NewDigitalPin("left", x, y + 15, 0) // left
                }
            };
        }

        public static NorGate NewNorGate(string name, double x, double y, double z)
        {
            return new NorGate()
            {
                Id = Guid.NewGuid(),
                Name = name,
                X = x,
                Y = y,
                Z = z,
                Pins =
                {
                    NewDigitalPin("top", x + 15, y, 0), // top
                    NewDigitalPin("right", x + 30 + 10, y + 15, 0), // right
                    NewDigitalPin("bottom", x + 15, y + 30, 0), // bottom
                    NewDigitalPin("left", x, y + 15, 0) // left
                }
            };
        }

        public static XorGate NewXorGate(string name, double x, double y, double z)
        {
            return new XorGate()
            {
                Id = Guid.NewGuid(),
                Name = name,
                X = x,
                Y = y,
                Z = z,
                Pins =
                {
                    NewDigitalPin("top", x + 15, y, 0), // top
                    NewDigitalPin("right", x + 30, y + 15, 0), // right
                    NewDigitalPin("bottom", x + 15, y + 30, 0), // bottom
                    NewDigitalPin("left", x, y + 15, 0) // left
                }
            };
        }

        public static XnorGate NewXnorGate(string name, double x, double y, double z)
        {
            return new XnorGate()
            {
                Id = Guid.NewGuid(),
                Name = name,
                X = x,
                Y = y,
                Z = z,
                Pins =
                {
                    NewDigitalPin("top", x + 15, y, 0), // top
                    NewDigitalPin("right", x + 30 + 10, y + 15, 0), // right
                    NewDigitalPin("bottom", x + 15, y + 30, 0), // bottom
                    NewDigitalPin("left", x, y + 15, 0) // left
                }
            };
        }

        public static TimerOnDelay NewTimerOnDelay(string name, double x, double y, double z, double delay)
        {
            return new TimerOnDelay(delay)
            {
                Id = Guid.NewGuid(),
                Name = name,
                X = x,
                Y = y,
                Z = z,
                Pins =
                {
                    NewDigitalPin("top", x + 15, y, 0), // top
                    NewDigitalPin("right", x + 30, y + 15, 0), // right
                    NewDigitalPin("bottom", x + 15, y + 30, 0), // bottom
                    NewDigitalPin("left", x, y + 15, 0) // left
                }
            };
        }

        public static TimerPulse NewTimerPulse(string name, double x, double y, double z, double delay)
        {
            return new TimerPulse(delay)
            {
                Id = Guid.NewGuid(),
                Name = name,
                X = x,
                Y = y,
                Z = z,
                Pins =
                {
                    NewDigitalPin("top", x + 15, y, 0), // top
                    NewDigitalPin("right", x + 30, y + 15, 0), // right
                    NewDigitalPin("bottom", x + 15, y + 30, 0), // bottom
                    NewDigitalPin("left", x, y + 15, 0) // left
                }
            };
        }
    }

    #endregion
}
