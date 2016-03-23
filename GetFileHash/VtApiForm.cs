using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace GetFileHash
{
    public partial class VtApiForm : Form
    {
        string vtApiKey = string.Empty;

        public VtApiForm()
        {
            InitializeComponent();

            try
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software", true);
                regKey.CreateSubKey("VirusTotal");
                regKey = regKey.OpenSubKey("VirusTotal", true);
                var apiKey = regKey.GetValue("APIkey");
                if (apiKey != null)               
                {
                    vtApiKey = apiKey.ToString();
                    apiBox.Text = vtApiKey;
                }
            }
            catch (Exception eX)
            {
                MessageBox.Show(eX.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if ((vtApiKey != apiBox.Text) && (!string.IsNullOrWhiteSpace(apiBox.Text)))
            {
                storeKey(apiBox.Text);
            }
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void storeKey(string keyVal)
        {
            try
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\VirusTotal", "APIkey", keyVal);
            }
            catch (Exception eX)
            {
                MessageBox.Show(eX.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
