using System;
using System.Net;
using System.Net.Sockets;

namespace Vincent.UidGenerator.Utils;

public static class HostInfoHelper
{
    static HostInfoHelper()
    {
        var dockerEnv = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER");
        IsDocker = dockerEnv != null && dockerEnv.Equals("true", StringComparison.OrdinalIgnoreCase);
        HostName = Dns.GetHostName();
        Ip = GetLocalIpAddress();
        if (string.IsNullOrWhiteSpace(HostName) || string.IsNullOrWhiteSpace(Ip))
        {
            throw new SystemException($"Missing hostName or ip. hostName:{HostName},Ip:{Ip}");
        }
    }

    public static string HostName { get; }
    public static string Ip { get; }
    public static bool IsDocker { get; }

    private static string GetLocalIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
}