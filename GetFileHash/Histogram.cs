using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace GetFileHash
{
    public partial class Histogram : UserControl
    {
        /// <summary>
        /// An array of all values which need to be displayed on the graph.
        /// </summary>
        private long[] myValues;
        /// <summary>
        /// The maximum value that needs to be displayed on the X axis.
        /// </summary>
        private float maxXvalue;
        /// <summary>
        /// The maximum value that needs to be displayed on the Y axis.
        /// </summary>
        private float maxYvalue;
        /// <summary>
        /// This gives the vertical unit used to scale our values.
        /// </summary>
        private float myYUnit;
        /// <summary>
        /// This gives the horizontal unit used to scale our values.
        /// </summary>
        private float myXUnit;
        /// <summary>
        /// The offset, in pixels, from the control margins.
        /// </summary>
        private int myOffset = 20;
        /// <summary>
        /// The default colour to use.
        /// </summary>
        private Color myColor = Color.Black;
        /// <summary>
        /// The default font to use.
        /// </summary>
        private Font myFont = new Font("Tahoma", 10);
        private Bitmap bitmap;
        private Graphics g;

        [Category("Histogram Options")]
        [Description("The distance from the margins for the histogram")]
        public int Offset
        {
            set
            {
                if (value > 0)
                {
                    myOffset = value;
                }
            }
            get
            {
                return myOffset;
            }
        }

        [Category("Histogram Options")]
        [Description("The color used within the control")]
        public Color DisplayColor
        {
            set
            {
                myColor = value;
            }
            get
            {
                return myColor;
            }
        }

        public Histogram()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Draw the histogram on the control.
        /// </summary>
        /// <param name="myValues">The values being drawn.</param>
        public void DrawHistogram(byte[] Values)
        {
            myValues = new long[256];

            // Tabulate the results
            foreach (byte myByte in Values)
            {
                myValues[myByte] += 1;
            }

            // Determine the largest value to display on the Y axis.
            maxYvalue = getMaxY(myValues);

            // Determine the largest value to display on the X axis.
            maxXvalue = getMaxX(myValues);

            ComputeXYUnitValues();
            Refresh();
        }

        /// <summary>
        /// Get the highest value from the array.
        /// </summary>
        /// <param name="Vals">The array of values in which we look.</param>
        /// <returns>Returns the maximum value.</returns>
        private long getMaxY(long[] Vals)
        {
            long max = 0;

            for (int i = 0; i < Vals.Length; i++)
            {
                if (Vals[i] > max)
                {
                    max = Vals[i];
                }
            }

            return max;
        }

        /// <summary>
        /// Determines the maximum value that needs to be displayed on the X axis.
        /// </summary>
        /// <param name="Vals">An array of values to parse.</param>
        /// <returns>Returns the largest non-zero X index.</returns>
        private int getMaxX(long[] Vals)
        {
            int retVal = myValues.Length - 1;

            for (int i = retVal; i >= 0; i--)
            {
                if (myValues[i] > 0)
                {
                    if (i != 255)
                    {
                        retVal = i + 1;
                    }
                    else
                    {
                        retVal = i;
                    }
                    break;
                }
            }

            return retVal;
        }

        private void ComputeXYUnitValues()
        {
            if (maxYvalue > 0)
            {
                myXUnit = (float)(Width - (2 * myOffset)) / maxXvalue;
                myYUnit = (float)(Height - (2 * myOffset)) / maxYvalue;
            }
        }

        private void Histogram_Paint(object sender, PaintEventArgs e)
        {
            if (myValues != null)
            {
                //Panel p = (Panel)sender;
                Histogram p = (Histogram)sender;
                Bitmap bitmap = new Bitmap(p.ClientSize.Width, p.ClientSize.Height);
                //g = e.Graphics;
                g = Graphics.FromImage(bitmap);

                // Calculate the position for the labels on the X axis at 25%, 50%, 75%, and 100%
                ArrayList xLabelsList = new ArrayList();
                xLabelsList.Add((float)0);
                xLabelsList.Add(maxXvalue / 4);
                xLabelsList.Add(maxXvalue / 2);
                xLabelsList.Add((maxXvalue / 4) * 3);
                xLabelsList.Add(maxXvalue);

                // Calculate the position for the horizontal grid
                ArrayList xGridList = new ArrayList();
                xGridList.Add(maxYvalue / 4 * 3);
                xGridList.Add(maxYvalue / 2);
                xGridList.Add(maxYvalue / 4);

                float halfBarWidth = myXUnit / 2;
                float myBarOffset = myOffset + halfBarWidth;

                // Loop over all of the values...
                for (float i = 0; i < maxXvalue; i++)
                {
                    // Plot the coresponding index for the maximum value.
                    if (myValues[Convert.ToInt32(i)] == maxYvalue)
                    {
                        // The width of the pen is given by the XUnit for the control.
                        Pen myPen = new Pen(new SolidBrush(Color.Red), myXUnit);

                        // Draw the line.
                        g.DrawLine(myPen,
                            new PointF(myBarOffset + (i * myXUnit), Height - myOffset),
                            new PointF(myBarOffset + (i * myXUnit), Height - myOffset - (myValues[Convert.ToInt32(i)] * myYUnit)));

                        SizeF mySize = g.MeasureString(i.ToString(), myFont);

                        if (WillItOverlap(xLabelsList, i, mySize))
                        {
                            // Label the position of the maximum value on the X axis.
                            float xVal = myBarOffset + (i * myXUnit) - (mySize.Width / 2);
                            float yVal = Height - myFont.Height;

                            g.DrawString(Convert.ToInt32(i).ToString(),
                                myFont, new SolidBrush(Color.Red), new PointF(xVal, yVal), StringFormat.GenericDefault);
                        }
                    }
                    else // For all the other values
                    {
                        Pen myPen = new Pen(new SolidBrush(myColor), myXUnit);

                        // Draw each line.
                        g.DrawLine(myPen,
                            new PointF(myBarOffset + (i * myXUnit), Height - myOffset),
                            new PointF(myBarOffset + (i * myXUnit), Height - myOffset - (myValues[Convert.ToInt32(i)] * myYUnit)));
                    }
                }

                // Now draw the horizontal grid lines
                foreach (float gridLine in xGridList)
                {
                    Pen penGrid = new Pen(new SolidBrush(Color.Gray), myYUnit);
                    penGrid.DashStyle = DashStyle.Dash;
                    g.DrawLine(penGrid,
                        new PointF(myOffset, (gridLine * myYUnit) + myOffset),
                        new PointF(myOffset + (maxXvalue * myXUnit), (gridLine * myYUnit) + myOffset));
                }

                // Now draw the X axis labels
                foreach (float tempVal in xLabelsList)
                {
                    float xVal = myBarOffset + (tempVal * myXUnit) - g.MeasureString(tempVal.ToString(), myFont).Width;
                    float yVal = Height - myFont.Height;

                    g.DrawString(Convert.ToUInt32(tempVal).ToString(),
                        myFont, new SolidBrush(myColor), new PointF(xVal, yVal), StringFormat.GenericDefault);
                }

                // Draw a border around the graph area
                Pen penLine = new Pen(new SolidBrush(Color.Black), myYUnit);
                // Left hand side
                g.DrawLine(penLine, new PointF(myOffset, myOffset), new PointF(myOffset, myOffset + (maxYvalue * myYUnit)));
                // Bottom edge
                g.DrawLine(penLine, new PointF(myOffset, myOffset + (maxYvalue * myYUnit)), new PointF(myOffset + (maxXvalue * myXUnit), myOffset + (maxYvalue * myYUnit)));
                // Right hand side
                g.DrawLine(penLine, new PointF(myOffset + (maxXvalue * myXUnit), myOffset + (maxYvalue * myYUnit)), new PointF(myOffset + (maxXvalue * myXUnit), myOffset));
                // Top edge
                g.DrawLine(penLine, new PointF(myOffset + (maxXvalue * myXUnit), myOffset), new PointF(myOffset, myOffset));
            }

            e.Graphics.DrawImage(bitmap, 0, 0);
        }

        /// <summary>
        /// Test wether the text will overlap with the default X axis labels.
        /// </summary>
        /// <param name="xLabelsList">The list of X axis label positions</param>
        /// <param name="labelPosition">The proposed label position for this label.</param>
        /// <param name="labelSize">The size of the label to write.</param>
        /// <returns>Returns TRUE if the text will not be obscured.</returns>
        private bool WillItOverlap(ArrayList xLabelsList, float labelPosition, SizeF labelSize)
        {
            bool isOK = true;

            float width = labelSize.Width;
            float leftEdge = labelPosition - (width / 2);
            float rightEdge = labelPosition + (width / 2);

            foreach (float tempVal in xLabelsList)
            {
                if ((tempVal > leftEdge) && (tempVal < rightEdge))
                {
                    isOK = false;
                }
            }

            return isOK;
        }

        private void Histogram_Resize(object sender, EventArgs e)
        {
            ComputeXYUnitValues();
            Refresh();
        }

        private void Histogram_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;

            if (me.Button == MouseButtons.Right)
            {
                DialogResult result = saveFileDialog1.ShowDialog();

                if (result == DialogResult.OK)
                {
                }
            }
        }
    }
}