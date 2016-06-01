using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
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
        MRUManager mruManager;
        FileReport fileReport;
        AlertStatus alertStatus = AlertStatus.None;

        private string fileNamePath = string.Empty;
        private bool apiAvailable = false;

        public FileHashForm()
        {
            InitializeComponent();

            try
            {
                // First, try to get the API key from the registry...
                var apiKey = checkRegistry();

                // Nothing found? Then ask for it to be entered...
                if (apiKey == null)
                {
                    VtApiForm vtApiForm = new VtApiForm();
                    DialogResult result = vtApiForm.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        // A key was entered, so read it back from the registry to make sure it was saved correctly.
                        apiKey = checkRegistry();
                    }
                }
                // After all the preliminary checks, if we have a valid API key, then configure it for use.
                if (apiKey != null)
                {
                    string vtApiKey = apiKey.ToString();
                    virusTotal = new VirusTotal(vtApiKey);
                    virusTotal.UseTLS = true;
                    apiAvailable = true;
                }
                else
                {
                    apiAvailable = false;
                }
            }
            catch (Exception eX)
            {
                MessageBox.Show(eX.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Disable the buttons and timer until it is valid for them to be used.
        /// </summary>
        private void DisableButtons()
        {
            trafficLight.Image = Properties.Resources.traffic_off;
            vtMessageTextBox.Text = string.Empty;
            trafficLightTimer.Enabled = false;
            resultsButton.Enabled = false;
            showHistogramButton.Enabled = false;
        }

        /// <summary>
        /// Enable the buttons for use.
        /// </summary>
        private void EnableButtons()
        {
            if (apiAvailable)
            {
                VirusTotalButton.Enabled = true;
            }
            else
            {
                VirusTotalButton.Enabled = false;
            }
            showHistogramButton.Enabled = true;
        }

        private void FileHashForm_Load(object sender, EventArgs e)
        {
            mruManager = new MRUManager(recentFilesToolStripMenuItem, "GetFileHash", recentFileGotClicked_handler, recentFilesGotCleared_handler);

            try
            {
                // First, check if there was a file name provided on the command line...
                string[] args = Environment.GetCommandLineArgs();

                var x = from s in args select s;
                int argCount = x.Count();

                if (argCount > 1)
                {
                    if (!string.IsNullOrWhiteSpace(args[1]))
                    {
                        string fileNamePath = args[1];

                        filePathBox.Text = fileNamePath;
                        DisableButtons();
                        if (calculateChecksums(fileNamePath))
                        {
                            // Now give it to the MRUManager
                            mruManager.AddRecentFile(fileNamePath);
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void recentFileGotClicked_handler(object obj, EventArgs evt)
        {
            fileNamePath = (obj as ToolStripItem).Text;

            if (!File.Exists(fileNamePath))
            {
                if (MessageBox.Show(string.Format("{0} doesn't exist. Remove from recent files?", fileNamePath), "File not found", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    mruManager.RemoveRecentFile(fileNamePath);
                }
                return;
            }

            filePathBox.Text = fileNamePath;
            DisableButtons();
            calculateChecksums(fileNamePath);
        }

        private void recentFilesGotCleared_handler(object obj, EventArgs evt)
        {
            // Prior to this function getting called, all recent files in the registry and 
            // in the program's 'Recent Files' menu are cleared.
        }

        /// <summary>
        /// Check if the API key was saved in the registry.
        /// </summary>
        /// <returns>Returns the API key.</returns>
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
            fileNamePath = openFileDialog.FileName;
            DisableButtons();

            if (calculateChecksums(fileNamePath))
            {
                //Now give it to the MRUManager
                mruManager.AddRecentFile(fileNamePath);
            }
        }

        private void chooseFileButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePathBox.Text = openFileDialog.FileName;
            }
        }

        /// <summary>
        /// Calculates the binary hash of the provided file, using the specified algorithm.
        /// </summary>
        /// <param name="fileName">The patch and file name to use.</param>
        /// <param name="algorithm">The hashing algorithm to use.</param>
        /// <returns>Returns the calculated hash.</returns>
        public static string GetHashFromFile(string fileName, HashAlgorithm algorithm)
        {
            string retVal = string.Empty;

            try
            {
                using (var stream = new BufferedStream(File.OpenRead(fileName), 100000))
                {
                    retVal = BitConverter.ToString(algorithm.ComputeHash(stream)).Replace("-", string.Empty);
                }
            }
            catch
            {

            }

            return retVal;
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
                fileNamePath = filePathBox.Text;
                DisableButtons();
                calculateChecksums(fileNamePath);
            }
        }

        /// <summary>
        /// Calculates the MD5, SHA1, and SHA256 hashes for the specified file.
        /// </summary>
        /// <param name="fileName">The path and file name to use.</param>
        /// <returns>Returns TRUE on success.</returns>
        private bool calculateChecksums(string fileName)
        {
            bool success = false;

            if (File.Exists(fileName))
            {
                string checksumMd5 = GetHashFromFile(fileName, Algorithms.MD5);
                md5TextBox.Text = checksumMd5;
                string checksumSha1 = GetHashFromFile(fileName, Algorithms.SHA1);
                sha1TextBox.Text = checksumSha1;
                string checksumSha256 = GetHashFromFile(fileName, Algorithms.SHA256);
                sha256TextBox.Text = checksumSha256;
                EnableButtons();
                success = true;
            }
            else
            {
                fileNamePath = string.Empty;
                filePathBox.Text = string.Empty;
                MessageBox.Show("Unable to open the file.", "File Access Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                success = false;
            }

            return success;
        }

        private void VirusTotalButton_Click(object sender, EventArgs e)
        {
            fileNamePath = filePathBox.Text;

            if (File.Exists(fileNamePath))
            {                
                FileInfo fileInfo = new FileInfo(fileNamePath);

                // Check if the file has been scanned previously
                fileReport = virusTotal.GetFileReport(fileInfo);
                bool hasBeenScannedBefore = fileReport.ResponseCode == ReportResponseCode.Present;

                // If the file has already been scanned, then the results are embedded inside of the report
                if (hasBeenScannedBefore)
                {
                    vtMessageTextBox.Text = string.Format("{0}, Detection Score = {1}/{2}", fileReport.VerboseMsg, fileReport.Positives, fileReport.Total);
                    if (fileReport.Positives > fileReport.Total / 2)
                    {
                        resultsButton.Enabled = true;
                        alertStatus = AlertStatus.Red;
                    }
                    else if (fileReport.Positives > 0)
                    {
                        resultsButton.Enabled = true;
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

        private void resultsButton_Click(object sender, EventArgs e)
        {
            if (fileReport != null)
            {
                ResultsForm rForm = new ResultsForm(fileReport);
                Screen[] screens = Screen.AllScreens;
                Rectangle bounds;

                if (screens.Length > 1)
                {
                    bounds = screens[1].WorkingArea;
                }
                else
                {
                    bounds = screens[0].WorkingArea;
                }
                
                rForm.SetBounds(bounds.X + (bounds.Width / 4), bounds.Y, bounds.Width / 2, bounds.Height);
                rForm.StartPosition = FormStartPosition.Manual;
                rForm.Show();
            }
        }

        private void showHistogramButton_Click(object sender, EventArgs e)
        {
            FileAnalysis analysis = new FileAnalysis(fileNamePath);
            analysis.Show();
        }
    }
}