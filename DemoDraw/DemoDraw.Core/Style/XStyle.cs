
namespace DemoDraw.Core.Style
{
    public class XStyle
    {
        public XColor Stroke { get; set; }
        public XColor Fill { get; set; }
        public double StrokeThickness { get; set; }

        public static XStyle Create(XColor stroke, XColor fill, double strokeThickness)
        {
            return new XStyle()
            {
                Stroke = stroke,
                Fill = fill,
                StrokeThickness = strokeThickness
            };
        }
    }
}
