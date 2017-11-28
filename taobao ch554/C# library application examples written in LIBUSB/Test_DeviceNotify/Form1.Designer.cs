namespace Test_DeviceNotify
{
    partial class Form1
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
            this.tNotify = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tNotify
            // 
            this.tNotify.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tNotify.Location = new System.Drawing.Point(0, 0);
            this.tNotify.Multiline = true;
            this.tNotify.Name = "tNotify";
            this.tNotify.ReadOnly = true;
            this.tNotify.Size = new System.Drawing.Size(740, 340);
            this.tNotify.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 340);
            this.Controls.Add(this.tNotify);
            this.Name = "Form1";
            this.Text = "Test Device Notify";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tNotify;
    }
}

