using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Reflection;
using DNSetter.Core;
using DNSetter.Windows;
using System.Text.Json;

namespace DNSetter
{
    public partial class MainForm : Form
    {
        private string dnsListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "list.json");
        private List<DnsEntry> dnsEntries = new();
        private readonly DnsService dnsService;
        private bool updatingDnsInputs;
        private bool isBusy;
        private bool updatingAdapters;
        private AdapterDnsState? currentAdapterState;

        public MainForm() : this(new DnsService(new WindowsNetworkDnsPlatform())) { }

        public MainForm(DnsService service)
        {
            dnsService = service;
            InitializeComponent();
            UiTheme.Apply(this, SetButton);
            SetUIEnabled(false);
        }

        private async void MainForm_Load(object? sender, EventArgs e)
        {
            try
            {
                SetUIEnabled(false);
                VersionLabel.Text = "v" + (Assembly.GetExecutingAssembly()
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+')[0]
                    ?? "2.0.0");

                if (!File.Exists(dnsListPath))
                {
                    // Create initial file with default entries if not exists
                    var defaultEntries = new List<DnsEntry>
            {
                new DnsEntry { Name = "Electro", IPs = ["78.157.42.100", "78.157.42.101"] },
                new DnsEntry { Name = "Shelter", IPs = ["94.103.125.157", "94.103.125.158"] },
                new DnsEntry { Name = "Beshkan", IPs = ["181.41.194.177", "181.41.194.186"] },
                new DnsEntry { Name = "Shecan", IPs = ["178.22.122.100", "185.51.200.2"] },
                new DnsEntry { Name = "403 Online", IPs = ["10.202.10.202", "10.202.10.102"] },
                new DnsEntry { Name = "Begzar", IPs = ["185.55.226.26", "185.55.225.25"] },
                new DnsEntry { Name = "Radar", IPs = ["10.202.10.10", "10.202.10.11"] },
                new DnsEntry { Name = "PishGaman", IPs = ["5.202.100.100", "5.202.100.101"] },
                new DnsEntry { Name = "Shatel", IPs = ["85.15.1.14", "85.15.1.15"] },
                new DnsEntry { Name = "Level3", IPs = ["209.244.0.3", "209.244.0.4"] },
                new DnsEntry { Name = "Cloudflare", IPs = ["1.1.1.1", "1.0.0.1"] },
                new DnsEntry { Name = "Google", IPs = ["8.8.8.8", "8.8.4.4"] },
            };
                    File.WriteAllText(dnsListPath, JsonSerializer.Serialize(defaultEntries, new JsonSerializerOptions { WriteIndented = true }));
                }

                string json = File.ReadAllText(dnsListPath);
                dnsEntries = JsonSerializer.Deserialize<List<DnsEntry>>(json) ?? new();
                // Migrate only the exact incorrect built-in pair; preserve custom entries.
                var legacyGoogle = dnsEntries.FirstOrDefault(x => x.Name == "Google" &&
                    x.IPs.SequenceEqual(new[] { "8.8.8.8", "4.2.2.4" }));
                if (legacyGoogle != null)
                {
                    legacyGoogle.IPs = ["8.8.8.8", "8.8.4.4"];
                    File.WriteAllText(dnsListPath, JsonSerializer.Serialize(dnsEntries,
                        new JsonSerializerOptions { WriteIndented = true }));
                }

                DnsList.Items.Clear();
                foreach (var entry in dnsEntries)
                {
                    DnsList.Items.Add(entry.Name);
                }

                // Keep the initial window inside the current screen's working area.
                var area = Screen.FromControl(this).WorkingArea;
                Size = new Size(Math.Min(Width, area.Width), Math.Min(Height, area.Height));
                await RefreshAdaptersAsync();
            }
            catch (Exception ex)
            {
                ShowOperationError(ex);
            }
            finally { SetUIEnabled(true); }
        }

        private NetworkAdapter SelectedAdapter => AdapterList.SelectedItem as NetworkAdapter
            ?? throw new InvalidOperationException("Select a network adapter first.");

