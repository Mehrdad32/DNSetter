namespace DNSetter.Core;

public sealed record DnsChangeResult(AdapterDnsState State, string? Warning);

public sealed class DnsChangeException(string message, Exception cause, bool rollbackSucceeded)
    : Exception(message, cause)
{
    public bool RollbackSucceeded { get; } = rollbackSucceeded;
}

public sealed class DnsService(INetworkDnsPlatform platform)
{
    private readonly SemaphoreSlim changeLock = new(1, 1);

    public Task<IReadOnlyList<NetworkAdapter>> GetAdaptersAsync() => platform.GetAdaptersAsync();
    public Task<AdapterDnsState> ReadAsync(Guid adapterId) => platform.ReadAsync(adapterId);

    public Task<DnsChangeResult> SetAsync(Guid adapterId, IEnumerable<string> servers) =>
        ChangeAsync(adapterId, DnsConfiguration.Manual(servers));

    public Task<DnsChangeResult> ResetAsync(Guid adapterId) =>
        ChangeAsync(adapterId, DnsConfiguration.Automatic);

    private async Task<DnsChangeResult> ChangeAsync(Guid adapterId, DnsConfiguration requested)
    {
        await changeLock.WaitAsync();
        try
        {
            // A fresh snapshot is mandatory: a previously selected adapter may have disappeared.
            var before = await platform.ReadAsync(adapterId);
            if (!before.Adapter.IsUp)
                throw new InvalidOperationException("The selected adapter is disconnected. Refresh and select a connected adapter.");
            if (before.Adapter.Id != adapterId)
                throw new InvalidOperationException("The network adapter identity changed. No settings were written.");

            AdapterDnsState after;
            try
            {
                await platform.WriteAsync(adapterId, requested);
                after = await VerifyAsync(adapterId, requested);
            }
            catch (Exception changeError)
            {
                try
                {
                    await platform.WriteAsync(adapterId, before.Configuration);
                    await VerifyAsync(adapterId, before.Configuration);
                }
                catch (Exception rollbackError)
                {
                    var original = before.Configuration.Mode == DnsMode.Automatic
                        ? "Automatic (DHCP)" : string.Join(", ", before.Configuration.Servers);
                    throw new DnsChangeException(
                        $"DNS change failed: {changeError.Message}\nAutomatic rollback also failed: {rollbackError.Message}\n" +
                        $"Check Windows network settings for '{before.Adapter.Name}' ({adapterId}).\nPrevious IPv4 DNS: {original}",
                        new AggregateException(changeError, rollbackError), false);
                }
                throw new DnsChangeException(
                    $"DNS change failed: {changeError.Message}\nThe previous IPv4 DNS configuration was restored on '{before.Adapter.Name}'.",
                    changeError, true);
            }

            string? warning = null;
            try { await platform.FlushCacheAsync(); }
            catch (Exception ex) { warning = $"DNS was changed and verified, but the DNS cache could not be cleared: {ex.Message}"; }
            return new(after, warning);
        }
        finally { changeLock.Release(); }
    }

    private async Task<AdapterDnsState> VerifyAsync(Guid adapterId, DnsConfiguration expected)
    {
        for (var attempt = 0; attempt < 6; attempt++)
        {
            var state = await platform.ReadAsync(adapterId);
            if (state.Adapter.Id == adapterId && expected.Matches(state.Configuration) &&
                (expected.Mode == DnsMode.Automatic ||
                 expected.Servers.SequenceEqual(state.EffectiveIpv4Servers, StringComparer.Ordinal)))
                return state;
            if (attempt < 5) await Task.Delay(200);
        }
        throw new InvalidOperationException("Windows did not report the requested IPv4 DNS configuration after the command completed.");
    }
}
