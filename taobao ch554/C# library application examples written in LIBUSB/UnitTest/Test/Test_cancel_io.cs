using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using LibUsbDotNet.Usb;
using LibUsbDotNet.Usb.Main;

namespace UnitTest.Test
{
    public class Test_cancel_io : ITest
    {
        #region ITest Members

        public void Run(Stream output)
        {
            byte[] _bTemp = new byte[64];
            StreamWriter sw = new StreamWriter(output);
            sw.WriteLine("--- Test_cancel_io ---");

            UsbDevice testDevice = PicTestDevice.TestDevice;
            if (testDevice != null)
            {
                Debug.Assert(testDevice.Open(), "Failed opening device.");

                int ret;
                ret = testDevice.SetConfiguration(1);
                Debug.Assert(ret == 0);
                sw.WriteLine("SetConfiguration(1):" + ret);

                ret = testDevice.ClaimInterface(0);
                Debug.Assert(ret == 0);
                sw.WriteLine("ClaimInterface(0):" + ret);

                byte bTestType = 0;
                Debug.Assert(PicTestDevice.GetTestType(ref bTestType));

                UsbEndpointReader reader = testDevice.OpenBulkEndpointReader(ReadEndpoints.Ep01);
                if (bTestType != 3)
                {
                    Debug.Assert(PicTestDevice.SetTestType(3, ref bTestType));
                    while (ret >= 0)
                    {
                        ret = reader.Read(_bTemp, 1000);
                        if (ret == 1 && _bTemp[0] == 0x80) break;
                    }
                }

                reader.DataReceivedEnabled = true;
                Application.DoEvents();
                Thread.Sleep(1000);
                Thread.Sleep(1000);
                reader.CancelIO();
                Application.DoEvents();
                reader.Dispose();

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