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
using LibUsbDotNet.Usb.Internal;
using LibUsbDotNet.Usb.Main;

namespace LibUsbDotNet.Usb
{
    /// <summary>
    /// Main class used to send data to a USB device.
    /// </summary>
    public class UsbEndpointWriter : UsbEndpointBase, IDisposable
    {
        /// <summary>
        /// Default write mTimeout.
        /// </summary>
        public const int DEF_WRITE_TIMEOUT = 5000;

        private WriteEndpoints mWriteEndpoint = WriteEndpoints.Ep01;

        internal UsbEndpointWriter(UsbDevice usbDevice, EndpointTypes epType, int packetSize, WriteEndpoints mWriteEndpoint)
            : base(usbDevice, epType, packetSize)
        {
            this.mWriteEndpoint = mWriteEndpoint;
        }

        /// <summary>
        /// The endpoint number as a <see cref="System.Byte"/>.
        /// </summary>
        public override byte EpNum
        {
            get { return (byte) mWriteEndpoint; }
        }

        /// <summary>
        /// The endpoint ID number that this instance is writing to.
        /// </summary>
        public WriteEndpoints WriteEndpoint
        {
            get { return mWriteEndpoint; }
        }

        #region PUBLIC METHODS

        /// <summary>
        /// Writes data to the current <see cref="UsbEndpointWriter"/>.
        /// </summary>
        /// <param name="buffer">The buffer storing the data to write.</param>
        /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
        /// <returns>
        /// Number of bytes transmitted or less than zero if an error occured.
        /// </returns>
        public int Write(byte[] buffer, int timeout)
        {
            return Write(buffer, 0, buffer.Length, timeout);
        }

        /// <summary>
        /// Writes data to the current <see cref="UsbEndpointWriter"/>.
        /// </summary>
        /// <param name="buffer">The buffer storing the data to write.</param>
        /// <param name="offset">The position in buffer to start writing the data from.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <param name="timeout">Maximum time to wait for the transfer to complete.  If the transfer times out, the IO operation will be cancelled.</param>
        /// <returns>
        /// Number of bytes transmitted or less than zero if an error occured.
        /// </returns>
        public int Write(byte[] buffer, int offset, int count, int timeout)
        {
            return Transfer(buffer, offset, count, timeout);
        }

        #endregion
    }
}