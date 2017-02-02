using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerViewWPF.Model
{
    class NetWorkCard
    {
        string ipAddress;
        string macAddress;
        public string IpAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }
        public string MacAddress
        {
            get { return macAddress; }
            set { macAddress = value; }
        }
    }
}
