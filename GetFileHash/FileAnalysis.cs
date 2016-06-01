using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GetFileHash
{
    public partial class FileAnalysis : Form
    {
        public FileAnalysis(string fileName)
        {
            InitializeComponent();

            if ((!string.IsNullOrWhiteSpace(fileName)) && File.Exists(fileName))
            {
                byte[] fileBytes = File.ReadAllBytes(fileName);
                histogram1.DrawHistogram(fileBytes);
                DataEntropyUTF8 entropy = new DataEntropyUTF8(fileName);
                textBox1.Text = entropy.Entropy.ToString();
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}