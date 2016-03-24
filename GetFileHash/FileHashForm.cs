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
    public partial class FileHashForm : Form
    {
        enum AlertStatus
        {
            None,
            Green,
            Yellow,
            Red
        };

        private const string ScanUrl = "http://www.google.com/";
        VirusTotal virusTotal;
        AlertStatus alertStatus = AlertStatus.None;
        MRUManager mruManager;

        public FileHashForm()
        {
            InitializeComponent();

            try
            {
                var apiKey = checkRegistry();

                // First attempt
                if (apiKey == null)
                {
                    VtApiForm vtApiForm = new VtApiForm();
                    DialogResult result = vtApiForm.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        apiKey = checkRegistry();
                    }
                }
                // Second attempt
                if (apiKey != null)
                {
                    string vtApiKey = apiKey.ToString();
                    virusTotal = new VirusTotal(vtApiKey);
                    virusTotal.UseTLS = true;
                    VirusTotalButton.Enabled = true;
                }
                else
                {
                    VirusTotalButton.Enabled = false;
                }
            }
            catch (Exception eX)
            {
                MessageBox.Show(eX.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FileHashForm_Load(object sender, EventArgs e)
        {
            mruManager = new MRUManager(recentFilesToolStripMenuItem, "GetFileHash", recentFileGotClicked_handler, recentFilesGotCleared_handler);
        }

        private void recentFileGotClicked_handler(object obj, EventArgs evt)
        {
            string fileNamePath = (obj as ToolStripItem).Text;

            if (!File.Exists(fileNamePath))
            {
                if (MessageBox.Show(string.Format("{0} doesn't exist. Remove from recent files?", fileNamePath), "File not found", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    mruManager.RemoveRecentFile(fileNamePath);
                }
                return;
            }

            filePathBox.Text = fileNamePath;
            trafficLight.Image = Properties.Resources.traffic_off;
            vtMessageTextBox.Text = string.Empty;
            trafficLightTimer.Enabled = false;
            calculateChecksums(fileNamePath);
        }

        private void recentFilesGotCleared_handler(object obj, EventArgs evt)
        {
            // Prior to this function getting called, all recent files in the registry and 
            // in the program's 'Recent Files' menu are cleared.
        }

        private object checkRegistry()
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software", true);
            regKey.CreateSubKey("VirusTotal");
            regKey = regKey.OpenSubKey("VirusTotal", true);
            var apiKey = regKey.GetValue("APIkey");

            return apiKey;
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            string fileNamePath = openFileDialog.FileName;
            trafficLight.Image = Properties.Resources.traffic_off;
            vtMessageTextBox.Text = string.Empty;
            trafficLightTimer.Enabled = false;
            calculateChecksums(fileNamePath);

            //Now give it to the MRUManager
            mruManager.AddRecentFile(fileNamePath);
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
                trafficLight.Image = Properties.Resources.traffic_off;
                vtMessageTextBox.Text = string.Empty;
                trafficLightTimer.Enabled = false;
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
                        alertStatus = AlertStatus.Red;
                    }
                    else if (fileReport.Positives > 0)
                    {
                        alertStatus = AlertStatus.Yellow;
                    }
                    else
                    {
                        alertStatus = AlertStatus.Green;
                    }
                    trafficLightTimer.Enabled = true;
                }
                else
                {
                    ScanResult scanResult = virusTotal.ScanFile(fileInfo);
                    vtMessageTextBox.Text = scanResult.VerboseMsg;
                }
            }
        }

        bool altTick = true;

        private void trafficLightTimer_Tick(object sender, EventArgs e)
        {
            if (altTick)
            {
                switch (alertStatus)
                {
                    case AlertStatus.None:
                        trafficLight.Image = Properties.Resources.traffic_off;
                        break;
                    case AlertStatus.Green:
                        trafficLight.Image = Properties.Resources.traffic_green;
                        break;
                    case AlertStatus.Yellow:
                        trafficLight.Image = Properties.Resources.traffic_yellow;
                        break;
                    case AlertStatus.Red:
                        trafficLight.Image = Properties.Resources.traffic_red;
                        break;
                }
                altTick = false;
            }
            else
            {
                trafficLight.Image = Properties.Resources.traffic_off;
                altTick = true;
            }
        }
    }
}