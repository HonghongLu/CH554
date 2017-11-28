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
using LibUsbDotNet.DeviceNotify.Internal;
using LibUsbDotNet.Usb.Main;

namespace LibUsbDotNet.DeviceNotify.Info
{
    /// <summary>
    /// Describes the USB device that caused the notification.
    /// </summary>
    public class UsbDeviceNotifyInfo
    {
        private readonly DEV_BROADCAST_DEVICEINTERFACE mBaseHdr = new DEV_BROADCAST_DEVICEINTERFACE();
        private string dbcc_name;
        private UsbSymbolicName mSymbolicName;

        internal UsbDeviceNotifyInfo(IntPtr LParam)
        {
            Marshal.PtrToStructure(LParam, mBaseHdr);
            IntPtr pName = new IntPtr(LParam.ToInt64() + Marshal.OffsetOf(typeof (DEV_BROADCAST_DEVICEINTERFACE), "nameHolder").ToInt64());
            dbcc_name = Marshal.PtrToStringAuto(pName);
        }

        /// <summary>
        /// The symbolc name class for this device.  For more information, see <see cref="UsbSymbolicName"/>.
        /// </summary>
        public UsbSymbolicName SymbolicName
        {
            get
            {
                if (ReferenceEquals(mSymbolicName, null))
                    mSymbolicName = new UsbSymbolicName(dbcc_name);

                return mSymbolicName;
            }
        }

        /// <summary>
        /// Gets the full name of the USB device that caused the notification.
        /// </summary>
        public string Name
        {
            get { return dbcc_name; }
        }

        /// <summary>
        /// Gets the Class Guid of the USB device that caused the notification.
        /// </summary>
        public Guid ClassGuid
        {
            get { return SymbolicName.ClassGuid; }
        }

        /// <summary>
        /// Parses and returns the VID from the <see cref="Name"/> property.
        /// </summary>
        public short IdVendor
        {
            get { return SymbolicName.Vid; }
        }

        /// <summary>
        /// Parses and returns the PID from the <see cref="Name"/> property.
        /// </summary>
        public short IdProduct
        {
            get { return SymbolicName.Pid; }
        }

        /// <summary>
        /// Parses and returns the serial number from the <see cref="Name"/> property.
        /// </summary>
        public string SerialNumber
        {
            get { return SymbolicName.SerialNumber; }
        }

        #region PUBLIC METHODS

        ///<summary>
        ///Returns a <see cref="T:System.String"></see> that represents the current <see cref="UsbDeviceNotifyInfo"></see>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.String"></see> that represents the current <see cref="UsbDeviceNotifyInfo"></see>.
        ///</returns>
        public override string ToString()
        {
            return SymbolicName.ToString();
        }

        #endregion
    }
}