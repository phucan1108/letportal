using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace LetPortal.Core.Utils
{
    public class NetUtil
    {
        public static string GetHostIp()
        {
            var name = Dns.GetHostName();
            return Dns.GetHostEntry(name).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork).ToString();
        }
    }
}
