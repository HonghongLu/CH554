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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using LibUsbDotNet.Usb.Info;
using LibUsbDotNet.Usb.Internal;
using LibUsbDotNet.Usb.Internal.API;
using LibUsbDotNet.Usb.Main;

namespace LibUsbDotNet.Usb
{
    /// <summary>
    /// Class containing device specific functions and members for communication with a USB device.
    /// </summary>
    public class UsbDevice
    {
        private LibUsbDevice mDev;
        private InfoDevice mInfoDevice;
        private LibUsb_DevHandle mHandle;
        private InfoConfigs mConfigs;
        private UsbEndpointList mActiveEndpoints;

        internal bool mbFoundDevice;

        internal UsbDevice(LibUsbDevice dev)
        {
            mDev = dev;
            mInfoDevice = new InfoDevice(this, mDev);
            mActiveEndpoints = new UsbEndpointList(this);
        }

        internal LibUsbDevice InternalLibUsbDevice
        {
            get { return mDev; }
        }

        /// <summary>An array of <see cref="UsbEndpointBase"/> classes that were created with <see cref="UsbDevice.OpenEndpointReader(ReadEndpoints, int, EndpointTypes, int)" /> or <see cref="UsbDevice.OpenEndpointWriter(WriteEndpoints, EndpointTypes, int)" /> functions.</summary>
        public UsbEndpointList ActiveEndpoints
        {
            get { return mActiveEndpoints; }
        }

        /// <summary>
        /// Gets the <see cref="InfoDevice"/> class for this <see cref="UsbDevice"/>.
        /// </summary>
        public InfoDevice Info
        {
            get { return mInfoDevice; }
        }

        /// <summary>
        /// Gets the cached configuration information for this <see cref="UsbDevice"/>.  To refresh configuration information use <see cref="RefreshConfigs"/>.
        /// </summary>
        public InfoConfigs Configs
        {
            get
            {
                if (ReferenceEquals(mConfigs, null))
                    RefreshConfigs();

                return mConfigs;
            }
        }

        internal LibUsb_DevHandle Handle
        {
            get { return mHandle; }
        }

        /// <summary>
        /// True if this <see cref="UsbDevice"/> has been opened and has a valid handle.
        /// </summary>
        public bool IsOpen
        {
            get { return mHandle.IsValid; }
        }

        #region PUBLIC METHODS

        ///<summary>Claims an interface for use in this <see cref="UsbDevice"/> class.</summary>
        ///<returns>0 on success or less than 0 on error.</returns>
        public int ClaimInterface(int iInterface)
        {
            if (!Open()) return ((int) ErrorCodes.EFAULT);

            int ret = LibUsbAPI.usb_claim_interface(mHandle, iInterface);
            if (ret < 0)
            {
                UsbGlobals.Error(this, UsbGlobals.LastError,"ClaimInterface", ret);
            }

            return ret;
        }

        /// <summary>Closes the device.</summary>
        /// <remarks><dl class="cBList"><dt>The <see cref="Close" /> function performs the following actions:</dt>
        /// <dd>Attempts to safely stop read threads on all <see cref="ActiveEndpoints" />.</dd>
        /// <dd>Aborts read threads on all <see cref="ActiveEndpoints" /> if they fail to stop gracefully.</dd>
        /// <dd>Closes and releases the internal device handle.</dd></dl></remarks>
        public bool Close()
        {
            if (mHandle.IsValid)
            {
                ActiveEndpoints.Clear();
                if (!mHandle.Close())
                {
                    UsbGlobals.Error(this,UsbGlobals.LastError, "Close", (int) ErrorCodes.EFAULT);
                    return false;
                }
            }
            return true;
        }

        /// <summary>Gets the active alternate interface for the specified interface.</summary>
        /// <remarks>On success, the <paramref name="bAltValue" /> contains the value of the current active alternate interface for the specified interface.</remarks>
        /// <returns>0 on success or less than 0 on error.</returns>
        public int GetAltInterface(int iInterface, ref byte bAltValue)
        {
            byte[] buf = new byte[1];
            int ret = IOControlMessage(LibUsbConst.USB_ENDPOINT_IN | LibUsbConst.USB_RECIP_INTERFACE, LibUsbConst.USB_REQ_GET_INTERFACE, 0, iInterface, buf, 1000);
            if (ret == 1)
            {
                bAltValue = buf[0];
            }

            return ret;
        }

