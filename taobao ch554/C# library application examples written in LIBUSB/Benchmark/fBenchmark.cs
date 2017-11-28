/*************************************************************************************
Copyright (C) 2007 Travis Robinson. All Rights Reserved

PIC Benchmark
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
*************************************************************************************/
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using LibUsbDotNet.Usb;
using LibUsbDotNet.Usb.Main;

namespace Benchmark
{
    public partial class fBenchmark : Form
    {
        private UsbDeviceList mDevList;
        private UsbDevice usb;

        private Thread mthWriteThreadEP1 = null;
        private DateTime mStartTimeEP1 = DateTime.MaxValue;
        private int mBytesRecvEP1 = 0;
        private int mByteErrorsEP1 = 0;
        private byte bValidatePosEP1 = 0;
        private UsbEndpointReader mEP1Reader = null;
        private UsbEndpointWriter mEP1Writer = null;
        private int miTestType = 0;
        private bool bWriteThreadEP1Enabled = false;

        public fBenchmark()
        {
            InitializeComponent();
        }

        #region PRIVATE METHODS

        private void cboDevice_DropDown(object sender, EventArgs e)
        {
            mDevList = UsbGlobals.DeviceList;
            cboDevice.Items.Clear();
            for (int i = 0; i < mDevList.Count; i++)
            {
                UsbDevice Device = mDevList[i];
                cboDevice.Items.Add(String.Format("Vendor:{0:X4} Product:{1:X4} {2}", Device.Info.IdVendor, Device.Info.IdProduct, Device.Info.ProductString));
            }
        }

