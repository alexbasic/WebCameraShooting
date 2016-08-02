using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    public class Device
    {
        public Device(short i)
        {
            id = i;
        }
        public short id;
        public string Name;
        public string Version;
    }
}
