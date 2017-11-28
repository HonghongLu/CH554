namespace UnitTest
{
    partial class fUsbTest
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.functionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usbstrerrorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cancelioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.readWriteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bigReadWriteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.functionsToolStripMenuItem,
            this.readWriteToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(292, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // functionsToolStripMenuItem
            // 
            this.functionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.usbstrerrorToolStripMenuItem,
            this.cancelioToolStripMenuItem});
            this.functionsToolStripMenuItem.Name = "functionsToolStripMenuItem";
            this.functionsToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.functionsToolStripMenuItem.Text = "Functions";
            // 
            // usbstrerrorToolStripMenuItem
            // 
            this.usbstrerrorToolStripMenuItem.Name = "usbstrerrorToolStripMenuItem";
            this.usbstrerrorToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.usbstrerrorToolStripMenuItem.Text = "usb_strerror";
            this.usbstrerrorToolStripMenuItem.Click += new System.EventHandler(this.usbstrerrorToolStripMenuItem_Click);
            // 
            // cancelioToolStripMenuItem
            // 
            this.cancelioToolStripMenuItem.Name = "cancelioToolStripMenuItem";
            this.cancelioToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.cancelioToolStripMenuItem.Text = "cancel_io";
            this.cancelioToolStripMenuItem.Click += new System.EventHandler(this.cancelioToolStripMenuItem_Click);
            // 
            // readWriteToolStripMenuItem
            // 
            this.readWriteToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bigReadWriteToolStripMenuItem});
            this.readWriteToolStripMenuItem.Name = "readWriteToolStripMenuItem";
            this.readWriteToolStripMenuItem.Size = new System.Drawing.Size(74, 20);
            this.readWriteToolStripMenuItem.Text = "Read/Write";
            // 
            // bigReadWriteToolStripMenuItem
            // 
            this.bigReadWriteToolStripMenuItem.Name = "bigReadWriteToolStripMenuItem";
            this.bigReadWriteToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.bigReadWriteToolStripMenuItem.Text = "Big ReadWrite";
            this.bigReadWriteToolStripMenuItem.Click += new System.EventHandler(this.bigReadWriteToolStripMenuItem_Click);
            // 
            // fUsbTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "fUsbTest";
            this.Text = "USB Unit Tester";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem functionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem usbstrerrorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cancelioToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem readWriteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bigReadWriteToolStripMenuItem;
    }
}

