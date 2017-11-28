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
using System.Security;
using System.Text;

namespace LibUsbDotNet.Usb.Internal.API
{
    [SuppressUnmanagedCodeSecurity]
    internal class LibUsbAPI
    {
        private const string LIBUSB_DLL = "libusb0.dll";

        private const CallingConvention CC = CallingConvention.Cdecl;

        #region DLLIMPORT EXTERN METHODS

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        public static extern int usb_bulk_setup_async(LibUsb_DevHandle dev, ref LibUsb_Context context, byte ep);


        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        public static extern int usb_cancel_async(LibUsb_Context context);

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        public static extern int usb_claim_interface(LibUsb_DevHandle dev, int interfaceNum);


        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        public static extern int usb_close(LibUsb_DevHandle dev);

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        public static extern int usb_control_msg(LibUsb_DevHandle dev, int requesttype, int request, int value, int index, byte[] bytes, int size, int timeout);


        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        public static extern int usb_find_busses();

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        public static extern int usb_find_devices();

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        public static extern int usb_free_async(ref LibUsb_Context context);

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        public static extern IntPtr usb_get_busses();


        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        public static extern int usb_get_string(LibUsb_DevHandle dev, int index, int langid, StringBuilder buf, int buflen);

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        public static extern int usb_get_string_simple(LibUsb_DevHandle dev, int index, StringBuilder buf, int buflen);

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        public static extern void usb_init();

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        public static extern int usb_interrupt_setup_async(LibUsb_DevHandle dev, ref LibUsb_Context context, byte ep);

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        public static extern int usb_isochronous_setup_async(LibUsb_DevHandle dev, ref LibUsb_Context context, byte ep, int pktsize);

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        public static extern LibUsb_DevHandle usb_open(LibUsbDevice dev);

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        public static extern int usb_reap_async(LibUsb_Context context, int timeout);

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        public static extern int usb_reap_async_nocancel(LibUsb_Context context, int timeout);

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        public static extern int usb_release_interface(LibUsb_DevHandle dev, int interfaceNum);

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        public static extern int usb_set_altinterface(LibUsb_DevHandle dev, int alternate);

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        public static extern int usb_set_configuration(LibUsb_DevHandle dev, int configuration);

        [DllImport(LIBUSB_DLL, CallingConvention = CC)]
        public static extern string usb_strerror();

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        public static extern int usb_submit_async(LibUsb_Context context, Byte[] bytes, int size);

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        public static extern int usb_submit_async(LibUsb_Context context, IntPtr bytes, int size);

        #endregion

        #region UNUSED

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        private static extern int usb_bulk_read(LibUsb_DevHandle dev, int ep, byte[] bytes, int size, int timeout);

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        private static extern int usb_bulk_write(LibUsb_DevHandle dev, int ep, byte[] bytes, int size, int timeout);

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        private static extern int usb_clear_halt(LibUsb_DevHandle dev, uint ep);

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        private static extern IntPtr usb_device(LibUsb_DevHandle dev);

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        private static extern int usb_get_descriptor(LibUsb_DevHandle udev, byte type, byte index, IntPtr buf, int size);

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        private static extern int usb_get_descriptor_by_endpoint(LibUsb_DevHandle udev, int ep, byte type, byte index, IntPtr buf, int size);

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        private static extern int usb_interrupt_read(LibUsb_DevHandle dev, int ep, byte[] bytes, int size, int timeout);

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        private static extern int usb_interrupt_write(LibUsb_DevHandle dev, int ep, byte[] bytes, int size, int timeout);

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        private static extern int usb_reset(LibUsb_DevHandle dev);

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        private static extern int usb_resetep(LibUsb_DevHandle dev, uint ep);

        [DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = true)]
        private static extern void usb_set_debug(int level);

        #endregion
    }
}