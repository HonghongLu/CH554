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
namespace LibUsbDotNet.Usb.Internal
{
    internal static class LibUsbConst
    {
        internal const string LIBUSB_BUS_NAME = "bus-0";
        internal const int LIBUSB_DEFAULT_TIMEOUT = 5000;
        internal const string LIBUSB_DEVICE_NAME = "\\\\.\\libusb0-";

        #region MAXIMUMS

        internal const int LIBUSB_MAX_DEVICES = 256;
        internal const int LIBUSB_MAX_ENDPOINTS = 32;
        internal const int LIBUSB_MAX_READ_WRITE = 65536;
        internal const int LIBUSB_PATH_MAX = 512;

        #endregion

        #region DESCRIPTOR TYPES

        internal const int USB_DT_HID = 0x21;
        internal const int USB_DT_HUB = 0x29;
        internal const int USB_DT_INTERFACE = 0x04;
        internal const int USB_DT_PHYSICAL = 0x23;
        internal const int USB_DT_REPORT = 0x22;
        internal const int USB_DT_STRING = 0x03;

        #endregion

        #region DESCRIPTOR SIZES

        internal const int USB_DT_ENDPOINT_AUDIO_SIZE = 9; /* Audio extension */
        internal const int USB_DT_HUB_NONVAR_SIZE = 7;
        internal const int USB_DT_INTERFACE_SIZE = 9;

        #endregion

        #region ENDPOINT DIRECTION

        internal const int USB_ENDPOINT_IN = 0x80;
        internal const int USB_ENDPOINT_OUT = 0x00;

        #endregion

        #region STANDARD RECIPIENTS

        internal const int USB_RECIP_DEVICE = 0x00;
        internal const int USB_RECIP_ENDPOINT = 0x02;
        internal const int USB_RECIP_INTERFACE = 0x01;
        internal const int USB_RECIP_OTHER = 0x03;

        #endregion

        #region STANDARD REQUESTS

        internal const int USB_REQ_CLEAR_FEATURE = 0x01;
        internal const int USB_REQ_GET_CONFIGURATION = 0x08;
        internal const int USB_REQ_GET_DESCRIPTOR = 0x06;
        internal const int USB_REQ_GET_INTERFACE = 0x0A;
        internal const int USB_REQ_GET_STATUS = 0x00;
        internal const int USB_REQ_SET_ADDRESS = 0x05;
        internal const int USB_REQ_SET_CONFIGURATION = 0x09;
        internal const int USB_REQ_SET_DESCRIPTOR = 0x07;
        internal const int USB_REQ_SET_FEATURE = 0x03;
        internal const int USB_REQ_SET_INTERFACE = 0x0B;
        internal const int USB_REQ_SYNCH_FRAME = 0x0C;

        #endregion

        #region STANDARD REQUEST TYPES

        internal const int USB_TYPE_CLASS = (0x01 << 5);
        internal const int USB_TYPE_RESERVED = (0x03 << 5);
        internal const int USB_TYPE_STANDARD = (0x00 << 5);
        internal const int USB_TYPE_VENDOR = (0x02 << 5);

        #endregion
    }
}