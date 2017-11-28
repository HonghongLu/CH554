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
namespace LibUsbDotNet.Usb.Info
{
    ///<summary>Array list containing available endpoints in a <see cref="InfoInterface"/>.</summary>
    ///<remarks>See the base class <see cref="InfosBase{T}"/> for a list of available members.</remarks>
    public class InfoEndpoints : InfosBase<InfoEndpoint>
    {
        // Methods
        internal InfoEndpoints()
        {
        }
    }

    ///<summary>Contains USB endpoint information</summary>
    public class InfoEndpoint
    {
        private readonly UsbDevice mUsbDevice;
        // Fields
        internal byte bDescriptorType;
        internal byte bEndpointAddress;
        internal byte bInterval;
        internal byte bLength;
        internal byte bmAttributes;
        internal byte bRefresh;
        internal byte bSynchAddress;
        internal ushort wMaxPacketSize;

        internal InfoEndpoint(UsbDevice usbDevice)
        {
            mUsbDevice = usbDevice;
        }

        ///<summary>D1:0 Transfer Type<br/>
        /// 00 = Control<br/>
        /// 01 = Isochronous<br/>
        /// 10 = Bulk<br/>
        /// 11 = Interrupt<br/> 
        /// The following only apply to isochronous endpoints or are set to 0.<br/>
        /// D3:2 Synchronisation Type:<br/>
        /// 00 = No Synchronisation<br/>
        /// 01 = Asynchronous<br/>
        /// 10 = Adaptive<br/>
        /// 11 = Synchronous <br/>
        /// D5:4 Usage Type:<br/> 
        /// 00 = Data endpoint<br/>
        /// 01 = Feedback endpoint<br/>
        /// 10 = Implicit feedback Data endpoint<br/>
        /// 11 = Reserved: Set to 0 <br/>
        /// D7:6 Reserved: Set to 0 <br/>
        /// </summary>
        public byte BmAttributes
        {
            get { return bmAttributes; }
        }

        ///<summary>ENDPOINT descriptor type (= 5)</summary>
        public byte DescriptorType
        {
            get { return bDescriptorType; }
        }

        ///<summary>The address of this endpoint within the device.<br/>
        ///D7: Direction<br/>
        ///0 = OUT, 1 = IN<br/>
        ///D6-D4: Set to 0<br/>
        ///D3-D0: Endpoint number
        ///</summary>
        public byte EndpointAddress
        {
            get { return bEndpointAddress; }
        }

        ///<summary>Interval for polling endpoint for data transfers. Expressed in frames (ms) for low/full speed or microframes (125us) for high speed. </summary>
        public byte Interval
        {
            get { return bInterval; }
        }

        ///<summary>Size of this descriptor in bytes</summary>
        public byte Length
        {
            get { return bLength; }
        }

        ///<summary>Maximum packet size this endpoint can send or receive when this configuration is selected </summary>
        public ushort MaxPacketSize
        {
            get { return wMaxPacketSize; }
        }

        ///<summary>Device specfic setting</summary>
        public byte Refresh
        {
            get { return bRefresh; }
        }

        ///<summary>Device specfic setting</summary>
        public byte SynchAddress
        {
            get { return bSynchAddress; }
        }

        #region PUBLIC METHODS

        ///<summary>
        /// Overriden: Returns a string representing the current <see cref="InfoEndpoint"/> class.
        ///</summary>
        public override string ToString()
        {
            object[] args = new object[] {bLength, bDescriptorType, bEndpointAddress, bmAttributes, wMaxPacketSize, bInterval, bRefresh, bSynchAddress};
            return string.Format("bLength:{0}\r\nbDescriptorType:0x{1,2:X2}\r\nbEndpointAddress:0x{2,2:X2}\r\nbmAttributes:0x{3,2:X2}\r\nwMaxPacketSize:{4}\r\nbInterval:{5}\r\nbRefresh:{6}\r\nbSynchAddress:0x{7,2:X2}", args);
        }

        #endregion
    }
}