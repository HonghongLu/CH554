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
using System.Windows.Forms;
using LibUsbDotNet.DeviceNotify.Internal;

namespace LibUsbDotNet.DeviceNotify
{
    /// <summary>
    /// Main class for managing system device notification events.
    /// </summary>
    /// <remarks>
    /// Use this class to detect when plug-n-play devices are inserted and removed from the PC.
    /// <code>
    /// Device notifier output to debug window.
    /// 
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Diagnostics;
    /// using System.Windows.Forms;
    /// using LibUsbDotNet.DeviceNotify;
    /// using LibUsbDotNet.Usb;
    /// using LibUsbDotNet.Usb.Main;
    /// namespace Test_DeviceNotify
    /// {
    ///     public partial class Form1 : Form
    ///     {
    ///         private DeviceNotifier mDevNotifier;
    /// 
    ///         public Form1()
    ///         {
    ///            // General Form1 initializaion
    /// 	  // ...
    /// 		
    /// 		// Create  a device notifier instance.
    ///             mDevNotifier = new DeviceNotifier();
    /// 
    /// 		// Add a handler to the OnDeviceNotify event.
    ///             mDevNotifier.OnDeviceNotify += mDevNotifier_OnDeviceNotify;
    ///         }
    ///         private void mDevNotifier_OnDeviceNotify(object sender, DeviceNotifyEventArgs e)
    ///         {
    ///             Debug.Print(e.Object.ToString());
    ///         }
    /// 
    ///            // General Form1 members.
    /// 	  // ...
    /// 
    /// 	}
    /// }
    /// </code>
    /// </remarks>
    public class DeviceNotifier
    {
        /// <summary>
        /// Main Notify event for all device notifications.
        /// </summary>
        public event EventHandler<DeviceNotifyEventArgs> OnDeviceNotify;

        private DevNotifyNativeWindow mNotifyWindow;
        private DEV_BROADCAST_DEVICEINTERFACE mDevInterface = new DEV_BROADCAST_DEVICEINTERFACE(new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED"));

        private SafeNotifyHandle mDevInterfaceHandle = null;

        ///<summary>
        /// Creates an instance of the <see cref="DeviceNotifier"/> class.
        ///</summary>
        public DeviceNotifier()
        {
            mNotifyWindow = new DevNotifyNativeWindow(OnHandleChange, OnDeviceChange);
        }

        #region DLLIMPORT EXTERN METHODS

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "RegisterDeviceNotificationA", CharSet = CharSet.Ansi)]
        private static extern SafeNotifyHandle RegisterDeviceNotification(IntPtr hRecipient, [MarshalAs(UnmanagedType.AsAny), In] object NotificationFilter, int Flags);

        #endregion

        internal bool registerDeviceInterface(IntPtr windowHandle)
        {
            if (mDevInterfaceHandle != null)
            {
                mDevInterfaceHandle.Dispose();
                mDevInterfaceHandle = null;
            }
            if (windowHandle != IntPtr.Zero)
            {
                mDevInterfaceHandle = RegisterDeviceNotification(windowHandle, mDevInterface, 0);
                if (mDevInterfaceHandle != null && !mDevInterfaceHandle.IsInvalid)
                    return true;
                else
                {
                    return false;
                }
            }
            return false;
        }

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool UnregisterDeviceNotification(IntPtr Handle);

        #region PRIVATE METHODS

        private void OnDeviceChange(ref Message m)
        {
            if (m.LParam.ToInt32() != 0)
            {
                EventHandler<DeviceNotifyEventArgs> temp = OnDeviceNotify;
                if (!ReferenceEquals(temp, null))
                {
                    DeviceNotifyEventArgs args;
                    DEV_BROADCAST_HDR hdr = new DEV_BROADCAST_HDR();
                    Marshal.PtrToStructure(m.LParam, hdr);
                    switch (hdr.dbch_DeviceType)
                    {
                        case DeviceType.PORT:
                        case DeviceType.VOLUME:
                        case DeviceType.DEVICEINTERFACE:
                            args = new DeviceNotifyEventArgs(hdr, m.LParam, (EventType) m.WParam.ToInt32());
                            break;
                        default:
                            args = null;
                            break;
                    }

                    if (!ReferenceEquals(args, null)) temp(this, args);
                }
            }
        }

        private void OnHandleChange(IntPtr NewWindowHandle)
        {
            bool bSuccess = registerDeviceInterface(NewWindowHandle);
            Debug.Print("registerDeviceInterface:" + bSuccess.ToString());
        }

        #endregion

        ///<summary>
        ///Releases the resources associated with this window. 
        ///</summary>
        ///
        ~DeviceNotifier()
        {
            if (mNotifyWindow != null) mNotifyWindow.DestroyHandle();
            mNotifyWindow = null;

            if (mDevInterfaceHandle != null) mDevInterfaceHandle.Dispose();
            mDevInterfaceHandle = null;
        }
    }
}