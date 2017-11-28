using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using LibUsbDotNet.DeviceNotify;
using LibUsbDotNet.Usb;
using LibUsbDotNet.Usb.Main;

namespace Test_DeviceNotify
{
    public partial class Form1 : Form
    {
        private DeviceNotifier mDevNotifier;

        public Form1()
        {
            InitializeComponent();
            mDevNotifier = new DeviceNotifier();
            mDevNotifier.OnDeviceNotify += new EventHandler<DeviceNotifyEventArgs>(mDevNotifier_OnDeviceNotify);
        }

        #region PRIVATE METHODS

        private void mDevNotifier_OnDeviceNotify(object sender, DeviceNotifyEventArgs e)
        {
            Invoke(new DeviceNotifyEventDelegate(OnDevNotify), new object[] {sender, e});
            Debug.Print(e.Object.ToString());
        }

        private void OnDevNotify(object sender, DeviceNotifyEventArgs e)
        {
            object[] o = new object[] {e.EventType.ToString(), DateTime.Now.ToString(), e.DeviceType.ToString(), e.Object.ToString()};
            string s = String.Format("{0} - Time: {1}  -  {2}\r\n{3}", o);

            if (e.DeviceType == DeviceType.DEVICEINTERFACE && e.EventType == EventType.DEVICEARRIVAL)
                s += "\r\n" + e.Device.SymbolicName.FullName;
            tNotify.Text += s;
        }

        #endregion

        #region Nested type: DeviceNotifyEventDelegate

        private delegate void DeviceNotifyEventDelegate(object sender, DeviceNotifyEventArgs e);

        #endregion
    }
}