        private async Task RefreshAdaptersAsync()
        {
            var previousId = (AdapterList.SelectedItem as NetworkAdapter)?.Id;
            currentAdapterState = null;
            DnsModeLabel.Text = "No adapter selected";
            AdapterStateLabel.Text = "Choose the adapter you want to configure.";
            CurrentAdapterDnsLabel.Clear();
            SetStatus("Reading network adapters…");
            var adapters = await dnsService.GetAdaptersAsync();
            updatingAdapters = true;
            try
            {
                AdapterList.Items.Clear();
                AdapterList.DisplayMember = nameof(NetworkAdapter.DisplayName);
                foreach (var adapter in adapters) AdapterList.Items.Add(adapter);
                var selected = previousId.HasValue ? adapters.FirstOrDefault(x => x.Id == previousId) : null;
                // Gateway presence does not prove which adapter carries the default route.
                // With multiple connected adapters (including VPNs), require an explicit choice.
                if (!previousId.HasValue)
                {
                    var connected = adapters.Where(x => x.IsUp).ToArray();
                    if (connected.Length == 1) selected = connected[0];
                }
                if (selected != null) AdapterList.SelectedItem = selected;
            }
            finally { updatingAdapters = false; }
            DnsList.SelectedIndex = -1;
            DnsTextOne.Clear();
            DnsTextTwo.Clear();
            if (AdapterList.SelectedItem is NetworkAdapter)
                ShowAdapterState(await dnsService.ReadAsync(SelectedAdapter.Id));
            else
                SetStatus(adapters.Count == 0 ? "No IPv4 network adapters found."
                    : previousId.HasValue ? "The previous adapter disappeared. Select an adapter."
                    : "Select the adapter whose IPv4 DNS you want to change.");
        }

        private void ShowAdapterState(AdapterDnsState state)
        {
            currentAdapterState = state;
            var current = state.EffectiveIpv4Servers;
            DnsModeLabel.Text = state.Configuration.Mode == DnsMode.Automatic ? "Automatic (DHCP)" : "Manual";
            AdapterStateLabel.Text = $"{(state.Adapter.IsUp ? "Connected" : "Disconnected")} · {state.Adapter.Description}";
            CurrentAdapterDnsLabel.Text = current.Count == 0 ? "No IPv4 DNS servers reported." : string.Join(Environment.NewLine, current);
            updatingDnsInputs = true;
            try
            {
                DnsList.SelectedIndex = -1;
                var matched = dnsEntries.FirstOrDefault(x => x.IPs.SequenceEqual(current));
                if (matched != null) DnsList.SelectedItem = matched.Name;
                DnsTextOne.Text = current.ElementAtOrDefault(0) ?? "";
                DnsTextTwo.Text = current.ElementAtOrDefault(1) ?? "";
            }
            finally { updatingDnsInputs = false; }
            SetStatus(state.Adapter.IsUp ? $"Ready · {state.Adapter.Name}" : $"{state.Adapter.Name} is disconnected.");
        }

        private void SetStatus(string text, bool error = false)
        {
            OperationStatusLabel.Text = text;
            OperationStatusLabel.ForeColor = error && !SystemInformation.HighContrast
                ? Color.FromArgb(153, 35, 43) : UiTheme.Text;
        }

