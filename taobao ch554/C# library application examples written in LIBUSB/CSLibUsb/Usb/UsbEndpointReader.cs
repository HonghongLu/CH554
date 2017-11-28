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
using System.Threading;
using System.Windows.Forms;
using LibUsbDotNet.Usb.Internal;
using LibUsbDotNet.Usb.Main;

namespace LibUsbDotNet.Usb
{
    /// <summary>
    /// Main class used to receive data from a USB device.
    /// </summary>
    public class UsbEndpointReader : UsbEndpointBase, IDisposable
    {
        /// <summary>
        /// The DataReceived Event is fired when new data arrives for the current <see cref="UsbEndpointReader"/>.
        /// </summary>
        public event EventHandler<DataReceivedArgs> DataReceived;

        /// <summary>
        /// Default read buffer size.
        /// </summary>
        public const int DEF_READ_BUFFER_SIZE = 16384;

        /// <summary>
        /// Default read mTimeout.
        /// </summary>
        public const int DEF_READ_TIMEOUT = 5000;

        private Thread mthReadThread;
        private bool mDataReceivedEnabled = false;
        private ReadEndpoints mReadEndpoint = ReadEndpoints.Ep01;
        private int mReadBufferSize = DEF_READ_BUFFER_SIZE;
        private EventWaitHandle mEventCancelReadThread = new EventWaitHandle(false, EventResetMode.ManualReset);

        internal UsbEndpointReader(UsbDevice usbDevice, EndpointTypes epType, int packetSize, int mReadBufferSize, ReadEndpoints mReadEndpoint)
            : base(usbDevice, epType, packetSize)
        {
            this.mReadBufferSize = mReadBufferSize;
            this.mReadEndpoint = mReadEndpoint;
        }

        /// <summary>
        /// Gets/Sets a value indicating if the <see cref="UsbEndpointReader.DataReceived"/> event should be used.
        /// </summary>
        /// <remarks>
        /// If DataReceivedEnabled is true the <see cref="UsbEndpointReader.Read(byte[] , int , int , int )"/> functions cannot be used.
        /// </remarks>
        public bool DataReceivedEnabled
        {
            get { return mDataReceivedEnabled; }
            set
            {
                if (value != mDataReceivedEnabled)
                {
                    startStopReadThread();
                }
            }
        }

        /// <summary>
        /// The endpoint ID number that this instance is reading from.
        /// </summary>
        public ReadEndpoints ReadEndpoint
        {
            get { return mReadEndpoint; }
        }

        /// <summary>
        /// The endpoint number as a <see cref="System.Byte"/>.
        /// </summary>
        public override byte EpNum
        {
            get { return (byte) mReadEndpoint; }
        }

        /// <summary>
        /// Size of the read buffer in bytes for the <see cref="UsbEndpointReader.DataReceived"/> event.
        /// </summary>
        public int ReadBufferSize
        {
            get { return mReadBufferSize; }
            set { mReadBufferSize = value; }
        }

        #region PUBLIC METHODS

        /// <summary>
        /// Reads data from the current <see cref="UsbEndpointReader"/>.
        /// </summary>
        /// <param name="buffer">The buffer to store the recieved data in.</param>
        /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
        /// <returns>
        /// Number of bytes transmitted or less than zero if an error occured.
        /// </returns>
        public int Read(byte[] buffer, int timeout)
        {
            return Read(buffer, 0, buffer.Length, timeout);
        }

        /// <summary>
        /// Reads data from the current <see cref="UsbEndpointReader"/>.
        /// </summary>
        /// <param name="buffer">The buffer to store the recieved data in.</param>
        /// <param name="offset">The position in buffer to start storing the data.</param>
        /// <param name="count">The maximum number of bytes to receive.</param>
        /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
        /// <returns>
        /// Number of bytes transmitted or less than zero if an error occured.
        /// </returns>
        public int Read(byte[] buffer, int offset, int count, int timeout)
        {
            return Transfer(buffer, offset, count, timeout);
        }

        #endregion

        #region PRIVATE METHODS

        private void ReadData(object obj)
        {
            int ret = 0;
            TransferContext transferContext = (TransferContext) obj;
            UsbEndpointReader reader = (UsbEndpointReader) transferContext.mEndpointBase;
            reader.mDataReceivedEnabled = true;

            try
            {
                while (ret >= 0 && !reader.mEventCancelReadThread.WaitOne(0, false))
                {
                    lock( oLockTransferContext)
                        transferContext.Reset();

                    ret = setupAsync(transferContext);

                    if (ret >= 0)
                    {
                        bool bContinue = true;
                        while (bContinue)
                        {
                            ret = submitAsync(transferContext);
                            if (ret >= 0)
                            {
                                ReapRetry:
                                ret = reapAsyncNoCancel(transferContext);

                                if (ret < 0)
                                {
                                    if (ret == (int) ErrorCodes.ETIMEDOUT)
                                    {
                                        if (!transferContext.mbAsyncCancelled && !reader.mEventCancelReadThread.WaitOne(0, false))
                                        {
                                            goto ReapRetry;
                                        }
                                        else
                                        {
                                            ret = (int) ErrorCodes.EINTR;
                                        }
                                    }
                                }
                            }

                            lock (oLockTransferContext)
                                bContinue = ((transferContext.mCurrentRemaining > 0) && (ret == transferContext.mRequested));
                        }

                        freeAsync(transferContext);
                    }

                    if (ret >= 0)
                    {
                        EventHandler<DataReceivedArgs> temp = reader.DataReceived;
                        if (temp != null)
                        {
                            temp(this, new DataReceivedArgs(transferContext.Buffer, transferContext.currentTransmitted));
                        }
                    }
                }
            }
            catch (ThreadAbortException)
            {
                Debug.Print("ThreadAbortException:" + GetType().FullName + ".ReadData");
            }
            finally
            {
                if (transferContext.mContext.IsValid) freeAsync(transferContext);

                reader.mDataReceivedEnabled = false;
                reader.TransferLock.Release();
            }
        }

        private void startStopReadThread()
        {
            if (IsDisposed) return;

            if (mDataReceivedEnabled)
            {
                mEventCancelReadThread.Set();

               int iLoopCount=0;
                while (mthReadThread.IsAlive)
                {
                    iLoopCount++;
                    if ((iLoopCount) > 1000)
                    {
                        mthReadThread.Abort();
                        UsbGlobals.Error(this, "Thread could not be gracefully stopped.","startStopReadThread", (int) ErrorCodes.ETHREADABORT);
                        break;
                    }
                    Application.DoEvents();
                    Thread.Sleep(1);
                    if (Context.mContext.IsValid) cancelAsync(Context);

                }
                iLoopCount = 0;
                while (mDataReceivedEnabled)
                {
                    iLoopCount++;
                    Application.DoEvents();
                    Thread.Sleep(1);
                    if ((iLoopCount) > 1000)
                    {
                        throw new LibUsbException(this, "startStopReadThread: The read thread is stalled.");
                    }
                }
                mthReadThread = null;
            }
            else
            {
                if (!TransferLock.WaitOne(0, false))
                {
                    UsbGlobals.Error(this, "Read thread could not be started because a transfer is allready pending.", "startStopReadThread", (int) ErrorCodes.EBUSY);
                    return;
                }

                mEventCancelReadThread.Reset();
                
                lock(oLockTransferContext)
                    Context.Setup(new byte[mReadBufferSize], 0, mReadBufferSize, Timeout.Infinite, false);

                mthReadThread = new Thread(ReadData);
                mthReadThread.Start(Context);
                Application.DoEvents();
            }
        }

        #endregion
    }
}