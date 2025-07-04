namespace DNSetter
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DnsList = new ComboBox();
            DnsTextOne = new TextBox();
            DnsTextTwo = new TextBox();
            SetButton = new Button();
            AddOrUpdateButton = new Button();
            TestSelectedDnsButton = new Button();
            TestAllDnsListButton = new Button();
            UnsetDnsButton = new Button();
            CheckCurrentDnsButton = new Button();
            CheckCensorshipButton = new Button();
            SuspendLayout();
            // 
            // DnsList
            // 
            DnsList.DropDownStyle = ComboBoxStyle.DropDownList;
            DnsList.FormattingEnabled = true;
            DnsList.Location = new Point(14, 14);
            DnsList.Name = "DnsList";
            DnsList.Size = new Size(241, 31);
            DnsList.TabIndex = 0;
            DnsList.SelectedIndexChanged += DnsList_SelectedIndexChanged;
            // 
            // DnsTextOne
            // 
            DnsTextOne.Location = new Point(14, 70);
            DnsTextOne.Name = "DnsTextOne";
            DnsTextOne.Size = new Size(241, 30);
            DnsTextOne.TabIndex = 1;
            // 
            // DnsTextTwo
            // 
            DnsTextTwo.Location = new Point(14, 106);
            DnsTextTwo.Name = "DnsTextTwo";
            DnsTextTwo.Size = new Size(241, 30);
            DnsTextTwo.TabIndex = 2;
            // 
            // SetButton
            // 
            SetButton.Location = new Point(14, 142);
            SetButton.Name = "SetButton";
            SetButton.Size = new Size(241, 37);
            SetButton.TabIndex = 3;
            SetButton.Text = "&Set DNS";
            SetButton.UseVisualStyleBackColor = true;
            SetButton.Click += SetButton_Click;
            // 
            // AddOrUpdateButton
            // 
            AddOrUpdateButton.Location = new Point(14, 185);
            AddOrUpdateButton.Name = "AddOrUpdateButton";
            AddOrUpdateButton.Size = new Size(241, 37);
            AddOrUpdateButton.TabIndex = 4;
            AddOrUpdateButton.Text = "&Add Or Update";
            AddOrUpdateButton.UseVisualStyleBackColor = true;
            AddOrUpdateButton.Click += AddOrUpdateButton_Click;
            // 
            // TestSelectedDnsButton
            // 
            TestSelectedDnsButton.Location = new Point(14, 228);
            TestSelectedDnsButton.Name = "TestSelectedDnsButton";
            TestSelectedDnsButton.Size = new Size(241, 37);
            TestSelectedDnsButton.TabIndex = 5;
            TestSelectedDnsButton.Text = "&Test DNS By PING";
            TestSelectedDnsButton.UseVisualStyleBackColor = true;
            TestSelectedDnsButton.Click += TestSelectedDnsButton_Click;
            // 
            // TestAllDnsListButton
            // 
            TestAllDnsListButton.Location = new Point(12, 271);
            TestAllDnsListButton.Name = "TestAllDnsListButton";
            TestAllDnsListButton.Size = new Size(241, 37);
            TestAllDnsListButton.TabIndex = 6;
            TestAllDnsListButton.Text = "T&est All DNS List";
            TestAllDnsListButton.UseVisualStyleBackColor = true;
            TestAllDnsListButton.Click += TestAllDnsListButton_Click;
            // 
            // UnsetDnsButton
            // 
            UnsetDnsButton.Location = new Point(12, 314);
            UnsetDnsButton.Name = "UnsetDnsButton";
            UnsetDnsButton.Size = new Size(241, 37);
            UnsetDnsButton.TabIndex = 7;
            UnsetDnsButton.Text = "&Unset DNS";
            UnsetDnsButton.UseVisualStyleBackColor = true;
            UnsetDnsButton.Click += UnsetDnsButton_Click;
            // 
            // CheckCurrentDnsButton
            // 
            CheckCurrentDnsButton.Location = new Point(12, 357);
            CheckCurrentDnsButton.Name = "CheckCurrentDnsButton";
            CheckCurrentDnsButton.Size = new Size(241, 37);
            CheckCurrentDnsButton.TabIndex = 8;
            CheckCurrentDnsButton.Text = "&Check System DNS";
            CheckCurrentDnsButton.UseVisualStyleBackColor = true;
            CheckCurrentDnsButton.Click += CheckCurrentDnsButton_Click;
            // 
            // CheckCensorshipButton
            // 
            CheckCensorshipButton.Location = new Point(12, 400);
            CheckCensorshipButton.Name = "CheckCensorshipButton";
            CheckCensorshipButton.Size = new Size(241, 37);
            CheckCensorshipButton.TabIndex = 9;
            CheckCensorshipButton.Text = "Chec&k Bypass Sanctions";
            CheckCensorshipButton.UseVisualStyleBackColor = true;
            CheckCensorshipButton.Click += CheckCensorshipButton_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(268, 460);
            Controls.Add(CheckCensorshipButton);
            Controls.Add(CheckCurrentDnsButton);
            Controls.Add(UnsetDnsButton);
            Controls.Add(TestAllDnsListButton);
            Controls.Add(TestSelectedDnsButton);
            Controls.Add(AddOrUpdateButton);
            Controls.Add(SetButton);
            Controls.Add(DnsTextTwo);
            Controls.Add(DnsTextOne);
            Controls.Add(DnsList);
            Font = new Font("Segoe UI", 10F);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "DNSetter";
            Load += MainForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox DnsList;
        private TextBox DnsTextOne;
        private TextBox DnsTextTwo;
        private Button SetButton;
        private Button AddOrUpdateButton;
        private Button TestSelectedDnsButton;
        private Button TestAllDnsListButton;
        private Button UnsetDnsButton;
        private Button CheckCurrentDnsButton;
        private Button CheckCensorshipButton;
    }
}
