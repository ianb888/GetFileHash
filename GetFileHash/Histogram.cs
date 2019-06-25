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
        private long[] _myValues;
        private long _myAverage = 0;
        /// <summary>
        /// The maximum value that needs to be displayed on the X axis.
        /// </summary>
        private float _maxXvalue;
        /// <summary>
        /// The maximum value that needs to be displayed on the Y axis.
        /// </summary>
        private float _maxYvalue;
        /// <summary>
        /// This gives the vertical unit used to scale our values.
        /// </summary>
        private float _myYUnit;
        /// <summary>
        /// This gives the horizontal unit used to scale our values.
        /// </summary>
        private float _myXUnit;
        /// <summary>
        /// The offset, in pixels, from the control margins.
        /// </summary>
        private int _marginWidth = 20;
        private int _leftMargin = 60;

        /// <summary>
        /// The default font to use.
        /// </summary>
        private readonly Font _myFont = new Font("Verdana", 10);

        //private Bitmap bitmap;
        private Graphics _graphics;

        [Category("Histogram Options")]
        [Description("The distance from the margins for the histogram")]
        public int Offset
        {
            set
            {
                if (value > 0)
                {
                    _marginWidth = value;
                }
            }
            get
            {
                return _marginWidth;
            }
        }

        [Category("Histogram Options")]
        [Description("The width of the left margin")]
        public int LeftMargin
        {
            set
            {
                if (value > 0)
                {
                    _leftMargin = value;
                }
            }
            get
            {
                return _leftMargin;
            }
        }

        [Category("Histogram Options")]
        [Description("The color used within the control")]
        public Color DisplayColor { set; get; } = Color.Black;

        [Category("Histogram Options")]
        [Description("Ignore bytes with zero value")]
        public bool HideZeroBytes { set; get; } = false;

        public Histogram()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Draw the histogram on the control.
        /// </summary>
        /// <param name="myValues">The values being drawn.</param>
        public void DrawHistogram(byte[] Values, bool hideZeroBytes)
        {
            HideZeroBytes = hideZeroBytes;

            if (HideZeroBytes)
            {
                _myValues = new long[255];
            }
            else
            {
                _myValues = new long[256];
            }

            // Tabulate the results
            foreach (byte myByte in Values)
            {
                // If _hideZeroBytes is true, then ignore bytes with a 0 value
                if (HideZeroBytes && (myByte != 0))
                {
                    // Because we are ignoring zero bytes, then we need to shift the index by 1
                    _myValues[myByte - 1] += 1;
                }
                else if (!HideZeroBytes)
                {
                    _myValues[myByte] += 1;
                }
            }

            // Calculate the average value
            foreach (long total in _myValues)
            {
                _myAverage += total;
            }
            _myAverage /= _myValues.LongLength;

            // Determine the largest value to display on the X axis.
            _maxXvalue = GetMaxX();

            // Determine the largest value to display on the Y axis.
            _maxYvalue = GetMaxY();

            ComputeXYUnitValues();
            Refresh();
        }

        /// <summary>
        /// Determines the maximum value that needs to be displayed on the X axis.
        /// </summary>
        /// <param name="Vals">An array of values to parse.</param>
        /// <returns>Returns the largest non-zero X index.</returns>
        private int GetMaxX()
        {
            int retVal = _myValues.Length - 1;

            // We loop backwards from the end of the array
            for (int i = retVal; i >= 0; i--)
            {
                // Is the values of this array item > 0 ?
                if (_myValues[i] > 0)
                {
                    //if (i != (_myValues.Length - 1))
                    if (i != retVal)
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

        /// <summary>
        /// Get the highest value from the array.
        /// </summary>
        /// <param name="Vals">The array of values in which we look.</param>
        /// <returns>Returns the maximum value.</returns>
        private long GetMaxY()
        {
            long max = 0;

            for (int i = 0; i < _myValues.Length; i++)
            {
                if (_myValues[i] > max)
                {
                    max = _myValues[i];
                }
            }

            return max;
        }

        private void ComputeXYUnitValues()
        {
            if (_maxYvalue > 0)
            {
                _myXUnit = (Width - (2 * _marginWidth) - _leftMargin) / _maxXvalue;
                _myYUnit = (Height - (2 * _marginWidth)) / _maxYvalue;
            }
        }

        private void Histogram_Paint(object sender, PaintEventArgs e)
        {
            if (_myValues != null)
            {
                Histogram _p = (Histogram)sender;
                Bitmap _bitmap = new Bitmap(_p.ClientSize.Width, _p.ClientSize.Height);
                _graphics = Graphics.FromImage(_bitmap);

                // The start and end points for each line are determined from the centre point of the line
                // So therefore _myBarOffset = _myOffset + half of the bar width
                float _myBarOffset = _leftMargin + _marginWidth + (_myXUnit / 2);

                // Calculate the position for the labels on the X axis at 25%, 50%, 75%, and 100%
                ArrayList xLabelsList = new ArrayList();
                // If we are hiding zero value bytes, then we need to remember that the array indexes are shifted by 1
                // In other words, the value of array index 63 will be the number of bytes in the file that had a value of 64
                if (HideZeroBytes)
                {
                    xLabelsList.Add((float)1);
                    xLabelsList.Add((float)Math.Round((_maxXvalue + 1) / 8, 0));
                    xLabelsList.Add((float)Math.Round((_maxXvalue + 1) / 4, 0));
                    xLabelsList.Add((float)Math.Round(((_maxXvalue + 1) / 8) * 3, 0));
                    xLabelsList.Add((float)Math.Round((_maxXvalue + 1) / 2, 0));
                    xLabelsList.Add((float)Math.Round(((_maxXvalue + 1) / 8) * 5, 0));
                    xLabelsList.Add((float)Math.Round(((_maxXvalue + 1) / 4) * 3, 0));
                    xLabelsList.Add((float)Math.Round(((_maxXvalue + 1) / 8) * 7, 0));
                    xLabelsList.Add((float)Math.Round(_maxXvalue + 1, 0));
                }
                else
                {
                    xLabelsList.Add((float)0);
                    xLabelsList.Add((float)Math.Round(_maxXvalue / 8, 0));
                    xLabelsList.Add((float)Math.Round(_maxXvalue / 4, 0));
                    xLabelsList.Add((float)Math.Round((_maxXvalue / 8) * 3, 0));
                    xLabelsList.Add((float)Math.Round(_maxXvalue / 2, 0));
                    xLabelsList.Add((float)Math.Round((_maxXvalue / 8) * 5, 0));
                    xLabelsList.Add((float)Math.Round((_maxXvalue / 4) * 3, 0));
                    xLabelsList.Add((float)Math.Round((_maxXvalue / 8) * 7, 0));
                    xLabelsList.Add((float)Math.Round(_maxXvalue, 0));
                }

                // Loop over all of the values...
                int intMaxX = Convert.ToInt32(_maxXvalue);
                for (int i = 0; i <= intMaxX; i++)
                {
                    // Draw the red bar for the maximum value.
                    if (_myValues[i] == _maxYvalue)
                    {
                        // The width of the pen is given by the XUnit for the control.
                        Pen myPen = new Pen(new SolidBrush(Color.Red), _myXUnit);

                        // Draw the line.
                        _graphics.DrawLine(myPen,
                            new PointF(_myBarOffset + (i * _myXUnit), Height - _marginWidth),
                            new PointF(_myBarOffset + (i * _myXUnit), Height - _marginWidth - (_myValues[Convert.ToInt32(i)] * _myYUnit)));

                        SizeF mySize = _graphics.MeasureString(i.ToString(), _myFont);

                        if (WillItOverlap(xLabelsList, i, mySize))
                        {
                            // Label the position of the maximum value on the X axis.
                            float xVal = _myBarOffset + (i * _myXUnit) - (mySize.Width / 2);
                            float yVal = Height - _myFont.Height;

                            _graphics.DrawString(Convert.ToInt32(i).ToString(), _myFont, new SolidBrush(Color.Red), new PointF(xVal, yVal), StringFormat.GenericDefault);
                        }
                    }
                    else // For all the other values
                    {
                        Pen myPen = new Pen(new SolidBrush(DisplayColor), _myXUnit);

                        // Draw each line.
                        _graphics.DrawLine(myPen,
                            new PointF(_myBarOffset + (i * _myXUnit), Height - _marginWidth),
                            new PointF(_myBarOffset + (i * _myXUnit), Height - _marginWidth - (_myValues[Convert.ToInt32(i)] * _myYUnit)));
                    }
                }

                StringFormat xAxisFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Near
                };

                // Now draw the X axis labels
                foreach (float tempVal in xLabelsList)
                {
                    float correctedVal = tempVal;

                    if (HideZeroBytes)
                    {
                        correctedVal = tempVal - 1;
                    }

                    string textToDisplay = tempVal.ToString();
                    float xVal = _myBarOffset + (correctedVal * _myXUnit) - (_graphics.MeasureString(textToDisplay, _myFont).Width / 2);
                    float yVal = Height - _myFont.Height;

                    // Write the text on the X axis
                    _graphics.DrawString(textToDisplay, _myFont, new SolidBrush(DisplayColor), new PointF(xVal + _myXUnit, yVal), xAxisFormat);

                    // Now, draw a short tick on the X axis to show which bar the label is indicating.
                    Pen penTick = new Pen(new SolidBrush(Color.Black), (_myXUnit / 4));
                    PointF tickTop = new PointF(_myBarOffset + (correctedVal * _myXUnit), Height - _marginWidth);
                    float tickLength = Height / (float)100 * (float)0.5;
                    PointF tickBottom = new PointF(_myBarOffset + (correctedVal * _myXUnit), (Height - _marginWidth) + tickLength);

                    _graphics.DrawLine(penTick, tickTop, tickBottom);
                }

                // Calculate the position for the horizontal grid
                ArrayList horizGridList = new ArrayList();
                horizGridList.Add(_maxYvalue);
                horizGridList.Add(_maxYvalue / 8 * 7);
                horizGridList.Add(_maxYvalue / 4 * 3);
                horizGridList.Add(_maxYvalue / 8 * 5);
                horizGridList.Add(_maxYvalue / 2);
                horizGridList.Add(_maxYvalue / 8 * 3);
                horizGridList.Add(_maxYvalue / 4);
                horizGridList.Add(_maxYvalue / 8 * 1);
                horizGridList.Add((float)0);

                StringFormat yAxisFormat = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center
                };

                // Now draw the horizontal grid lines
                for (int iteration = 0; iteration < horizGridList.Count; iteration++)
                {
                    float gridLine = (float)horizGridList[iteration];
                    var valueToDisplay = (Math.Round((float)horizGridList[iteration], 0) - _maxYvalue) * -1;
                    string labelText =  valueToDisplay.ToString("N0");

                    // Only draw the intermediate lines, not the minumum and maximum lines
                    if ((gridLine != 0) && (gridLine != _maxYvalue))
                    {
                        Pen penGrid = new Pen(new SolidBrush(Color.Gray), (_myXUnit / 4));
                        penGrid.DashStyle = DashStyle.Dash;
                        _graphics.DrawLine(penGrid,
                            new PointF(_marginWidth + _leftMargin, (gridLine * _myYUnit) + _marginWidth),
                            new PointF(_marginWidth + (_maxXvalue * _myXUnit) + _myXUnit + _leftMargin, (gridLine * _myYUnit) + _marginWidth));
                    }

                    // Now add the text label
                    float xVal = (_leftMargin - _graphics.MeasureString(labelText, _myFont).Width) + (_marginWidth / 2);
                    float yVal = (gridLine * _myYUnit) + _marginWidth;

                    // Write the text on the X axis
                    _graphics.DrawString(labelText, _myFont, new SolidBrush(DisplayColor), new PointF(xVal, yVal), yAxisFormat);

                    // Now, draw a short tick on the Y axis to show which bar the label is indicating.
                    Pen penTick = new Pen(new SolidBrush(Color.Black), (_myXUnit / 4));
                    float tickLength = _myXUnit;
                    PointF tickLeft = new PointF(_marginWidth + _leftMargin - tickLength, (gridLine * _myYUnit) + _marginWidth);
                    PointF tickRight = new PointF(_marginWidth + _leftMargin, (gridLine * _myYUnit) + _marginWidth);

                    _graphics.DrawLine(penTick, tickLeft, tickRight);
                }

                // Now draw a line to show the average value
                Pen averagePen = new Pen(new SolidBrush(Color.Green), (_myXUnit / 4))
                {
                    DashStyle = DashStyle.DashDot
                };
                _graphics.DrawLine(averagePen,
                    new PointF(_marginWidth + _leftMargin, (_myAverage * _myYUnit) + _marginWidth),
                    new PointF(_marginWidth + (_maxXvalue * _myXUnit) + _myXUnit + _leftMargin, (_myAverage * _myYUnit) + _marginWidth));

                // Draw a border around the graph area
                Pen penLine = new Pen(new SolidBrush(Color.Black), (_myXUnit / 4));

                PointF topLeft = new PointF(_marginWidth + _leftMargin, _marginWidth);
                PointF botLeft = new PointF(_marginWidth + _leftMargin, _marginWidth + (_maxYvalue * _myYUnit));
                PointF topRight = new PointF(_marginWidth + (_maxXvalue * _myXUnit) + _myXUnit + _leftMargin, _marginWidth);
                PointF botRight = new PointF(_marginWidth + (_maxXvalue * _myXUnit) + _myXUnit + _leftMargin, _marginWidth + (_maxYvalue * _myYUnit));

                // Left hand side
                _graphics.DrawLine(penLine, topLeft, botLeft);
                // Bottom edge
                _graphics.DrawLine(penLine, botLeft, botRight);
                // Right hand side
                _graphics.DrawLine(penLine, topRight, botRight);
                // Top edge
                _graphics.DrawLine(penLine, topLeft, topRight);

                e.Graphics.DrawImage(_bitmap, 0, 0);
            }
        }

        /// <summary>
        /// Test whether the text will overlap with the default X axis labels.
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
                // Check if tempVal falls between the limits of the existing label
                if ((tempVal < rightEdge) && (tempVal > leftEdge))
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
                saveFileDialog1.FileName = "Histogram.bmp";
                saveFileDialog1.Filter = "Bitmap Image|*.bmp";
                saveFileDialog1.ValidateNames = true;

                DialogResult result = saveFileDialog1.ShowDialog();

                if (result == DialogResult.OK)
                {
                    using (Bitmap bitmap = new Bitmap(Width, Height))
                    {
                        DrawToBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
                        bitmap.Save(saveFileDialog1.FileName, ImageFormat.Bmp);
                    }
                }
            }
        }
    }
}