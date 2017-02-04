using ServerViewWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.IO;
using System.Diagnostics;

namespace ServerViewWPF.ViewModel
{
    public class WMIManager
    {
        public Server WMICall(string servername)
        {
            // Create Server obj.
            Server server = new Server();
            ConnectionOptions options = new ConnectionOptions();
            // bruger den konto som programmet bliver kørt af.
            options.Impersonation = System.Management.ImpersonationLevel.Impersonate;
            // Viser hvor vi vil søge henne, dette er WMI root på servern
            ManagementScope scope = new ManagementScope("\\\\" + servername + "\\root\\cimv2", options);
            // Forbinder til server
            try
            {
                scope.Connect();

                //Query system for Operating System information
                ObjectQuery OSquery = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
                ManagementObjectSearcher OSsearcher = new ManagementObjectSearcher(scope, OSquery);
                ManagementObjectCollection OSqueryCollection = OSsearcher.Get();
                foreach (ManagementObject OS in OSqueryCollection)
                {
                    server.Name = OS.GetPropertyValue("csname").ToString();
                    server.OsVer = OS.GetPropertyValue("Caption").ToString();
                }
                //Query system for Computer System information
                ObjectQuery CSquery = new ObjectQuery("SELECT * FROM Win32_ComputerSystem");
                ManagementObjectSearcher CSsearcher = new ManagementObjectSearcher(scope, CSquery);
                ManagementObjectCollection CSqueryCollection = CSsearcher.Get();
                foreach (ManagementObject CS in CSqueryCollection)
                {
                    server.Ram = (Convert.ToInt64(CS.GetPropertyValue("TotalPhysicalMemory"))) / 1048576;
                }
                //Query system for DiskDrive information
                ObjectQuery LDquery = new ObjectQuery("SELECT * FROM Win32_LogicalDisk");
                ManagementObjectSearcher LDsearcher = new ManagementObjectSearcher(scope, LDquery);
                ManagementObjectCollection LDqueryCollection = LDsearcher.Get();
                foreach (ManagementObject LD in LDqueryCollection)
                {
                    Harddisk harddisk = new Harddisk();
                    harddisk.DriveLetter = LD["DeviceID"].ToString();
                    harddisk.MbSize = (Convert.ToInt64(LD["Size"])) / 1048576;
                    server.Hdd.Add(harddisk);
                }
                //Query system for network information
                ObjectQuery NCquery = new ObjectQuery("SELECT MacAddress,IPAddress FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'");
                ManagementObjectSearcher NCsearcher = new ManagementObjectSearcher(scope, NCquery);
                ManagementObjectCollection NCqueryCollection = NCsearcher.Get();
                foreach (ManagementObject NC in NCqueryCollection)
                {
                    NetWorkCard netWorkCard = new NetWorkCard();
                    int nrIP = ((System.Array)(NC["IPAddress"])).Length;
                    for (int c = 0; c < nrIP; c++)
                    {
                        netWorkCard.IpAddress = ((Array)(NC["IPAddress"])).GetValue(c).ToString();
                        netWorkCard.MacAddress = NC["MacAddress"].ToString();
                    }
                    server.NetWorkCard.Add(netWorkCard);
                }
                // Return Server obj.
                return server;
            }
            catch
            {
                Debug.WriteLine("WMI Error");
                return null;
            } 
        }
    }
}