        private void ShowOperationError(Exception exception)
        {
            SetStatus(exception.Message, error: true);
            MessageBox.Show(this, exception.Message, "DNSetter", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private bool HasValidDnsInput()
        {
            if (string.IsNullOrWhiteSpace(DnsTextOne.Text)) return false;
            try { DnsConfiguration.Manual([DnsTextOne.Text, DnsTextTwo.Text]); return true; }
            catch (ArgumentException) { return false; }
        }

        private void DnsInput_TextChanged(object? sender, EventArgs e)
        {
            if (updatingDnsInputs) return;
            DnsList.SelectedIndex = -1;
            UpdateActionAvailability();
            if (!isBusy)
                SetStatus(HasValidDnsInput() ? "DNS changes are ready to apply to the selected adapter."
                    : "Enter a valid primary IPv4 DNS. The secondary address is optional.");
        }

        private void UpdateActionAvailability()
        {
            var validInput = HasValidDnsInput();
            SetButton.Enabled = !isBusy && validInput && currentAdapterState?.Adapter.IsUp == true;
            AddOrUpdateButton.Enabled = !isBusy && validInput;
            TestSelectedDnsButton.Enabled = !isBusy && validInput;
            UnsetDnsButton.Enabled = !isBusy && currentAdapterState?.Adapter.IsUp == true;
            CheckCurrentDnsButton.Enabled = !isBusy && AdapterList.SelectedItem is NetworkAdapter;
        }

        private async Task RunUiOperationAsync(Func<Task> operation)
        {
            if (isBusy) return;
            SetUIEnabled(false);
            SetStatus("Working…");
            try { await operation(); }
            catch (Exception ex) { ShowOperationError(ex); }
            finally { SetUIEnabled(true); }
        }

        private async void AdapterList_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (updatingAdapters || isBusy) return;
            await RunUiOperationAsync(async () =>
            {
                currentAdapterState = null;
                DnsModeLabel.Text = "Reading…";
                AdapterStateLabel.Text = "Reading the selected adapter.";
                CurrentAdapterDnsLabel.Clear();
                SetStatus("Reading selected adapter…");
                DnsList.SelectedIndex = -1;
                DnsTextOne.Clear();
                DnsTextTwo.Clear();
                ShowAdapterState(await dnsService.ReadAsync(SelectedAdapter.Id));
            });
        }

        private async void RefreshAdaptersButton_Click(object? sender, EventArgs e) =>
            await RunUiOperationAsync(RefreshAdaptersAsync);

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (isBusy && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                return;
            }
            base.OnFormClosing(e);
        }

        private void DnsList_SelectedIndexChanged(object? sender, EventArgs e)
        {
            var selected = dnsEntries.FirstOrDefault(x => x.Name == DnsList.SelectedItem?.ToString());
            if (selected == null) return;
            updatingDnsInputs = true;
            try
            {
                DnsTextOne.Text = selected.IPs.ElementAtOrDefault(0) ?? "";
                DnsTextTwo.Text = selected.IPs.ElementAtOrDefault(1) ?? "";
            }
            finally { updatingDnsInputs = false; }
            UpdateActionAvailability();
            if (!isBusy) SetStatus($"{selected.Name} selected. Click Apply DNS to use it on the selected adapter.");
        }

        private async void SetButton_Click(object? sender, EventArgs e)
        {
            await ChangeDnsAsync(reset: false);
        }

        private async Task ChangeDnsAsync(bool reset)
        {
            await RunUiOperationAsync(async () =>
            {
                var adapter = SelectedAdapter;
                DnsChangeResult result;
                try
                {
                    result = reset ? await dnsService.ResetAsync(adapter.Id)
                        : await dnsService.SetAsync(adapter.Id, [DnsTextOne.Text, DnsTextTwo.Text]);
                }
                catch
                {
                    // Re-read after a rollback so the form does not display the attempted values as current.
                    try { ShowAdapterState(await dnsService.ReadAsync(adapter.Id)); }
                    catch { currentAdapterState = null; CurrentAdapterDnsLabel.Text = "Unable to read this adapter. Refresh before changing DNS."; }
                    throw;
                }
                ShowAdapterState(result.State);
                var message = reset ? $"IPv4 DNS on '{adapter.Name}' is now Automatic (DHCP)."
                    : $"IPv4 DNS on '{adapter.Name}' was changed and verified.";
                if (result.Warning != null) message += "\n\n" + result.Warning;
                SetStatus(message);
            });
        }

        private void AddOrUpdateButton_Click(object? sender, EventArgs e)
        {
            try
            {
                string ip1 = DnsTextOne.Text.Trim();
                string ip2 = DnsTextTwo.Text.Trim();
                var servers = DnsConfiguration.Manual([ip1, ip2]).Servers.ToList();

                string name = $"Custom DNS ({ip1})";
                var existing = dnsEntries.FirstOrDefault(x => x.Name == name);
                if (existing != null)
                {
                    existing.IPs = servers;
                }
                else
                {
                    dnsEntries.Add(new DnsEntry { Name = name, IPs = servers });
                    DnsList.Items.Add(name);
                }

                File.WriteAllText(dnsListPath, JsonSerializer.Serialize(dnsEntries, new JsonSerializerOptions { WriteIndented = true }));
                DnsList.SelectedItem = name;
                SetStatus($"Preset saved: {name}");
            }
            catch (Exception ex)
            {
                ShowOperationError(ex);
            }
        }

