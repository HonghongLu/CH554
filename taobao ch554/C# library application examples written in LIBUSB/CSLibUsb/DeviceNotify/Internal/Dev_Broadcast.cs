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

namespace LibUsbDotNet.DeviceNotify.Internal
{
    [StructLayout(LayoutKind.Sequential)]
    internal class DEV_BROADCAST_HDR
    {
        public int dbch_Size;
        public DeviceType dbch_DeviceType;
        public int dbch_Reserved;

        internal DEV_BROADCAST_HDR()
        {
            dbch_Size = (int) Marshal.SizeOf(this);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class DEV_BROADCAST_VOLUME : DEV_BROADCAST_HDR
    {
        public int dbcv_unitmask;
        public short dbcv_flags;

        public DEV_BROADCAST_VOLUME()
        {
            dbch_Size = (int) Marshal.SizeOf(this);
            dbch_DeviceType = DeviceType.VOLUME;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class DEV_BROADCAST_PORT : DEV_BROADCAST_HDR
    {
        private char nameHolder;

        public DEV_BROADCAST_PORT()
        {
            dbch_Size = (int) Marshal.SizeOf(this);
            dbch_DeviceType = DeviceType.PORT;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class DEV_BROADCAST_DEVICEINTERFACE : DEV_BROADCAST_HDR
    {
        public Guid dbcc_classguid = Guid.Empty;
        private char nameHolder;

        public DEV_BROADCAST_DEVICEINTERFACE()
        {
            dbch_Size = (int) Marshal.SizeOf(this);
            dbch_DeviceType = DeviceType.DEVICEINTERFACE;
        }

        public DEV_BROADCAST_DEVICEINTERFACE(Guid guid)
            : this()
        {
            dbcc_classguid = guid;
        }
    }
}