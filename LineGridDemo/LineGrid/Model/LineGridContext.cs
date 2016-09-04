
namespace LineGrid.Model
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion

    #region

    public class LineGridContext : NotifyObject
    {
        public LineGridContext()
            : base()
        {
            Initialize();

            Refresh();
        }

        #region Properties

        private LineItemList lines;
        private double width;
        private double height;
        private double rows;
        private double columns;
        private double rowHeight;
        private double columnWidth;

        public LineItemList Lines
        {
            get { return lines; }
            set
            {
                if (value != lines)
                {
                    lines = value;
                    Notify("Lines");
                }
            }
        }

        public double Width
        {
            get { return width; }
            set
            {
                if (value != width)
                {
                    width = value;
                    Notify("Width");
                }
            }
        }

        public double Height
        {
            get { return height; }
            set
            {
                if (value != height)
                {
                    height = value;
                    Notify("Height");
                }
            }
        }

        public double Rows
        {
            get { return rows; }
            set
            {
                if (value != rows)
                {
                    rows = value;
                    Notify("Rows");

                    Refresh();
                }
            }
        }

        public double Columns
        {
            get { return columns; }
            set
            {
                if (value != columns)
                {
                    columns = value;
                    Notify("Columns");

                    Refresh();
                }
            }
        }

        public double RowHeight
        {
            get { return rowHeight; }
            set
            {
                if (value != rowHeight)
                {
                    rowHeight = value;
                    Notify("RowHeight");

                    Refresh();
                }
            }
        }

        public double ColumnWidth
        {
            get { return columnWidth; }
            set
            {
                if (value != columnWidth)
                {
                    columnWidth = value;
                    Notify("ColumnWidth");

                    Refresh();
                }
            }
        }

        #endregion

        #region Methods

        public void Refresh()
        {
            if (lines == null)
            {
                throw new NullReferenceException();
            }

            double thicknessHalf = 1.0 / 2.0;

            double width = ColumnWidth * Columns;
            double height = RowHeight * Rows;
            double rowHeight = RowHeight;
            double columnWidth = ColumnWidth;

            if (lines != null)
            {
                lines.Clear();
            }

            // horizontal grid lines
            for (double y = 0.0; y <= Rows; y += 1.0)
            {
                var line = new LineItem()
                {
                    X1 = thicknessHalf,
                    X2 = width - thicknessHalf,
                    Y1 = y * rowHeight + thicknessHalf,
                    Y2 = y * rowHeight + thicknessHalf
                };

                lines.Add(line);
            }

            // vertical grid lines
            for (double x = 0.0; x <= Columns; x += 1.0)
            {
                var line = new LineItem()
                {
                    X1 = x * columnWidth + thicknessHalf,
                    X2 = x * columnWidth + thicknessHalf,
                    Y1 = height - thicknessHalf,
                    Y2 = thicknessHalf
                };

                lines.Add(line);
            }

            Width = width + 2 * thicknessHalf;
            Height = height + 2 * thicknessHalf;
        }

        private void Initialize()
        {
            lines = new LineItemList();

            rows = 10;
            columns = 10;

            rowHeight = 20;
            columnWidth = 20;
        }

        #endregion
    }

    #endregion
}
