/// <summary>
/// Draw Clouds
/// (C) 2010 Wiesław Šoltés. All Rights Reserved.
///
/// Draw simple clouds by using two points set by user using left mouse clicks.
/// </summary>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace DrawClouds
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Initialize main form.
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            this.Text = "Draw Clouds";
        }

        /// <summary>
        /// Define cloud objects.
        /// </summary>
        public class Arc
        {
            public PointF pStart = new PointF();
            public PointF pEnd = new PointF();
        }

        public class Cloud
        {
            public List<Arc> Arcs = new List<Arc>();
        }

        /// <summary>
        /// Clouds data.
        /// </summary>
        private List<Cloud> clouds = new List<Cloud>();
        private Cloud tmpCloud = new Cloud();
        private PointF pFirst = new PointF();
        private PointF pTmpStart = new PointF();
        private PointF pTmpEnd = new PointF();
        private PointF pMouseOver = new PointF();
        private PointF pMouseOverPrev = new PointF();

        /// <summary>
        /// Start and end flags used for drawing.
        /// </summary>
        private bool haveStart = false;

        /// <summary>
        /// Size of cloud grip rectangle.
        /// </summary>
        private float gripSize = 8f;

        /// <summary>
        /// Cloud pen size;
        /// </summary>
        private float penSize = 3f;

        /// <summary>
        /// Always close cloud.
        /// </summary>
        private bool alwaysClose = true;

        /// <summary>
        /// Snap to grid variables
        /// </summary>
        private int gridSpacing = 5;
        private int m_X1;
        private int m_Y1;
        private int m_X2;
        private int m_Y2;
        private bool snapToGrid = true;

        /// <summary>
        /// Calculate snap to grid point coordinates.
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public void SnapToGrid(ref int X, ref int Y)
        {
            X = gridSpacing * (int)(X / gridSpacing);
            Y = gridSpacing * (int)(Y / gridSpacing);
        }

        /// <summary>
        /// Calculate angle between the line defined by two points and the horizontal axis.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public float PointAngle(PointF p1, PointF p2)
        {
            double angle = Math.Atan2(p2.Y - p1.Y, p2.X - p1.X) * 180 / Math.PI;
            return (float) angle;
        }

        /// <summary>
        /// Calculate cloud path.
        /// </summary>
        /// <param name="pStart"></param>
        /// <param name="pEnd"></param>
        /// <returns></returns>
        private GraphicsPath CalculateCloudPath(PointF pStart, PointF pEnd)
        {
            GraphicsPath path = new GraphicsPath();

            // Calculate angle between start and move points.
            float startAngle = PointAngle(pStart, pEnd);

            // Calculate arc dimensions.
            float sweepAngle = 180.0f;
            float a = Math.Abs(pStart.X - pEnd.X);
            float b = Math.Abs(pStart.Y - pEnd.Y);
            float c = (float)Math.Sqrt(Math.Pow((double)a, 2d) + Math.Pow((double)b, 2d));

            // If arc > 0 then draw to screen.
            if (c > 0f)
            {
                // Add arc to path and get start and end points.
                RectangleF rcArc = new RectangleF(pStart.X, pStart.Y, c, c);
                path.AddArc(rcArc, startAngle, sweepAngle);

                // Translate arc position to start and end points set by user.
                PointF pArcStart = new PointF(path.PathPoints.First().X, path.PathPoints.First().Y);
                PointF pArcEnd = new PointF(path.PathPoints.Last().X, path.PathPoints.Last().Y);
                float offsetX = pStart.X - pArcEnd.X;
                float offsetY = pEnd.Y - pArcStart.Y;
                Matrix matrix = new Matrix();
                matrix.Translate(offsetX, offsetY);
                path.Transform(matrix);

                return path;
            }

            // Return null if its not possible to calculate path points.
            return null;
        }

        /// <summary>
        /// Check if grip rectangle is visble.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool IsGripVisible(PointF p)
        {
            bool visible = false;

            // Calculate grip rectangles.
            RectangleF rcGrip = new RectangleF(p.X - this.gripSize / 2f, p.Y - this.gripSize / 2f, gripSize, this.gripSize);
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(rcGrip);

            // Check if grip rectangle is visble.
            if (path.IsVisible(this.pMouseOver))
                visible = true;

            path.Dispose();
            return visible;
        }

        /// <summary>
        /// Draw grip rectangle.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="brush"></param>
        /// <param name="pen"></param>
        /// <param name="p"></param>
        private void DrawGrip(ref Graphics g, ref SolidBrush brush, ref Pen pen, PointF p)
        {
            // Calculate grip rectangle.
            RectangleF rcGrip = new RectangleF(p.X - this.gripSize / 2f, p.Y - this.gripSize / 2f, gripSize, this.gripSize);

            // Draw grip rectangle.
            g.FillRectangle(brush, rcGrip);
            g.DrawRectangle(pen, rcGrip.X, rcGrip.Y, rcGrip.Width, rcGrip.Height);
        }

        /// <summary>
        /// Draw clouds.
        /// </summary>
        /// <param name="e"></param>
        private void DrawClouds(Graphics g)
        {
            // Create grip brush and pen.
            SolidBrush gripBrush = new SolidBrush(Color.FromArgb(128, Color.Purple));
            Pen gripPen = new Pen(Color.FromArgb(192, Color.Purple), 1);

            SolidBrush gripBrushSelected = new SolidBrush(Color.FromArgb(128, Color.SteelBlue));
            Pen gripPenSelected = new Pen(Color.FromArgb(192, Color.SteelBlue), 1);

            // Create cloud pen.
            Pen penClouds = new Pen(Color.FromArgb(192, Color.Blue), penSize);

            Pen penTmpCloud = new Pen(Color.FromArgb(192, Color.Lime), penSize);
            Pen penTmpArc = new Pen(Color.FromArgb(192, Color.Red), penSize);

            // Set graphics quality to very high.
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PageUnit = GraphicsUnit.Pixel;

            // Draw all existing clouds.
            if (this.clouds.Count > 0)
            {
                //Debug.WriteLine("Drawing clouds: " + this.clouds.Count.ToString());

                foreach (Cloud cloud in this.clouds)
                {
                    foreach (Arc arc in cloud.Arcs)
                    {
                        //Debug.WriteLine("Drawing cloud arc: " + arc.pStart.ToString() + " | " + arc.pEnd.ToString());

                        // Calculate loud arc points.
                        GraphicsPath path = this.CalculateCloudPath(arc.pStart, arc.pEnd);

                        // Draw arc path.
                        if (path != null)
                        {
                            if (path.IsVisible(this.pMouseOver) || this.IsGripVisible(path.PathPoints.First()) || this.IsGripVisible(path.PathPoints.Last()))
                            {
                                g.DrawPath(penTmpCloud, path);

                                // Draw arc grip rectangles.
                                if (this.IsGripVisible(path.PathPoints.First()))
                                    this.DrawGrip(ref g, ref gripBrushSelected, ref gripPenSelected, path.PathPoints.First());
                                else
                                    this.DrawGrip(ref g, ref gripBrush, ref gripPen, path.PathPoints.First());

                                if (this.IsGripVisible(path.PathPoints.Last()))
                                    this.DrawGrip(ref g, ref gripBrushSelected, ref gripPenSelected, path.PathPoints.Last());
                                else
                                    this.DrawGrip(ref g, ref gripBrush, ref gripPen, path.PathPoints.Last());
                            }
                            //else if (this.selectionTool)
                            //{
                            //    g.DrawPath(penTmpArc, path);
                            //
                            //    // Draw arc grip rectangles.
                            //
                            //}
                            else
                            {
                                g.DrawPath(penClouds, path);
                            }

                            path.Dispose();
                        }
                    }
                }
            }

            // Draw temp cloud.
            if (this.tmpCloud.Arcs.Count > 0)
            {
                //Debug.WriteLine("Drawing arcs in tmp cloud: " + this.tmpCloud.Arcs.Count);

                foreach (Arc arc in this.tmpCloud.Arcs)
                {
                    //Debug.WriteLine("Drawing tmp cloud arc: " + arc.pStart.ToString() + " | " + arc.pEnd.ToString());

                    // Calculate loud arc points.
                    GraphicsPath path = this.CalculateCloudPath(arc.pStart, arc.pEnd);

                    // Draw arc path.
                    if (path != null)
                    {
                        g.DrawPath(penTmpCloud, path);

                        // Draw arc grip rectangles.
                        if (this.IsGripVisible(path.PathPoints.First()))
                            this.DrawGrip(ref g, ref gripBrushSelected, ref gripPenSelected, path.PathPoints.Last());
                        else
                            this.DrawGrip(ref g, ref gripBrush, ref gripPen, path.PathPoints.Last());

                        path.Dispose();
                    }
                }
            }

            // Draw new arc.
            if (this.haveStart)
            {
                //Debug.WriteLine("Drawing new arc: " + this.pTmpStart.ToString() + " | " + this.pTmpEnd.ToString());

                // Get start point and end point of the Arc.
                PointF pStart = new PointF(this.pTmpStart.X, this.pTmpStart.Y);
                PointF pEnd = new PointF(this.pTmpEnd.X, this.pTmpEnd.Y);

                // Calculate cloud arc points.
                GraphicsPath path = this.CalculateCloudPath(pStart, pEnd);
                if (path != null)
                {
                    // Draw arc path.
                    g.DrawPath(penTmpArc, path);

                    // Draw arc grip rectangles.
                    this.DrawGrip(ref g, ref gripBrush, ref gripPen, pStart);
                    this.DrawGrip(ref g, ref gripBrush, ref gripPen, pEnd);

                    path.Dispose();
                }      
            }

            // Clean-up memory after drawing everything to screen.
            gripBrush.Dispose();
            gripPen.Dispose();

            gripBrushSelected .Dispose();
            gripPenSelected.Dispose();

            penClouds.Dispose();
            penTmpCloud.Dispose();
            penTmpArc.Dispose();
        }

        /// <summary>
        /// Paint to screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            this.DrawClouds(e.Graphics);
        }

        /// <summary>
        /// Handle mouse movement.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            //if(this.selectionTool)
            //{
                m_X2 = e.X;
                m_Y2 = e.Y;

                if (this.snapToGrid)
                    this.SnapToGrid(ref m_X2, ref m_Y2);

      
                // Save prevoius point.
                this.pMouseOverPrev = new PointF(this.pMouseOver.X, this.pMouseOver.Y);

                // Set new point.
                this.pMouseOver = new PointF(m_X2, m_Y2);

                // Redraw clouds if point has changed.
                if (pMouseOver != this.pMouseOverPrev)
                    this.Invalidate();
            //}


            if (this.haveStart)
            {
                //m_X2 = e.X;
                //m_Y2 = e.Y;

                //if (this.snapToGrid)
                //    this.SnapToGrid(ref m_X2, ref m_Y2);

                this.pTmpEnd = new PointF(m_X2, m_Y2);
                this.Invalidate();
            }

        }

        /// <summary>
        /// Handle user clicks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            // Use left mouse button to create new cloud points.
            if (e.Button == MouseButtons.Left)
            {
                if(this.haveStart)
                {
                    m_X1 = e.X;
                    m_Y1 = e.Y;

                    if (this.snapToGrid)
                        SnapToGrid(ref m_X1, ref m_Y1);

                    // Add next arc to arcs list.
                    Arc newArc = new Arc();
                    newArc.pStart = new PointF(pTmpStart.X, pTmpStart.Y);
                    newArc.pEnd = new PointF(m_X1, m_Y1);
                    this.tmpCloud.Arcs.Add(newArc);

                    //Debug.WriteLine("Left click added new arc to tmp cloud: " + this.tmpCloud.Arcs.Count.ToString());

                    m_X2 = m_X1;
                    m_Y2 = m_Y1;

                    // Begin new Arc.
                    this.pTmpStart = new PointF(this.pTmpEnd.X, this.pTmpEnd.Y);
                    this.pTmpEnd = new PointF(m_X2, m_Y2);

                    //Debug.WriteLine("Left click (already have start): " + this.pTmpStart.ToString() + " | " + this.pTmpEnd.ToString());
                }
                else
                {
                    m_X1 = e.X;
                    m_Y1 = e.Y;

                    if (this.snapToGrid)
                        SnapToGrid(ref m_X1, ref m_Y1);

                    m_X2 = m_X1;
                    m_Y2 = m_Y1;

                    // Initialize new Arc.
                    this.pTmpStart = new PointF(m_X1, m_Y1);
                    this.pTmpEnd = new PointF(m_X2, m_Y2);
                    this.pFirst = new PointF(m_X1, m_Y1);
                    this.haveStart = true;

                    //Debug.WriteLine("Left click new start point: " + this.pTmpStart.ToString() + " | " + this.pTmpEnd.ToString());
                }

                // Redraw clouds.
                this.Invalidate();
            }

            // Use right mouse button to finish the cloud.
            if (e.Button == MouseButtons.Right)
            {
                this.haveStart = false;

                if (this.tmpCloud.Arcs.Count > 0)
                {
                    if (this.alwaysClose)
                    {
                        // Add next arc to arcs list.
                        Arc newArc = new Arc();
                        newArc.pStart = new PointF(pTmpStart.X, pTmpStart.Y);
                        newArc.pEnd = new PointF(pFirst.X, pFirst.Y);
                        this.tmpCloud.Arcs.Add(newArc);
                    }

                    // Add new cloud to the clouds list.
                    Cloud newCloud = new Cloud();
                    foreach (Arc arc in this.tmpCloud.Arcs)
                    {
                        Arc newArc = new Arc();
                        newArc.pStart = new PointF(arc.pStart.X, arc.pStart.Y);
                        newArc.pEnd = new PointF(arc.pEnd.X, arc.pEnd.Y);
                        newCloud.Arcs.Add(newArc);
                    }

                    this.clouds.Add(newCloud);

                    // Clear temp cloud.
                    this.tmpCloud.Arcs.Clear();

                    //Debug.WriteLine("Right click added new cloud: " + this.clouds.Count.ToString());
                }

                // Redraw clouds.
                this.Invalidate();
            }
        }

        /// <summary>
        /// Enable or disable snap to grid option.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void snapToGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.snapToGrid = this.snapToGridToolStripMenuItem.Checked;
        }

        /// <summary>
        /// Awlays close the cloud.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void alwaysCloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.alwaysClose = this.alwaysCloseToolStripMenuItem.Checked;
        }

        /// <summary>
        /// Show about dialog box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Draw Clouds\n" +
                "(C) 2010 Wiesław Šoltés. All Rights Reserved.",
                "About...", 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Information);
        }

        /// <summary>
        /// Create new project.
        /// </summary>
        private void Project_New()
        {
            this.haveStart = false;
            this.tmpCloud.Arcs.Clear();
            this.clouds.Clear();

            this.Invalidate();
        }

        /// <summary>
        /// Open project from a file.
        /// </summary>
        /// <param name="fileName"></param>
        private void Project_Open(string fileName)
        {
            this.Project_New();

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<Cloud>));
            using (System.IO.TextReader outputStream = new StreamReader(fileName))
            {
                this.clouds = (List<Cloud>)serializer.Deserialize(outputStream);
            }
        }

        /// <summary>
        /// Save project to a file.
        /// </summary>
        /// <param name="fileName"></param>
        private void Project_Save(string fileName)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<Cloud>));
            using (System.IO.TextWriter outputStream = new StreamWriter(fileName))
            {
                serializer.Serialize(outputStream, this.clouds);
            }
        }

        /// <summary>
        /// Export project to a EMF file.
        /// </summary>
        /// <param name="fileName"></param>
        private void Project_ExportEMF(string fileName)
        {
            try
            {
                Graphics g = this.CreateGraphics();
                IntPtr ipHdc = g.GetHdc();
                Metafile mf = new Metafile(fileName, ipHdc);

                g.ReleaseHdc(ipHdc);
                g.Dispose();

                Graphics dg = Graphics.FromImage(mf);
                this.DrawClouds(dg);

                dg.Dispose();
                mf.Dispose();
            }
            catch
            {
                MessageBox.Show("Failed to save metafile to disk!", 
                    "Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Export project to a Image file.
        /// </summary>
        /// <param name="fileName"></param>
        private void Project_ExportImage(string fileName, ImageFormat format)
        {
            try
            {
                Bitmap bmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height, PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(bmp);
                this.DrawClouds(g);
                bmp.MakeTransparent(this.BackColor);
                bmp.Save(fileName, format);
                g.Dispose();
            }
            catch
            {
                MessageBox.Show("Failed to save image to a file!",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Create new project.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Project_New();
        }

        /// <summary>
        ///  Load project from a file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Filter = "Cloud Project (*.clouds)|*.clouds||";
            this.openFileDialog1.FilterIndex = 1;
            this.openFileDialog1.Title = "Open cloud project";
            this.openFileDialog1.DefaultExt = "clouds";
            this.openFileDialog1.FileName = "";
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.Project_Open(this.openFileDialog1.FileName);
            }
        }

        /// <summary>
        /// Save project to a file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1.Filter = "Cloud Project (*.clouds)|*.clouds||";
            this.saveFileDialog1.FilterIndex = 1;
            this.saveFileDialog1.Title = "Save cloud project";
            this.saveFileDialog1.DefaultExt = "clouds";
            this.saveFileDialog1.FileName = "project";
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.Project_Save(this.saveFileDialog1.FileName);
            }
        }

        /// <summary>
        /// Export project to a EMF file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportToEMFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1.Filter = "EMF Files (*.emf)|*.emf||";
            this.saveFileDialog1.FilterIndex = 1;
            this.saveFileDialog1.Title = "Export cloud project to emf file";
            this.saveFileDialog1.DefaultExt = "emf";
            this.saveFileDialog1.FileName = "project";
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.Project_ExportEMF(this.saveFileDialog1.FileName);
            }
        }

        /// <summary>
        /// Export project to a Image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportToImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1.Filter =
                "PNG Files (*.png)|*.png|" +
                "JPG Files (*.jpg)|*.jpg|" +
                "BMP Files (*.bmp)|*.bmp|" +
                "GIF Files (*.gif)|*.gif|" +
                "TIFF Files (*.tiff)|*.tiff||";
            this.saveFileDialog1.FilterIndex = 1;
            this.saveFileDialog1.Title = "Export cloud project to image file";
            this.saveFileDialog1.DefaultExt = "jpg";
            this.saveFileDialog1.FileName = "project";
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                switch (this.saveFileDialog1.FilterIndex)
                {
                    case 1: this.Project_ExportImage(this.saveFileDialog1.FileName, ImageFormat.Png);
                        break;
                    case 2: this.Project_ExportImage(this.saveFileDialog1.FileName, ImageFormat.Jpeg);
                        break;
                    case 3: this.Project_ExportImage(this.saveFileDialog1.FileName, ImageFormat.Bmp);
                        break;
                    case 4: this.Project_ExportImage(this.saveFileDialog1.FileName, ImageFormat.Gif);
                        break;
                    case 5: this.Project_ExportImage(this.saveFileDialog1.FileName, ImageFormat.Tiff);
                        break;
                };
            }
        }

        /// <summary>
        /// Close main form and exit application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
