using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VirusTotalNET;
using VirusTotalNET.Objects;

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

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
