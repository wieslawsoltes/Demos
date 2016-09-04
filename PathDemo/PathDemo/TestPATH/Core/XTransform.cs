// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace TestPATH
{
    public class XTransform
    {
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double ScaleX { get; set; }
        public double ScaleY { get; set; }
        public double SkewAngleX { get; set; }
        public double SkewAngleY { get; set; }
        public double RotateAngle { get; set; }
        public double OffsetX { get; set; }
        public double OffsetY { get; set; }

        public static XTransform Create(
            double centerX = 0.0,
            double centerY = 0.0,
            double scaleX = 1.0,
            double scaleY = 1.0,
            double skewAngleX = 0.0,
            double skewAngleY = 0.0,
            double rotateAngle = 0.0,
            double offsetX = 0.0,
            double offsetY = 0.0)
        {
            return new XTransform()
            {
                CenterX = centerX,
                CenterY = centerY,
                ScaleX = scaleX,
                ScaleY = scaleY,
                SkewAngleX = skewAngleX,
                SkewAngleY = skewAngleY,
                RotateAngle = rotateAngle,
                OffsetX = offsetX,
                OffsetY = offsetY
            };
        }
    }
}
