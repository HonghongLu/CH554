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
    ///<summary>Array list containing all available configurations <see cref="InfoDevice"/>.</summary>
    ///<remarks>See the base class <see cref="InfosBase{T}"/> for a list of available members.</remarks>
    public class InfoConfigs : InfosBase<InfoConfig>
    {
        // Methods
        internal InfoConfigs()
        {
        }
    }

    ///<summary>Contains USB device config information</summary>
    public class InfoConfig
    {
        private readonly UsbDevice mUsbDevice;
        // Fields
        internal byte[] aExtra;
        internal byte bConfigurationValue;
        internal byte bDescriptorType;
        internal byte bLength;
        internal byte bmAttributes;
        internal byte bMaxPower;
        internal byte bNumInterfaces;
        internal byte iConfiguration;
        internal InfoInterfaces mUSBInfoInterfaces;
        internal ushort wTotalLength;
        private String mConfigurationString;

        internal InfoConfig(UsbDevice usbDevice)
        {
            mUsbDevice = usbDevice;
        }

        ///<summary>
        /// D7: Must be set to 1<br/>
        /// D6: Self-powered<br/>
        /// D5: Remote Wakeup<br/>
        /// D4...D0: Set to 0
        ///</summary>
        public byte BmAttributes
        {
            get { return bmAttributes; }
        }

        ///<summary>
        /// Index of string descriptor describing configuration - is set to 0 if no string
        ///</summary>
        public byte ConfigurationIndex
        {
            get { return iConfiguration; }
        }

        ///<summary>
        /// string descriptor describing configuration
        ///</summary>
        public string ConfigurationString
        {
            get
            {
                if (ReferenceEquals(mConfigurationString, null))
                {
                    mConfigurationString = "";
                    if (iConfiguration > 0)
                    {
                        mUsbDevice.GetString(iConfiguration, ref mConfigurationString);
                    }
                }
                return mConfigurationString;
            }
        }

        ///<summary>
        /// Value used by Set Configuration to select this configuration
        ///</summary>
        public byte ConfigurationValue
        {
            get { return bConfigurationValue; }
        }

        ///<summary>
        /// CONFIGURATION descriptor type (= 2)
        ///</summary>
        public byte DescriptorType
        {
            get { return bDescriptorType; }
        }

        ///<summary>
        ///	Custom, device specific descriptor information.
        ///</summary>
        public byte[] Extra
        {
            get { return aExtra; }
        }

        ///<summary>
        /// Size of this descriptor in bytes
        ///</summary>
        public byte Length
        {
            get { return bLength; }
        }

        ///<summary>
        /// Maximum current drawn by device in this configuration. In units of 2mA. So 50 means 100 mA.
        ///</summary>
        public byte MaxPower
        {
            get { return bMaxPower; }
        }

        ///<summary>
        /// Number of interfaces supported by this configuration
        ///</summary>
        public byte NumInterfaces
        {
            get { return bNumInterfaces; }
        }

        ///<summary>
        /// Total number of bytes in this descriptor and all the following descriptors.
        ///</summary>
        public ushort TotalLength
        {
            get { return wTotalLength; }
        }

        ///<summary>
        /// Array of available interfaces for this configuration
        ///</summary>
        public InfoInterfaces InfoInterfaceList
        {
            get { return mUSBInfoInterfaces; }
        }

        #region PUBLIC METHODS

        ///<summary>
        /// Overriden: Returns a string representing the current <see cref="InfoConfig"/> class.
        ///</summary>
        public override string ToString()
        {
            object[] args = new object[] {bLength, bDescriptorType, wTotalLength, bNumInterfaces, bConfigurationValue, iConfiguration, ConfigurationString, bmAttributes, MaxPower};
            return string.Format("bLength:{0}\r\nbDescriptorType:0x{1,2:X2}\r\nwTotalLength:{2}\r\nbNumInterfaces:{3}\r\nbConfigurationValue:{4}\r\niConfiguration:{5}\r\nsConfiguration:{6}\r\nbmAttributes:0x{7,2:X2}\r\nMaxPower:{8}", args);
        }

        #endregion
    }
}