        private void cboDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboDevice.SelectedIndex != -1)
                cmdOpenClose.Enabled = true;
        }

        private void cboTestType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!usb.IsOpen) return;

            int iCurrentTest = miTestType;
            int iNewTest = 0;
            switch (cboTestType.Text)
            {
                case "None":
                    iNewTest = 0;
                    break;
                case "Read":
                    iNewTest = 1;
                    break;
                case "Write":
                    iNewTest = 2;
                    break;
                case "Read/Write":
                    iNewTest = 3;
                    break;
                default:
                    iNewTest = 0;
                    break;
            }

            if (iNewTest == iCurrentTest) return;

            cboTestType.Enabled = false;

            stopReadWrite();

            byte[] dat = {0x00, 0x00};
            int ret = usb.IOControlMessage(0xC0, 14, iNewTest, 0, dat, 50);
            if (ret == 1)
            {
                Debug.Print("Test Selected:" + dat[0]);
            }

            byte[] _bTemp = new byte[64];
            Int64 tickStart = DateTime.Now.Ticks;
            while ((DateTime.Now.Ticks - tickStart) < 50000000)
            {
                ret = mEP1Reader.Read(_bTemp, 1000);
                if (ret == 1 && _bTemp[0] == 0x80)
                    break;
                Application.DoEvents();
            }
            ResetBenchmark();
            miTestType = iNewTest;

            if (iNewTest == 2)
            {
                mthWriteThreadEP1 = new Thread(new ThreadStart(WriteThreadEP1_NoRecv));
                bWriteThreadEP1Enabled = true;
                mthWriteThreadEP1.Start();
            }
            else if (iNewTest == 3)
            {
                mthWriteThreadEP1 = new Thread(new ThreadStart(WriteThreadEP1));
                bWriteThreadEP1Enabled = true;
                mthWriteThreadEP1.Start();
            }

            if (iNewTest == 0)
            {
                ResetBenchmark();
                panTest.Enabled = false;
            }
            else
                panTest.Enabled = true;

            if (iNewTest != 2)
                mEP1Reader.DataReceivedEnabled = true;

            cboTestType.Enabled = true;
        }

        private void closeTestDevice()
        {
            if (!ReferenceEquals(usb, null))
            {
                if (usb.IsOpen)
                {
                    mEP1Reader.DataReceived -= mBulkInOut_DataReceived;
                    stopReadWrite();
                    mEP1Reader.Dispose();
                    mEP1Writer.Dispose();
                    usb.ReleaseInterface(0);
                    usb.SetConfiguration(0);
                    usb.ActiveEndpoints.Clear();
                    usb.Close();
                }
                usb = null;
                mEP1Reader = null;
                mEP1Writer = null;
            }
        }

        private void cmdGetTestType_Click(object sender, EventArgs e)
        {
            cmdGetTestType.Enabled = false;
            if (usb != null && usb.IsOpen)
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
            cmdGetTestType.Enabled = true;
        }

        private void cmdOpenClose_Click(object sender, EventArgs e)
        {
            cmdOpenClose.Enabled = false;
            if (cmdOpenClose.Text == "Open")
            {
                // Open Device
                UsbDevice Device = mDevList[cboDevice.SelectedIndex];
                openAsTestDevice(Device);
                cmdOpenClose.Text = "Close";
                panDevice.Enabled = true;
            }
            else
            {
                closeTestDevice();
                cmdOpenClose.Text = "Open";
                panDevice.Enabled = false;
            }
            cmdOpenClose.Enabled = true;
        }

        private void fBenchmark_FormClosed(object sender, FormClosedEventArgs e)
        {
            Debug.Print("fBenchmark_FormClosed");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ReferenceEquals(null, usb))
                closeTestDevice();
            UsbGlobals.OnUsbError -= UsbGlobals_OnUsbError;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UsbGlobals.OnUsbError += UsbGlobals_OnUsbError;
        }

        private void GetConfigValue_Click(object sender, EventArgs e)
        {
            byte bCfgVal = 0;
            if (usb.GetConfiguration(ref bCfgVal) >= 0)
            {
                Debug.Print("Config Value:" + bCfgVal);
            }
        }

        private bool getTestType(out byte testType)
        {
            byte[] dat = {0x00, 0x00};

            if (usb.IsOpen)
            {
                int ret = usb.IOControlMessage(0xC0, 15, 0, 0, dat, 5000);
                if (ret == 1)
                {
                    testType = dat[0];
                    return true;
                }

                testType = 0;
                return false;
            }

            throw new Exception("Device Not Opened");
        }

        private void mBulkInOut_DataReceived(object sender, DataReceivedArgs e)
        {
            if (InvokeRequired)
                Invoke(new DataRecvDelegate(OnDataReceived), new object[] {sender, e});
            else
                OnDataReceived(sender, e);
        }

        private void OnDataReceived(object sender, DataReceivedArgs e)
        {
            switch (miTestType)
            {
                case 1:
                    OnDataReceived_Benchmark(sender, e);
                    break;
                case 3:
                    OnDataReceived_Benchmark(sender, e);
                    break;
                default:
                    break;
            }
        }

        private void OnDataReceived_Benchmark(object sender, DataReceivedArgs e)
        {
            for (int i = 0; i < e.Count; i++)
            {
                if (e.Buffer[i] == bValidatePosEP1)
                    bValidatePosEP1++;
                else if (e.Buffer[i] == 0)
                    bValidatePosEP1 = 1;
                else
                {
                    bValidatePosEP1 = e.Buffer[i];
                    mByteErrorsEP1++;
                }
            }
            if (mStartTimeEP1 == DateTime.MaxValue)
            {
                mStartTimeEP1 = DateTime.Now;
                return;
            }
            mBytesRecvEP1 += e.Count;

            TimeSpan tsDiff = DateTime.Now - mStartTimeEP1;

            if (tsDiff.TotalMilliseconds > 500)
            {
                double dDataRate = Math.Round(mBytesRecvEP1/(tsDiff.TotalMilliseconds/1000));
                if (mByteErrorsEP1 > 0)
                    lDataRateEP1.Text = "E" + mByteErrorsEP1.ToString() + ":" + dDataRate.ToString();
                else
                    lDataRateEP1.Text = dDataRate.ToString();
            }
        }

        private void OnDataReceived_Loop(object sender, DataReceivedArgs e)
        {
            UsbEndpointBase epSendender = (UsbEndpointBase) sender;
            String sTemp = "";
            for (int i = 0; i < e.Count; i++)
                sTemp += e.Buffer[i].ToString("X2");

            Debug.Print(" Len:" + e.Count.ToString() + " Data:" + sTemp);
        }

        private void openAsTestDevice(UsbDevice dev)
        {
            if (!ReferenceEquals(usb, null))
                closeTestDevice();

            usb = dev;
            usb.Open();
            if (usb.SetConfiguration(1) == 0)
            {
                if (usb.ClaimInterface(0) == 0)
                {
                    mEP1Reader = usb.OpenBulkEndpointReader(ReadEndpoints.Ep01);
                    mEP1Writer = usb.OpenBulkEndpointWriter(WriteEndpoints.Ep01);

                    mEP1Reader.DataReceived += mBulkInOut_DataReceived;
                    mEP1Reader.DataReceivedEnabled = true;
                    return;
                }
            }

            throw new Exception("Failed opening device.");
        }

        private void ResetBenchmark()
        {
            mByteErrorsEP1 = 0;

            bValidatePosEP1 = 0;

            mBytesRecvEP1 = 0;

            mStartTimeEP1 = DateTime.MaxValue;
        }

        private void SetDataRate(double dDataRate)
        {
            lDataRateEP1.Text = dDataRate.ToString();
        }

        private void stopReadWrite()
        {
            if (mthWriteThreadEP1 != null && bWriteThreadEP1Enabled)
            {
                bWriteThreadEP1Enabled = false;
                mEP1Writer.CancelIO();
                while (mthWriteThreadEP1.IsAlive) Application.DoEvents();

                mthWriteThreadEP1 = null;
            }

            mEP1Reader.DataReceivedEnabled = false;
        }

        private static void UsbGlobals_OnUsbError(object sender, UsbError e)
        {
            Debug.Print(e.ToString());
        }


        private void WriteThreadEP1()
        {
            byte[] dat = new byte[8192];
            for (uint i = 0; i < 8192; i++)
                dat[i] = (byte) (i & 0xff);

            int ret;
            while (bWriteThreadEP1Enabled)
            {
                ret = mEP1Writer.Write(dat, 5000);
                Thread.Sleep(0);
            }
        }

        private void WriteThreadEP1_NoRecv()
        {
            ResetBenchmark();
            mStartTimeEP1 = DateTime.Now;
            byte[] dat = new byte[8192];
            for (uint i = 0; i < 8192; i++)
                dat[i] = (byte) (i & 0xff);
            while (bWriteThreadEP1Enabled)
            {
                int ret = mEP1Writer.Write(dat, 5000);

                if (ret >= 0)
                {
                    mBytesRecvEP1 += (int) ret;

                    TimeSpan tsDiff = DateTime.Now - mStartTimeEP1;

                    if (tsDiff.TotalMilliseconds > 500)
                    {
                        double dDataRate = Math.Round(mBytesRecvEP1/(tsDiff.TotalMilliseconds/1000));
                        Invoke(new SetDataRateDelegate(SetDataRate), new object[] {dDataRate});
                    }
                }
                Thread.Sleep(0);
            }
        }

        #endregion

        #region Nested type: DataRecvDelegate

        private delegate void DataRecvDelegate(object sender, DataReceivedArgs e);

        #endregion

        #region Nested type: SetDataRateDelegate

        private delegate void SetDataRateDelegate(double dDataRate);

        #endregion
    }
}