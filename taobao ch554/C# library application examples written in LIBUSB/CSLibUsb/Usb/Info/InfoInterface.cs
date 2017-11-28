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

namespace LibUsbDotNet.Usb.Info
{
    ///<summary>Array list containing available interfaces in a <see cref="InfosBase{T}"/>.</summary>
    ///<remarks>See the base class <see cref="InfoConfig"/> for a list of available members.</remarks>
    public class InfoInterfaces : InfosBase<InfoInterface>
    {
        // Methods
        internal InfoInterfaces()
        {
        }
    }

    ///<summary>Contains USB interface information</summary>
    public class InfoInterface
    {
        private readonly UsbDevice mUsbDevice;
        // Fields
        internal byte bAlternateSetting;
        internal byte bDescriptorType;
        internal byte bInterfaceClass;
        internal byte bInterfaceNumber;
        internal byte bInterfaceProtocol;
        internal byte bInterfaceSubClass;
        internal byte bLength;
        internal byte bNumEndpoints;
        internal byte iInterface;
        internal InfoEndpoints usbInfoEndpoints;
        private String mInterfaceString;

        // Methods
        internal InfoInterface(UsbDevice usbDevice)
        {
            mUsbDevice = usbDevice;
        }

        ///<summary>Value used to select this alternate setting for this interface.</summary>
        public byte AlternateSetting
        {
            get { return bAlternateSetting; }
        }

        ///<summary>
        /// INTERFACE descriptor type (= 4)
        ///</summary>
        public byte DescriptorType
        {
            get { return bDescriptorType; }
        }

        ///<summary>
        /// Array of available endpoints for this interface
        ///</summary>
        public InfoEndpoints InfoEndpointList
        {
            get { return usbInfoEndpoints; }
        }

        ///<summary>Class code assigned by USB-IF<br/>
        /// 00h is a reserved value <br/>
        /// FFh means vendor-defined class <br/>
        /// Any other value must be a class code 
        ///</summary>
        public byte InterfaceClass
        {
            get { return bInterfaceClass; }
        }

        ///<summary>Index of string descriptor describing interface - set to 0 if no string</summary>
        public byte InterfaceIndex
        {
            get { return iInterface; }
        }

        ///<summary>Number identifying this interface. Zero-based value.</summary>
        public byte InterfaceNumber
        {
            get { return bInterfaceNumber; }
        }

        ///<summary>Protocol Code assigned by USB-IF</summary>
        public byte InterfaceProtocol
        {
            get { return bInterfaceProtocol; }
        }

        ///<summary>String descriptor describing interface</summary>
        public string InterfaceString
        {
            get
            {
                if (ReferenceEquals(mInterfaceString, null))
                {
                    mInterfaceString = "";
                    if (iInterface > 0)
                    {
                        mUsbDevice.GetString(iInterface, ref mInterfaceString);
                    }
                }
                return mInterfaceString;
            }
        }


        ///<summary>SubClass Code assigned by USB-IF</summary>
        public byte InterfaceSubClass
        {
            get { return bInterfaceSubClass; }
        }

        ///<summary>
        /// Size of this descriptor in bytes
        ///</summary>
        public byte Length
        {
            get { return bLength; }
        }

        ///<summary>Number of endpoints used by this interface. Doesn't include control endpoint 0. </summary>
        public byte NumEndpoints
        {
            get { return bNumEndpoints; }
        }

        #region PUBLIC METHODS

        ///<summary>
        /// Overriden: Returns a string representing the current <see cref="InfoInterface"/> class.
        ///</summary>
        public override string ToString()
        {
            object[] args = new object[] {bLength, bDescriptorType, bInterfaceNumber, bAlternateSetting, bNumEndpoints, bInterfaceClass, bInterfaceSubClass, bInterfaceProtocol, iInterface, InterfaceString};
            return string.Format("bLength:{0}\r\nbDescriptorType:0x{1,2:X2}\r\nbInterfaceNumber:{2}\r\nbAlternateSetting:{3}\r\nbNumEndpoints:{4}\r\nbInterfaceClass:0x{5,2:X2}\r\nbInterfaceSubClass:0x{6,2:X2}\r\nbInterfaceProtocol:0x{7,2:X2}\r\niInterface:{8}\r\nsInterface:{9}\r\n", args);
        }

        #endregion
    }
}