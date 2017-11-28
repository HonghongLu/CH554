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
using Microsoft.Win32.SafeHandles;

namespace LibUsbDotNet.DeviceNotify.Internal
{
    /// <summary>
    /// Simple classes like this are what .NET application reliable.
    /// This virtually guarantees that any notify handle that gets registered will get unregistered.
    /// </summary>
    internal class SafeNotifyHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeNotifyHandle()
            : base(true)
        {
        }

        public SafeNotifyHandle(IntPtr pHandle)
            : base(true)
        {
            SetHandle(pHandle);
        }

        protected override bool ReleaseHandle()
        {
            if (handle != IntPtr.Zero)
            {
                bool bSuccess = DeviceNotifier.UnregisterDeviceNotification(handle);
                handle = IntPtr.Zero;
                return bSuccess;
            }
            return false;
        }
    }
}