using System.Diagnostics;
using System.IO;
using LibUsbDotNet.Usb;
using LibUsbDotNet.Usb.Main;

namespace UnitTest.Test
{
    public class Test_usb_strerror : ITest
    {
        #region ITest Members

        public void Run(Stream output)
        {
            StreamWriter sw = new StreamWriter(output);
            sw.WriteLine("--- Test_usb_strerror Test ---");

            UsbDevice testDevice = PicTestDevice.TestDevice;
            if (testDevice != null)
            {
                int ret;
                ret = testDevice.SetConfiguration(1);
                Debug.Assert(ret == 0);
                sw.WriteLine("SetConfiguration(1):" + ret);

                ret = testDevice.ClaimInterface(0);
                Debug.Assert(ret == 0);
                sw.WriteLine("ClaimInterface(0):" + ret);

                ret = testDevice.SetConfiguration(0);
                Debug.Assert(ret == (int) ErrorCodes.EINVAL);
                sw.WriteLine("SetConfiguration(0):" + ret);

                sw.WriteLine("LastError:" + UsbGlobals.LastError);

                ret = testDevice.ReleaseInterface(0);
                Debug.Assert(ret == 0);
                sw.WriteLine("ReleaseInterface(0):" + ret);

                ret = testDevice.SetConfiguration(0);
                Debug.Assert(ret == 0);
                sw.WriteLine("SetConfiguration(0):" + ret);

                testDevice.Close();
                sw.WriteLine("SUCCESS!");
            }
            else
            {
                sw.WriteLine("Failed opening device.");
            }
            sw.WriteLine("------------------------------");
            sw.Flush();
        }

        #endregion
    }
}