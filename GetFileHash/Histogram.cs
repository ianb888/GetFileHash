using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GetFileHash
{
    public partial class Histogram : UserControl
    {
        private long myMaxValue;
        private long[] myValues;
        private int maxXvalue;
        private float myYUnit; // This gives the vertical unit used to scale our values.
        private float myXUnit; // This gives the horizontal unit used to scale our values.
        private int myOffset = 20; // The offset, in pixels, from the control margins.

        private Color myColor = Color.Black;
        private Font myFont = new Font("Tahoma", 10);

        [Category("Histogram Options")]
        [Description("The distance from the margins for the histogram")]
        public int Offset
        {
            set
            {
                if (value > 0)
                    myOffset = value;
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

            // Determine the highest value to display.
            myMaxValue = getMaxY(myValues);

            // Determine the largest value we need to display on the X axis.
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
            if (myMaxValue > 0)
            {
                myYUnit = (float)(Height - (2 * myOffset)) / myMaxValue;
                myXUnit = (float)(Width - (2 * myOffset)) / (maxXvalue);
            }
        }

        private void Histogram_Paint(object sender, PaintEventArgs e)
        {
            if (myValues != null)
            {
                Graphics g = e.Graphics;

                // Calculate the position for the labels on the X axis at 25%, 50%, 75%, and 100%
                ArrayList xLabelsList = new ArrayList();
                xLabelsList.Add(0);
                xLabelsList.Add(maxXvalue / 4);
                xLabelsList.Add(maxXvalue / 2);
                xLabelsList.Add((maxXvalue / 4) * 3);
                xLabelsList.Add(maxXvalue);

                // Loop over all of the values...
                for (int i = 0; i < maxXvalue; i++)
                {
                    // Plot the coresponding index for the maximum value.
                    if (myValues[i] == myMaxValue)
                    {
                        // The width of the pen is given by the XUnit for the control.
                        Pen myPen = new Pen(new SolidBrush(Color.Red), myXUnit);

                        // Draw the line.
                        g.DrawLine(myPen,
                            new PointF(myOffset + (i * myXUnit), Height - myOffset),
                            new PointF(myOffset + (i * myXUnit), Height - myOffset - myValues[i] * myYUnit));

                        SizeF mySize = g.MeasureString(i.ToString(), myFont);

                        if (WillItOverlap(xLabelsList, i, mySize))
                        {
                            // Label the position of the maximum value on the X axis.
                            float xVal = myOffset + (i * myXUnit) - (mySize.Width / 2);
                            float yVal = Height - myFont.Height;

                            g.DrawString(i.ToString(), myFont, new SolidBrush(Color.Red), new PointF(xVal, yVal), StringFormat.GenericDefault);
                        }
                    }
                    else // For all the other values
                    {
                        Pen myPen = new Pen(new SolidBrush(myColor), myXUnit);

                        // Draw each line.
                        g.DrawLine(myPen,
                            new PointF(myOffset + (i * myXUnit), Height - myOffset),
                            new PointF(myOffset + (i * myXUnit), Height - myOffset - myValues[i] * myYUnit));
                    }
                }

                // Draw the labels for 0 and for the length of the array beeing plotted.
                //g.DrawString("0", myFont, new SolidBrush(myColor), new PointF(myOffset, Height - myFont.Height), StringFormat.GenericDefault);

                foreach (int tempVal in xLabelsList)
                {
                    g.DrawString(tempVal.ToString(), myFont,
                        new SolidBrush(myColor),
                        new PointF(myOffset + (tempVal * myXUnit) - g.MeasureString(tempVal.ToString(), myFont).Width,
                        Height - myFont.Height),
                        StringFormat.GenericDefault);
                }

                //g.DrawString((myValues.Length - 1).ToString(), myFont,
                //    new SolidBrush(myColor),
                //    new PointF(myOffset + (myValues.Length * myXUnit) - g.MeasureString((myValues.Length - 1).ToString(), myFont).Width,
                //    Height - myFont.Height),
                //    StringFormat.GenericDefault);
            }
        }

        private bool WillItOverlap(ArrayList xLabelsList, int labelPosition, SizeF labelSize)
        {
            bool isOK = true;

            int width = Convert.ToInt32(labelSize.Width);
            int leftEdge = labelPosition - (width / 2);
            int rightEdge = labelPosition + (width / 2);

            foreach (int tempVal in xLabelsList)
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
    }
}