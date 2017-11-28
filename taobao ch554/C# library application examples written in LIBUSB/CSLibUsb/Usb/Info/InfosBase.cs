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
using System.Collections.Generic;

namespace LibUsbDotNet.Usb.Info
{
    ///<summary><span class="cImportant">This class is not meant to be used directly.</span> <br/> This is a templated List class used as a base in the following classes:<br/>
    ///<see cref="InfoDevices"/>,
    ///<see cref="InfoEndpoints"/>,
    ///<see cref="InfoInterfaces"/>,
    ///<see cref="InfoConfigs"/>
    ///
    ///</summary>
    public class InfosBase<T>
    {
        // Fields
        private List<T> mList;

        // Methods
        internal InfosBase()
        {
            mList = new List<T>();
        }

        ///<summary>Gets the number of elements actually contained in the ArrayList.</summary>
        ///<returns>The number of elements actually contained in the ArrayList.</returns>
        public int Count
        {
            get { return mList.Count; }
        }

        ///<summary>Gets or sets the element at the specified index.</summary>
        ///<returns>The element at the specified index.</returns>
        ///<param name="index">The zero-based index of the element to get or set.</param>
        public T this[int index]
        {
            get { return mList[index]; }
        }

        #region PUBLIC METHODS

        ///<summary>Determines whether an element is in list.</summary>
        ///<returns>true if item is found; otherwise, false.</returns>
        ///<param name="item">The object to locate in the list.</param>
        public virtual bool Contains(T item)
        {
            return mList.Contains(item);
        }

        ///<summary>Returns an enumerator that iterates through the list.</summary>
        ///<returns>An Enumerator class for the list.</returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>) mList.GetEnumerator();
        }

        ///<summary>Determines the index of a specific item..</summary>
        ///<returns>The index of item if found in the list; otherwise, -1.</returns>
        ///<param name="item">The object to locate.</param>
        public virtual int IndexOf(T item)
        {
            return mList.IndexOf(item);
        }

        #endregion

        internal virtual void Add(T item)
        {
            mList.Add(item);
        }

        internal virtual void mClear()
        {
            mList.Clear();
        }

        internal virtual void mRemove(T item)
        {
            mList.Remove(item);
        }

        internal virtual void mRemoveAt(int index)
        {
            mList.RemoveAt(index);
        }

        // Properties
    }
}