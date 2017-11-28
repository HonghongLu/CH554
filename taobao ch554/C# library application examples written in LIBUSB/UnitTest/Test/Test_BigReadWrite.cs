using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using LibUsbDotNet.Usb;
using LibUsbDotNet.Usb.Main;

namespace UnitTest.Test
{
    public class Test_BigReadWrite : ITest
    {

        #region ITest Members

        public int TEST_SIZE = 512000 - 1;
        private byte[] BytesReceived;
        private byte[] BytesToSend;
        private int miBytesReceived=0;
        private object oReceiveCount=new object();

         public void Run(System.IO.Stream output, int count)
         {
             miBytesReceived = 0;
             BytesReceived = new byte[TEST_SIZE];
             BytesToSend = new byte[TEST_SIZE];
             Random rnd = new Random();
             rnd.NextBytes(BytesToSend);

             byte[] _bTemp = new byte[64];
             StreamWriter sw = new StreamWriter(output);
             sw.WriteLine("--- Test_BigReadWrite ---"); sw.Flush();

             UsbDevice testDevice = PicTestDevice.TestDevice;
             if (testDevice != null)
             {
                 Debug.Assert(testDevice.Open(), "Failed opening device.");

                 int ret = 0;
                 PicTestDevice.OpenAndConfigure(sw);

                 byte bTestType = 0;
                 Debug.Assert(PicTestDevice.GetTestType(ref bTestType));

                 UsbEndpointReader reader = testDevice.OpenBulkEndpointReader(ReadEndpoints.Ep01);
                 UsbEndpointWriter writer = testDevice.OpenBulkEndpointWriter(WriteEndpoints.Ep01);
                 if (bTestType != 3)
                 {
                     Debug.Assert(PicTestDevice.SetTestType(3, ref bTestType));
                     while (ret >= 0)
                     {
                         ret = reader.Read(_bTemp, 1000);
                         if (ret == 1 && _bTemp[0] == 0x80) break;
                     }
                 }

                 reader.DataReceived += new EventHandler<DataReceivedArgs>(reader_DataReceived);
                 for (int iLoop = 0; iLoop < count; iLoop++)
                 {
                     miBytesReceived = 0;
                     reader.DataReceivedEnabled = true;
                     System.Windows.Forms.Application.DoEvents();
                     Debug.Assert(writer.Write(BytesToSend, Timeout.Infinite) == TEST_SIZE);
                     while (ReceiveCount < TEST_SIZE)
                     {
                         System.Windows.Forms.Application.DoEvents();
                         Thread.Sleep(1000);
                     }
                     reader.DataReceivedEnabled = false;
                     System.Windows.Forms.Application.DoEvents();
                     sw.WriteLine("SUCCESS:" + TEST_SIZE); sw.Flush();

                 }
                 reader.DataReceived -= reader_DataReceived;
                 reader.Dispose();
                 writer.Dispose();

                 PicTestDevice.CloseAndDeConfigure(sw);
                 


             }             
         }
       public void Run(System.IO.Stream output)
        {
           Run(output, 1);
        }

        private static bool CompareBytes(byte[] send, byte[] received)
        {
            if (send.Length!=received.Length) return false;
            for (int i = 0; i < send.Length; i++)
            {
                if (send[i]!=received[i])
                    return false;
            }
            return true;
        }

        private int ReceiveCount
        {
            get
            {
                lock(oReceiveCount)
                    return miBytesReceived;
            }
            set
            {
                lock (oReceiveCount)
                    miBytesReceived = value;
            }
        }
        void reader_DataReceived(object sender, DataReceivedArgs e)
        {
            Array.Copy(e.Buffer, 0, BytesReceived, ReceiveCount, e.Count);
            ReceiveCount += e.Count;
        }

        #endregion
    }
}
