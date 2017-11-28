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
using LibUsbDotNet.DeviceNotify.Info;
using LibUsbDotNet.DeviceNotify.Internal;
using LibUsbDotNet.Usb.Internal;

namespace LibUsbDotNet.DeviceNotify
{
    /// <summary>
    /// Type of notification device.
    /// </summary>
    [HexDisplay]
    public enum DeviceType
    {
        /// <summary>
        /// oem-defined device type.
        /// </summary>
        OEM = 0x00000000,
        /// <summary>
        /// devnode number.
        /// </summary>
        DEVNODE = 0x00000001,
        /// <summary>
        /// logical volume.
        /// </summary>
        VOLUME = 0x00000002,
        /// <summary>
        /// serial, parallel.
        /// </summary>
        PORT = 0x00000003,
        /// <summary>
        /// network resource.
        /// </summary>
        NET = 0x00000004,
        /// <summary>
        /// device interface class
        /// </summary>
        DEVICEINTERFACE = 0x00000005,
        /// <summary>
        /// file system handle.
        /// </summary>
        HANDLE = 0x00000006
    }

    /// <summary>
    /// Type of notification event.
    /// </summary>
    [HexDisplay]
    public enum EventType
    {
        /// <summary>
        /// A custom event has occurred.
        /// </summary>
        CUSTOMEVENT = 0x8006,
        /// <summary>
        /// A device or piece of media has been inserted and is now available.
        /// </summary>
        DEVICEARRIVAL = 0x8000,
        /// <summary>
        /// Permission is requested to remove a device or piece of media. Any application can deny this request and cancel the removal.
        /// </summary>
        DEVICEQUERYREMOVE = 0x8001,
        /// <summary>
        /// A request to remove a device or piece of media has been canceled.
        /// </summary>
        DEVICEQUERYREMOVEFAILED = 0x8002,
        /// <summary>
        /// A device or piece of media has been removed.
        /// </summary>
        DEVICEREMOVECOMPLETE = 0x8004,
        /// <summary>
        /// A device or piece of media is about to be removed. Cannot be denied.
        /// </summary>
        DEVICEREMOVEPENDING = 0x8003,
        /// <summary>
        /// A device-specific event has occurred.
        /// </summary>
        DEVICETYPESPECIFIC = 0x8005
    }

    /// <summary>
    /// Describes the device notify event
    /// </summary>
    public class DeviceNotifyEventArgs : EventArgs
    {
        private readonly DEV_BROADCAST_HDR mBaseHdr;
        private readonly EventType mEventType;

        private object mObject;
        private VolumeNotifyInfo mVolume;
        private PortNotifyInfo mPort;
        private UsbDeviceNotifyInfo mDevice;

        internal DeviceNotifyEventArgs(DEV_BROADCAST_HDR hdr, IntPtr ptrHdr, EventType eventType)
        {
            mBaseHdr = hdr;
            mEventType = eventType;

            switch (mBaseHdr.dbch_DeviceType)
            {
                case DeviceType.VOLUME:
                    mVolume = new VolumeNotifyInfo(ptrHdr);
                    mObject = mVolume;
                    break;
                case DeviceType.PORT:
                    mPort = new PortNotifyInfo(ptrHdr);
                    mObject = mPort;
                    break;
                case DeviceType.DEVICEINTERFACE:
                    mDevice = new UsbDeviceNotifyInfo(ptrHdr);
                    mObject = mDevice;
                    break;
            }
        }

        /// <summary>
        /// Gets the <see cref="VolumeNotifyInfo"/> class.
        /// </summary>
        /// <remarks>
        /// This value is null if the <see cref="DeviceNotifyEventArgs.DeviceType"/> is not set to <see cref="LibUsbDotNet.DeviceNotify.DeviceType.VOLUME"/>
        /// </remarks>
        public VolumeNotifyInfo Volume
        {
            get { return mVolume; }
        }

        /// <summary>
        /// Gets the <see cref="PortNotifyInfo"/> class.
        /// </summary>
        /// <remarks>
        /// This value is null if the <see cref="DeviceNotifyEventArgs.DeviceType"/> is not set to <see cref="LibUsbDotNet.DeviceNotify.DeviceType.PORT"/>
        /// </remarks>
        public PortNotifyInfo Port
        {
            get { return mPort; }
        }

        /// <summary>
        /// Gets the <see cref="UsbDeviceNotifyInfo"/> class.
        /// </summary>
        /// <remarks>
        /// This value is null if the <see cref="DeviceNotifyEventArgs.DeviceType"/> is not set to <see cref="LibUsbDotNet.DeviceNotify.DeviceType.DEVICEINTERFACE"/>
        /// </remarks>
        public UsbDeviceNotifyInfo Device
        {
            get { return mDevice; }
        }

        /// <summary>
        /// Gets the <see cref="EventType"/> for this notification.
        /// </summary>
        public EventType EventType
        {
            get { return mEventType; }
        }

        /// <summary>
        /// Gets the <see cref="DeviceType"/> for this notification.
        /// </summary>
        public DeviceType DeviceType
        {
            get { return mBaseHdr.dbch_DeviceType; }
        }

        /// <summary>
        /// Gets the notification class as an object.
        /// </summary>
        /// <remarks>
        /// This value is never null.
        /// </remarks>
        public object Object
        {
            get { return mObject; }
        }

        #region PUBLIC METHODS

        ///<summary>
        ///Returns a <see cref="T:System.String"></see> that represents the current <see cref="DeviceNotifyEventArgs"></see>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.String"></see> that represents the current <see cref="DeviceNotifyEventArgs"></see>.
        ///</returns>
        public override string ToString()
        {
            object[] o = {DeviceType, EventType, mObject.ToString()};
            return string.Format("[DeviceType:{0}] [EventType:{1}] {2}", o);
        }

        #endregion
    }
}