        /// <summary>Gets the active configuration of the opened device.</summary>
        /// <remarks>On success, the <paramref name="bCfgValue" /> contains the value of the current active configuration for this <see cref="UsbDevice"/>.</remarks>
        /// <returns>0 on success or less than 0 on error.</returns>
        public int GetConfiguration(ref byte bCfgValue)
        {
            byte[] buf = new byte[1];
            int ret = IOControlMessage(LibUsbConst.USB_ENDPOINT_IN | LibUsbConst.USB_RECIP_DEVICE, LibUsbConst.USB_REQ_GET_CONFIGURATION, 0, 0, buf, 1000);
            if (ret == 1)
            {
                bCfgValue = buf[0];
            }

            return ret;
        }

        /// <summary>
        /// Retrieves a string descriptor.
        /// </summary>
        /// <param name="stringIndex">The index of the string to be retrieved.</param>
        /// <param name="returnString">On success, the <see cref="System.String"/> for the specified <paramref name="stringIndex"/>.</param>
        /// <returns>On success, the length of the string.  On failure, Less than zero.</returns>
        public int GetString(int stringIndex, ref String returnString)
        {
            returnString = "";
            if (!Open()) return ((int) ErrorCodes.EFAULT);

            StringBuilder sb = new StringBuilder(255);
            int ret = LibUsbAPI.usb_get_string_simple(mHandle, stringIndex, sb, sb.Capacity);

            if (ret > 0) returnString = sb.ToString(0, ret);

            Debug.WriteIf(ret < 0, "Failed getting string index #" + stringIndex);
            return ret;
        }

        /// <summary>
        /// Retrieves a string descriptor.
        /// </summary>
        /// <param name="stringIndex">The index of the string to be retrieved.</param>
        /// <param name="langID">The language id of the string.  Default is 0x409.</param>
        /// <param name="returnString">On success, the <see cref="System.String"/> for the specified <paramref name="stringIndex"/> and <paramref name="langID"/>.</param>
        /// <returns></returns>
        public int GetString(int stringIndex, int langID, ref String returnString)
        {
            if (langID == 0) langID = 0x409;
            returnString = "";
            if (!Open()) return ((int) ErrorCodes.EFAULT);

            StringBuilder sb = new StringBuilder(255);
            int ret = LibUsbAPI.usb_get_string(mHandle, stringIndex, langID, sb, sb.Capacity);
            if (ret > 0) returnString = sb.ToString(0, ret);

            Debug.WriteIf(ret < 0, "Failed getting string index #" + stringIndex + " langID 0x" + langID.ToString("X4"));
            return ret;
        }

        /// <summary>
        /// Sends/receives a control message to/from the current <see cref="UsbDevice"/>.
        /// </summary>
        /// <param name="requestType">USB request type.</param>
        /// <param name="request">USB request.</param>
        /// <param name="value">USB value.</param>
        /// <param name="index">USB index.</param>
        /// <param name="bytes">Buffer to send/recv from device.</param>
        /// <param name="timeout">Maximum amount of time to wait for the function to complete.</param>
        /// <returns>On Success, the number of bytes transmitted. Less than zero on failure.</returns>
        public int IOControlMessage(int requestType, int request, int value, int index, Byte[] bytes, int timeout)
        {
            if (!Open()) return ((int) ErrorCodes.EFAULT);
            int ret = LibUsbAPI.usb_control_msg(mHandle, requestType, request, value, index, bytes, bytes == null ? 0 : bytes.Length, timeout);
            if (ret < 0)
            {
                UsbGlobals.Error(this, UsbGlobals.LastError, "IOControlMessage", ret);
            }
            return ret;
        }

        /// <summary>Opens this <see cref="UsbDevice"/> for communication.</summary>
        /// <returns>True if the device was opened successfully.</returns>
        public bool Open()
        {
            if (!mHandle.IsValid)
            {
                mHandle.Open(mDev);
                if (!mHandle.IsValid)
                    UsbGlobals.Error(this, UsbGlobals.LastError, "Open", (int) ErrorCodes.EFAULT);
            }

            return mHandle.IsValid;
        }

