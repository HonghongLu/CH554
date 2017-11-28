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
using System.Security;
using LibUsbDotNet.Usb.Internal.API;

namespace LibUsbDotNet.Usb.Info
{
    ///<summary>Contains USB device information</summary>
    [SuppressUnmanagedCodeSecurity]
    public class InfoDevice
    {
        // Fields
        internal ushort bcdDevice;
        internal ushort bcdUSB;
        internal byte bDescriptorType;
        internal byte bDeviceClass;
        internal byte bDeviceProtocol;
        internal byte bDeviceSubClass;
        internal byte bLength;
        internal byte bMaxPacketSize0;
        internal byte bNumConfigurations;
        internal ushort idProduct;
        internal ushort idVendor;
        internal byte iManufacturer;
        internal byte iProduct;
        internal byte iSerial;
        internal string mFilename;

        private UsbDevice mUsbDevice;
        private String mManufacturerString;
        private String mProductString;
        private String mSerialString;

        // Methods
        internal InfoDevice(UsbDevice usbDevice, LibUsbDevice dev)
        {
            mUsbDevice = usbDevice;
            bcdDevice = dev.mDeviceDescriptor.bcdDevice;
            bcdUSB = dev.mDeviceDescriptor.bcdUSB;
            bDescriptorType = dev.mDeviceDescriptor.bDescriptorType;
            bDeviceClass = dev.mDeviceDescriptor.bDeviceClass;
            bDeviceProtocol = dev.mDeviceDescriptor.bDeviceProtocol;
            bDeviceSubClass = dev.mDeviceDescriptor.bDeviceSubClass;
            bLength = dev.mDeviceDescriptor.bLength;
            bMaxPacketSize0 = dev.mDeviceDescriptor.bMaxPacketSize0;
            bNumConfigurations = dev.mDeviceDescriptor.bNumConfigurations;
            mFilename = dev.mFilename;
            idProduct = dev.mDeviceDescriptor.idProduct;
            idVendor = dev.mDeviceDescriptor.idVendor;
            iManufacturer = dev.mDeviceDescriptor.iManufacturer;
            iProduct = dev.mDeviceDescriptor.iProduct;
            iSerial = dev.mDeviceDescriptor.iSerialNumber;
        }

        ///<summary>Device release number in binary coded decimal</summary>
        public ushort BcdDevice
        {
            get { return bcdDevice; }
        }

        ///<summary>USB Spec release number</summary>
        public ushort BcdUSB
        {
            get { return bcdUSB; }
        }

        ///<summary>DEVICE descriptor type (= 1)</summary>
        public byte DescriptorType
        {
            get { return bDescriptorType; }
        }

        ///<summary>Class code assigned by USB-IF<br/>
        /// 00h means each interface defines its own class<br/>
        /// FFh means vendor-defined class<br/>
        /// Any other value must be a class code 
        /// </summary>
        public byte DeviceClass
        {
            get { return bDeviceClass; }
        }

        ///<summary>Protocol Code assigned by USB-IF</summary>
        public byte DeviceProtocol
        {
            get { return bDeviceProtocol; }
        }

        ///<summary>SubClass Code assigned by USB-IF</summary>
        public byte DeviceSubClass
        {
            get { return bDeviceSubClass; }
        }

        ///<summary>Unique filename describing device.</summary>
        public string Filename
        {
            get { return mFilename; }
        }

        ///<summary>Product ID - assigned by the manufacturer</summary>
        public ushort IdProduct
        {
            get { return idProduct; }
        }

        ///<summary>Vendor ID - must be obtained from USB-IF</summary>
        public ushort IdVendor
        {
            get { return idVendor; }
        }

        ///<summary>Size of this descriptor in bytes</summary>
        public byte Length
        {
            get { return bLength; }
        }

        ///<summary>Index of string descriptor describing manufacturer - set to 0 if no string</summary>
        public byte ManufacturerIndex
        {
            get { return iManufacturer; }
        }

        ///<summary>String descriptor describing manufacturer</summary>
        public string ManufacturerString
        {
            get
            {
                if (ReferenceEquals(mManufacturerString, null))
                {
                    mManufacturerString = "";
                    if (iManufacturer > 0)
                    {
                        mUsbDevice.GetString(iManufacturer, ref mManufacturerString);
                    }
                }
                return mManufacturerString;
            }
        }


        ///<summary>Max packet size for endpoint 0. Must be 8, 16, 32 or 64</summary>
        public byte MaxPacketSize0
        {
            get { return bMaxPacketSize0; }
        }

        ///<summary>Number of possible configurations</summary>
        public byte NumConfigurations
        {
            get { return bNumConfigurations; }
        }

        ///<summary>Index of string descriptor describing product - set to 0 if no string</summary>
        public byte ProductIndex
        {
            get { return iProduct; }
        }

        ///<summary>String descriptor describing product</summary>
        public string ProductString
        {
            get
            {
                if (ReferenceEquals(mProductString, null))
                {
                    mProductString = "";
                    if (iProduct > 0)
                    {
                        mUsbDevice.GetString(iProduct, ref mProductString);
                    }
                }
                return mProductString;
            }
        }

        ///<summary>String descriptor describing device serial number</summary>
        public string SerialString
        {
            get
            {
                if (ReferenceEquals(mSerialString, null))
                {
                    mSerialString = "";
                    if (iSerial > 0)
                    {
                        mUsbDevice.GetString(iSerial, ref mSerialString);
                    }
                }
                return mSerialString;
            }
        }

        ///<summary>Index of string descriptor describing device serial number - set to 0 if no string</summary>
        public byte SerialNumberIndex
        {
            get { return iSerial; }
        }

        #region PUBLIC METHODS

        ///<summary>
        /// Overriden: Returns a string representing the current <see cref="InfoDevice"/> class.
        ///</summary>
        public override string ToString()
        {
            object[] args = new object[] {bLength, bDescriptorType, bcdUSB, bDeviceClass, bDeviceSubClass, bDeviceProtocol, bMaxPacketSize0, idVendor, idProduct, bcdDevice, bNumConfigurations, iManufacturer, iProduct, iSerial, ManufacturerString, ProductString, SerialString, mFilename};
            return string.Format("bLength:{0}\r\nbDescriptorType:{1}\r\nbcdUSB:0x{2,4:X4}\r\nbDeviceClass:0x{3,2:X2}\r\nbDeviceSubClass:0x{4,2:X2}\r\nbDeviceProtocol:0x{5,2:X2}\r\nbMaxPacketSize0:{6}\r\nidVendor:0x{7,4:X4}\r\nidProduct:0x{8,4:X4}\r\nbcdDevice:0x{9,4:X4}\r\nbNumConfigurations:{10}\r\niManufacturer:{11}\r\niProduct:{12}\r\niSerialNumber:{13}\r\nsManufacturer:{14}\r\nsProduct:{15}\r\nsSerialNumber:{16}\r\nsFilename:{17}\r\n", args);
        }

        #endregion
    }

    ///<summary>Array list containing available USB devices.</summary>
    ///<remarks>See the base class <see cref="InfosBase{T}"/> for a list of available members.</remarks>
    public class InfoDevices : InfosBase<InfoDevice>
    {
        // Methods
        internal InfoDevices()
        {
        }
    }
}