using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DNSetter
{
    public partial class DnsList : Form
    {
        private readonly List<DnsEntry> _dnsEntries;

        public DnsList(List<DnsEntry> dnsEntries)
        {
            InitializeComponent();
            _dnsEntries = dnsEntries;
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await LoadPingResultsAsync();
        }

        private async Task LoadPingResultsAsync()
        {
            DnsListGrid.Columns.Clear();
            DnsListGrid.Columns.Add("Service", "Service");
            DnsListGrid.Columns.Add("IP1", "IP #1");
            DnsListGrid.Columns.Add("Ping1", "Ping #1 (ms)");
            DnsListGrid.Columns.Add("IP2", "IP #2");
            DnsListGrid.Columns.Add("Ping2", "Ping #2 (ms)");

            var ping = new Ping();

            foreach (var entry in _dnsEntries)
            {
                string ip1 = entry.IPs.ElementAtOrDefault(0) ?? "";
                string ip2 = entry.IPs.ElementAtOrDefault(1) ?? "";

                string ping1 = await PingAddressAsync(ping, ip1);
                string ping2 = await PingAddressAsync(ping, ip2);

                DnsListGrid.Rows.Add(entry.Name, ip1, ping1, ip2, ping2);
            }
        }

        private async Task<string> PingAddressAsync(Ping ping, string ip)
        {
            if (string.IsNullOrWhiteSpace(ip)) return "N/A";

            try
            {
                var reply = await ping.SendPingAsync(ip, 1000);
                return reply.Status == IPStatus.Success ? $"{reply.RoundtripTime}" : "Failed";
            }
            catch
            {
                return "Error";
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
