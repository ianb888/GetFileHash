using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;
using Microsoft.Win32;
using VirusTotalNET;
using VirusTotalNET.Objects;

namespace GetFileHash
{
    public partial class Form1 : Form
    {
        private const string ScanUrl = "http://www.google.com/";
        VirusTotal virusTotal;

        public Form1()
        {
            InitializeComponent();

            try
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software", true);
                regKey.CreateSubKey("VirusTotal");
                regKey = regKey.OpenSubKey("VirusTotal", true);
                var apiKey = regKey.GetValue("APIkey");
                if (apiKey == null)
                {
                    MessageBox.Show("You need to obtain your API key from VirusTotal and save it in the registry as HKCU/Software/VirusTotal/APIkey", "Missing API Key", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    VirusTotalButton.Enabled = false;
                }
                else
                {
                    string vtApiKey = apiKey.ToString();
                    virusTotal = new VirusTotal(vtApiKey);
                    virusTotal.UseTLS = true;
                }
            }
            catch (Exception eX)
            {
                MessageBox.Show(eX.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void VirusTotalButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(filePathBox.Text))
            {
                FileInfo fileInfo = new FileInfo(filePathBox.Text);

                // Check if the file has been scanned previously
                FileReport fileReport = virusTotal.GetFileReport(fileInfo);
                bool hasBeenScannedBefore = fileReport.ResponseCode == ReportResponseCode.Present;

                // If the file has already been scanned, then the results are embedded inside of the report
                if (hasBeenScannedBefore)
                {
                    vtMessageTextBox.Text = string.Format("{0}, Detection Score = {1}/{2}", fileReport.VerboseMsg, fileReport.Positives, fileReport.Total);
                    if (fileReport.Positives > fileReport.Total / 2)
                    {
                        trafficLight.Image = Properties.Resources.traffic_red;
                    }
                    else if (fileReport.Positives > 0)
                    {
                        trafficLight.Image = Properties.Resources.traffic_yellow;
                    }
                    else
                    {
                        trafficLight.Image = Properties.Resources.traffic_green;
                    }
                }
                else
                {
                    ScanResult scanResult = virusTotal.ScanFile(fileInfo);
                    vtMessageTextBox.Text = scanResult.VerboseMsg;
                }
            }
        }
    }
}