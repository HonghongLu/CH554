using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using UnitTest.Test;

namespace UnitTest
{
    public partial class fUsbTest : Form
    {
        public fUsbTest()
        {
            InitializeComponent();
        }

        #region PRIVATE METHODS

        private void cancelioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Test_cancel_io t = new Test_cancel_io();
            MemoryStream mStream = new MemoryStream();
            t.Run(mStream);

            mStream.Seek(0, SeekOrigin.Begin);

            StreamReader sw = new StreamReader(mStream);
            Debug.Print(sw.ReadToEnd());
        }

        private void usbstrerrorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Test_usb_strerror t = new Test_usb_strerror();
            MemoryStream mStream = new MemoryStream();
            t.Run(mStream);

            mStream.Seek(0, SeekOrigin.Begin);

            StreamReader sw = new StreamReader(mStream);
            Debug.Print(sw.ReadToEnd());
        }

        #endregion

        private void bigReadWriteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Test_BigReadWrite t = new Test_BigReadWrite();
            MemoryStream mStream = new MemoryStream();
            t.Run(mStream);

            mStream.Seek(0, SeekOrigin.Begin);

            StreamReader sw = new StreamReader(mStream);
            Debug.Print(sw.ReadToEnd());

        }
    }
}