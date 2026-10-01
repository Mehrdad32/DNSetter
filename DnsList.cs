using System.Net.NetworkInformation;

namespace DNSetter;

public partial class DnsList : Form
{
    private readonly List<DnsEntry> dnsEntries;
    private CancellationTokenSource? cancellation;

    public DnsList(List<DnsEntry> entries)
    {
        InitializeComponent();
        UiTheme.Apply(this);
        dnsEntries = entries.Select(x => new DnsEntry { Name = x.Name, IPs = [.. x.IPs] }).ToList();
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        using var source = new CancellationTokenSource();
        cancellation = source;
        try { await LoadPingResultsAsync(source.Token); }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            if (!IsDisposed) ResultsStatusLabel.Text = $"Test failed: {ex.Message}";
        }
        finally { cancellation = null; }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        if (!e.Cancel) cancellation?.Cancel();
    }

    private async Task LoadPingResultsAsync(CancellationToken token)
    {
        DnsListGrid.Columns.Clear();
        DnsListGrid.Columns.Add("Service", "Preset");
        DnsListGrid.Columns.Add("IP1", "Primary DNS");
        DnsListGrid.Columns.Add("Ping1", "Ping / status");
        DnsListGrid.Columns.Add("IP2", "Secondary DNS");
        DnsListGrid.Columns.Add("Ping2", "Ping / status");
        foreach (DataGridViewColumn column in DnsListGrid.Columns)
        {
            column.MinimumWidth = column.Index == 0 ? 130 : 110;
            column.FillWeight = column.Index == 0 ? 130 : 100;
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
        }
        DnsListGrid.Columns[1].DefaultCellStyle.Font = new Font("Consolas", 10F);
        DnsListGrid.Columns[3].DefaultCellStyle.Font = DnsListGrid.Columns[1].DefaultCellStyle.Font;
        foreach (var entry in dnsEntries)
            DnsListGrid.Rows.Add(entry.Name, entry.IPs.ElementAtOrDefault(0) ?? "", "Waiting…",
                entry.IPs.ElementAtOrDefault(1) ?? "", "Waiting…");
        ResultsProgress.Maximum = Math.Max(1, dnsEntries.Count);
        ResultsStatusLabel.Text = $"Testing 0 of {dnsEntries.Count} presets…";
        using var limit = new SemaphoreSlim(4);
        var completed = 0;
        var tasks = dnsEntries.Select(async (entry, index) =>
        {
            await limit.WaitAsync(token);
            try
            {
                var values = await Task.WhenAll(entry.IPs.Take(2).Select(ip => PingAddressAsync(ip, token)));
                token.ThrowIfCancellationRequested();
                if (IsDisposed) return;
                DnsListGrid.Rows[index].Cells[2].Value = values.ElementAtOrDefault(0) ?? "—";
                DnsListGrid.Rows[index].Cells[4].Value = values.ElementAtOrDefault(1) ?? "—";
                ResultsProgress.Value = ++completed;
                ResultsStatusLabel.Text = $"Tested {completed} of {dnsEntries.Count} presets";
            }
            finally { limit.Release(); }
        });
        await Task.WhenAll(tasks);
        if (!IsDisposed)
        {
            ResultsStatusLabel.Text = $"Finished · {completed} presets";
            foreach (DataGridViewColumn column in DnsListGrid.Columns)
                column.SortMode = DataGridViewColumnSortMode.Automatic;
        }
    }

    private static async Task<string> PingAddressAsync(string ip, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(ip)) return "—";
        try
        {
            using var ping = new Ping();
            var reply = await ping.SendPingAsync(ip, 1500);
            token.ThrowIfCancellationRequested();
            return reply.Status == IPStatus.Success ? $"{reply.RoundtripTime} ms" : reply.Status.ToString();
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception) { return "No ICMP reply"; }
    }

    private void CloseButton_Click(object? sender, EventArgs e) => Close();
}