        /// <summary>
        /// Opens an endpoint for reading
        /// </summary>
        /// <param name="readEndpoint">Endpoint number for read operations.</param>
        /// <param name="readBufferSize">Size of the read buffer allocated for the <see cref="UsbEndpointReader.DataReceived"/> event.</param>
        /// <returns>An <see cref="UsbEndpointReader"/> class ready for reading.
        /// If the specified endpoint has allready been opened, the original <see cref="UsbEndpointReader"/> object will be returned.
        /// </returns>
        public UsbEndpointReader OpenBulkEndpointReader(ReadEndpoints readEndpoint, int readBufferSize)
        {
            UsbEndpointReader epNew = new UsbEndpointReader(this, EndpointTypes.Bulk, 0, readBufferSize, readEndpoint);
            return (UsbEndpointReader) mActiveEndpoints.Add(epNew);
        }

        /// <summary>
        /// Opens an endpoint for reading
        /// </summary>
        /// <param name="readEndpoint">Endpoint number for read operations.</param>
        /// <returns>A <see cref="UsbEndpointReader"/> class ready for reading.
        /// If the specified endpoint has allready been opened, the original <see cref="UsbEndpointReader"/> object will be returned.
        /// </returns>
        public UsbEndpointReader OpenBulkEndpointReader(ReadEndpoints readEndpoint)
        {
            UsbEndpointReader epNew = new UsbEndpointReader(this, EndpointTypes.Bulk, 0, UsbEndpointReader.DEF_READ_BUFFER_SIZE, readEndpoint);
            return (UsbEndpointReader) mActiveEndpoints.Add(epNew);
        }

        /// <summary>
        /// Opens an endpoint for writing
        /// </summary>
        /// <param name="writeEndpoint">Endpoint number for read operations.</param>
        /// <returns>A <see cref="UsbEndpointWriter"/> class ready for writing.
        /// If the specified endpoint has allready been opened, the original <see cref="UsbEndpointWriter"/> object will be returned.
        /// </returns>
        public UsbEndpointWriter OpenBulkEndpointWriter(WriteEndpoints writeEndpoint)
        {
            UsbEndpointWriter epNew = new UsbEndpointWriter(this, EndpointTypes.Bulk, 0, writeEndpoint);
            return (UsbEndpointWriter) mActiveEndpoints.Add(epNew);
        }

        /// <summary>
        /// Opens an endpoint for reading
        /// </summary>
        /// <param name="readEndpoint">Endpoint number for read operations.</param>
        /// <param name="readBufferSize">Size of the read buffer allocated for the <see cref="UsbEndpointReader.DataReceived"/> event.</param>
        /// <param name="endPointType">One of the <see cref="EndpointTypes"/> enumerations.</param>
        /// <param name="packetSize">The packet size to use when endPointType is set to <see cref="EndpointTypes.Isochronous"/>.</param>
        /// <returns>A <see cref="UsbEndpointReader"/> class ready for reading.
        /// If the specified endpoint has allready been opened, the original <see cref="UsbEndpointReader"/> object will be returned.
        /// </returns>
        public UsbEndpointReader OpenEndpointReader(ReadEndpoints readEndpoint, int readBufferSize, EndpointTypes endPointType, int packetSize)
        {
            UsbEndpointReader epNew = new UsbEndpointReader(this, endPointType, packetSize, readBufferSize, readEndpoint);
            return (UsbEndpointReader) mActiveEndpoints.Add(epNew);
        }

        /// <summary>
        /// Opens an endpoint for writing
        /// </summary>
        /// <param name="writeEndpoint">Endpoint number for read operations.</param>
        /// <param name="endPointType">One of the <see cref="EndpointTypes"/> enumerations.</param>
        /// <param name="packetSize">The packet size to use when endPointType is set to <see cref="EndpointTypes.Isochronous"/>.</param>
        /// <returns>A <see cref="UsbEndpointWriter"/> class ready for writing.
        /// If the specified endpoint has allready been opened, the original <see cref="UsbEndpointWriter"/> object will be returned.
        /// </returns>
        public UsbEndpointWriter OpenEndpointWriter(WriteEndpoints writeEndpoint, EndpointTypes endPointType, int packetSize)
        {
            UsbEndpointWriter epNew = new UsbEndpointWriter(this, endPointType, packetSize, writeEndpoint);
            return (UsbEndpointWriter) mActiveEndpoints.Add(epNew);
        }

