using ServerViewWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace ServerViewWPF.ViewModel
{
    class WMIManager
    {
        Server WMICall(string servername)
        {
            // Create Server obj.
            Server server = new Server();
            ConnectionOptions options = new ConnectionOptions();
            // bruger den konto som programmet bliver kørt af.
            options.Impersonation = System.Management.ImpersonationLevel.Impersonate;
            // Viser hvor vi vil søge henne, dette er WMI root på servern
            ManagementScope scope = new ManagementScope("\\\\" + servername + "\\root\\cimv2", options);
            // Forbinder til server
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


            // Return Server obj.
            return server;
        }
    }
}
