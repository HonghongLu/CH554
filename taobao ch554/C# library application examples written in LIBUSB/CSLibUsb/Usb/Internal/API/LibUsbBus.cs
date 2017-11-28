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
    internal class LibUsbBus
    {
        private IntPtr mpNextBus = IntPtr.Zero;
        private IntPtr mpPrevBus = IntPtr.Zero;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LibUsbConst.LIBUSB_PATH_MAX)]
        public string mDirName = String.Empty;

        private IntPtr mpRootDevice = IntPtr.Zero;
        public uint mLocation = 0;

        public LibUsbBus NextBus
        {
            get
            {
                if (mpNextBus == IntPtr.Zero) return null;

                LibUsbBus rtnBus = new LibUsbBus();
                Marshal.PtrToStructure(mpNextBus, rtnBus);
                return rtnBus;
            }
        }

        public LibUsbBus PrevBus
        {
            get
            {
                if (mpPrevBus == IntPtr.Zero) return null;

                LibUsbBus rtnBus = new LibUsbBus();
                Marshal.PtrToStructure(mpPrevBus, rtnBus);
                return rtnBus;
            }
        }

        public LibUsbDevice RootDevice
        {
            get
            {
                if (mpRootDevice == IntPtr.Zero)
                {
                    return null;
                }
                LibUsbDevice rtnDev = new LibUsbDevice();
                Marshal.PtrToStructure(mpRootDevice, rtnDev);
                return rtnDev;
            }
        }
    } ;
}