using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Policy;
using System.Text.Json;

namespace DNSetter
{
    public partial class MainForm : Form
    {
        private string dnsListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "list.json");
        private List<DnsEntry> dnsEntries = new();

        public MainForm()
        {
            InitializeComponent();
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                TitleLabel.Text += "v" + Assembly.GetExecutingAssembly().GetName().Version!.ToString();

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
                new DnsEntry { Name = "Google", IPs = ["8.8.8.8", "4.2.2.4"] },
            };
                    File.WriteAllText(dnsListPath, JsonSerializer.Serialize(defaultEntries, new JsonSerializerOptions { WriteIndented = true }));
                }

                string json = File.ReadAllText(dnsListPath);
                dnsEntries = JsonSerializer.Deserialize<List<DnsEntry>>(json) ?? new();

                DnsList.Items.Clear();
                foreach (var entry in dnsEntries)
                {
                    DnsList.Items.Add(entry.Name);
                }

                await SetCurrentDnsUIAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private async Task SetCurrentDnsUIAsync()
        {
            try
            {
                List<string> currentDns = new();

                foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.OperationalStatus == OperationalStatus.Up &&
                        nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        var ipProps = nic.GetIPProperties();
                        var dnsAddresses = ipProps.DnsAddresses
                                                  .Where(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                                  .Select(ip => ip.ToString());

                        currentDns.AddRange(dnsAddresses);
                    }
                }

                // Remove duplicates, take only first 2
                currentDns = currentDns.Distinct().Take(2).ToList();

                DnsTextOne.Text = currentDns.ElementAtOrDefault(0) ?? "";
                DnsTextTwo.Text = currentDns.ElementAtOrDefault(1) ?? "";

                // Match with existing entries
                foreach (var entry in dnsEntries)
                {
                    var ips = entry.IPs.Select(ip => ip.Trim()).ToList();
                    if (ips.Count >= 2 &&
                        currentDns.Count >= 2 &&
                        ips[0] == currentDns[0] &&
                        ips[1] == currentDns[1])
                    {
                        DnsList.SelectedItem = entry.Name;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to detect current DNS:\n" + ex.Message, "DNS Detection Error");
            }
        }

        private void DnsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var selected = dnsEntries.FirstOrDefault(x => x.Name == DnsList.SelectedItem?.ToString());
                if (selected != null)
                {
                    DnsTextOne.Text = selected.IPs.ElementAtOrDefault(0) ?? "";
                    DnsTextTwo.Text = selected.IPs.ElementAtOrDefault(1) ?? "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private void SetButton_Click(object sender, EventArgs e)
        {
            try
            {
                string ip1 = DnsTextOne.Text.Trim();
                string ip2 = DnsTextTwo.Text.Trim();

                if (!IPAddress.TryParse(ip1, out _) || !IPAddress.TryParse(ip2, out _))
                {
                    MessageBox.Show("Invalid IP format.");
                    return;
                }

                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.OperationalStatus == OperationalStatus.Up &&
                        nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        string name = nic.Name;

                        Process.Start(new ProcessStartInfo("netsh", $"interface ip set dns name=\"{name}\" static {ip1}")
                        {
                            Verb = "runas",
                            UseShellExecute = true
                        })?.WaitForExit();

                        Process.Start(new ProcessStartInfo("netsh", $"interface ip add dns name=\"{name}\" {ip2} index=2")
                        {
                            Verb = "runas",
                            UseShellExecute = true
                        })?.WaitForExit();
                    }
                }

                // Flush DNS
                Process.Start(new ProcessStartInfo("ipconfig", "/flushdns")
                {
                    Verb = "runas",
                    UseShellExecute = true
                });

                MessageBox.Show("DNS changed successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private void AddOrUpdateButton_Click(object sender, EventArgs e)
        {
            try
            {
                string ip1 = DnsTextOne.Text.Trim();
                string ip2 = DnsTextTwo.Text.Trim();
                if (!IPAddress.TryParse(ip1, out _) || !IPAddress.TryParse(ip2, out _))
                {
                    MessageBox.Show("Invalid IP address format.");
                    return;
                }

                string name = $"Custom DNS ({ip1})";
                var existing = dnsEntries.FirstOrDefault(x => x.Name == name);
                if (existing != null)
                {
                    existing.IPs = new List<string> { ip1, ip2 };
                }
                else
                {
                    dnsEntries.Add(new DnsEntry { Name = name, IPs = new List<string> { ip1, ip2 } });
                    DnsList.Items.Add(name);
                }

                File.WriteAllText(dnsListPath, JsonSerializer.Serialize(dnsEntries, new JsonSerializerOptions { WriteIndented = true }));
                MessageBox.Show("DNS entry saved.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private void TestSelectedDnsButton_Click(object sender, EventArgs e)
        {
            try
            {
                var selected = dnsEntries.FirstOrDefault(x => x.Name == DnsList.SelectedItem?.ToString());
                if (selected == null) return;

                string results = "";
                foreach (var ip in selected.IPs)
                {
                    try
                    {
                        var ping = new Ping();
                        var reply = ping.Send(ip, 1000);
                        results += $"{ip}: {reply?.RoundtripTime} ms\n";
                    }
                    catch
                    {
                        results += $"{ip}: Failed\n";
                    }
                }

                MessageBox.Show(results, "Ping Result");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private async void TestAllDnsListButton_Click(object sender, EventArgs e)
        {
            try
            {
                SetUIEnabled(false);

                var ping = new Ping();
                var tasks = new List<Task<string>>();

                foreach (var entry in dnsEntries)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        string entryResult = $"{entry.Name}:\n";
                        foreach (var ip in entry.IPs)
                        {
                            try
                            {
                                var reply = await ping.SendPingAsync(ip, 1000);
                                entryResult += $"  {ip}: {reply.RoundtripTime} ms\n";
                            }
                            catch
                            {
                                entryResult += $"  {ip}: Failed\n";
                            }
                        }
                        entryResult += "\n";
                        return entryResult;
                    }));
                }

                var resultsArray = await Task.WhenAll(tasks);
                string finalResults = string.Join("", resultsArray);

                var dnsListForm = new DnsList(dnsEntries);
                dnsListForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
            finally
            {
                SetUIEnabled(true);
            }
        }

        private void UnsetDnsButton_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        string name = nic.Name;
                        Process.Start(new ProcessStartInfo("netsh", $"interface ip set dns name=\"{name}\" dhcp")
                        {
                            Verb = "runas",
                            UseShellExecute = true
                        })?.WaitForExit();
                    }
                }

                MessageBox.Show("DNS unset (back to DHCP).");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private void CheckCurrentDnsButton_Click(object sender, EventArgs e)
        {
            try
            {
                string result = "";
                foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.OperationalStatus == OperationalStatus.Up &&
                        nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        var ipProps = nic.GetIPProperties();
                        var dnsAddresses = ipProps.DnsAddresses;

                        result += $"Adapter: {nic.Name}\n";
                        if (dnsAddresses.Count == 0)
                        {
                            result += "  DNS: (none or DHCP)\n";
                        }
                        else
                        {
                            foreach (var dns in dnsAddresses)
                                result += $"  DNS: {dns}\n";
                        }
                        result += "\n";
                    }
                }

                MessageBox.Show(result, "Current DNS Settings");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private async void CheckCensorshipButton_Click(object sender, EventArgs e)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(5);

                var response = await httpClient.GetAsync("https://gemini.google.com/");
                string html = await response.Content.ReadAsStringAsync();

                if (html.Contains("Error 403 (Forbidden)!!1"))
                {
                    MessageBox.Show("❌ This DNS does NOT bypass filtering (Gemini returns 403).", "Bypass Test");
                }
                else if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("✅ This DNS bypasses filtering (Gemini is reachable).", "Bypass Test");
                }
                else
                {
                    MessageBox.Show($"⚠ Unexpected status code: {response.StatusCode}", "Bypass Test");
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"❓ Could not reach site.\n{ex.Message}", "Bypass Test");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Bypass Test");
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/Mehrdad32/DNSetter",
                UseShellExecute = true 
            });
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://www.mehrdad32.ir/7070/dnsetter-free-application/",
                UseShellExecute = true
            });
        }

        private void SetUIEnabled(bool enabled)
        {
            DnsList.Enabled = enabled;
            DnsTextOne.Enabled = enabled;
            DnsTextTwo.Enabled = enabled;
            SetButton.Enabled = enabled;
            AddOrUpdateButton.Enabled = enabled;
            TestSelectedDnsButton.Enabled = enabled;
            TestAllDnsListButton.Enabled = enabled;
            UnsetDnsButton.Enabled = enabled;
            CheckCensorshipButton.Enabled = enabled;
            CheckCurrentDnsButton.Enabled = enabled;
        }
    }

    public class DnsEntry
    {
        public required string Name { get; set; }
        public required List<string> IPs { get; set; }
    }
}
