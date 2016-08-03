using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public class Camera: IDisposable
    {
        private int deviceHandle;
        private int index;

        public Camera(int index, Control previewControl)
        {
            index = 0;
            string deviceIndexAsString = Convert.ToString(index);
            var windowWidth = previewControl.ClientSize.Width;
            var windowHeight = previewControl.ClientSize.Height;
            deviceHandle = WmCap.capCreateCaptureWindowA(deviceIndexAsString, WmCap.WS_VISIBLE | WmCap.WS_CHILD, 0, 0, windowWidth,
                windowHeight, (int)(previewControl.Handle), 0);

            if (WmCap.SendMessage(deviceHandle, WmCap.WM_CAP_DRIVER_CONNECT, index, 0) < 1)
            {
                throw new Exception("Connection to driver error.");
            }

            WmCap.SendMessage(deviceHandle, WmCap.WM_CAP_SET_SCALE, -1, 0);
            WmCap.SendMessage(deviceHandle, WmCap.WM_CAP_SET_PREVIEWRATE, 0x42, 0);
            WmCap.SendMessage(deviceHandle, WmCap.WM_CAP_SET_PREVIEW, -1, 0);

            WmCap.SetWindowPos(deviceHandle, 1, 0, 0, windowWidth, windowHeight, 6);
        }

        public void Dispose()
        {
            WmCap.SendMessage(deviceHandle, WmCap.WM_CAP_DRIVER_DISCONNECT, index, 0);
            WmCap.DestroyWindow(deviceHandle);
        }

        public void ShowConfigDialog()
        {
            WmCap.SendMessage(deviceHandle, WmCap.WM_CAP_DLG_VIDEOFORMAT, 0, 0);
        }

        public void TakeSnapshot(string filePath)
        {
            IntPtr hBmp = Marshal.StringToHGlobalAnsi(filePath);
            WmCap.SendMessage(deviceHandle, WmCap.WM_CAP_SAVEDIB, 0, hBmp.ToInt32());
        }

        public static DeviceDescription[] GetListCameras()
        {
            return WmCap.GetAllCapturesDevices();
        }
    }
}
