using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace WindowsFormsApplication1
{
    public class WmCap
    {
        public const int WM_USER = 1024;
        public const int WM_CAP_CONNECT = 1034;
        public const int WM_CAP_DISCONNECT = 1035;
        public const int WM_CAP_GT_FRAME = 1084;
        public const int WM_CAP_GET_FRAME = 1084;
        public const int WM_CAP_COPY = 1054;
        public const int WM_CAP_START = WM_USER;
        public const int WM_CAP_DLG_VIDEOFORMAT = WM_CAP_START + 41;
        public const int WM_CAP_DLG_VIDEOSOURCE = WM_CAP_START + 42;
        public const int WM_CAP_DLG_VIDEODISPLAY = WM_CAP_START + 43;
        public const int WM_CAP_GET_VIDEOFORMAT = WM_CAP_START + 44;
        public const int WM_CAP_SET_VIDEOFORMAT = WM_CAP_START + 45;
        public const int WM_CAP_DLG_VIDEOCOMPRESSION = WM_CAP_START + 46;
        public const int WM_CAP_SET_PREVIEW = WM_CAP_START + 50;
        public const short WM_CAP = 0x400;
        public const int WM_CAP_DRIVER_CONNECT = 0x40a;
        public const int WM_CAP_DRIVER_DISCONNECT = 0x40b;
        public const int WM_CAP_EDIT_COPY = 0x41e;
        public const int WM_CAP_SET_OVERLAY = 0x433;
        public const int WM_CAP_SET_PREVIEWRATE = 0x434;
        public const int WM_CAP_SET_SCALE = 0x435;
        public const int WS_CHILD = 0x40000000;
        public const int WS_VISIBLE = 0x10000000;
        public const int WM_CAP_SAVEDIB = WM_CAP_START + 25;

        [DllImport("avicap32.dll")]
        public static extern bool capGetDriverDescriptionA(short wDriverIndex, [MarshalAs(UnmanagedType.VBByRefStr)] ref String lpszName, int cbName, [MarshalAs(UnmanagedType.VBByRefStr)] ref String lpszVer, int cbVer);

        [DllImport("user32")]
        public static extern int SetWindowPos(int hwnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        [DllImport("user32", EntryPoint = "SendMessage")]
        public static extern int SendBitmapMessage(IntPtr hWnd, uint wMsg, int wParam, ref BITMAPINFO lParam);

        [DllImport("user32")]
        public static extern bool DestroyWindow(int hwnd);

        [DllImport("user32", EntryPoint = "SendMessage")]
        public static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);

        [DllImport("avicap32.dll", EntryPoint = "capCreateCaptureWindowA")]
        public static extern int capCreateCaptureWindowA(string lpszWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, int hwndParent, int nID);

        [DllImport("user32", EntryPoint = "OpenClipboard")]
        public static extern int OpenClipboard(int hWnd);
 
        [DllImport("user32", EntryPoint = "EmptyClipboard")]
        public static extern int EmptyClipboard();

        [DllImport("user32", EntryPoint = "CloseClipboard")]
        public static extern int CloseClipboard();
 
        [DllImport("user32.dll")]
        public static extern IntPtr GetClipboardData(uint uFormat);

        public static DeviceDescription[] GetAllCapturesDevices()
        {
            String dName = "".PadRight(100); String dVersion = "".PadRight(100);

            var devices = new List<DeviceDescription>();

            for (short i = 0; i < 10; i++)
            {
                if (capGetDriverDescriptionA(i,
                ref dName, 100, ref dVersion,
                100))
                {
                    DeviceDescription d = new DeviceDescription(i);
                    d.Name = dName.Trim();
                    d.Version = dVersion.Trim();

                    devices.Add(d);
                }
            }
            return devices.ToArray();
        }

    }
}
