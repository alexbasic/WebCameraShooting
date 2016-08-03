using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    public class DeviceDescription
    {
        public short Id { get;set; }
        public string Name { get; set; }
        public string Version { get; set; }

        public DeviceDescription(short deviceId)
        {
            Id = deviceId;
        }
    }
}
