using System.Drawing.Imaging;
using DNSetter;
using DNSetter.Core;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        var output = Path.GetFullPath(args.FirstOrDefault() ?? "ui-preview");
        Directory.CreateDirectory(output);
        var platform = new PreviewPlatform();
        using var main = new MainForm(new DnsService(platform));
        Exception? failure = null;
        main.Shown += async (_, _) =>
        {
            try
            {
                await Task.Yield();
                var provider = Find<ComboBox>(main, "DnsList");
                var primary = Find<TextBox>(main, "DnsTextOne");
                var secondary = Find<TextBox>(main, "DnsTextTwo");
                var current = Find<TextBox>(main, "CurrentAdapterDnsLabel");
                var apply = Find<Button>(main, "SetButton");
                provider.SelectedItem = "Cloudflare";
                Require(primary.Text == "1.1.1.1" && secondary.Text == "1.0.0.1", "Preset selection failed.");
                Require(current.Text.Contains("8.8.8.8"), "Pending input was displayed as current DNS.");
                Require(apply.Enabled, "Valid preset cannot be applied.");
                Capture(main, Path.Combine(output, "main.png"));
                primary.Text = "invalid";
                Require(!apply.Enabled, "Invalid DNS can be applied.");
                primary.Text = "1.1.1.1";
                apply.PerformClick();
                await Task.Yield();
                Require(platform.Writes == 1 && current.Text.Contains("1.1.1.1"), "Apply did not update the selected adapter.");
                Find<Button>(main, "UnsetDnsButton").PerformClick();
                await Task.Yield();
                Require(platform.State.Configuration.Mode == DnsMode.Automatic, "Automatic DNS failed.");
                Require(platform.State.EffectiveIpv6Servers.Single() == "2001:db8::53", "IPv6 changed.");
                provider.SelectedItem = "Cloudflare";
                main.ClientSize = new Size(680, 600);
                Capture(main, Path.Combine(output, "main-compact.png"));
                var status = Find<Label>(main, "OperationStatusLabel");
                var statusBounds = main.RectangleToClient(status.RectangleToScreen(status.ClientRectangle));
                Require(statusBounds.Bottom <= main.ClientRectangle.Bottom && statusBounds.Top >= 0,
                    "Operation result is outside the visible compact window.");
                main.Font = new Font("Segoe UI", 15F);
                main.ClientSize = new Size(1000, 1000);
                Capture(main, Path.Combine(output, "main-large-text.png"));

                using var results = new DnsList([]);
                results.Show(main);
                await Task.Yield();
                var grid = Find<DataGridView>(results, "DnsListGrid");
                grid.Rows.Add("Cloudflare", "1.1.1.1", "12 ms", "1.0.0.1", "14 ms");
                grid.Rows.Add("Google", "8.8.8.8", "21 ms", "8.8.4.4", "22 ms");
                grid.Rows.Add("Local preset", "192.0.2.53", "TimedOut", "", "—");
                Find<Label>(results, "ResultsStatusLabel").Text = "Finished · 3 presets (preview data)";
                Capture(results, Path.Combine(output, "results.png"));
                results.Close();
                Console.WriteLine("PASS UI smoke: preset/input separation, validation, Apply, Automatic and IPv6 preservation.");
                Console.WriteLine("Captured native WinForms previews at normal, compact and larger text sizes.");
            }
            catch (Exception ex) { failure = ex; Console.Error.WriteLine(ex); }
            finally { main.Close(); }
        };
        Application.Run(main);
        return failure == null ? 0 : 1;
    }

    private static T Find<T>(Control root, string name) where T : Control =>
        (T)root.Controls.Find(name, true).Single();

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static void Capture(Form form, string path)
    {
        form.PerformLayout();
        Application.DoEvents();
        // Capture the complete form including non-client frame, without using screenshots of the runner desktop.
        using var full = new Bitmap(form.Width, form.Height);
        form.DrawToBitmap(full, new Rectangle(Point.Empty, full.Size));
        full.Save(path, ImageFormat.Png);
    }
}

internal sealed class PreviewPlatform : INetworkDnsPlatform
{
    private static readonly Guid Id = Guid.Parse("10000000-0000-0000-0000-000000000001");
    public AdapterDnsState State { get; private set; } = new(
        new(Id, 12, "Ethernet", "Intel Ethernet adapter", "Ethernet", true, true),
        DnsConfiguration.Manual(["8.8.8.8", "8.8.4.4"]), ["8.8.8.8", "8.8.4.4"], ["2001:db8::53"]);
    public int Writes { get; private set; }
    public Task<IReadOnlyList<NetworkAdapter>> GetAdaptersAsync() => Task.FromResult<IReadOnlyList<NetworkAdapter>>([State.Adapter]);
    public Task<AdapterDnsState> ReadAsync(Guid adapterId) => Task.FromResult(State);
    public Task WriteAsync(Guid adapterId, DnsConfiguration configuration)
    {
        if (adapterId != Id) throw new InvalidOperationException("Incorrect adapter selected.");
        Writes++;
        State = State with { Configuration = configuration, EffectiveIpv4Servers = configuration.Mode == DnsMode.Automatic
            ? ["192.0.2.1"] : configuration.Servers };
        return Task.CompletedTask;
    }
    public Task FlushCacheAsync() => Task.CompletedTask;
}
