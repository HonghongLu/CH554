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

namespace LibUsbDotNet.Usb.Main
{
    /// <summary>
    /// Error storage class containing information related to the current <see cref="UsbError"/>.
    /// </summary>
    public class UsbError : EventArgs
    {
        /// <summary>
        /// The full object name that the error occured in.
        /// </summary>
        public readonly object Sender;

        private readonly string Description;

        /// <summary>
        /// The function in <see cref="Sender"/> that the error occured in.
        /// </summary>
        public readonly string FunctionName;

        /// <summary>
        /// The negative result error code.
        /// </summary>
        public readonly int ErrorCode;

        internal UsbError(object sender, string description, string functionName, int errorCode)
        {
            Sender = sender;
            this.Description = description;
            FunctionName = functionName;
            ErrorCode = errorCode;
        }

        #region PUBLIC METHODS

        /// <summary>
        /// Gets a <see cref="System.String"/> representing this <see cref="UsbError"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> representing this <see cref="UsbError"/>.</returns>
        public override string ToString()
        {
            return string.Format("Sender:{0}\r\nDescription:{1}\r\nFunctionName:{2}\r\nErrorCode:{3}", Sender, Description, FunctionName, ErrorCode);
        }

        #endregion
    }
}