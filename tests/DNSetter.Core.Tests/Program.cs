using DNSetter.Core;

// Package-free regression runner. Never calls Windows APIs or changes real DNS.
var tests = new (string Name, Func<Task> Run)[]
{
    ("Strict IPv4 validation before writing", async () =>
    {
        foreach (var value in new[] { "", "1.1.1", "0x01010101", "16843009", "1.1.1.999", "::1", "0.0.0.0", "224.1.1.1", "255.255.255.255", "1.1.1.1 & calc" })
        {
            var fake = new FakePlatform();
            await Throws<ArgumentException>(() => new DnsService(fake).SetAsync(fake.A, [value]));
            Assert(fake.Writes.Count == 0, "Invalid input caused a write.");
        }
    }),
    ("Optional secondary, whitespace and duplicates", () =>
    {
        var config = DnsConfiguration.Manual([" 1.1.1.1 ", "", "1.1.1.1", "1.0.0.1"]);
        Assert(config.Servers.SequenceEqual(new[] { "1.1.1.1", "1.0.0.1" }), "Normalization failed.");
        Assert(DnsConfiguration.Manual(["127.0.0.1", "10.202.10.10"]).Servers.Count == 2, "Local resolvers rejected.");
        return Task.CompletedTask;
    }),
    ("Set changes only the selected adapter", async () =>
    {
        var fake = new FakePlatform();
        var other = fake.States[fake.B];
        var result = await new DnsService(fake).SetAsync(fake.A, ["1.1.1.1", "1.0.0.1"]);
        Assert(fake.Writes.All(x => x.Id == fake.A), "Another adapter was written.");
        Assert(fake.States[fake.B] == other, "Other adapter changed.");
        Assert(result.State.Configuration.Servers.SequenceEqual(new[] { "1.1.1.1", "1.0.0.1" }), "Incorrect result.");
        Assert(result.State.EffectiveIpv6Servers.SequenceEqual(new[] { "2001:db8::53" }), "IPv6 changed.");
        Assert(fake.FlushCount == 1, "Cache was not flushed.");
    }),
    ("Reset writes Automatic even with static IP configuration", async () =>
    {
        var fake = new FakePlatform();
        var result = await new DnsService(fake).ResetAsync(fake.A);
        Assert(result.State.Configuration.Mode == DnsMode.Automatic, "Reset did not set automatic DNS.");
        Assert(fake.Writes.Single().Id == fake.A, "Reset touched another adapter.");
        Assert(result.State.EffectiveIpv4Servers.Single() == "192.168.1.1", "Automatic DNS should still show the learned server.");
    }),
    ("Reads do not combine adapters", async () =>
    {
        var fake = new FakePlatform();
        var state = await new DnsService(fake).ReadAsync(fake.B);
        Assert(state.EffectiveIpv4Servers.Single() == "9.9.9.9", "Adapters were combined.");
        Assert(fake.Writes.Count == 0, "Read modified settings.");
    }),
    ("Disconnected adapter is rejected before writing", async () =>
    {
        var fake = new FakePlatform();
        fake.States[fake.A] = fake.States[fake.A] with { Adapter = fake.States[fake.A].Adapter with { IsUp = false } };
        await Throws<InvalidOperationException>(() => new DnsService(fake).ResetAsync(fake.A));
        Assert(fake.Writes.Count == 0, "Disconnected adapter was written.");
    }),
    ("Disappeared adapter never falls back to another one", async () =>
    {
        var fake = new FakePlatform();
        fake.States.Remove(fake.A);
        await Throws<InvalidOperationException>(() => new DnsService(fake).SetAsync(fake.A, ["1.1.1.1"]));
        Assert(fake.Writes.Count == 0, "Missing adapter caused fallback write.");
    }),
    ("Mismatched adapter identity is rejected before writing", async () =>
    {
        var fake = new FakePlatform();
        fake.States[fake.A] = fake.States[fake.B];
        await Throws<InvalidOperationException>(() => new DnsService(fake).ResetAsync(fake.A));
        Assert(fake.Writes.Count == 0, "Unexpected identity caused a write.");
    }),
    ("Partial set restores every original manual DNS in order", async () =>
    {
        var fake = new FakePlatform { FailFirstWriteAfterMutation = true };
        var before = fake.States[fake.A].Configuration;
        var ex = await Throws<DnsChangeException>(() => new DnsService(fake).SetAsync(fake.A, ["1.1.1.1", "1.0.0.1"]));
        Assert(ex.RollbackSucceeded, "Rollback was not verified.");
        Assert(before.Matches(fake.States[fake.A].Configuration), "Original DNS order/list lost.");
        Assert(fake.Writes.All(x => x.Id == fake.A) && fake.Writes.Count == 2, "Incorrect rollback target.");
    }),
    ("Partial set restores automatic DNS mode", async () =>
    {
        var fake = new FakePlatform { FailFirstWriteAfterMutation = true };
        fake.States[fake.A] = fake.States[fake.A] with { Configuration = DnsConfiguration.Automatic };
        var ex = await Throws<DnsChangeException>(() => new DnsService(fake).SetAsync(fake.A, ["1.1.1.1"]));
        Assert(ex.RollbackSucceeded && fake.States[fake.A].Configuration.Mode == DnsMode.Automatic, "Automatic mode lost.");
    }),
    ("Failed reset restores manual settings", async () =>
    {
        var fake = new FakePlatform { FailFirstWriteAfterMutation = true };
        var before = fake.States[fake.A].Configuration;
        await Throws<DnsChangeException>(() => new DnsService(fake).ResetAsync(fake.A));
        Assert(before.Matches(fake.States[fake.A].Configuration), "Reset failure lost manual settings.");
    }),
    ("Successful command with wrong settings is not a success", async () =>
    {
        var fake = new FakePlatform { IgnoreFirstWrite = true };
        var before = fake.States[fake.A].Configuration;
        var ex = await Throws<DnsChangeException>(() => new DnsService(fake).SetAsync(fake.A, ["1.1.1.1"]));
        Assert(ex.RollbackSucceeded && before.Matches(fake.States[fake.A].Configuration), "Verification failed to roll back.");
    }),
    ("Configured DNS with wrong effective addresses fails verification", async () =>
    {
        var fake = new FakePlatform { WrongEffectiveFirstWrite = true };
        await Throws<DnsChangeException>(() => new DnsService(fake).SetAsync(fake.A, ["1.1.1.1"]));
        Assert(fake.Writes.Count == 2, "Incorrect effective DNS was accepted.");
    }),
    ("Rollback failure reports original adapter and DNS", async () =>
    {
        var fake = new FakePlatform { FailEveryWrite = true };
        var ex = await Throws<DnsChangeException>(() => new DnsService(fake).ResetAsync(fake.A));
        Assert(!ex.RollbackSucceeded && ex.Message.Contains(fake.A.ToString()) && ex.Message.Contains("8.8.4.4"), "Missing recovery details.");
        Assert(fake.FlushCount == 0, "Failure was treated as success.");
    }),
    ("Cache flush failure keeps verified DNS and returns warning", async () =>
    {
        var fake = new FakePlatform { FailFlush = true };
        var result = await new DnsService(fake).SetAsync(fake.A, ["1.1.1.1"]);
        Assert(result.Warning != null && fake.Writes.Count == 1, "Cache failure should not roll back DNS.");
        Assert(result.State.Configuration.Servers.Single() == "1.1.1.1", "Verified DNS lost.");
    }),
    ("Concurrent changes are serialized including rollback", async () =>
    {
        var fake = new FakePlatform { WriteDelay = 25 };
        var service = new DnsService(fake);
        await Task.WhenAll(service.SetAsync(fake.A, ["1.1.1.1"]), service.SetAsync(fake.B, ["9.9.9.9"]));
        Assert(fake.MaxConcurrentWrites == 1, "Concurrent network writes occurred.");
    }),
    ("Lock is released after a failed operation", async () =>
    {
        var fake = new FakePlatform { FailFirstWriteAfterMutation = true };
        var service = new DnsService(fake);
        await Throws<DnsChangeException>(() => service.SetAsync(fake.A, ["1.1.1.1"]));
        await service.SetAsync(fake.B, ["9.9.9.9"]).WaitAsync(TimeSpan.FromSeconds(2));
    })
};
var failed = 0;
foreach (var test in tests)
{
    try { await test.Run(); Console.WriteLine($"PASS {test.Name}"); }
    catch (Exception ex) { failed++; Console.Error.WriteLine($"FAIL {test.Name}: {ex}"); }
}
Console.WriteLine($"{tests.Length - failed}/{tests.Length} tests passed.");
return failed == 0 ? 0 : 1;

