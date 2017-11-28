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
using System.Runtime.InteropServices;
using LibUsbDotNet.Usb.Internal.API;

namespace LibUsbDotNet.Usb.Internal
{
    internal class TransferContext
    {
        internal readonly UsbEndpointBase mEndpointBase;
        internal int mOriginalOffset;
        internal int mOriginalCount;
        internal int mTimeout;
        internal bool mbCancelIoOnTimeout;

        private byte[] mBuffer;
        private GCHandle mgcBuffer;

        internal LibUsb_Context mContext;
        internal int mCurrentOffset;
        internal int mCurrentRemaining;
        internal int currentTransmitted;
        internal int mRequested;
        internal bool mbAsyncCancelled = true;

        public TransferContext(UsbEndpointBase usbEndpointBase)
        {
            mEndpointBase = usbEndpointBase;
        }

        internal byte[] Buffer
        {
            get { return mBuffer; }
        }

        internal IntPtr PtrBuf
        {
            get { return Marshal.UnsafeAddrOfPinnedArrayElement(mBuffer, mCurrentOffset); }
        }

        internal int RequestCount
        {
            get
            {
                mRequested = mCurrentRemaining > LibUsbConst.LIBUSB_MAX_READ_WRITE ? LibUsbConst.LIBUSB_MAX_READ_WRITE : mCurrentRemaining;
                return mRequested;
            }
        }

        internal void IncrementTransfer(int amount)
        {
            currentTransmitted += amount;
            mCurrentOffset += amount;
            mCurrentRemaining -= amount;
        }

        internal void Reset()
        {
            mCurrentOffset = mOriginalOffset;
            mCurrentRemaining = mOriginalCount;
            mRequested = 0;
            currentTransmitted = 0;
        }

        internal void Setup(byte[] buffer, int offset, int count, int timeout, bool bCancelIoOnTimeout)
        {
            mBuffer = buffer;
            mOriginalOffset = offset;
            mOriginalCount = count;
            mTimeout = timeout;
            mbCancelIoOnTimeout = bCancelIoOnTimeout;
            mgcBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            mCurrentOffset = offset;
            mCurrentRemaining = count;
            mbAsyncCancelled = true;
            mRequested = 0;
        }
    }
}