        /// <summary>
        /// Opens an endpoint for reading
        /// </summary>
        /// <param name="readEndpoint">Endpoint number for read operations.</param>
        /// <param name="readBufferSize">Size of the read buffer allocated for the <see cref="UsbEndpointReader.DataReceived"/> event.</param>
        /// <returns>A <see cref="UsbEndpointReader"/> class ready for reading.
        /// If the specified endpoint has allready been opened, the original <see cref="UsbEndpointReader"/> object will be returned.
        /// </returns>
        public UsbEndpointReader OpenInterruptEndpointReader(ReadEndpoints readEndpoint, int readBufferSize)
        {
            UsbEndpointReader epNew = new UsbEndpointReader(this, EndpointTypes.Interrupt, 0, readBufferSize, readEndpoint);
            return (UsbEndpointReader) mActiveEndpoints.Add(epNew);
        }

        /// <summary>
        /// Opens an endpoint for reading
        /// </summary>
        /// <param name="readEndpoint">Endpoint number for read operations.</param>
        /// <returns>A <see cref="UsbEndpointReader"/> class ready for reading.
        /// If the specified endpoint has allready been opened, the original <see cref="UsbEndpointReader"/> object will be returned.
        /// </returns>
        public UsbEndpointReader OpenInterruptEndpointReader(ReadEndpoints readEndpoint)
        {
            UsbEndpointReader epNew = new UsbEndpointReader(this, EndpointTypes.Interrupt, 0, UsbEndpointReader.DEF_READ_BUFFER_SIZE, readEndpoint);
            return (UsbEndpointReader) mActiveEndpoints.Add(epNew);
        }

        /// <summary>
        /// Opens an endpoint for writing
        /// </summary>
        /// <param name="writeEndpoint">Endpoint number for read operations.</param>
        /// <returns>A <see cref="UsbEndpointWriter"/> class ready for writing.
        /// If the specified endpoint has allready been opened, the original <see cref="UsbEndpointWriter"/> object will be returned.
        /// </returns>
        public UsbEndpointWriter OpenInterruptEndpointWriter(WriteEndpoints writeEndpoint)
        {
            UsbEndpointWriter epNew = new UsbEndpointWriter(this, EndpointTypes.Interrupt, 0, writeEndpoint);
            return (UsbEndpointWriter) mActiveEndpoints.Add(epNew);
        }

