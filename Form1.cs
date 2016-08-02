using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        [DllImport("avicap32.dll")]
        protected static extern bool capGetDriverDescriptionA(short wDriverIndex, [MarshalAs(UnmanagedType.VBByRefStr)] ref String lpszName, int cbName, [MarshalAs(UnmanagedType.VBByRefStr)] ref String lpszVer, int cbVer);
        [DllImport("user32")]
        protected static extern int SetWindowPos(int hwnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        [DllImport("user32", EntryPoint = "SendMessage")]
        public static extern int SendBitmapMessage(IntPtr hWnd, uint wMsg, int wParam, ref BITMAPINFO lParam);

        [DllImport("user32")]
        protected static extern bool DestroyWindow(int hwnd);
        #region API Declarations

        //Abaixo tremos todas as chamadas das APIs do Sistema Operacional Windows. 
        //Essas chamadas fazem a interface do nosso controle com a WebCam e com o SO. 

        //Esta chamada é uma das mais importantes e é vital para o funcionamento do SO.  
        [DllImport("user32", EntryPoint = "SendMessage")]
        public static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);

        //Esta API cria a instância da webcam para que possamos acessa-la. 
        [DllImport("avicap32.dll", EntryPoint = "capCreateCaptureWindowA")]
        public static extern int capCreateCaptureWindowA(string lpszWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, int hwndParent, int nID);

        //Esta API abre a área de tranferência para que possamos buscar os dados da webcam. 
        [DllImport("user32", EntryPoint = "OpenClipboard")]
        public static extern int OpenClipboard(int hWnd);

        //Esta API limpa a área de transferência. 
        [DllImport("user32", EntryPoint = "EmptyClipboard")]
        public static extern int EmptyClipboard();

        //Esta API fecha a área de tranferência após utilização. 
        [DllImport("user32", EntryPoint = "CloseClipboard")]
        public static extern int CloseClipboard();

        //Esta API recupera os dados da área de tranferência para a utilização. 
        [DllImport("user32.dll")]
        extern static IntPtr GetClipboardData(uint uFormat);

        #endregion


        #region API Constants

        //Esas constantes são predefinidas pelo SO 

        public const int WM_USER = 1024;

        public const int WM_CAP_CONNECT = 1034;
        public const int WM_CAP_DISCONNECT = 1035;
        public const int WM_CAP_GT_FRAME = 1084;
        public const int WM_CAP_COPY = 1054;

        public const int WM_CAP_START = WM_USER;

        public const int WM_CAP_DLG_VIDEOFORMAT = WM_CAP_START + 41;
        public const int WM_CAP_DLG_VIDEOSOURCE = WM_CAP_START + 42;
        public const int WM_CAP_DLG_VIDEODISPLAY = WM_CAP_START + 43;
        public const int WM_CAP_GET_VIDEOFORMAT = WM_CAP_START + 44;
        public const int WM_CAP_SET_VIDEOFORMAT = WM_CAP_START + 45;
        public const int WM_CAP_DLG_VIDEOCOMPRESSION = WM_CAP_START + 46;
        public const int WM_CAP_SET_PREVIEW = WM_CAP_START + 50;

        //public const int WM_CAP_GT_FRAME = 1084;
        private const int WM_CAP_GET_FRAME = 1084;

        private const short WM_CAP = 0x400;
        private const int WM_CAP_DRIVER_CONNECT = 0x40a;
        private const int WM_CAP_DRIVER_DISCONNECT = 0x40b;
        private const int WM_CAP_EDIT_COPY = 0x41e;
        //private const int WM_CAP_SET_PREVIEW = 0x432;
        private const int WM_CAP_SET_OVERLAY = 0x433;
        private const int WM_CAP_SET_PREVIEWRATE = 0x434;
        private const int WM_CAP_SET_SCALE = 0x435;
        private const int WS_CHILD = 0x40000000;
        private const int WS_VISIBLE = 0x10000000;

        private const int WM_CAP_SAVEDIB = WM_CAP_START + 25;

        #endregion


        public List<Device> devices = new List<Device>();

        private int index;
        private int deviceHandle;

        private bool pathNotSeted = true;
        private string filePath = "";

        public Device[] GetAllCapturesDevices()
        {
            String dName = "".PadRight(100); String dVersion = "".PadRight(100);

            for (short i = 0; i < 10; i++)
            {
                if (capGetDriverDescriptionA(i,
                ref dName, 100, ref dVersion,
                100))
                {
                    Device d = new Device(i);
                    d.Name = dName.Trim();
                    d.Version = dVersion.Trim();

                    devices.Add(d);
                }
            }
            return devices.ToArray();// (Device[])devices.ToArray(typeof(Device));
        }

        public Form1()
        {
            InitializeComponent();
            this.ClientSize = new System.Drawing.Size(640,480);
            InitCam();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        private void InitCam() 
        {
            var t = GetAllCapturesDevices();
            index = 0;
            string deviceIndex = Convert.ToString(index);
            var windowWidth = pictureBox1.ClientSize.Width;
            var windowHeight = pictureBox1.ClientSize.Height;
            deviceHandle = capCreateCaptureWindowA(deviceIndex, WS_VISIBLE | WS_CHILD, 0, 0, windowWidth,
                windowHeight, (int)(this.pictureBox1.Handle), 0);

            if (SendMessage(deviceHandle, WM_CAP_DRIVER_CONNECT, index, 0) > 0)
            {
                SendMessage(deviceHandle, WM_CAP_SET_SCALE, -1, 0);
                SendMessage(deviceHandle, WM_CAP_SET_PREVIEWRATE, 0x42, 0);
                SendMessage(deviceHandle, WM_CAP_SET_PREVIEW, -1, 0);

                SetWindowPos(deviceHandle, 1, 0, 0, windowWidth, windowHeight, 6);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*var f = new BITMAPINFO();
            f.bmiHeader = new BITMAPINFOHEADER();
            SendBitmapMessage((IntPtr)deviceHandle, WM_CAP_GET_VIDEOFORMAT, Marshal.SizeOf(f), ref f);
            f.bmiHeader.biSize = (uint)Marshal.SizeOf(f.bmiHeader);
            f.bmiHeader.biWidth = 160;
            f.bmiHeader.biHeight = 120;
            f.bmiHeader.biSizeImage = (uint)(f.bmiHeader.biWidth * f.bmiHeader.biHeight * f.bmiHeader.biBitCount / 8);
            SendBitmapMessage((IntPtr)deviceHandle, WM_CAP_SET_VIDEOFORMAT, Marshal.SizeOf(f), ref f);
             */
            SendMessage(deviceHandle, WM_CAP_DLG_VIDEOFORMAT, 0, 0);
            
        }

        private void CleanUp()
        {
            SendMessage(deviceHandle, WM_CAP_DRIVER_DISCONNECT, index, 0);

            DestroyWindow(deviceHandle);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(pathNotSeted)
            {
                var result = folderBrowserDialog1.ShowDialog();
                if (result != System.Windows.Forms.DialogResult.OK) return;
                filePath = folderBrowserDialog1.SelectedPath;
                pathNotSeted = false;
                label1.Text = filePath;
            }

            IntPtr hBmp = Marshal.StringToHGlobalAnsi(filePath+"\\"+DateTime.Now.Ticks.ToString()+".bmp");
            SendMessage(deviceHandle, WM_CAP_SAVEDIB, 0, hBmp.ToInt32());

            var g = this.CreateGraphics();
            g.Clear(Color.Red);
            Thread.Sleep(1000);
            g.Clear(Color.White);
            g.Dispose();

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CleanUp();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAPINFOHEADER
    {
        public uint biSize;
        public int biWidth;
        public int biHeight;
        public ushort biPlanes;
        public ushort biBitCount;
        public uint biCompression;
        public uint biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public uint biClrUsed;
        public uint biClrImportant;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAPINFO
    {
        public BITMAPINFOHEADER bmiHeader;
        public int bmiColors;
    }
}
