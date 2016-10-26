using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GetFileHash
{
    public partial class Histogram : UserControl
    {
        /// <summary>
        /// An array of all values which need to be displayed on the graph.
        /// </summary>
        private long[] _myValues;
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
        private int _myOffset = 20;
        /// <summary>
        /// The default colour to use.
        /// </summary>
        private Color _myColor = Color.Black;
        /// <summary>
        /// The default font to use.
        /// </summary>
        private Font _myFont = new Font("Tahoma", 10);
        private bool _hideZeroBytes = false;
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
                    _myOffset = value;
                }
            }
            get
            {
                return _myOffset;
            }
        }

        [Category("Histogram Options")]
        [Description("The color used within the control")]
        public Color DisplayColor
        {
            set
            {
                _myColor = value;
            }
            get
            {
                return _myColor;
            }
        }

        [Category("Histogram Options")]
        [Description("Ignore bytes with zero value")]
        public bool HideZeroBytes
        {
            set
            {
                _hideZeroBytes = value;
            }
            get
            {
                return _hideZeroBytes;
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
        public void DrawHistogram(byte[] Values, bool hideZeroBytes)
        {
            _hideZeroBytes = hideZeroBytes;

            if (_hideZeroBytes)
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
                if (_hideZeroBytes && (myByte != 0))
                {
                    // Because we are ignoring zero bytes, then we need to shift the index by 1
                    _myValues[myByte - 1] += 1;
                }
                else if (!_hideZeroBytes)
                {
                    _myValues[myByte] += 1;
                }
            }

            // Determine the largest value to display on the X axis.
            _maxXvalue = getMaxX();

            // Determine the largest value to display on the Y axis.
            _maxYvalue = getMaxY();

            ComputeXYUnitValues();
            Refresh();
        }

        /// <summary>
        /// Determines the maximum value that needs to be displayed on the X axis.
        /// </summary>
        /// <param name="Vals">An array of values to parse.</param>
        /// <returns>Returns the largest non-zero X index.</returns>
        private int getMaxX()
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
        private long getMaxY()
        {
            long max = 0;
            int foundAt = 0;

            for (int i = 0; i < _myValues.Length; i++)
            {
                if (_myValues[i] > max)
                {
                    foundAt = i;
                    max = _myValues[i];
                }
            }

            return max;
        }

        private void ComputeXYUnitValues()
        {
            if (_maxYvalue > 0)
            {
                _myXUnit = (Width - (2 * _myOffset)) / _maxXvalue;
                _myYUnit = (Height - (2 * _myOffset)) / _maxYvalue;
            }
        }

        private void Histogram_Paint(object sender, PaintEventArgs e)
        {
            // ToDo : The Y axis doesn't properly scale the maximum value if zeroes are hidden

            if (_myValues != null)
            {
                Histogram _p = (Histogram)sender;
                Bitmap _bitmap = new Bitmap(_p.ClientSize.Width, _p.ClientSize.Height);
                _graphics = Graphics.FromImage(_bitmap);

                // Calculate the position for the labels on the X axis at 25%, 50%, 75%, and 100%
                ArrayList xLabelsList = new ArrayList();
                xLabelsList.Add((float)0);
                xLabelsList.Add(_maxXvalue / 4);
                xLabelsList.Add(_maxXvalue / 2);
                xLabelsList.Add((_maxXvalue / 4) * 3);
                xLabelsList.Add(_maxXvalue);

                // Calculate the position for the horizontal grid
                ArrayList xGridList = new ArrayList();
                xGridList.Add(_maxYvalue / 4 * 3);
                xGridList.Add(_maxYvalue / 2);
                xGridList.Add(_maxYvalue / 4);

                // The start and end points for each line are determined from the centre point of the line
                // So therefore _myBarOffset = _myOffset + half of the bar width
                float _myBarOffset = _myOffset + (_myXUnit / 2);

                // Loop over all of the values...
                int intMaxX = Convert.ToInt32(_maxXvalue);
                for (int i = 0; i <= intMaxX; i++)
                {
                    // Plot the coresponding index for the maximum value.
                    if (_myValues[i] == _maxYvalue)
                    {
                        // The width of the pen is given by the XUnit for the control.
                        Pen myPen = new Pen(new SolidBrush(Color.Red), _myXUnit);

                        // Draw the line.
                        _graphics.DrawLine(myPen,
                            new PointF(_myBarOffset + (i * _myXUnit), Height - _myOffset),
                            new PointF(_myBarOffset + (i * _myXUnit), Height - _myOffset - (_myValues[Convert.ToInt32(i)] * _myYUnit)));

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
                        Pen myPen = new Pen(new SolidBrush(_myColor), _myXUnit);

                        // Draw each line.
                        _graphics.DrawLine(myPen,
                            new PointF(_myBarOffset + (i * _myXUnit), Height - _myOffset),
                            new PointF(_myBarOffset + (i * _myXUnit), Height - _myOffset - (_myValues[Convert.ToInt32(i)] * _myYUnit)));
                    }
                }

                // Now draw the horizontal grid lines
                foreach (float gridLine in xGridList)
                {
                    Pen penGrid = new Pen(new SolidBrush(Color.Gray), _myYUnit);
                    penGrid.DashStyle = DashStyle.Dash;
                    _graphics.DrawLine(penGrid,
                        new PointF(_myOffset, (gridLine * _myYUnit) + _myOffset),
                        new PointF(_myOffset + (_maxXvalue * _myXUnit) + _myXUnit, (gridLine * _myYUnit) + _myOffset));
                }

                // Now draw the X axis labels
                foreach (float tempVal in xLabelsList)
                {
                    float xVal = _myBarOffset + (tempVal * _myXUnit) - _graphics.MeasureString(tempVal.ToString(), _myFont).Width;
                    float yVal = Height - _myFont.Height;

                    _graphics.DrawString(Convert.ToUInt32(tempVal).ToString(), _myFont, new SolidBrush(_myColor), new PointF(xVal + _myXUnit, yVal), StringFormat.GenericDefault);
                }

                // Draw a border around the graph area
                Pen penLine = new Pen(new SolidBrush(Color.Black), _myYUnit);

                PointF topLeft = new PointF(_myOffset, _myOffset);
                PointF bottomLeft = new PointF(_myOffset, _myOffset + (_maxYvalue * _myYUnit));
                PointF topRight = new PointF(_myOffset + (_maxXvalue * _myXUnit) + _myXUnit, _myOffset + _myYUnit);
                PointF bottomRight = new PointF(_myOffset + (_maxXvalue * _myXUnit) + _myXUnit, _myOffset + (_maxYvalue * _myYUnit));

                // Left hand side
                _graphics.DrawLine(penLine, topLeft, bottomLeft);
                // Bottom edge
                _graphics.DrawLine(penLine, bottomLeft, bottomRight);
                // Right hand side
                _graphics.DrawLine(penLine, topRight, bottomRight);
                // Top edge
                _graphics.DrawLine(penLine, topLeft, topRight);

                e.Graphics.DrawImage(_bitmap, 0, 0);
            }
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
                DialogResult result = saveFileDialog1.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // ToDo : Save the graph as a bitmap image
                }
            }
            else if (me.Button == MouseButtons.Left)
            {
                // ToDo : Figure out which bar was clicked on and display the value
            }
        }
    }
}