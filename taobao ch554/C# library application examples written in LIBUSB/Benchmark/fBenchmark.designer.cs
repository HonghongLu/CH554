namespace Benchmark
{
    partial class fBenchmark
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
            this.cboDevice = new System.Windows.Forms.ComboBox();
            this.cmdOpenClose = new System.Windows.Forms.Button();
            this.panTest = new System.Windows.Forms.Panel();
            this.tErrors = new System.Windows.Forms.TextBox();
            this.lDataRateEP1 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panDevice = new System.Windows.Forms.Panel();
            this.GetConfigValue = new System.Windows.Forms.Button();
            this.cmdGetTestType = new System.Windows.Forms.Button();
            this.cboTestType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panTest.SuspendLayout();
            this.panDevice.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboDevice
            // 
            this.cboDevice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboDevice.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboDevice.FormattingEnabled = true;
            this.cboDevice.Location = new System.Drawing.Point(145, 12);
            this.cboDevice.Name = "cboDevice";
            this.cboDevice.Size = new System.Drawing.Size(218, 22);
            this.cboDevice.TabIndex = 21;
            this.cboDevice.SelectedIndexChanged += new System.EventHandler(this.cboDevice_SelectedIndexChanged);
            this.cboDevice.DropDown += new System.EventHandler(this.cboDevice_DropDown);
            // 
            // cmdOpenClose
            // 
            this.cmdOpenClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOpenClose.Enabled = false;
            this.cmdOpenClose.Location = new System.Drawing.Point(369, 10);
            this.cmdOpenClose.Name = "cmdOpenClose";
            this.cmdOpenClose.Size = new System.Drawing.Size(75, 23);
            this.cmdOpenClose.TabIndex = 20;
            this.cmdOpenClose.Text = "Open";
            this.cmdOpenClose.UseVisualStyleBackColor = true;
            this.cmdOpenClose.Click += new System.EventHandler(this.cmdOpenClose_Click);
            // 
            // panTest
            // 
            this.panTest.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panTest.Controls.Add(this.tErrors);
            this.panTest.Controls.Add(this.lDataRateEP1);
            this.panTest.Controls.Add(this.label1);
            this.panTest.Enabled = false;
            this.panTest.Location = new System.Drawing.Point(0, 27);
            this.panTest.Name = "panTest";
            this.panTest.Size = new System.Drawing.Size(443, 101);
            this.panTest.TabIndex = 22;
            // 
            // tErrors
            // 
            this.tErrors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tErrors.BackColor = System.Drawing.SystemColors.Info;
            this.tErrors.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.tErrors.Location = new System.Drawing.Point(6, 34);
            this.tErrors.Multiline = true;
            this.tErrors.Name = "tErrors";
            this.tErrors.ReadOnly = true;
            this.tErrors.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tErrors.Size = new System.Drawing.Size(433, 64);
            this.tErrors.TabIndex = 21;
            // 
            // lDataRateEP1
            // 
            this.lDataRateEP1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lDataRateEP1.Location = new System.Drawing.Point(157, 9);
            this.lDataRateEP1.Name = "lDataRateEP1";
            this.lDataRateEP1.Size = new System.Drawing.Size(282, 13);
            this.lDataRateEP1.TabIndex = 20;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Data Rate EP1 (Bytes/sec):";
            // 
            // panDevice
            // 
            this.panDevice.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panDevice.Controls.Add(this.GetConfigValue);
            this.panDevice.Controls.Add(this.cmdGetTestType);
            this.panDevice.Controls.Add(this.cboTestType);
            this.panDevice.Controls.Add(this.label2);
            this.panDevice.Controls.Add(this.panTest);
            this.panDevice.Enabled = false;
            this.panDevice.Location = new System.Drawing.Point(0, 39);
            this.panDevice.Name = "panDevice";
            this.panDevice.Size = new System.Drawing.Size(443, 128);
            this.panDevice.TabIndex = 23;
            // 
            // GetConfigValue
            // 
            this.GetConfigValue.Location = new System.Drawing.Point(87, 3);
            this.GetConfigValue.Name = "GetConfigValue";
            this.GetConfigValue.Size = new System.Drawing.Size(75, 23);
            this.GetConfigValue.TabIndex = 26;
            this.GetConfigValue.Text = "Get Cfg";
            this.GetConfigValue.UseVisualStyleBackColor = true;
            this.GetConfigValue.Click += new System.EventHandler(this.GetConfigValue_Click);
            // 
            // cmdGetTestType
            // 
            this.cmdGetTestType.Location = new System.Drawing.Point(6, 3);
            this.cmdGetTestType.Name = "cmdGetTestType";
            this.cmdGetTestType.Size = new System.Drawing.Size(75, 23);
            this.cmdGetTestType.TabIndex = 25;
            this.cmdGetTestType.Text = "Get Test";
            this.cmdGetTestType.UseVisualStyleBackColor = true;
            this.cmdGetTestType.Click += new System.EventHandler(this.cmdGetTestType_Click);
            // 
            // cboTestType
            // 
            this.cboTestType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTestType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTestType.FormattingEnabled = true;
            this.cboTestType.Items.AddRange(new object[] {
            "None",
            "Read",
            "Write",
            "Read/Write"});
            this.cboTestType.Location = new System.Drawing.Point(284, 0);
            this.cboTestType.Name = "cboTestType";
            this.cboTestType.Size = new System.Drawing.Size(156, 21);
            this.cboTestType.TabIndex = 24;
            this.cboTestType.SelectedIndexChanged += new System.EventHandler(this.cboTestType_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(201, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Select Test:";
            // 
            // fBenchmark
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 168);
            this.Controls.Add(this.panDevice);
            this.Controls.Add(this.cboDevice);
            this.Controls.Add(this.cmdOpenClose);
            this.MinimumSize = new System.Drawing.Size(375, 120);
            this.Name = "fBenchmark";
            this.Text = "PIC Endpoint Benchmark";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.fBenchmark_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panTest.ResumeLayout(false);
            this.panTest.PerformLayout();
            this.panDevice.ResumeLayout(false);
            this.panDevice.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cboDevice;
        private System.Windows.Forms.Button cmdOpenClose;
        private System.Windows.Forms.Panel panTest;
        private System.Windows.Forms.Label lDataRateEP1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panDevice;
        private System.Windows.Forms.ComboBox cboTestType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tErrors;
        private System.Windows.Forms.Button cmdGetTestType;
        private System.Windows.Forms.Button GetConfigValue;
    }
}

