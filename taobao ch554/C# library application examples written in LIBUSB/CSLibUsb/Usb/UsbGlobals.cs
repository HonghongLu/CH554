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
using System.Diagnostics;
using System.Runtime.InteropServices;
using LibUsbDotNet.Usb.Internal.API;
using LibUsbDotNet.Usb.Main;

namespace LibUsbDotNet.Usb
{
    /// <summary>
    /// Global functions and members common to all USB tasks.
    /// </summary>
    public static class UsbGlobals
    {
        /// <summary>
        /// This global event is fired for all usb related errors.
        /// </summary>
        public static event EventHandler<UsbError> OnUsbError;

        private static UsbDeviceList mDeviceList = new UsbDeviceList();
        private static bool mbInit = false;
        private static object oLockDeviceList = new object();

        /// <summary>
        /// Contains a global list of all usb devices that are attached to the PC and available for use. 
        /// </summary>
        /// <remarks>
        /// This is the starting point for all usb operations.  To open and use a USB device you must first find the <see cref="UsbDevice"/> class in this <see cref="DeviceList"/>.
        /// </remarks>
        ///<returns>
        ///A list of <see cref="UsbDevice"></see> classes that represent all usb devices attached to the PC.
        ///</returns>
        public static UsbDeviceList DeviceList
        {
            get
            {
                lock (oLockDeviceList)
                {
                    if (!mbInit)
                    {
                        LibUsbAPI.usb_init();
                        mbInit = true;
                    }

                    LibUsbAPI.usb_find_busses();
                    LibUsbAPI.usb_find_devices();

                    IntPtr pBusses = LibUsbAPI.usb_get_busses();
                    LibUsbBus Bus = new LibUsbBus();
                    Marshal.PtrToStructure(pBusses, Bus);

                    mDeviceList.SetAllAbandoned();
                    while (Bus != null)
                    {
                        LibUsbDevice Dev = Bus.RootDevice;
                        while (Dev != null)
                        {
                            if (mDeviceList.FindAndSet(Dev) == -1)
                                mDeviceList.Add(Dev);
                            Dev = Dev.NextDevice;
                        }
                        Bus = Bus.NextBus;
                    }
                    mDeviceList.RemoveAbandoned();
                    return mDeviceList;
                }
            }
        }

        /// <summary>
        /// Get a string representation of the last error encountered by all usb functions.
        /// </summary>
        public static string LastError
        {
            get { return LibUsbAPI.usb_strerror(); }
        }

        internal static void Error(object sender, string description, string functionName, int ret)
        {
            UsbError e = new UsbError(sender, description,functionName, ret);
            EventHandler<UsbError> temp = OnUsbError;
            if (temp != null)
                temp(sender, e);
            else
                Debug.Print(e.ToString());                
        }
    }
}