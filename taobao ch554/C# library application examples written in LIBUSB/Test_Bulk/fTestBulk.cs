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
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LibUsbDotNet.Usb;
using LibUsbDotNet.Usb.Main;

namespace Test_Bulk
{
    public partial class fTestBulk : Form
    {
        public const int VID = 0x04d8;
        public const int PID = 0x0f01;
        private UsbDeviceList mDevList;
        private UsbDevice mDev;
        private UsbEndpointWriter mEpWriter;
        private UsbEndpointReader mEpReader;

        public fTestBulk()
        {
            InitializeComponent();
            refreshDeviceList();
        }

        #region PRIVATE METHODS

        private void cboDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void chkRead_CheckedChanged(object sender, EventArgs e)
        {
            if (mEpReader != null)
            {
                chkRead.Enabled = false;
                if (chkRead.Checked)
                {
                    mEpReader.DataReceivedEnabled = true;
                    cmdRead.Enabled = false;
                }
                else
                {
                    mEpReader.DataReceivedEnabled = false;
                    cmdRead.Enabled = true;
                }
                chkRead.Enabled = true;
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tRecv.Text = "";
        }

        private void closeDevice()
        {
            if (mDev != null)
            {
                if (mEpReader != null)
                {
                    mEpReader.DataReceivedEnabled = false;
                    mEpReader.DataReceived -= mEp_DataReceived;
                    mEpReader.Dispose();
                    mEpReader = null;
                }
                if (mEpWriter != null)
                {
                    mEpWriter.Dispose();
                    mEpWriter = null;
                }
                mDev.ReleaseInterface(0);
                mDev.SetConfiguration(0);

                mDev.Close();
                mDev = null;
            }
            panTransfer.Enabled = false;
        }

        private void cmdGetTestType_Click(object sender, EventArgs e)
        {
            if (mDev != null && mDev.IsOpen)
            {
                //bool bRecvEnabled = mEP1.DataReceivedEnabled;
                //mEP1.DataReceivedEnabled = false;
                byte bTestType;
                if (getTestType(out bTestType))
                {
                    Debug.Print("Test Type:" + bTestType);
                }
                //mEP1.DataReceivedEnabled = bRecvEnabled;
            }
        }

        private void cmdRead_Click(object sender, EventArgs e)
        {
            cmdRead.Enabled = false;
            byte[] readBuffer = new byte[64];

            int ret;
            ret = mEpReader.Read(readBuffer, 1000);
            if (ret >= 0)
            {
                tsStatus.Text = ret + " bytes read.";
                showBytes(readBuffer, (uint) ret);
            }
            else
                tsStatus.Text = "No data to read! ErrorCode:" + ret;

            cmdRead.Enabled = true;
        }

        private void cmdWrite_Click(object sender, EventArgs e)
        {
            cmdWrite.Enabled = false;
            byte[] bytesToWrite = Encoding.UTF8.GetBytes(tWrite.Text);

            int ret;
            ret = mEpWriter.Write(bytesToWrite, 1000);
            if (ret >= 0)
            {
                tsStatus.Text = ret + " bytes written.";
            }
            else
                tsStatus.Text = "Write failed!";

            cmdWrite.Enabled = true;
        }

        private void cndOpen_Click(object sender, EventArgs e)
        {
            cmdOpen.Enabled = false;
            if (cmdOpen.Text == "Open")
            {
                if (cboDevices.SelectedIndex > 0)
                {
                    if (openDevice(cboDevices.SelectedIndex))
                    {
                        cmdOpen.Text = "Close";
                    }
                }
            }
            else
            {
                closeDevice();
                cmdOpen.Text = "Open";
            }
            cmdOpen.Enabled = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            closeDevice();
            Application.Exit();
        }

        private void fTestBulk_FormClosing(object sender, FormClosingEventArgs e)
        {
            closeDevice();
        }

        private void getConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte bCgfValue = 0;
            int ret = mDev.GetConfiguration(ref bCgfValue);
            if (ret >= 0)
            {
                tsStatus.Text = "Configuration Value:" + bCgfValue;
            }
            else
                tsStatus.Text = "Failed getting configuration value!";
        }


