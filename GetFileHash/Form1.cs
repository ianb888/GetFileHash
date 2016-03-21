using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace GetFileHash
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            string fileNamePath = openFileDialog.FileName;
            calculateChecksums(fileNamePath);
        }

        private void openFileDialog_HelpRequest(object sender, EventArgs e)
        {

        }

        private void chooseFileButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileNamePath = openFileDialog.FileName;
                filePathBox.Text = fileNamePath;
            }
        }

        public static string GetHashFromFile(string fileName, HashAlgorithm algorithm)
        {
            using (var stream = new BufferedStream(File.OpenRead(fileName), 100000))
            {
                return BitConverter.ToString(algorithm.ComputeHash(stream)).Replace("-", string.Empty);
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem menuItem = sender as ToolStripItem;
            if (menuItem != null)
            {
                ContextMenuStrip owner = menuItem.Owner as ContextMenuStrip;
                if (owner != null)
                {
                    TextBox sourceControl = owner.SourceControl as TextBox;
                    Clipboard.SetText(sourceControl.Text);
                }
            }
        }

        private void filePathBox_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(filePathBox.Text))
            {
                calculateChecksums(filePathBox.Text);
            }
        }

        private void calculateChecksums(string fileNamePath)
        {
            string checksumMd5 = GetHashFromFile(fileNamePath, Algorithms.MD5);
            md5TextBox.Text = checksumMd5;
            string checksumSha1 = GetHashFromFile(fileNamePath, Algorithms.SHA1);
            sha1TextBox.Text = checksumSha1;
            string checksumSha256 = GetHashFromFile(fileNamePath, Algorithms.SHA256);
            sha256TextBox.Text = checksumSha256;
        }
    }
}