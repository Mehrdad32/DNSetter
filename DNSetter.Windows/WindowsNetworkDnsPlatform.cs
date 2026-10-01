using System.Diagnostics;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Principal;
using Microsoft.Win32;
using DNSetter.Core;

namespace DNSetter.Windows;

public sealed class WindowsNetworkDnsPlatform : INetworkDnsPlatform
{
    public Task<IReadOnlyList<NetworkAdapter>> GetAdaptersAsync() => Task.Run<IReadOnlyList<NetworkAdapter>>(() =>
    {
        var result = new List<NetworkAdapter>();
        foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (nic.NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                !nic.Supports(NetworkInterfaceComponent.IPv4) || !Guid.TryParse(nic.Id, out var id)) continue;
            try { result.Add(ToAdapter(nic, id)); }
            catch (NetworkInformationException) { /* Adapter disappeared during enumeration. */ }
        }
        return result.OrderByDescending(x => x.IsUp).ThenByDescending(x => x.HasIpv4Gateway)
            .ThenBy(x => x.Name, StringComparer.CurrentCultureIgnoreCase).ToArray();
    });

    public Task<AdapterDnsState> ReadAsync(Guid adapterId) => Task.Run(() =>
    {
        var nic = Resolve(adapterId);
        var adapter = ToAdapter(nic, adapterId);
        var properties = nic.GetIPProperties();
        using var key = Registry.LocalMachine.OpenSubKey(
            $@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces\{adapterId:B}");
        if (key is null)
            throw new InvalidOperationException("Cannot read the IPv4 DNS configuration for the selected adapter.");
        var manual = (key.GetValue("NameServer") as string ?? "")
            .Split([',', ';', ' ', '\t', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        var configuration = manual.Length == 0 ? DnsConfiguration.Automatic : DnsConfiguration.Manual(manual);
        return new AdapterDnsState(adapter, configuration,
            properties.DnsAddresses.Where(x => x.AddressFamily == AddressFamily.InterNetwork)
                .Select(x => x.ToString()).ToArray(),
            properties.DnsAddresses.Where(x => x.AddressFamily == AddressFamily.InterNetworkV6)
                .Select(x => x.ToString()).ToArray());
    });

    public async Task WriteAsync(Guid adapterId, DnsConfiguration configuration)
    {
        using var identity = WindowsIdentity.GetCurrent();
        if (!new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator))
            throw new InvalidOperationException("Run DNSetter as Administrator to change DNS.");
        // Revalidate even if the caller constructed the record directly.
        if (configuration.Mode == DnsMode.Manual)
            configuration = DnsConfiguration.Manual(configuration.Servers);
        else if (configuration.Mode != DnsMode.Automatic)
            throw new ArgumentException("Unknown DNS configuration mode.");

        if (configuration.Mode == DnsMode.Automatic)
        {
            await RunAsync("netsh.exe", ["interface", "ipv4", "set", "dnsservers", InterfaceArgument(adapterId), "source=dhcp"]);
            return;
        }
        await RunAsync("netsh.exe", ["interface", "ipv4", "set", "dnsservers", InterfaceArgument(adapterId),
            "source=static", $"address={configuration.Servers[0]}", "validate=no"]);
        for (var i = 1; i < configuration.Servers.Count; i++)
            await RunAsync("netsh.exe", ["interface", "ipv4", "add", "dnsservers", InterfaceArgument(adapterId),
                $"address={configuration.Servers[i]}", $"index={(i + 1).ToString(CultureInfo.InvariantCulture)}", "validate=no"]);
    }

    public Task FlushCacheAsync() => RunAsync("ipconfig.exe", ["/flushdns"]);

    private static string InterfaceArgument(Guid adapterId) =>
        $"name={ToAdapter(Resolve(adapterId), adapterId).InterfaceIndex.ToString(CultureInfo.InvariantCulture)}";

    private static NetworkInterface Resolve(Guid adapterId) => NetworkInterface.GetAllNetworkInterfaces()
        .FirstOrDefault(x => Guid.TryParse(x.Id, out var id) && id == adapterId &&
            x.NetworkInterfaceType != NetworkInterfaceType.Loopback && x.Supports(NetworkInterfaceComponent.IPv4))
        ?? throw new InvalidOperationException("The selected adapter no longer exists. Refresh the adapter list.");

    private static NetworkAdapter ToAdapter(NetworkInterface nic, Guid id)
    {
        var properties = nic.GetIPProperties();
        var ipv4 = properties.GetIPv4Properties()
            ?? throw new InvalidOperationException("The selected adapter has no IPv4 interface.");
        return new(id, ipv4.Index, nic.Name, nic.Description, nic.NetworkInterfaceType.ToString(),
            nic.OperationalStatus == OperationalStatus.Up,
            properties.GatewayAddresses.Any(x => x.Address.AddressFamily == AddressFamily.InterNetwork &&
                !x.Address.Equals(System.Net.IPAddress.Any)));
    }

    private static async Task RunAsync(string executable, IEnumerable<string> arguments)
    {
        // Explicit system path, no shell, no interpolated adapter names, no repeated UAC prompts.
        var start = new ProcessStartInfo(Path.Combine(Environment.SystemDirectory, executable))
        {
            UseShellExecute = false, CreateNoWindow = true,
            RedirectStandardOutput = true, RedirectStandardError = true
        };
        foreach (var argument in arguments) start.ArgumentList.Add(argument);
        using var process = Process.Start(start) ?? throw new InvalidOperationException($"Could not start {executable}.");
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        try { await process.WaitForExitAsync(timeout.Token); }
        catch (OperationCanceledException)
        {
            if (!process.HasExited) process.Kill(entireProcessTree: true);
            await process.WaitForExitAsync();
            await Task.WhenAll(stdout, stderr);
            throw new TimeoutException($"{executable} did not finish within 15 seconds.");
        }
        var output = (await stdout).Trim();
        var error = (await stderr).Trim();
        if (process.ExitCode != 0)
            throw new InvalidOperationException($"{executable} exited with code {process.ExitCode}. {output} {error}".Trim());
    }
}
