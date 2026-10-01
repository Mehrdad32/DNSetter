namespace DNSetter.Core;

public sealed record NetworkAdapter(Guid Id, int InterfaceIndex, string Name, string Description,
    string Kind, bool IsUp, bool HasIpv4Gateway)
{
    public string DisplayName => $"{Name} — {Kind} / {(IsUp ? "Connected" : "Disconnected")}";
}

public sealed record AdapterDnsState(NetworkAdapter Adapter, DnsConfiguration Configuration,
    IReadOnlyList<string> EffectiveIpv4Servers, IReadOnlyList<string> EffectiveIpv6Servers);

public interface INetworkDnsPlatform
{
    Task<IReadOnlyList<NetworkAdapter>> GetAdaptersAsync();
    Task<AdapterDnsState> ReadAsync(Guid adapterId);
    // Implementations must resolve adapterId again before every write, never use a cached index.
    Task WriteAsync(Guid adapterId, DnsConfiguration configuration);
    Task FlushCacheAsync();
}