        private bool getTestType(out byte testType)
        {
            byte[] dat = {0x00, 0x00};

            if (mDev.IsOpen)
            {
                int ret = mDev.IOControlMessage(0xC0, 15, 0, 0, dat, 500);
                if (ret >= 0)
                {
                    testType = dat[0];
                    return true;
                }

                testType = 0;
                return false;
            }

            throw new Exception("Device Not Opened");
        }

        private void mEp_DataReceived(object sender, DataReceivedArgs e)
        {
            Invoke(new OnDataReceivedDelegate(OnDataReceived), new object[] {sender, e});
        }

        private void OnDataReceived(object sender, DataReceivedArgs e)
        {
            showBytes(e.Buffer, (uint) e.Count);
        }

        private bool openDevice(int index)
        {
            int ret;

            bool bRtn = false;

            closeDevice();
            chkRead.CheckedChanged -= chkRead_CheckedChanged;
            chkRead.Checked = false;
            cmdRead.Enabled = true;
            chkRead.CheckedChanged += chkRead_CheckedChanged;

            mDev = mDevList[index];
            mDev.Open();

            Debug.Assert(mDev.SetConfiguration(1) == 0);
            Debug.Assert(mDev.ClaimInterface(0) == 0);

            mEpReader = mDev.OpenBulkEndpointReader(ReadEndpoints.Ep01);
            mEpWriter = mDev.OpenBulkEndpointWriter(WriteEndpoints.Ep01);
            mEpReader.DataReceived += mEp_DataReceived;
            panTransfer.Enabled = true;

            byte bTestType;
            if (mDev.Info.IdVendor == VID && mDev.Info.IdProduct == PID)
                if (getTestType(out bTestType))
                {
                    if (bTestType != 3)
                    {
                        ret = mDev.IOControlMessage(0xC0, 14, 3, 0, new byte[0], 1000);
                        if (ret >= 0)
                        {
                            byte[] _bTemp = new byte[64];
                            Int64 tickStart = DateTime.Now.Ticks;
                            while ((DateTime.Now.Ticks - tickStart) < 50000000)
                            {
                                ret = mEpReader.Read(_bTemp, 1000);
                                if (ret >= 0)
                                {
                                    if ((ret == 1) && (_bTemp[0] == 0x80))
                                    {
                                        bRtn = true;
                                        break;
                                    }
                                }
                                Application.DoEvents();
                            }
                        }
                    }
                    else
                        bRtn = true;
                }
            if (bRtn)
                tsStatus.Text = "Device Opened.";
            else
                tsStatus.Text = "Device Failed to Opened!";

            return bRtn;
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
                string sAdd = string.Format("Vid:0x{0:X4} Pid:0x{1:X4} {2} {3} {4}", device.Info.IdVendor, device.Info.IdVendor, device.Info.ManufacturerString, device.Info.ProductString, device.Info.SerialString);
                cboDevices.Items.Add(sAdd);
            }
            cboDevices.SelectedIndexChanged += cboDevices_SelectedIndexChanged;

            if (mDevList.Count == 0)
            {
                tsNumDevices.ForeColor = Color.Red;
            }
            else
            {
                tsNumDevices.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
            }
        }

        private void showBytes(byte[] readBuffer, uint uiTransmitted)
        {
            if (ckShowAsHex.Checked)
            {
                string s = "";
                for (int i = 0; i < uiTransmitted; i++)
                {
                    s += readBuffer[i].ToString("X2") + " ";
                }
                tRecv.Text += s;
            }
            else
            {
                tRecv.Text += Encoding.UTF8.GetString(readBuffer, 0, (int) uiTransmitted);
            }
        }

        private void standardRequestsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (mDev != null && mDev.IsOpen)
            {
                standardRequestsToolStripMenuItem.Enabled = true;
            }
            else
            {
                standardRequestsToolStripMenuItem.Enabled = false;
            }
        }

        private void tsRefresh_Click(object sender, EventArgs e)
        {
            refreshDeviceList();
        }

        #endregion

        #region Nested type: OnDataReceivedDelegate

        private delegate void OnDataReceivedDelegate(object sender, DataReceivedArgs e);

        #endregion
    }
}