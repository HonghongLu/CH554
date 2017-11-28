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
using System.Collections;
using System.Collections.Generic;
using LibUsbDotNet.Usb.Internal.API;

namespace LibUsbDotNet.Usb.Main
{
    /// <summary>
    /// Device list.
    /// </summary>
    public class UsbDeviceList : IEnumerable<UsbDevice>
    {
        private List<UsbDevice> mList = new List<UsbDevice>();

        /// <summary>
        /// Gets the <see cref="UsbDevice"></see> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item.</param>
        /// <returns>The <see cref="UsbDevice"></see> item at the specified index.</returns>
        ///<exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="UsbDeviceList"></see>.</exception>
        public UsbDevice this[int index]
        {
            get { return mList[index]; }
        }

        ///<summary>
        ///Gets the number of elements contained in the <see cref="UsbDeviceList"></see>.
        ///</summary>
        ///
        ///<returns>
        ///The number of elements contained in the <see cref="UsbDeviceList"></see>.
        ///</returns>
        ///
        public int Count
        {
            get { return mList.Count; }
        }

        #region IEnumerable<UsbDevice> Members

        ///<summary>
        ///Returns <see cref="UsbDevice"></see> enumerator that iterates through the collection.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="UsbDevice"></see> enumerator that can be used to iterate through the collection.
        ///</returns>
        public IEnumerator<UsbDevice> GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        ///<summary>
        ///Returns an enumerator that iterates through a collection.
        ///</summary>
        ///
        ///<returns>
        ///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        #endregion

        #region PUBLIC METHODS

        ///<summary>
        ///Determines whether the <see cref="UsbDeviceList"></see> contains a specific value.
        ///</summary>
        ///
        ///<returns>
        ///true if item is found in the <see cref="UsbDeviceList"></see>; otherwise, false.
        ///</returns>
        ///
        ///<param name="item">The <see cref="UsbDevice"></see> to locate in the <see cref="UsbDeviceList"></see>.</param>
        public bool Contains(UsbDevice item)
        {
            return mList.Contains(item);
        }


        ///<summary>
        ///Determines the index of a specific <see cref="UsbDevice"></see> in the <see cref="UsbDeviceList"></see>.
        ///</summary>
        ///
        ///<returns>
        ///The index of item if found in the list; otherwise, -1.
        ///</returns>
        ///
        ///<param name="item">The <see cref="UsbDevice"></see> to locate in the <see cref="UsbDeviceList"></see>.</param>
        public int IndexOf(UsbDevice item)
        {
            return mList.IndexOf(item);
        }

        #endregion

        internal UsbDevice Add(LibUsbDevice item)
        {
            UsbDevice newDevice = new UsbDevice(item);
            newDevice.mbFoundDevice = true;
            mList.Add(newDevice);
            return newDevice;
        }

        internal void Clear()
        {
            while (mList.Count > 0)
                Remove(mList[0]);
        }

        internal int FindAndSet(LibUsbDevice item)
        {
            for (int i = 0; i < mList.Count; i++)
            {
                UsbDevice device = mList[i];
                if (device.InternalLibUsbDevice == item)
                {
                    device.mbFoundDevice = true;
                    return i;
                }
            }
            return -1;
        }

        internal void Remove(UsbDevice item)
        {
            mList.Remove(item);
        }

        internal void RemoveAbandoned()
        {
            for (int i = 0; i < mList.Count; i++)
            {
                UsbDevice device = mList[i];
                if (device.mbFoundDevice == false)
                {
                    device.Close();
                    mList.RemoveAt(i);
                    i--;
                }
            }
        }

        internal void RemoveAt(int index)
        {
            UsbDevice item = mList[index];
            Remove(item);
        }

        internal void SetAllAbandoned()
        {
            foreach (UsbDevice device in mList)
                device.mbFoundDevice = false;
        }
    }
}