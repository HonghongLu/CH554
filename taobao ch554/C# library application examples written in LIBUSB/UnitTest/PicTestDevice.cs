using System;
using System.Diagnostics;
using System.IO;
using LibUsbDotNet.Usb;

namespace UnitTest
{
    public class PicTestDevice
    {
        public const int VID = 0x04d8;
        public const int PID = 0x0f01;

        private static UsbDevice mTestDevice;

        public static UsbDevice TestDevice
        {
            get
            {
                UsbDevice t;
                Open(out t);
                return t;
            }
        }

        #region PUBLIC METHODS
        public  static void OpenAndConfigure(StreamWriter sw)
        {
            Debug.Assert(TestDevice.Open(), "Failed opening device.");

            int ret;
            ret = TestDevice.SetConfiguration(1);
            Debug.Assert(ret == 0);
            if (sw != null) sw.WriteLine("SetConfiguration(1):" + ret);

            ret = TestDevice.ClaimInterface(0);
            Debug.Assert(ret == 0);
            if (sw != null) sw.WriteLine("ClaimInterface(0):" + ret);
        }
        public static void CloseAndDeConfigure(StreamWriter sw)
        {
            int ret;
            ret = TestDevice.ReleaseInterface(0);
            Debug.Assert(ret == 0);
            if (sw != null) sw.WriteLine("ReleaseInterface(0):" + ret);

            ret = TestDevice.SetConfiguration(0);
            Debug.Assert(ret == 0);
            if (sw != null) sw.WriteLine("SetConfiguration(0):" + ret);
            Debug.Assert(TestDevice.Close());
        }
        public static bool GetTestType(ref byte testType)
        {
            byte[] dat = {0x00, 0x00};

            if (TestDevice.IsOpen)
            {
                int ret = TestDevice.IOControlMessage(0xC0, 15, 0, 0, dat, 500);
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

        public static bool SetTestType(int iNewTest, ref byte bLastTestType)
        {
            byte[] dat = {0x00, 0x00};

            if (TestDevice.IsOpen)
            {
                int ret = TestDevice.IOControlMessage(0xC0, 14, iNewTest, 0, dat, 500);
                if (ret >= 0)
                {
                    bLastTestType = dat[0];
                    return true;
                }

                return false;
            }

            throw new Exception("Device Not Opened");
        }

        #endregion

        #region PRIVATE METHODS

        private static UsbDevice Find()
        {
            foreach (UsbDevice usbDevice in UsbGlobals.DeviceList)
            {
                if (usbDevice.Info.IdVendor == VID && usbDevice.Info.IdProduct == PID)
                {
                    return usbDevice;
                }
            }
            return null;
        }

        private static bool Open(out UsbDevice picTestUsbDevice)
        {
            if (ReferenceEquals(mTestDevice, null))
            {
                picTestUsbDevice = Find();

                if (picTestUsbDevice == null)
                    return false;
                else
                    mTestDevice = picTestUsbDevice;

                return true;
            }
            else
            {
                picTestUsbDevice = mTestDevice;
                return true;
            }
        }

        #endregion
    }
}