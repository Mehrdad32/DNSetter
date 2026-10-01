#nullable enable
namespace DNSetter
{
    partial class DnsList
    {
        private System.ComponentModel.IContainer? components;

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(DnsList));
            var root = new TableLayoutPanel();
            var heading = new TableLayoutPanel();
            var footer = new TableLayoutPanel();
            CloseButton = new Button();
            DnsListGrid = new DataGridView();
            ResultsStatusLabel = new Label();
            ResultsProgress = new ProgressBar();
            ((System.ComponentModel.ISupportInitialize)DnsListGrid).BeginInit();
            SuspendLayout();
            root.Dock = DockStyle.Fill;
            root.ColumnCount = 1;
            root.RowCount = 3;
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.Padding = new Padding(20);
            root.BackColor = UiTheme.Background;
            heading.Dock = DockStyle.Fill;
            heading.AutoSize = true;
            heading.ColumnCount = 1;
            heading.Margin = new Padding(0, 0, 0, 16);
            heading.Controls.Add(new Label
            {
                Text = "DNS preset latency", Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                AutoSize = true, Margin = Padding.Empty
            }, 0, 0);
            heading.Controls.Add(new Label
            {
                Text = "ICMP ping · An unanswered ping does not mean DNS resolution will fail.",
                AutoSize = true, Dock = DockStyle.Fill, ForeColor = UiTheme.Muted, Margin = new Padding(0, 6, 0, 0)
            }, 0, 1);
            DnsListGrid.Name = "DnsListGrid";
            DnsListGrid.Dock = DockStyle.Fill;
            DnsListGrid.AllowUserToAddRows = false;
            DnsListGrid.AllowUserToDeleteRows = false;
            DnsListGrid.AllowUserToResizeRows = false;
            DnsListGrid.ReadOnly = true;
            DnsListGrid.MultiSelect = false;
            DnsListGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DnsListGrid.RowHeadersVisible = false;
            DnsListGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DnsListGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DnsListGrid.BackgroundColor = UiTheme.Surface;
            DnsListGrid.BorderStyle = BorderStyle.FixedSingle;
            DnsListGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            DnsListGrid.GridColor = SystemInformation.HighContrast ? SystemColors.ControlDark : Color.FromArgb(229, 234, 241);
            DnsListGrid.RowTemplate.Height = 34;
            DnsListGrid.DefaultCellStyle.Padding = new Padding(8, 5, 8, 5);
            DnsListGrid.DefaultCellStyle.BackColor = UiTheme.Surface;
            DnsListGrid.DefaultCellStyle.ForeColor = UiTheme.Text;
            DnsListGrid.AlternatingRowsDefaultCellStyle.BackColor = UiTheme.ReadOnly;
            DnsListGrid.ColumnHeadersDefaultCellStyle.Padding = new Padding(8);
            DnsListGrid.AccessibleName = "DNS preset ICMP ping results";
            DnsListGrid.Margin = Padding.Empty;
            DnsListGrid.TabIndex = 0;
            footer.Dock = DockStyle.Fill;
            footer.AutoSize = true;
            footer.ColumnCount = 3;
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            footer.Margin = new Padding(0, 12, 0, 0);
            ResultsStatusLabel.Name = "ResultsStatusLabel";
            ResultsStatusLabel.Text = "Preparing tests…";
            ResultsStatusLabel.AutoSize = true;
            ResultsStatusLabel.Anchor = AnchorStyles.Left;
            ResultsStatusLabel.Margin = Padding.Empty;
            ResultsProgress.Size = new Size(140, 12);
            ResultsProgress.Anchor = AnchorStyles.Left;
            ResultsProgress.Margin = new Padding(12, 0, 16, 0);
            CloseButton.Name = "CloseButton";
            CloseButton.Text = "&Close";
            CloseButton.TabIndex = 1;
            CloseButton.Click += CloseButton_Click;
            footer.Controls.Add(ResultsStatusLabel, 0, 0);
            footer.Controls.Add(ResultsProgress, 1, 0);
            footer.Controls.Add(CloseButton, 2, 0);
            root.Controls.Add(heading, 0, 0);
            root.Controls.Add(DnsListGrid, 0, 1);
            root.Controls.Add(footer, 0, 2);
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Font = new Font("Segoe UI", 10F);
            ForeColor = UiTheme.Text;
            ClientSize = new Size(900, 560);
            MinimumSize = new Size(760, 400);
            CancelButton = CloseButton;
            Icon = (Icon?)resources.GetObject("$this.Icon");
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "DNSetter · DNS preset latency";
            Controls.Add(root);
            ((System.ComponentModel.ISupportInitialize)DnsListGrid).EndInit();
            ResumeLayout(true);
        }

        private Button CloseButton = null!;
        private DataGridView DnsListGrid = null!;
        private Label ResultsStatusLabel = null!;
        private ProgressBar ResultsProgress = null!;
    }
}
