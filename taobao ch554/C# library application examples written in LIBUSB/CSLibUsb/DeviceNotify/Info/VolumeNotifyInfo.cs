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
    /// Describes the storage volume that caused the notification.
    /// </summary>
    public class VolumeNotifyInfo
    {
        private const int DBTF_MEDIA = 0x0001;
        private const int DBTF_NET = 0x0002;

        private DEV_BROADCAST_VOLUME mBaseHdr = new DEV_BROADCAST_VOLUME();

        internal VolumeNotifyInfo(IntPtr LParam)
        {
            Marshal.PtrToStructure(LParam, mBaseHdr);
        }

        /// <summary>
        /// Gets the letter representation of the unitmask.
        /// </summary>
        public char Letter
        {
            get
            {
                Int32 tempMask = Unitmask;
                for (byte b = 65; b < (65 + 32); b++)
                {
                    Byte bValue = b;
                    if (bValue > 90)
                        bValue -= 43;
                    if ((tempMask & 0x1) == 1)
                        return (char) bValue;

                    tempMask >>= 1;
                }

                return (char) 63;
            }
        }

        ///<summary>
        /// If true, change affects media in drive. If false, change affects physical device or drive.
        ///</summary>
        public bool ChangeAffectsMediaInDrive
        {
            get { return ((Flags & DBTF_MEDIA) == DBTF_MEDIA); }
        }

        /// <summary>
        /// If True, the indicated logical volume is a network volume
        /// </summary>
        public bool IsNetworkVolume
        {
            get { return ((Flags & DBTF_NET) == DBTF_NET); }
        }

        /// <summary>
        /// Raw DEV_BROADCAST_VOLUME flags.
        /// </summary>
        public short Flags
        {
            get { return mBaseHdr.dbcv_flags; }
        }

        /// <summary>
        /// Gets the bit unit mask of the device. IE (bit 0 = A:, bit 1 = B:, etc..)
        /// </summary>
        public int Unitmask
        {
            get { return mBaseHdr.dbcv_unitmask; }
        }

        #region PUBLIC METHODS

        ///<summary>
        ///Returns a <see cref="T:System.String"></see> that represents the current <see cref="VolumeNotifyInfo"></see>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.String"></see> that represents the current <see cref="VolumeNotifyInfo"></see>.
        ///</returns>
        public override string ToString()
        {
            return string.Format("[Letter:{0}] [IsNetworkVolume:{1}] [ChangeAffectsMediaInDrive:{2}] ", Letter, IsNetworkVolume, ChangeAffectsMediaInDrive);
        }

        #endregion
    }
}