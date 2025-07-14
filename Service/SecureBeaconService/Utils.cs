using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace SecureBeaconService
{
    internal static class Utils
    {
        public static Uri UriFromAddressAndPort(string address, int port)
        {
            if (address.StartsWith("http") == false)
            {
                address = $"{(port == 80 ? "http": "https")}://{address}";
            }
            var uri = new Uri(address);
            var pathAndQueryIndex = address.IndexOf(uri.PathAndQuery);
            if (pathAndQueryIndex != -1)
            {
                address = address.Substring(0, pathAndQueryIndex) + $":{port}{uri.PathAndQuery}";
                uri = new Uri(address);
            }
            return uri;
        }

        public static string GetLikelyPublicIPAddress()
        {
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Skip interfaces that are not up or are loopback/tunnel
                if (ni.OperationalStatus != OperationalStatus.Up ||
                    ni.NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                    ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel)
                    continue;

                var ipProps = ni.GetIPProperties();

                // Let's filter out anything that has no default gateway address as this probably isn't the most likely public IP address.
                if (!ipProps.GatewayAddresses.Any(g => g.Address.AddressFamily == AddressFamily.InterNetwork &&
                                                        !IPAddress.IsLoopback(g.Address) &&
                                                        !g.Address.Equals(IPAddress.Any)))
                    continue;

                // Look for a suitable IPv4 address
                foreach (var addr in ipProps.UnicastAddresses)
                {
                    if (addr.Address.AddressFamily == AddressFamily.InterNetwork &&
                        !IPAddress.IsLoopback(addr.Address))
                    {
                        return addr.Address.ToString();
                    }
                }
            }

            return "127.0.0.1"; // Hardly ideal, but fallback to localhost if no public IP is found.
        }


    }
}
