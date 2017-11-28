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
    /// <summary>
    /// Interface descriptor
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct LibUsb_AltInterfaceDescriptor
    {
        public byte bLength;
        public byte bDescriptorType;
        public byte bInterfaceNumber;
        public byte bAlternateSetting;
        public byte bNumEndpoints;
        public byte bInterfaceClass;
        public byte bInterfaceSubClass;
        public byte bInterfaceProtocol;
        public byte iInterface;

        public IntPtr endpoint;

        public IntPtr extra; /* Extra descriptors */
        public int extralen;

        #region PUBLIC METHODS

        public LibUsb_EndpointDescriptor GetEndpoint(int iEndpoint)
        {
            if (endpoint == IntPtr.Zero)
            {
                return new LibUsb_EndpointDescriptor();
            }

            IntPtr SelEndpoint = new IntPtr(endpoint.ToInt64() + (iEndpoint*Marshal.SizeOf(typeof (LibUsb_EndpointDescriptor))));
            return (LibUsb_EndpointDescriptor) Marshal.PtrToStructure(SelEndpoint, typeof (LibUsb_EndpointDescriptor));
        }

        #endregion
    } ;
}