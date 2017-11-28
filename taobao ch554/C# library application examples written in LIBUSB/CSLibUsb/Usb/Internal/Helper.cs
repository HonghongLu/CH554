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
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace LibUsbDotNet.Usb.Internal
{
    internal static class Helper
    {
        #region PUBLIC METHODS

        public static Dictionary<string, int> GetEnumData(Type type)
        {
            Dictionary<string, int> dictEnum = new Dictionary<string, int>();
            FieldInfo[] enumFields = type.GetFields();
            for (int iField = 1; iField < enumFields.Length; iField++)
            {
                object oValue = enumFields[iField].GetRawConstantValue();
                dictEnum.Add(enumFields[iField].Name, Convert.ToInt32(oValue));
            }

            return dictEnum;
        }

        public static string ShowAsHex(object standardValue)
        {
            if (standardValue == null) return "";
            if (standardValue is Int64) return "0x" + ((Int64) standardValue).ToString("X16");
            if (standardValue is UInt32) return "0x" + ((UInt32) standardValue).ToString("X8");
            if (standardValue is Int32) return "0x" + ((Int32) standardValue).ToString("X8");
            if (standardValue is UInt16) return "0x" + ((UInt16) standardValue).ToString("X4");
            if (standardValue is Int16) return "0x" + ((Int16) standardValue).ToString("X4");
            if (standardValue is Byte) return "0x" + ((Byte) standardValue).ToString("X2");

            return "";
        }

        #endregion

        internal static void BytesToObject(byte[] sourceBytes, int iStartIndex, int iLength, object destObject)
        {
            GCHandle gch = GCHandle.Alloc(destObject, GCHandleType.Pinned);

            IntPtr ptrDestObject = gch.AddrOfPinnedObject();
            Marshal.Copy(sourceBytes, iStartIndex, ptrDestObject, iLength);

            gch.Free();
        }
    }
}