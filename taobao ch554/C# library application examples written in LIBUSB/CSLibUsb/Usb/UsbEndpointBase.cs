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
using System.Threading;
using LibUsbDotNet.Usb.Internal;
using LibUsbDotNet.Usb.Internal.API;
using LibUsbDotNet.Usb.Main;

namespace LibUsbDotNet.Usb
{
    /// <summary>
    /// Base abstract class for endpoint IO operations.
    /// </summary>
    public abstract class UsbEndpointBase
    {
        internal UsbDevice mUsbDevice;
        internal EndpointTypes mEpType;
        internal int mPacketSize = 0;


        private TransferContext mTransferContext;
        private Semaphore mTransferLock = new Semaphore(1, 1);
        internal bool mbDisposed = false;
        internal object oLockTransferContext = new object();

        internal UsbEndpointBase(UsbDevice usbDevice, EndpointTypes epType, int packetSize)
        {
            mPacketSize = packetSize;
            mEpType = epType;
            mUsbDevice = usbDevice;
            mTransferContext = new TransferContext(this);
        }

        /// <summary>
        /// True if this <see cref="UsbEndpointBase"/> has been disposed by the <see cref="Dispose"/> function.  If an endpoint is disposed it can no longer be used.
        /// <see cref="ErrorCodes"/>.<see cref="ErrorCodes.ENODEV"/> will be retuned if a function call is made to a disposed endpoint.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return mbDisposed;
            }
        }
        internal Semaphore TransferLock
        {
            get { return mTransferLock; }
        }

        internal TransferContext Context
        {
            get { return mTransferContext; }
        }

        /// <summary>
        /// The endpoint number as a <see cref="System.Byte"/>.
        /// </summary>
        public abstract byte EpNum { get; }

        /// <summary>
        /// The endpoint type of this <see cref="UsbEndpointBase"/> class.
        /// </summary>
        public EndpointTypes EndpointType
        {
            get { return mEpType; }
        }

        /// <summary>
        /// Gets a value indicating the packet size used for Isochronous transfers.
        /// </summary>
        public int ISOPacketSize
        {
            get { return mPacketSize; }
        }

        #region PUBLIC METHODS

        /// <summary>
        /// Cancels any pending operation for this endpoint.
        /// </summary>
        public int CancelIO()
        {
            int ret = 0;
            if (mbDisposed) return (int) ErrorCodes.ENODEV;

            if (mTransferContext.mContext.IsValid)
            {
                ret = cancelAsync(mTransferContext);
            }
            return ret;
        }

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        public void Dispose()
        {
            if (!mbDisposed)
            {
                if (GetType() == typeof (UsbEndpointReader))
                {
                    UsbEndpointReader reader = (UsbEndpointReader) this;
                    if (reader.DataReceivedEnabled) reader.DataReceivedEnabled = false;
                    System.Windows.Forms.Application.DoEvents();
                }
                System.Windows.Forms.Application.DoEvents();
                freeAsync(mTransferContext);
                System.Windows.Forms.Application.DoEvents();
                mUsbDevice.ActiveEndpoints.removeFromList(this);
                System.Windows.Forms.Application.DoEvents();
                mbDisposed = true;
            }
        }

        /// <summary>
        /// Reads or Writes data (depending on the <see cref="EpNum"/>) to/from the current <see cref="UsbEndpointReader"/>.
        /// </summary>
        /// <param name="buffer">The buffer for the tranfer.</param>
        /// <param name="offset">The position in buffer to start storing the data.</param>
        /// <param name="count">The number of bytes to send or the maximum number of bytes to receive.</param>
        /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
        /// <returns>
        /// Number of bytes transmitted or less than zero if an error occured.
        /// </returns>
        public int Transfer(byte[] buffer, int offset, int count, int timeout)
        {
            if (mbDisposed) return (int) ErrorCodes.ENODEV;

            if (!TransferLock.WaitOne(0, false)) return (int) ErrorCodes.EBUSY;

            int ret = -1;

            try
            {
                lock (oLockTransferContext)
                    Context.Setup(buffer, offset, count, timeout, true);

                ret = transferSync(Context);
            }
            catch (Exception ex)
            {
                ret = (int) ErrorCodes.EEXCEPTION;
                UsbGlobals.Error(this, ex.ToString(), "Transfer", ret);
            }
            finally
            {
                freeAsync(Context);
                TransferLock.Release();
            }
            return ret;
        }

        #endregion

        internal int cancelAsync(TransferContext transferContext)
        {
            lock (oLockTransferContext)
                return cancelAsync_NL(transferContext);
        }

        internal int freeAsync(TransferContext transferContext)
        {
            lock (oLockTransferContext)
            {
                int ret = 0;
                if (!transferContext.mContext.IsValid) return ret;

                cancelAsync_NL(transferContext);

                ret = LibUsbAPI.usb_free_async(ref transferContext.mContext);

                if (ret < 0 || transferContext.mContext.IsValid)
                    UsbGlobals.Error(this, UsbGlobals.LastError, "freeAsync", ret);

                return ret;
            }
        }

        internal int reapAsync(TransferContext transferContext)
        {
            int ret = -1;
            ret = LibUsbAPI.usb_reap_async(transferContext.mContext, transferContext.mTimeout);
            lock (oLockTransferContext)
                transferContext.mbAsyncCancelled = true;

            if (ret < 0 && ret != (int) ErrorCodes.ETIMEDOUT)
                UsbGlobals.Error(this, UsbGlobals.LastError, "reapAsync", ret);
            else if (ret >= 0)
            {
                lock (oLockTransferContext)
                    transferContext.IncrementTransfer(ret);
            }
            return ret;
        }

        internal int reapAsyncNoCancel(TransferContext transferContext)
        {
            int ret = -1;
            lock (oLockTransferContext)
            {
                if (transferContext.mbAsyncCancelled)
                    return (int) ErrorCodes.ETHREADABORT;
            }
            ret = LibUsbAPI.usb_reap_async_nocancel(transferContext.mContext, transferContext.mTimeout);

            lock (oLockTransferContext)
            {
                if (ret < 0 && ret != (int) ErrorCodes.ETIMEDOUT)
                {
                    transferContext.mbAsyncCancelled = true;
                    UsbGlobals.Error(this, UsbGlobals.LastError, "reapAsyncNoCancel", ret);
                }
                else if (ret >= 0)
                {
                    transferContext.mbAsyncCancelled = true;
                    transferContext.IncrementTransfer(ret);
                }

                return ret;
            }
        }

        internal int setupAsync(TransferContext transferContext)
        {
            int ret = -1;
            lock (oLockTransferContext)
            {
                transferContext.Reset();
                switch (mEpType)
                {
                    case EndpointTypes.Bulk:
                        ret = LibUsbAPI.usb_bulk_setup_async(mUsbDevice.Handle, ref transferContext.mContext, EpNum);
                        break;
                    case EndpointTypes.Interrupt:
                        ret = LibUsbAPI.usb_interrupt_setup_async(mUsbDevice.Handle, ref transferContext.mContext, EpNum);
                        break;
                    case EndpointTypes.Isochronous:
                        ret = LibUsbAPI.usb_isochronous_setup_async(mUsbDevice.Handle, ref transferContext.mContext, EpNum, mPacketSize);
                        break;
                }
                if (ret < 0 || !transferContext.mContext.IsValid)
                    UsbGlobals.Error(this, UsbGlobals.LastError, "setupAsync", ret);

                return ret;
            }
        }

        internal int submitAsync(TransferContext transferContext)
        {
            int ret = -1;

            lock (oLockTransferContext)
                ret = LibUsbAPI.usb_submit_async(transferContext.mContext, transferContext.PtrBuf, transferContext.RequestCount);

            if (ret < 0)
                UsbGlobals.Error(this, UsbGlobals.LastError, "submitAsync", ret);
            else
            {
                lock (oLockTransferContext)
                    transferContext.mbAsyncCancelled = false;
            }

            return ret;
        }

        internal int transferSync(TransferContext transferContext)
        {
            int ret;

            ret = setupAsync(transferContext);

            if (ret < 0)
            {
                return ret;
            }
            bool bContinue = true;
            while (bContinue)
            {
                ret = submitAsync(transferContext);

                if (ret < 0)
                {
                    freeAsync(transferContext);
                    return ret;
                }

                ret = reapAsync(transferContext);

                if (ret < 0)
                {
                    freeAsync(transferContext);
                    return ret;
                }

                lock (oLockTransferContext)
                    bContinue = ((transferContext.mCurrentRemaining > 0) && (ret == transferContext.mRequested));
            }

            freeAsync(transferContext);

            return transferContext.currentTransmitted;
        }

        #region PRIVATE METHODS

        /// <summary>
        /// All functions that call this function must first lock the <see cref="oLockTransferContext"/> object to be thread safe.
        /// </summary>
        /// <param name="transferContext"></param>
        /// <returns></returns>
        private int cancelAsync_NL(TransferContext transferContext)
        {
            int ret = 0;
            if (transferContext.mbAsyncCancelled) return ret;

            transferContext.mbAsyncCancelled = true;
            ret = LibUsbAPI.usb_cancel_async(transferContext.mContext);

            if (ret < 0)
                UsbGlobals.Error(this, UsbGlobals.LastError, "cancelAsync", ret);
            return ret;
        }

        #endregion
    }
}