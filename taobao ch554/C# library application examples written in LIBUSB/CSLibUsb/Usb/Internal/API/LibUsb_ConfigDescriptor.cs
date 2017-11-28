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
    internal struct LibUsb_ConfigDescriptor
    {
        public byte bLength;
        public byte bDescriptorType;
        public ushort wTotalLength;
        public byte bNumInterfaces;
        public byte bConfigurationValue;
        public byte iConfiguration;
        public byte bmAttributes;
        public byte MaxPower;

        public IntPtr iface;

        public IntPtr extra; /* Extra descriptors */
        public int extralen;

        #region PUBLIC METHODS

        public LibUsb_InterfaceDescriptor GetInterface(int iFace)
        {
            if (iface == IntPtr.Zero)
            {
                return new LibUsb_InterfaceDescriptor();
            }
            IntPtr SelIFace = new IntPtr(iface.ToInt64() + (iFace*Marshal.SizeOf(typeof (LibUsb_InterfaceDescriptor))));
            return (LibUsb_InterfaceDescriptor) Marshal.PtrToStructure(SelIFace, typeof (LibUsb_InterfaceDescriptor));
        }

        #endregion
    } ;
}