using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using LogicCanvas.Model.Core;
using LogicCanvas.Model.Logic;

namespace LogicCanvas.Extensions
{
    public static class LineLogicItemExtensions
    {
        public static bool Compare(this LineLogicItem oldLine, LineLogicItem newLine)
        {
            if (
                oldLine.IsNew == newLine.IsNew &&
                oldLine.IsModified == newLine.IsModified &&
                oldLine.IsDeleted == newLine.IsDeleted &&
                oldLine.X1 == newLine.X1 &&
                oldLine.Y1 == newLine.Y1 &&
                oldLine.X2 == newLine.X2 &&
                oldLine.Y2 == newLine.Y2 &&
                oldLine.X == newLine.X &&
                oldLine.Y == newLine.Y &&
                oldLine.Z == newLine.Z)
            {
                return true;
            }
            return false;
        }

        public static bool CheckForModification(this LineLogicItem oldLine, LineLogicItem newLine)
        {
            if (oldLine.X1 == newLine.X1 &&
                oldLine.Y1 == newLine.Y1 &&
                oldLine.X2 == newLine.X2 &&
                oldLine.Y2 == newLine.Y2)
            {
                return false;
            }
            return true;
        }

        public static LineLogicItem Copy(LineLogicItem source)
        {
            LineLogicItem destination = new LineLogicItem()
            {
                IsNew = source.IsNew,
                IsModified = source.IsModified,
                IsDeleted = source.IsDeleted,
                X1 = source.X1,
                Y1 = source.Y1,
                X2 = source.X2,
                Y2 = source.Y2,
                X = source.X,
                Y = source.Y,
                Z = source.Z
            };
            return destination;
        }

        public static void FromPoints(this LineLogicItem item, Point startPoint, Point endPoint)
        {
            item.X1 = startPoint.X;
            item.Y1 = startPoint.Y;
            item.X2 = endPoint.X;
            item.Y2 = endPoint.Y;
        }
    }
}
