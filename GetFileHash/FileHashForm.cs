using System;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;
using GetFileHash.Properties;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using VirusTotalNET;
using VirusTotalNET.Results;

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

            // This code fixes the bug whereby the Form Designer gets corrupted after adding a custom settings provider.
            PortableSettingsProvider portableSettingsProvider = new PortableSettingsProvider();
            Settings.Default.Providers.Add(portableSettingsProvider);
            foreach (SettingsProperty property in Settings.Default.Properties)
            {
                property.Provider = portableSettingsProvider;
            }

#if DEBUG
            CreateShortcut("Encrypt File (Debug)", Environment.GetFolderPath(Environment.SpecialFolder.SendTo), Assembly.GetExecutingAssembly().Location, "Create an Encrypted File");
#else
            CreateShortcut("Encrypt File", Environment.GetFolderPath(Environment.SpecialFolder.SendTo), Assembly.GetExecutingAssembly().Location, "Create an Encrypted File");
#endif

            try
            {
                // First, try to get the API key from the registry...
                var apiKey = CheckRegistry();

                // Nothing found? Then ask for it to be entered...
                if (apiKey == null)
                {
                    VtApiForm vtApiForm = new VtApiForm();
                    DialogResult result = vtApiForm.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        // A key was entered, so read it back from the registry to make sure it was saved correctly.
                        apiKey = CheckRegistry();
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
            trafficLight.Image = Resources.traffic_off;
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
            mruManager = new MRUManager(recentFilesToolStripMenuItem, "GetFileHash", RecentFileGotClicked_handler, RecentFilesGotCleared_handler);

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
                        if (CalculateChecksums(fileNamePath))
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

        private void RecentFileGotClicked_handler(object obj, EventArgs evt)
        {
            fileNamePath = (obj as ToolStripItem).Text;

            if (!System.IO.File.Exists(fileNamePath))
            {
                if (MessageBox.Show(string.Format("{0} doesn't exist. Remove from recent files?", fileNamePath), "File not found", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    mruManager.RemoveRecentFile(fileNamePath);
                }
                return;
            }

            filePathBox.Text = fileNamePath;
            DisableButtons();
            CalculateChecksums(fileNamePath);
        }

        private void RecentFilesGotCleared_handler(object obj, EventArgs evt)
        {
            // Prior to this function getting called, all recent files in the registry and 
            // in the program's 'Recent Files' menu are cleared.
        }

        /// <summary>
        /// Check if the API key was saved in the registry.
        /// </summary>
        /// <returns>Returns the API key.</returns>
        private object CheckRegistry()
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software", true);
            regKey.CreateSubKey("VirusTotal");
            regKey = regKey.OpenSubKey("VirusTotal", true);
            var apiKey = regKey.GetValue("APIkey");

            return apiKey;
        }

        private void OpenFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            fileNamePath = openFileDialog.FileName;
            DisableButtons();

            if (CalculateChecksums(fileNamePath))
            {
                //Now give it to the MRUManager
                mruManager.AddRecentFile(fileNamePath);
            }
        }

        private void ChooseFileButton_Click(object sender, EventArgs e)
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
                using (var stream = new BufferedStream(System.IO.File.OpenRead(fileName), 100000))
                {
                    retVal = BitConverter.ToString(algorithm.ComputeHash(stream)).Replace("-", string.Empty);
                }
            }
            catch (Exception eX)
            {
                MessageBox.Show(eX.GetType().ToString() + ": " + eX.Message, eX.GetType().ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return retVal;
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripItem menuItem)
            {
                if (menuItem.Owner is ContextMenuStrip owner)
                {
                    TextBox sourceControl = owner.SourceControl as TextBox;
                    Clipboard.SetText(sourceControl.Text);
                }
            }
        }

        private void FilePathBox_TextChanged(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(filePathBox.Text))
            {
                fileNamePath = filePathBox.Text;
                DisableButtons();
                CalculateChecksums(fileNamePath);
            }
        }

        /// <summary>
        /// Calculates the MD5, SHA1, and SHA256 hashes for the specified file.
        /// </summary>
        /// <param name="fileName">The path and file name to use.</param>
        /// <returns>Returns TRUE on success.</returns>
        private bool CalculateChecksums(string fileName)
        {
            bool success = false;

            if (System.IO.File.Exists(fileName))
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

        private async void VirusTotalButton_Click(object sender, EventArgs e)
        {
            fileNamePath = filePathBox.Text;

            if (System.IO.File.Exists(fileNamePath))
            {
                FileInfo fileInfo = new FileInfo(fileNamePath);

                // Check if the file has been scanned previously
                fileReport = await virusTotal.GetFileReportAsync(fileInfo);
                bool hasBeenScannedBefore = fileReport.ResponseCode == VirusTotalNET.ResponseCodes.FileReportResponseCode.Present;

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
                    ScanResult scanResult = await virusTotal.ScanFileAsync(fileInfo);
                    vtMessageTextBox.Text = scanResult.VerboseMsg;
                }
            }
        }

        bool altTick = true;

        private void TrafficLightTimer_Tick(object sender, EventArgs e)
        {
            if (altTick)
            {
                switch (alertStatus)
                {
                    case AlertStatus.None:
                        trafficLight.Image = Resources.traffic_off;
                        break;
                    case AlertStatus.Green:
                        trafficLight.Image = Resources.traffic_green;
                        break;
                    case AlertStatus.Yellow:
                        trafficLight.Image = Resources.traffic_yellow;
                        break;
                    case AlertStatus.Red:
                        trafficLight.Image = Resources.traffic_red;
                        break;
                }
                altTick = false;
            }
            else
            {
                trafficLight.Image = Resources.traffic_off;
                altTick = true;
            }
        }

        private void ResultsButton_Click(object sender, EventArgs e)
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

        private void ShowHistogramButton_Click(object sender, EventArgs e)
        {
            FileAnalysis analysis = new FileAnalysis(fileNamePath);
            analysis.Show();
        }

        private static void CreateShortcut(string shortcutName, string shortcutPath, string targetFileLocation, string shortcutDescription = "")
        {
            if (string.IsNullOrWhiteSpace(shortcutName))
            {
                throw new ArgumentNullException("shortcutName", "The shortcut name must be specified");
            }
            if (string.IsNullOrWhiteSpace(shortcutPath))
            {
                throw new ArgumentNullException("shortcutPath", "The shortcut path must be specified");
            }
            if (string.IsNullOrWhiteSpace(targetFileLocation))
            {
                throw new ArgumentNullException("targetFileLocation", "The target file name must be specified");
            }

            string shortcutLocation = Path.Combine(shortcutPath, shortcutName + ".lnk");

            if (System.IO.File.Exists(shortcutLocation))
            {
                System.IO.File.Delete(shortcutLocation);
            }

            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

            shortcut.Description = shortcutDescription;         // The description of the shortcut
            shortcut.IconLocation = targetFileLocation + ",0";  // The icon of the shortcut
            shortcut.TargetPath = targetFileLocation;           // The path of the file that will launch when the shortcut is run
            shortcut.Save();                                    // Save the shortcut
        }
    }
}