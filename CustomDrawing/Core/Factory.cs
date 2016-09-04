#region References

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

#endregion

namespace CustomDrawing.Core
{
    #region Factory

    public class Factory
    {
        #region Fields

        private int nextId = 0;

        #endregion

        #region Id

        private int NewId()
        {
            return nextId++;
        }

        private void SetId(int id)
        {
            nextId = id;
        }

        #endregion

        #region Create

        public Pin CreatePin(Element parent, float x, float y, PinTypes type)
        {
            return new Pin()
            {
                Id = NewId(),
                Parent = parent,
                X = x,
                Y = y,
                TransX = x,
                TransY = y,
                Type = type
            };
        }

        public Line CreateLine(Element parent, float x0, float y0, float x1, float y1)
        {
            return new Line()
            {
                Id = NewId(),
                Parent = parent,
                Start = CreatePin(parent, x0, y0, PinTypes.Snap),
                End = CreatePin(parent, x1, y1, PinTypes.Snap)
            };
        }

        public Line CreateLine(Element parent, Pin p0, Pin p1)
        {
            return new Line()
            {
                Id = NewId(),
                Parent = parent,
                Start = p0,
                End = p1
            };
        }

        public Rectangle CreateRectangle(Element parent, float x0, float y0, float x1, float y1)
        {
            return new Rectangle()
            {
                Id = NewId(),
                Parent = parent,
                TopLeft = CreatePin(parent, x0, y0, PinTypes.Snap),
                BottomRight = CreatePin(parent, x1, y1, PinTypes.Snap)
            };
        }

        public Rectangle CreateRectangle(Element parent, Pin tl, Pin br)
        {
            return new Rectangle()
            {
                Id = NewId(),
                Parent = parent,
                TopLeft = tl,
                BottomRight = br
            };
        }

        public Text CreateText(Element parent, float x, float y, string text, int halign, int valign)
        {
            return new Text()
            {
                Id = NewId(),
                Parent = parent,
                Origin = CreatePin(parent, x, y, PinTypes.Snap),
                HAlign = halign,
                VAlign = valign,
                Value = text
            };
        }

        public Text CreateText(Element parent, Pin origin, string text, int halign, int valign)
        {
            return new Text()
            {
                Id = NewId(),
                Parent = parent,
                Origin = origin,
                HAlign = halign,
                VAlign = valign,
                Value = text
            };
        }

        public Custom CreateCustom(Element parent, float x, float y)
        {
            return new Custom()
            {
                Id = NewId(),
                Parent = parent,
                Origin = CreatePin(parent, x, y, PinTypes.Snap),
                Children = new List<Element>(),
                Variables = new List<Element>()
            };
        }

        public Custom CreateCustom(Element parent, Pin origin)
        {
            return new Custom()
            {
                Id = NewId(),
                Parent = parent,
                Origin = origin,
                Children = new List<Element>(),
                Variables = new List<Element>()
            };
        }

        public Reference CreateReference(Element parent, Custom custom, float x, float y)
        {
            return new Reference()
            {
                Id = NewId(),
                Parent = parent,
                Origin = CreatePin(parent, x, y, PinTypes.Origin),
                Content = custom,
                Connectors = new List<Pin>(),
                Variables = new Dictionary<int, string>()
            };
        }

        public Reference CreateReference(Element parent, Custom custom, Pin origin)
        {
            return new Reference()
            {
                Id = NewId(),
                Parent = parent,
                Origin = origin,
                Content = custom,
                Connectors = new List<Pin>()
            };
        }

        #endregion
    } 

    #endregion
}