        ///<summary>Gets and refreshes all available configuration information this <see cref="UsbDevice"/>.</summary>
        ///<returns>A <see cref="InfoConfigs"/> list containing all device config information.</returns>
        public InfoConfigs RefreshConfigs()
        {
            mConfigs = new InfoConfigs();

            StringBuilder sbTemp = new StringBuilder(255);

            for (int iConfig = 0; iConfig < mDev.mDeviceDescriptor.bNumConfigurations; iConfig++)
            {
                LibUsb_ConfigDescriptor usb_config = mDev.GetConfig(iConfig);
                InfoConfig config = new InfoConfig(this);
                mConfigs.Add(config);

                config.bLength = usb_config.bLength;
                config.bDescriptorType = usb_config.bDescriptorType;
                config.wTotalLength = usb_config.wTotalLength;
                config.bNumInterfaces = usb_config.bNumInterfaces;
                config.bConfigurationValue = usb_config.bConfigurationValue;
                config.iConfiguration = usb_config.iConfiguration;
                config.bmAttributes = usb_config.bmAttributes;
                config.bMaxPower = usb_config.MaxPower;

                if (usb_config.extralen > 0 && usb_config.extra != IntPtr.Zero)
                {
                    config.aExtra = new byte[usb_config.extralen];
                    Marshal.Copy(usb_config.extra, config.aExtra, 0, usb_config.extralen);
                }

                config.mUSBInfoInterfaces = new InfoInterfaces();
                for (int iInterface = 0; iInterface < usb_config.bNumInterfaces; iInterface++)
                {
                    LibUsb_InterfaceDescriptor usb_int = usb_config.GetInterface(iInterface);

                    for (int iAlt = 0; iAlt < usb_int.mNumAltSetting; iAlt++)
                    {
                        LibUsb_AltInterfaceDescriptor usb_id = usb_int.GetAltInterface(iAlt);
                        InfoInterface UsbInt = new InfoInterface(this);
                        config.mUSBInfoInterfaces.Add(UsbInt);

                        UsbInt.bLength = usb_id.bLength;
                        UsbInt.bDescriptorType = usb_id.bDescriptorType;
                        UsbInt.bInterfaceNumber = usb_id.bInterfaceNumber;
                        UsbInt.bAlternateSetting = usb_id.bAlternateSetting;
                        UsbInt.bNumEndpoints = usb_id.bNumEndpoints;
                        UsbInt.bInterfaceClass = usb_id.bInterfaceClass;
                        UsbInt.bInterfaceSubClass = usb_id.bInterfaceSubClass;
                        UsbInt.bInterfaceProtocol = usb_id.bInterfaceProtocol;
                        UsbInt.iInterface = usb_id.iInterface;

                        UsbInt.usbInfoEndpoints = new InfoEndpoints();
                        for (int iEndpoint = 0; iEndpoint < usb_id.bNumEndpoints; iEndpoint++)
                        {
                            InfoEndpoint endPoint = new InfoEndpoint(this);
                            UsbInt.usbInfoEndpoints.Add(endPoint);
                            LibUsb_EndpointDescriptor usb_endpoint = usb_id.GetEndpoint(iEndpoint);

                            endPoint.bLength = usb_endpoint.bLength;
                            endPoint.bDescriptorType = usb_endpoint.bDescriptorType;
                            endPoint.bEndpointAddress = usb_endpoint.bEndpointAddress;
                            endPoint.bmAttributes = usb_endpoint.bmAttributes;
                            endPoint.wMaxPacketSize = usb_endpoint.wMaxPacketSize;
                            endPoint.bInterval = usb_endpoint.bInterval;
                            endPoint.bRefresh = usb_endpoint.bRefresh;
                            endPoint.bSynchAddress = usb_endpoint.bSynchAddress;
                        } //Endpoints
                    } //Alt Interfaces
                } //Interfaces
            } //Configs
            return mConfigs;
        }

        ///<summary>Releases a claimed interface.  See <see cref="ClaimInterface"/>.</summary>
        ///<returns>0 on success or less than 0 on error.</returns>
        public int ReleaseInterface(int iInterface)
        {
            if (!Open()) return ((int) ErrorCodes.EFAULT);

            int ret = LibUsbAPI.usb_release_interface(mHandle, iInterface);
            if (ret < 0)
            {
                UsbGlobals.Error(this, UsbGlobals.LastError, "ReleaseInterface", ret); 
            }

            return ret;
        }

        ///<summary>Sets the alternate interface to use for the claimed interface.</summary>
        ///<remarks>The <paramref name="iAltInterface"/> parameter is the value as specified in the descriptor field <see cref="InfoInterface.AlternateSetting"/>.</remarks>
        ///<returns>0 on success or less than 0 on error.</returns>
        public int SetAltInterface(int iAltInterface)
        {
            if (!Open()) return ((int) ErrorCodes.EFAULT);

            int ret = LibUsbAPI.usb_set_altinterface(mHandle, iAltInterface);
            if (ret < 0)
            {
                UsbGlobals.Error(this, UsbGlobals.LastError, "SetAltInterface", ret);
            }

            return ret;
        }

        /// <summary>Sets the active configuration of the opened device.</summary>
        /// <remarks>The <paramref name="iConfig" /> parameter is the value as specified in the descriptor field <see cref="InfoConfig.ConfigurationValue" /></remarks>
        /// <returns>0 on success or less than 0 on error.</returns>
        public int SetConfiguration(int iConfig)
        {
            if (!Open()) return ((int) ErrorCodes.EFAULT);

            int ret = LibUsbAPI.usb_set_configuration(mHandle, iConfig);
            if (ret < 0)
            {
                UsbGlobals.Error(this, UsbGlobals.LastError, "SetConfiguration", ret);
            }

            return ret;
        }

        #endregion

        ///<summary>
        ///Allows a <see cref="UsbDevice"></see> to attempt to free resources and perform other cleanup operations before the <see cref="UsbDevice"></see> is reclaimed by garbage collection.
        ///</summary>
        ///
        ~UsbDevice()
        {
            Close();
        }
    }
}