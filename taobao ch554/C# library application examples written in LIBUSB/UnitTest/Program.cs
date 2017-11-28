using System;
using System.Windows.Forms;

namespace UnitTest
{
    internal static class Program
    {
        #region PRIVATE METHODS

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new fUsbTest());
        }

        #endregion
    }
}