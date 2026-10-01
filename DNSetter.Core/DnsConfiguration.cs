using System.Net;
using System.Net.Sockets;

namespace DNSetter.Core;

public enum DnsMode { Automatic, Manual }

public sealed record DnsConfiguration(DnsMode Mode, IReadOnlyList<string> Servers)
{
    public static DnsConfiguration Automatic { get; } = new(DnsMode.Automatic, Array.Empty<string>());

    public static DnsConfiguration Manual(IEnumerable<string> addresses)
    {
        var servers = addresses.Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(ParseServer).Distinct(StringComparer.Ordinal).ToArray();
        if (servers.Length == 0)
            throw new ArgumentException("Enter at least one IPv4 DNS server.");
        return new(DnsMode.Manual, servers);
    }

    private static string ParseServer(string value)
    {
        var text = value.Trim();
        // IPAddress.TryParse also accepts shorthand, integer and hexadecimal IPv4 forms.
        var parts = text.Split('.');
        if (parts.Length != 4 || parts.Any(p => p.Length == 0 || p.Any(c => c < '0' || c > '9')) ||
            !IPAddress.TryParse(text, out var ip) || ip.AddressFamily != AddressFamily.InterNetwork)
            throw new ArgumentException($"Invalid IPv4 DNS server: {text}");
        var bytes = ip.GetAddressBytes();
        if (bytes[0] == 0 || bytes[0] >= 224 || ip.Equals(IPAddress.Broadcast))
            throw new ArgumentException($"DNS server must be an IPv4 unicast address: {text}");
        // Loopback is valid for a local DNS resolver. Private addresses are valid too.
        return ip.ToString();
    }

    public bool Matches(DnsConfiguration other) => Mode == other.Mode &&
        (Mode == DnsMode.Automatic || Servers.SequenceEqual(other.Servers, StringComparer.Ordinal));
}
