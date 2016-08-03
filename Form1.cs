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
        public IEnumerable<DeviceDescription> _devices;
        private Camera _camera;

        private bool pathNotSeted = true;
        private string filePath = "";

        public Form1()
        {                  
            InitializeComponent();
            this.ClientSize = new System.Drawing.Size(640,480);

            _devices = Camera.GetListCameras();
            var cameraIndex = 0;
            _camera = new Camera(cameraIndex, pictureBox1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _camera.ShowConfigDialog(); 
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

            _camera.TakeSnapshot(filePath + "\\" + DateTime.Now.Ticks.ToString() + ".bmp");

            var g = this.CreateGraphics();
            g.Clear(Color.Red);
            Thread.Sleep(1000);
            g.Clear(Color.White);
            g.Dispose();

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _camera.Dispose();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            
        }
    }
}
