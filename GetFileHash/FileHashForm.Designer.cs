namespace GetFileHash
{
    partial class FileHashForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileHashForm));
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.chooseFileButton = new System.Windows.Forms.Button();
            this.recentFilesMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.recentFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filePathBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.sha256TextBox = new System.Windows.Forms.TextBox();
            this.copyMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label3 = new System.Windows.Forms.Label();
            this.sha1TextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.md5TextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.exitButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.vtMessageTextBox = new System.Windows.Forms.TextBox();
            this.trafficLight = new System.Windows.Forms.PictureBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.trafficLightTimer = new System.Windows.Forms.Timer(this.components);
            this.VirusTotalButton = new System.Windows.Forms.Button();
            this.resultsButton = new System.Windows.Forms.Button();
            this.recentFilesMenuStrip.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.copyMenuStrip.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trafficLight)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "All files|*.*";
            this.openFileDialog.SupportMultiDottedExtensions = true;
            this.openFileDialog.Title = "Choose the file to open";
            this.openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog_FileOk);
            this.openFileDialog.HelpRequest += new System.EventHandler(this.openFileDialog_HelpRequest);
            // 
            // chooseFileButton
            // 
            this.chooseFileButton.AutoSize = true;
            this.chooseFileButton.ContextMenuStrip = this.recentFilesMenuStrip;
            this.chooseFileButton.Location = new System.Drawing.Point(13, 13);
            this.chooseFileButton.Name = "chooseFileButton";
            this.chooseFileButton.Size = new System.Drawing.Size(136, 23);
            this.chooseFileButton.TabIndex = 0;
            this.chooseFileButton.Text = "Choose a file";
            this.chooseFileButton.UseVisualStyleBackColor = true;
            this.chooseFileButton.Click += new System.EventHandler(this.chooseFileButton_Click);
            // 
            // recentFilesMenuStrip
            // 
            this.recentFilesMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.recentFilesToolStripMenuItem});
            this.recentFilesMenuStrip.Name = "copyMenuStrip";
            this.recentFilesMenuStrip.Size = new System.Drawing.Size(137, 26);
            // 
            // recentFilesToolStripMenuItem
            // 
            this.recentFilesToolStripMenuItem.Name = "recentFilesToolStripMenuItem";
            this.recentFilesToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.recentFilesToolStripMenuItem.Text = "Recent Files";
            // 
            // filePathBox
            // 
            this.filePathBox.ContextMenuStrip = this.recentFilesMenuStrip;
            this.filePathBox.Location = new System.Drawing.Point(155, 15);
            this.filePathBox.Name = "filePathBox";
            this.filePathBox.Size = new System.Drawing.Size(741, 20);
            this.filePathBox.TabIndex = 1;
            this.filePathBox.TextChanged += new System.EventHandler(this.filePathBox_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.sha256TextBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.sha1TextBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.md5TextBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(155, 41);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(599, 105);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Checksums";
            // 
            // sha256TextBox
            // 
            this.sha256TextBox.ContextMenuStrip = this.copyMenuStrip;
            this.sha256TextBox.Location = new System.Drawing.Point(84, 71);
            this.sha256TextBox.Name = "sha256TextBox";
            this.sha256TextBox.ReadOnly = true;
            this.sha256TextBox.Size = new System.Drawing.Size(509, 20);
            this.sha256TextBox.TabIndex = 7;
            this.sha256TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // copyMenuStrip
            // 
            this.copyMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem});
            this.copyMenuStrip.Name = "copyMenuStrip";
            this.copyMenuStrip.Size = new System.Drawing.Size(103, 26);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "SHA-256 :";
            // 
            // sha1TextBox
            // 
            this.sha1TextBox.ContextMenuStrip = this.copyMenuStrip;
            this.sha1TextBox.Location = new System.Drawing.Point(84, 45);
            this.sha1TextBox.Name = "sha1TextBox";
            this.sha1TextBox.ReadOnly = true;
            this.sha1TextBox.Size = new System.Drawing.Size(509, 20);
            this.sha1TextBox.TabIndex = 5;
            this.sha1TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "SHA-1 :";
            // 
            // md5TextBox
            // 
            this.md5TextBox.ContextMenuStrip = this.copyMenuStrip;
            this.md5TextBox.Location = new System.Drawing.Point(84, 19);
            this.md5TextBox.Name = "md5TextBox";
            this.md5TextBox.ReadOnly = true;
            this.md5TextBox.Size = new System.Drawing.Size(509, 20);
            this.md5TextBox.TabIndex = 3;
            this.md5TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "MD5 :";
            // 
            // exitButton
            // 
            this.exitButton.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.ControlDark;
            this.exitButton.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.ControlLight;
            this.exitButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exitButton.Location = new System.Drawing.Point(13, 154);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(136, 50);
            this.exitButton.TabIndex = 3;
            this.exitButton.Text = "Exit";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.vtMessageTextBox);
            this.groupBox2.Location = new System.Drawing.Point(155, 153);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(599, 52);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "VirusTotal Results";
            // 
            // vtMessageTextBox
            // 
            this.vtMessageTextBox.Location = new System.Drawing.Point(7, 20);
            this.vtMessageTextBox.Name = "vtMessageTextBox";
            this.vtMessageTextBox.ReadOnly = true;
            this.vtMessageTextBox.Size = new System.Drawing.Size(586, 20);
            this.vtMessageTextBox.TabIndex = 0;
            this.vtMessageTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // trafficLight
            // 
            this.trafficLight.BackColor = System.Drawing.Color.Silver;
            this.trafficLight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.trafficLight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trafficLight.Image = global::GetFileHash.Properties.Resources.traffic_off;
            this.trafficLight.Location = new System.Drawing.Point(3, 16);
            this.trafficLight.Name = "trafficLight";
            this.trafficLight.Size = new System.Drawing.Size(129, 144);
            this.trafficLight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.trafficLight.TabIndex = 6;
            this.trafficLight.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.trafficLight);
            this.groupBox3.Location = new System.Drawing.Point(761, 42);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(135, 163);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "File Safety Rating";
            // 
            // trafficLightTimer
            // 
            this.trafficLightTimer.Interval = 750;
            this.trafficLightTimer.Tick += new System.EventHandler(this.trafficLightTimer_Tick);
            // 
            // VirusTotalButton
            // 
            this.VirusTotalButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VirusTotalButton.Location = new System.Drawing.Point(13, 42);
            this.VirusTotalButton.Name = "VirusTotalButton";
            this.VirusTotalButton.Size = new System.Drawing.Size(136, 50);
            this.VirusTotalButton.TabIndex = 4;
            this.VirusTotalButton.Text = "Search VirusTotal";
            this.VirusTotalButton.UseVisualStyleBackColor = true;
            this.VirusTotalButton.Click += new System.EventHandler(this.VirusTotalButton_Click);
            // 
            // resultsButton
            // 
            this.resultsButton.Enabled = false;
            this.resultsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resultsButton.Location = new System.Drawing.Point(13, 98);
            this.resultsButton.Name = "resultsButton";
            this.resultsButton.Size = new System.Drawing.Size(136, 50);
            this.resultsButton.TabIndex = 8;
            this.resultsButton.Text = "Show Detailed Results";
            this.resultsButton.UseVisualStyleBackColor = true;
            this.resultsButton.Click += new System.EventHandler(this.resultsButton_Click);
            // 
            // FileHashForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(908, 213);
            this.Controls.Add(this.resultsButton);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.VirusTotalButton);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.filePathBox);
            this.Controls.Add(this.chooseFileButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(924, 252);
            this.MinimumSize = new System.Drawing.Size(924, 252);
            this.Name = "FileHashForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Get File Hash";
            this.Load += new System.EventHandler(this.FileHashForm_Load);
            this.recentFilesMenuStrip.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.copyMenuStrip.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trafficLight)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button chooseFileButton;
        private System.Windows.Forms.TextBox filePathBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox sha256TextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox sha1TextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox md5TextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.ContextMenuStrip copyMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox vtMessageTextBox;
        private System.Windows.Forms.PictureBox trafficLight;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Timer trafficLightTimer;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip recentFilesMenuStrip;
        public System.Windows.Forms.ToolStripMenuItem recentFilesToolStripMenuItem;
        private System.Windows.Forms.Button VirusTotalButton;
        private System.Windows.Forms.Button resultsButton;
    }
}

