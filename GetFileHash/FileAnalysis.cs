using System;
using System.IO;
using System.Windows.Forms;

namespace GetFileHash
{
    public partial class FileAnalysis : Form
    {
        private byte[] fileBytes = null;
        public FileAnalysis(string fileName)
        {
            InitializeComponent();

            if (Properties.Settings.Default.HideZero)
            {
                ignoreZeroCheckbox.Checked = true;
            }

            if ((!string.IsNullOrWhiteSpace(fileName)) && File.Exists(fileName))
            {
                fileBytes = File.ReadAllBytes(fileName);
                histogram1.DrawHistogram(fileBytes, Properties.Settings.Default.HideZero);
                DataEntropyUTF8 entropy = new DataEntropyUTF8(fileName);
                textBox1.Text = entropy.Entropy.ToString();
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ignoreZero_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreZeroCheckbox.Checked)
            {
                Properties.Settings.Default.HideZero = true;
                histogram1.HideZeroBytes = true;
            }
            else
            {
                Properties.Settings.Default.HideZero = false;
                histogram1.HideZeroBytes = false;
            }
            Properties.Settings.Default.Save();

            if (fileBytes != null)
            {
                histogram1.DrawHistogram(fileBytes, Properties.Settings.Default.HideZero);
            }
        }
    }
}