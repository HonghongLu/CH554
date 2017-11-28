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

namespace LibUsbDotNet.Usb.Main
{
    /// <summary>
    /// Endpoint data received arguments returned by the <see cref="UsbEndpointReader.DataReceived"/> event.
    /// </summary>
    public class DataReceivedArgs : EventArgs
    {
        private readonly byte[] bytesReceived;
        private readonly int count;

        internal DataReceivedArgs(byte[] Bytes, int Size)
        {
            bytesReceived = Bytes;
            count = Size;
        }

        /// <summary>
        /// Gets the buffer of the received data.
        /// </summary>
        /// <remarks>
        /// Use the <see cref="DataReceivedArgs.Count"/> property to determine the number of bytes actually received.
        /// </remarks>
        public byte[] Buffer
        {
            get { return bytesReceived; }
        }

        /// <summary>
        /// Gets the number of bytes received.
        /// </summary>
        public int Count
        {
            get { return count; }
        }
    }
}