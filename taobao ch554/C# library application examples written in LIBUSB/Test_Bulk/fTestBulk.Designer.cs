namespace Test_Bulk
{
    partial class fTestBulk
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsNumDevices = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsRefresh = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmdOpen = new System.Windows.Forms.Button();
            this.cboDevices = new System.Windows.Forms.ComboBox();
            this.panTransfer = new System.Windows.Forms.Panel();
            this.ckShowAsHex = new System.Windows.Forms.CheckBox();
            this.tWrite = new System.Windows.Forms.TextBox();
            this.chkRead = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tRecv = new System.Windows.Forms.TextBox();
            this.ctxRecvTextBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmdRead = new System.Windows.Forms.Button();
            this.cmdWrite = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.standardRequestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getInterfaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panTransfer.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.ctxRecvTextBox.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.tsNumDevices,
            this.tsRefresh,
            this.tsStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 276);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(496, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(81, 17);
            this.toolStripStatusLabel1.Text = "Devices Found:";
            // 
            // tsNumDevices
            // 
            this.tsNumDevices.Name = "tsNumDevices";
            this.tsNumDevices.Size = new System.Drawing.Size(13, 17);
            this.tsNumDevices.Text = "0";
            // 
            // tsRefresh
            // 
            this.tsRefresh.IsLink = true;
            this.tsRefresh.Name = "tsRefresh";
            this.tsRefresh.Size = new System.Drawing.Size(45, 17);
            this.tsRefresh.Text = "Refresh";
            this.tsRefresh.Click += new System.EventHandler(this.tsRefresh_Click);
            // 
            // tsStatus
            // 
            this.tsStatus.Name = "tsStatus";
            this.tsStatus.Size = new System.Drawing.Size(342, 17);
            this.tsStatus.Spring = true;
            this.tsStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmdOpen);
            this.groupBox1.Controls.Add(this.cboDevices);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 24);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(496, 39);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select Device";
            // 
            // cmdOpen
            // 
            this.cmdOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOpen.Location = new System.Drawing.Point(416, 14);
            this.cmdOpen.Name = "cmdOpen";
            this.cmdOpen.Size = new System.Drawing.Size(75, 23);
            this.cmdOpen.TabIndex = 1;
            this.cmdOpen.Text = "Open";
            this.cmdOpen.UseVisualStyleBackColor = true;
            this.cmdOpen.Click += new System.EventHandler(this.cndOpen_Click);
            // 
            // cboDevices
            // 
            this.cboDevices.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboDevices.FormattingEnabled = true;
            this.cboDevices.Location = new System.Drawing.Point(3, 16);
            this.cboDevices.Name = "cboDevices";
            this.cboDevices.Size = new System.Drawing.Size(384, 21);
            this.cboDevices.TabIndex = 0;
            this.cboDevices.SelectedIndexChanged += new System.EventHandler(this.cboDevices_SelectedIndexChanged);
            // 
            // panTransfer
            // 
            this.panTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panTransfer.Controls.Add(this.ckShowAsHex);
            this.panTransfer.Controls.Add(this.tWrite);
            this.panTransfer.Controls.Add(this.chkRead);
            this.panTransfer.Controls.Add(this.groupBox2);
            this.panTransfer.Controls.Add(this.cmdRead);
            this.panTransfer.Controls.Add(this.cmdWrite);
            this.panTransfer.Enabled = false;
            this.panTransfer.Location = new System.Drawing.Point(0, 69);
            this.panTransfer.Name = "panTransfer";
            this.panTransfer.Size = new System.Drawing.Size(496, 202);
            this.panTransfer.TabIndex = 3;
            // 
            // ckShowAsHex
            // 
            this.ckShowAsHex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ckShowAsHex.AutoSize = true;
            this.ckShowAsHex.Checked = true;
            this.ckShowAsHex.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckShowAsHex.Location = new System.Drawing.Point(400, 37);
            this.ckShowAsHex.Name = "ckShowAsHex";
            this.ckShowAsHex.Size = new System.Drawing.Size(90, 17);
            this.ckShowAsHex.TabIndex = 6;
            this.ckShowAsHex.Text = "Show As Hex";
            this.ckShowAsHex.UseVisualStyleBackColor = true;
            // 
            // tWrite
            // 
            this.tWrite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tWrite.Location = new System.Drawing.Point(96, 4);
            this.tWrite.Multiline = true;
            this.tWrite.Name = "tWrite";
            this.tWrite.Size = new System.Drawing.Size(394, 20);
            this.tWrite.TabIndex = 5;
            // 
            // chkRead
            // 
            this.chkRead.AutoSize = true;
            this.chkRead.Location = new System.Drawing.Point(96, 37);
            this.chkRead.Name = "chkRead";
            this.chkRead.Size = new System.Drawing.Size(151, 17);
            this.chkRead.TabIndex = 4;
            this.chkRead.Text = "Use Data Received Event";
            this.chkRead.UseVisualStyleBackColor = true;
            this.chkRead.CheckedChanged += new System.EventHandler(this.chkRead_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.tRecv);
            this.groupBox2.Location = new System.Drawing.Point(0, 60);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(496, 137);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Data Received";
            // 
            // tRecv
            // 
            this.tRecv.ContextMenuStrip = this.ctxRecvTextBox;
            this.tRecv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tRecv.Location = new System.Drawing.Point(3, 16);
            this.tRecv.MaxLength = 0;
            this.tRecv.Multiline = true;
            this.tRecv.Name = "tRecv";
            this.tRecv.ReadOnly = true;
            this.tRecv.Size = new System.Drawing.Size(490, 118);
            this.tRecv.TabIndex = 2;
            // 
            // ctxRecvTextBox
            // 
            this.ctxRecvTextBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem});
            this.ctxRecvTextBox.Name = "ctxRecvTextBox";
            this.ctxRecvTextBox.Size = new System.Drawing.Size(111, 26);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // cmdRead
            // 
            this.cmdRead.Location = new System.Drawing.Point(15, 31);
            this.cmdRead.Name = "cmdRead";
            this.cmdRead.Size = new System.Drawing.Size(75, 23);
            this.cmdRead.TabIndex = 1;
            this.cmdRead.Text = "Read";
            this.cmdRead.UseVisualStyleBackColor = true;
            this.cmdRead.Click += new System.EventHandler(this.cmdRead_Click);
            // 
            // cmdWrite
            // 
            this.cmdWrite.Location = new System.Drawing.Point(15, 2);
            this.cmdWrite.Name = "cmdWrite";
            this.cmdWrite.Size = new System.Drawing.Size(75, 23);
            this.cmdWrite.TabIndex = 0;
            this.cmdWrite.Text = "Write";
            this.cmdWrite.UseVisualStyleBackColor = true;
            this.cmdWrite.Click += new System.EventHandler(this.cmdWrite_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.deviceToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(496, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // deviceToolStripMenuItem
            // 
            this.deviceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.standardRequestsToolStripMenuItem});
            this.deviceToolStripMenuItem.Name = "deviceToolStripMenuItem";
            this.deviceToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.deviceToolStripMenuItem.Text = "Device";
            this.deviceToolStripMenuItem.DropDownOpening += new System.EventHandler(this.standardRequestsToolStripMenuItem_DropDownOpening);
            // 
            // standardRequestsToolStripMenuItem
            // 
            this.standardRequestsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.getConfigurationToolStripMenuItem,
            this.getInterfaceToolStripMenuItem});
            this.standardRequestsToolStripMenuItem.Name = "standardRequestsToolStripMenuItem";
            this.standardRequestsToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.standardRequestsToolStripMenuItem.Text = "Standard Requests";
            // 
            // getConfigurationToolStripMenuItem
            // 
            this.getConfigurationToolStripMenuItem.Name = "getConfigurationToolStripMenuItem";
            this.getConfigurationToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.getConfigurationToolStripMenuItem.Text = "Get Configuration";
            this.getConfigurationToolStripMenuItem.Click += new System.EventHandler(this.getConfigurationToolStripMenuItem_Click);
            // 
            // fTestBulk
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 298);
            this.Controls.Add(this.panTransfer);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "fTestBulk";
            this.Text = "USB Test Bulk";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.fTestBulk_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.panTransfer.ResumeLayout(false);
            this.panTransfer.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ctxRecvTextBox.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel tsNumDevices;
        private System.Windows.Forms.ToolStripStatusLabel tsRefresh;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cboDevices;
        private System.Windows.Forms.Panel panTransfer;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tRecv;
        private System.Windows.Forms.Button cmdRead;
        private System.Windows.Forms.Button cmdWrite;
        private System.Windows.Forms.TextBox tWrite;
        private System.Windows.Forms.CheckBox chkRead;
        private System.Windows.Forms.Button cmdOpen;
        private System.Windows.Forms.ToolStripStatusLabel tsStatus;
        private System.Windows.Forms.ContextMenuStrip ctxRecvTextBox;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.CheckBox ckShowAsHex;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deviceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem standardRequestsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem getConfigurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem getInterfaceToolStripMenuItem;
    }
}