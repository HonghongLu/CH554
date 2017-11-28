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

namespace LibUsbDotNet.Usb.Internal.API
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal class LibUsbDevice : IEquatable<LibUsbDevice>
    {
        private IntPtr mpNextDevice = IntPtr.Zero;
        private IntPtr mpPrevDevice = IntPtr.Zero;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LibUsbConst.LIBUSB_PATH_MAX)]
        public string mFilename = String.Empty;

        private IntPtr mpBus = IntPtr.Zero;

        public LibUsb_DeviceDescriptor mDeviceDescriptor = new LibUsb_DeviceDescriptor();
        private IntPtr mpConfig = IntPtr.Zero;

        private LibUsb_DevHandle mDevHandle; // Darwin support
        public byte mDevNum;
        public byte mNumChildren;
        private IntPtr mpChildren;


        public LibUsbDevice NextDevice
        {
            get
            {
                if (mpNextDevice == IntPtr.Zero) return null;

                LibUsbDevice rtnDev = new LibUsbDevice();
                Marshal.PtrToStructure(mpNextDevice, rtnDev);
                return rtnDev;
            }
        }

        public LibUsbDevice PrevDevice
        {
            get
            {
                if (mpPrevDevice == IntPtr.Zero) return null;

                LibUsbDevice rtnDev = new LibUsbDevice();
                Marshal.PtrToStructure(mpPrevDevice, rtnDev);
                return rtnDev;
            }
        }

        public LibUsbBus DeviceBusLocation
        {
            get
            {
                if (mpBus == IntPtr.Zero) return null;

                LibUsbBus rtnBus = new LibUsbBus();
                Marshal.PtrToStructure(mpBus, rtnBus);
                return rtnBus;
            }
        }

        #region IEquatable<LibUsbDevice> Members

        public bool Equals(LibUsbDevice libUsbDevice)
        {
            if (libUsbDevice == null) return false;
            if (!Equals(mpNextDevice, libUsbDevice.mpNextDevice)) return false;
            if (!Equals(mDeviceDescriptor, libUsbDevice.mDeviceDescriptor)) return false;
            if (!Equals(mpConfig, libUsbDevice.mpConfig)) return false;
            return true;
        }

        #endregion

        #region PUBLIC METHODS

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as LibUsbDevice);
        }

        public LibUsb_ConfigDescriptor GetConfig(int configIndex)
        {
            if (mpConfig == IntPtr.Zero) return new LibUsb_ConfigDescriptor();


            IntPtr configPtr = new IntPtr(mpConfig.ToInt64() + configIndex*Marshal.SizeOf(typeof (LibUsb_ConfigDescriptor)));
            return (LibUsb_ConfigDescriptor) Marshal.PtrToStructure(configPtr, typeof (LibUsb_ConfigDescriptor));
        }

        public override int GetHashCode()
        {
            int result = mpNextDevice.GetHashCode();
            result = 29*result + mDeviceDescriptor.GetHashCode();
            result = 29*result + mpConfig.GetHashCode();
            return result;
        }

        #endregion

        public static bool operator !=(LibUsbDevice libUsbDevice1, LibUsbDevice libUsbDevice2)
        {
            return !Equals(libUsbDevice1, libUsbDevice2);
        }

        public static bool operator ==(LibUsbDevice libUsbDevice1, LibUsbDevice libUsbDevice2)
        {
            return Equals(libUsbDevice1, libUsbDevice2);
        }
    } ;
}