        private async void TestSelectedDnsButton_Click(object? sender, EventArgs e)
        {
            await RunUiOperationAsync(async () =>
            {
                var servers = DnsConfiguration.Manual([DnsTextOne.Text, DnsTextTwo.Text]).Servers;
                var results = await Task.WhenAll(servers.Select(async ip =>
                {
                    try
                    {
                        using var ping = new Ping();
                        var reply = await ping.SendPingAsync(ip, 1500);
                        return reply.Status == IPStatus.Success ? $"{ip}: {reply.RoundtripTime} ms"
                            : $"{ip}: {reply.Status}";
                    }
                    catch (PingException ex) { return $"{ip}: {ex.InnerException?.Message ?? ex.Message}"; }
                }));
                SetStatus("ICMP ping results" + Environment.NewLine + string.Join(Environment.NewLine, results));
            });
        }

        private async void TestAllDnsListButton_Click(object? sender, EventArgs e)
        {
            await RunUiOperationAsync(() =>
            {
                using var resultsForm = new DnsList(dnsEntries);
                resultsForm.ShowDialog(this);
                SetStatus("Preset ping test finished. ICMP results do not verify DNS resolution.");
                return Task.CompletedTask;
            });
        }

        private async void UnsetDnsButton_Click(object? sender, EventArgs e)
        {
            await ChangeDnsAsync(reset: true);
        }

        private async void CheckCurrentDnsButton_Click(object? sender, EventArgs e)
        {
            await RunUiOperationAsync(async () =>
            {
                var state = await dnsService.ReadAsync(SelectedAdapter.Id);
                ShowAdapterState(state);
                var configured = state.Configuration.Mode == DnsMode.Automatic
                    ? "Automatic (DHCP)" : "Manual: " + string.Join(", ", state.Configuration.Servers);
                MessageBox.Show(this,
                    $"Adapter: {state.Adapter.Name}\n{state.Adapter.Description}\n" +
                    $"ID: {state.Adapter.Id}\nInterface index: {state.Adapter.InterfaceIndex}\n" +
                    $"IPv4 configuration: {configured}\n" +
                    $"Effective IPv4 DNS: {string.Join(", ", state.EffectiveIpv4Servers)}\n" +
                    $"Effective IPv6 DNS (read only): {string.Join(", ", state.EffectiveIpv6Servers)}",
                    "Selected adapter DNS");
            });
        }

        private async void CheckCensorshipButton_Click(object? sender, EventArgs e)
        {
            await RunUiOperationAsync(async () =>
            {
                using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(8) };
                using var response = await httpClient.GetAsync("https://gemini.google.com/", HttpCompletionOption.ResponseHeadersRead);
                SetStatus($"gemini.google.com returned HTTP {(int)response.StatusCode} ({response.StatusCode}).\n" +
                    "This checks site reachability through Windows/proxy settings; it does not prove that a DNS preset bypasses restrictions.");
            });
        }

        private void linkLabel1_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/Mehrdad32/DNSetter",
                UseShellExecute = true 
            });
        }

        private void linkLabel2_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://www.mehrdad32.ir/7070/dnsetter-free-application/",
                UseShellExecute = true
            });
        }

        private void SetUIEnabled(bool enabled)
        {
            isBusy = !enabled;
            BusyProgressBar.Visible = !enabled;
            UseWaitCursor = !enabled;
            AdapterList.Enabled = enabled;
            RefreshAdaptersButton.Enabled = enabled;
            DnsList.Enabled = enabled;
            DnsTextOne.Enabled = enabled;
            DnsTextTwo.Enabled = enabled;
            TestAllDnsListButton.Enabled = enabled;
            CheckCensorshipButton.Enabled = enabled;
            UpdateActionAvailability();
        }
    }

    public class DnsEntry
    {
        public required string Name { get; set; }
        public required List<string> IPs { get; set; }
    }
}
