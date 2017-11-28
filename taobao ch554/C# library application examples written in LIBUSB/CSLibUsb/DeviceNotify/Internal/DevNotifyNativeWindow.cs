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
using System.Windows.Forms;

namespace LibUsbDotNet.DeviceNotify.Internal
{
    internal class DevNotifyNativeWindow : NativeWindow
    {
        private const int WM_DEVICECHANGE = 0x219;
        private const string mWindowCaption = "{18662f14-0871-455c-bf99-eff135425e3a}";
        private readonly OnHandleChangeDelegate delHandleChanged;
        private readonly OnDeviceChangeDelegate delDeviceChange;

        internal DevNotifyNativeWindow(OnHandleChangeDelegate delHandleChanged, OnDeviceChangeDelegate delDeviceChange)
        {
            this.delHandleChanged = delHandleChanged;
            this.delDeviceChange = delDeviceChange;

            CreateParams cp = new CreateParams();
            cp.Caption = mWindowCaption;
            cp.X = -100;
            cp.Y = -100;
            cp.Width = 50;
            cp.Height = 50;
            CreateHandle(cp);
        }

        protected override void OnHandleChange()
        {
            delHandleChanged(Handle);
            base.OnHandleChange();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_DEVICECHANGE)
            {
                delDeviceChange(ref m);
            }
            base.WndProc(ref m);
        }

        #region Nested type: OnDeviceChangeDelegate

        internal delegate void OnDeviceChangeDelegate(ref Message m);

        #endregion

        #region Nested type: OnHandleChangeDelegate

        internal delegate void OnHandleChangeDelegate(IntPtr windowHandle);

        #endregion
    }
}