using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OMSServices.Utils
{
    public class AssemblyHelper
    {
        public static string GetEntryAssemblyVersion()
        {
            // source link: https://stackoverflow.com/a/36351902
            return System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
        }

        public static string GetIPV4AddressesOfHost()
        {
            // source link: https://www.geeksforgeeks.org/c-sharp-program-to-find-the-ip-address-of-the-machine/
            // Get the Name of HOST
            string hostName = Dns.GetHostName();
            var ipv4AddressesList = Dns.GetHostEntry(hostName).AddressList.Where(x => x.AddressFamily.Equals(AddressFamily.InterNetwork)).Select(x => x.ToString());
            string ipv4Addresses = string.Join(", ", ipv4AddressesList);
            return ipv4Addresses;
        }
    }
}
