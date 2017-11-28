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

namespace LibUsbDotNet.Usb.Main
{
    /// <summary>
    /// Endpoint list.
    /// </summary>
    public class UsbEndpointList : IEnumerable<UsbEndpointBase>
    {
        private readonly UsbDevice mUsbDevice;
        private List<UsbEndpointBase> mList = new List<UsbEndpointBase>();

        internal UsbEndpointList(UsbDevice usbDevice)
        {
            mUsbDevice = usbDevice;
        }

        /// <summary>
        /// Gets the <see cref="UsbEndpointBase"></see> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item.</param>
        /// <returns>The <see cref="UsbEndpointBase"></see> item at the specified index.</returns>
        ///<exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="UsbEndpointList"></see>.</exception>
        public UsbEndpointBase this[int index]
        {
            get { return mList[index]; }
        }

        ///<summary>
        ///Gets the number of elements contained in the <see cref="UsbEndpointList"></see>.
        ///</summary>
        ///
        ///<returns>
        ///The number of elements contained in the <see cref="UsbEndpointList"></see>.
        ///</returns>
        ///
        public int Count
        {
            get { return mList.Count; }
        }

        #region IEnumerable<UsbEndpointBase> Members

        ///<summary>
        ///Returns <see cref="UsbEndpointBase"></see> enumerator that iterates through the collection.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="UsbEndpointBase"></see> enumerator that can be used to iterate through the collection.
        ///</returns>
        public IEnumerator<UsbEndpointBase> GetEnumerator()
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
        ///Removes all items from the <see cref="UsbEndpointList"></see>.
        ///</summary>
        public void Clear()
        {
            while (mList.Count > 0)
                Remove(mList[0]);
        }

        ///<summary>
        ///Determines whether the <see cref="UsbEndpointList"></see> contains a specific value.
        ///</summary>
        ///
        ///<returns>
        ///true if item is found in the <see cref="UsbEndpointList"></see>; otherwise, false.
        ///</returns>
        ///
        ///<param name="item">The <see cref="UsbEndpointBase"></see> to locate in the <see cref="UsbEndpointList"></see>.</param>
        public bool Contains(UsbEndpointBase item)
        {
            return mList.Contains(item);
        }


        ///<summary>
        ///Determines the index of a specific <see cref="UsbEndpointBase"></see> in the <see cref="UsbEndpointList"></see>.
        ///</summary>
        ///
        ///<returns>
        ///The index of item if found in the list; otherwise, -1.
        ///</returns>
        ///
        ///<param name="item">The <see cref="UsbEndpointBase"></see> to locate in the <see cref="UsbEndpointList"></see>.</param>
        public int IndexOf(UsbEndpointBase item)
        {
            return mList.IndexOf(item);
        }

        ///<summary>
        ///Removes the specified <see cref="UsbEndpointBase"></see> in the <see cref="UsbEndpointList"></see>.
        ///</summary>
        ///
        ///<param name="item">The <see cref="UsbEndpointBase"></see> to remove in the <see cref="UsbEndpointList"></see>.</param>
        public void Remove(UsbEndpointBase item)
        {
            item.Dispose();
        }

        ///<summary>
        ///Removes the <see cref="UsbEndpointList"></see> item at the specified index.
        ///</summary>
        ///
        ///<param name="index">The zero-based index of the item to remove.</param>
        ///<exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="UsbEndpointList"></see>.</exception>
        public void RemoveAt(int index)
        {
            UsbEndpointBase item = mList[index];
            Remove(item);
        }

        #endregion

        internal UsbEndpointBase Add(UsbEndpointBase item)
        {
            foreach (UsbEndpointBase endpoint in mList)
            {
                if (endpoint.EpNum == item.EpNum)
                    return endpoint;
            }
            mList.Add(item);
            return item;
        }

        internal bool removeFromList(UsbEndpointBase item)
        {
            return mList.Remove(item);
        }
    }
}