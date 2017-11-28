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
using LibUsbDotNet.DeviceNotify.Internal;

namespace LibUsbDotNet.DeviceNotify.Info
{
    /// <summary>
    /// Notify information for a communication port
    /// </summary>
    public class PortNotifyInfo
    {
        private DEV_BROADCAST_PORT mBaseHdr = new DEV_BROADCAST_PORT();
        private string dbcp_name;

        internal PortNotifyInfo(IntPtr LParam)
        {
            Marshal.PtrToStructure(LParam, mBaseHdr);
            IntPtr pName = new IntPtr(LParam.ToInt64() + Marshal.OffsetOf(typeof (DEV_BROADCAST_PORT), "nameHolder").ToInt64());
            dbcp_name = Marshal.PtrToStringAuto(pName);
        }

        /// <summary>
        /// Gets the name of the port that caused the event.
        /// </summary>
        public string Name
        {
            get { return dbcp_name; }
        }

        #region PUBLIC METHODS

        ///<summary>
        ///Returns a <see cref="T:System.String"></see> that represents the current <see cref="PortNotifyInfo"></see>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.String"></see> that represents the current <see cref="PortNotifyInfo"></see>.
        ///</returns>
        public override string ToString()
        {
            return string.Format("[Port Name:{0}] ", Name);
        }

        #endregion
    }
}