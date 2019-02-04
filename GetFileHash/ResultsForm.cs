using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using VirusTotalNET.Objects;
using VirusTotalNET.Results;

namespace GetFileHash
{
    public partial class ResultsForm : Form
    {
        public ResultsForm(FileReport fileReport)
        {
            InitializeComponent();
            splitContainer1.FixedPanel = FixedPanel.Panel2;

            foreach (KeyValuePair<string, ScanEngine> result in fileReport.Scans.OrderBy(p => p.Key))
            {
                dataGridView1.Rows.Add(new object[] { result.Key, result.Value.Detected.ToString(), result.Value.Result });
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