static void Assert(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}

static async Task<T> Throws<T>(Func<Task> action) where T : Exception
{
    try { await action(); }
    catch (T ex) { return ex; }
    throw new Exception($"Expected {typeof(T).Name}.");
}

sealed class FakePlatform : INetworkDnsPlatform
{
    public Guid A { get; } = Guid.NewGuid();
    public Guid B { get; } = Guid.NewGuid();
    public Dictionary<Guid, AdapterDnsState> States { get; } = new();
    public List<(Guid Id, DnsConfiguration Configuration)> Writes { get; } = new();
    public int FlushCount { get; private set; }
    public bool FailFirstWriteAfterMutation { get; init; }
    public bool FailEveryWrite { get; init; }
    public bool IgnoreFirstWrite { get; init; }
    public bool WrongEffectiveFirstWrite { get; init; }
    public bool FailFlush { get; init; }
    public int WriteDelay { get; init; }
    public int MaxConcurrentWrites { get; private set; }
    private int concurrentWrites;

    public FakePlatform()
    {
        States[A] = MakeState(A, "Ethernet", ["8.8.8.8", "8.8.4.4", "192.168.1.53"]);
        States[B] = MakeState(B, "Wi-Fi", ["9.9.9.9"]);
    }
    private static AdapterDnsState MakeState(Guid id, string name, string[] servers) => new(
        new(id, 1, name, name, "Ethernet", true, true), DnsConfiguration.Manual(servers), servers, ["2001:db8::53"]);
    public Task<IReadOnlyList<NetworkAdapter>> GetAdaptersAsync() =>
        Task.FromResult<IReadOnlyList<NetworkAdapter>>(States.Values.Select(x => x.Adapter).ToArray());
    public Task<AdapterDnsState> ReadAsync(Guid id) => Task.FromResult(States.TryGetValue(id, out var state)
        ? state : throw new InvalidOperationException("Adapter missing."));
    public async Task WriteAsync(Guid id, DnsConfiguration configuration)
    {
        MaxConcurrentWrites = Math.Max(MaxConcurrentWrites, ++concurrentWrites);
        try
        {
            if (WriteDelay > 0) await Task.Delay(WriteDelay);
            Writes.Add((id, configuration));
            if (FailEveryWrite) throw new InvalidOperationException("Access denied.");
            if (IgnoreFirstWrite && Writes.Count == 1) return;
            var effective = configuration.Mode == DnsMode.Automatic ? new[] { "192.168.1.1" } : configuration.Servers;
            if (WrongEffectiveFirstWrite && Writes.Count == 1) effective = new[] { "192.168.1.99" };
            States[id] = States[id] with { Configuration = configuration, EffectiveIpv4Servers = effective };
            if (FailFirstWriteAfterMutation && Writes.Count == 1) throw new InvalidOperationException("Second server failed.");
        }
        finally { concurrentWrites--; }
    }
    public Task FlushCacheAsync()
    {
        FlushCount++;
        return FailFlush ? Task.FromException(new InvalidOperationException("Cache flush failed.")) : Task.CompletedTask;
    }
}
