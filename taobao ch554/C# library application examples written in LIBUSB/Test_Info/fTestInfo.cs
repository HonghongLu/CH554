/*
Copyright (C) 2007 Travis Robinson. All Rights Reserved

www.picmicrochip.com
travis@picmicrochip.com

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
or visit www.gnu.org.
*/
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using LibUsbDotNet.Usb;
using LibUsbDotNet.Usb.Info;
using LibUsbDotNet.Usb.Main;

namespace Test_Info
{
    public partial class fTestInfo : Form
    {
        private UsbDeviceList mDevList;

        public fTestInfo()
        {
            InitializeComponent();
            tsVersion.Text = "LibUsbDotNet " + LibUsbDotNetVersion;
            refreshDeviceList();
        }

        public string LibUsbDotNetVersion
        {
            get
            {
                Assembly assembly = Assembly.GetAssembly(typeof (UsbDevice));
                string[] assemblyKvp = assembly.FullName.Split(',');
                foreach (string s in assemblyKvp)
                {
                    string[] sKeyPair = s.Split('=');
                    if (sKeyPair[0].ToLower().Trim() == "version")
                    {
                        return sKeyPair[1].Trim();
                    }
                }
                return null;
            }
        }

        #region PRIVATE METHODS

        private void addDevice(UsbDevice device, string display)
        {
            TreeNode tvDevice = tvInfo.Nodes.Add(display);
            string[] sDeviceAdd = device.Info.ToString().Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in sDeviceAdd)
                tvDevice.Nodes.Add(s);


            foreach (InfoConfig cfgInfo in device.Configs)
            {
                TreeNode tvConfig = tvDevice.Nodes.Add("Config " + cfgInfo.ConfigurationValue);
                string[] sCfgAdd = cfgInfo.ToString().Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in sCfgAdd)
                    tvConfig.Nodes.Add(s);
                if (cfgInfo.ConfigurationIndex > 0) tvConfig.Nodes.Add("ConfigString:" + cfgInfo.ConfigurationString);

                if (cfgInfo.InfoInterfaceList.Count == 0) continue;

                TreeNode tvInterfaces = tvConfig; //.Nodes.Add("Interfaces");
                foreach (InfoInterface interfaceInfo in cfgInfo.InfoInterfaceList)
                {
                    TreeNode tvInterface = tvInterfaces.Nodes.Add("Interface [" + interfaceInfo.InterfaceNumber + "," + interfaceInfo.AlternateSetting + "]");
                    string[] sInterfaceAdd = interfaceInfo.ToString().Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s in sInterfaceAdd)
                        tvInterface.Nodes.Add(s);
                    if (interfaceInfo.InterfaceIndex > 0) tvConfig.Nodes.Add("InterfaceString:" + interfaceInfo.InterfaceString);

                    if (interfaceInfo.InfoEndpointList.Count == 0) continue;

                    TreeNode tvEndpoints = tvInterface; //.Nodes.Add("Endpoints");
                    foreach (InfoEndpoint endpointInfo in interfaceInfo.InfoEndpointList)
                    {
                        TreeNode tvEndpoint = tvEndpoints.Nodes.Add("Endpoint 0x" + ((byte) endpointInfo.EndpointAddress).ToString("X2"));
                        string[] sEndpointAdd = endpointInfo.ToString().Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string s in sEndpointAdd)
                            tvEndpoint.Nodes.Add(s);
                    }
                }
                if (cfgInfo.Extra != null && cfgInfo.Extra.Length > 0)
                {
                    TreeNode tvCustomDeviceDescr = tvConfig.Nodes.Add("Custom Descriptors Length:" + cfgInfo.Extra.Length);
                }
            }
        }

        private void cboDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboDevices.SelectedIndex >= 0)
            {
                tvInfo.Nodes.Clear();
                addDevice(mDevList[cboDevices.SelectedIndex], cboDevices.Text);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void refreshDeviceList()
        {
            cboDevices.SelectedIndexChanged -= cboDevices_SelectedIndexChanged;
            mDevList = UsbGlobals.DeviceList;
            tsNumDevices.Text = mDevList.Count.ToString();
            cboDevices.Sorted = false;
            cboDevices.Items.Clear();
            foreach (UsbDevice device in mDevList)
            {
                string sAdd = string.Format("Vid:0x{0:X4} Pid:0x{1:X4} {2} {3} {4}", device.Info.IdVendor, device.Info.IdProduct, device.Info.ManufacturerString, device.Info.ProductString, device.Info.SerialString);

                cboDevices.Items.Add(sAdd);
            }
            cboDevices.SelectedIndexChanged += cboDevices_SelectedIndexChanged;

            if (mDevList.Count == 0)
            {
                tsNumDevices.ForeColor = Color.Red;
                tvInfo.Nodes.Clear();
                tvInfo.Nodes.Add("No USB devices found.");
                tvInfo.Nodes.Add("A device must be installed which uses the LibUsb-Win32 driver.");
                tvInfo.Nodes.Add("Or");
                tvInfo.Nodes.Add("The LibUsb-Win32 kernel service must be enabled.");
            }
            else
            {
                tsNumDevices.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
            }
        }

        private void tsRefresh_Click(object sender, EventArgs e)
        {
            refreshDeviceList();
        }

        #endregion
    }
}