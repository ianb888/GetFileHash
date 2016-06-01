using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GetFileHash
{
    public partial class Histogram : UserControl
    {
        long myMaxValue;
        private long[] myValues;
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

            foreach (byte myByte in Values)
            {
                myValues[myByte] += 1;
            }

            myMaxValue = getMaxY(myValues);
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
                    max = Vals[i];
            }
            return max;
        }

        //private long getMaxX(long[] Vals)
        //{

        //}

        private void ComputeXYUnitValues()
        {
            if (myMaxValue > 0)
            {
                myYUnit = (float)(Height - (2 * myOffset)) / myMaxValue;
                myXUnit = (float)(Width - (2 * myOffset)) / (myValues.Length - 1);
            }
        }

        private void Histogram_Paint(object sender, PaintEventArgs e)
        {
            if (myValues != null)
            {
                Graphics g = e.Graphics;
                Pen myPen = new Pen(new SolidBrush(myColor), myXUnit);
                // The width of the pen is given by the XUnit for the control.
                for (int i = 0; i < myValues.Length; i++)
                {
                    // Draw each line.
                    g.DrawLine(myPen,
                        new PointF(myOffset + (i * myXUnit), Height - myOffset),
                        new PointF(myOffset + (i * myXUnit), Height - myOffset - myValues[i] * myYUnit));

                    // Plot the coresponding index for the maximum value.
                    if (myValues[i] == myMaxValue)
                    {
                        SizeF mySize = g.MeasureString(i.ToString(), myFont);

                        g.DrawString(i.ToString(), myFont, new SolidBrush(myColor),
                            new PointF(myOffset + (i * myXUnit) - (mySize.Width / 2), Height - myFont.Height),
                            StringFormat.GenericDefault);
                    }
                }

                // Draw the indexes for 0 and for the length of the array beeing plotted.
                g.DrawString("0", myFont, new SolidBrush(myColor), new PointF(myOffset, Height - myFont.Height), StringFormat.GenericDefault);

                ArrayList list = new ArrayList();
                list.Add(myValues.Length / 4);
                list.Add(myValues.Length / 2);
                list.Add((myValues.Length / 4) * 3);

                foreach (int tempVal in list)
                {
                    g.DrawString(tempVal.ToString(), myFont,
                        new SolidBrush(myColor),
                        new PointF(myOffset + (tempVal * myXUnit) - g.MeasureString(tempVal.ToString(), myFont).Width,
                        Height - myFont.Height),
                        StringFormat.GenericDefault);
                }

                g.DrawString((myValues.Length - 1).ToString(), myFont,
                    new SolidBrush(myColor),
                    new PointF(myOffset + (myValues.Length * myXUnit) - g.MeasureString((myValues.Length - 1).ToString(), myFont).Width,
                    Height - myFont.Height),
                    StringFormat.GenericDefault);
            }
        }

        private void Histogram_Resize(object sender, EventArgs e)
        {
            ComputeXYUnitValues();
            Refresh();
        }
    }
}