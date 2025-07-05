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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
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
            pictureBox1 = new PictureBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            TitleLabel = new Label();
            label5 = new Label();
            linkLabel1 = new LinkLabel();
            linkLabel2 = new LinkLabel();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // DnsList
            // 
            DnsList.DropDownStyle = ComboBoxStyle.DropDownList;
            DnsList.FormattingEnabled = true;
            DnsList.Location = new Point(153, 159);
            DnsList.Name = "DnsList";
            DnsList.Size = new Size(241, 31);
            DnsList.TabIndex = 0;
            DnsList.SelectedIndexChanged += DnsList_SelectedIndexChanged;
            // 
            // DnsTextOne
            // 
            DnsTextOne.Location = new Point(153, 205);
            DnsTextOne.Name = "DnsTextOne";
            DnsTextOne.Size = new Size(241, 30);
            DnsTextOne.TabIndex = 1;
            // 
            // DnsTextTwo
            // 
            DnsTextTwo.Location = new Point(153, 241);
            DnsTextTwo.Name = "DnsTextTwo";
            DnsTextTwo.Size = new Size(241, 30);
            DnsTextTwo.TabIndex = 2;
            // 
            // SetButton
            // 
            SetButton.Location = new Point(19, 309);
            SetButton.Name = "SetButton";
            SetButton.Size = new Size(179, 37);
            SetButton.TabIndex = 3;
            SetButton.Text = "&Set DNS";
            SetButton.UseVisualStyleBackColor = true;
            SetButton.Click += SetButton_Click;
            // 
            // AddOrUpdateButton
            // 
            AddOrUpdateButton.Location = new Point(19, 352);
            AddOrUpdateButton.Name = "AddOrUpdateButton";
            AddOrUpdateButton.Size = new Size(179, 37);
            AddOrUpdateButton.TabIndex = 4;
            AddOrUpdateButton.Text = "&Add Or Update";
            AddOrUpdateButton.UseVisualStyleBackColor = true;
            AddOrUpdateButton.Click += AddOrUpdateButton_Click;
            // 
            // TestSelectedDnsButton
            // 
            TestSelectedDnsButton.Location = new Point(19, 395);
            TestSelectedDnsButton.Name = "TestSelectedDnsButton";
            TestSelectedDnsButton.Size = new Size(179, 37);
            TestSelectedDnsButton.TabIndex = 5;
            TestSelectedDnsButton.Text = "&Test DNS By PING";
            TestSelectedDnsButton.UseVisualStyleBackColor = true;
            TestSelectedDnsButton.Click += TestSelectedDnsButton_Click;
            // 
            // TestAllDnsListButton
            // 
            TestAllDnsListButton.Location = new Point(215, 309);
            TestAllDnsListButton.Name = "TestAllDnsListButton";
            TestAllDnsListButton.Size = new Size(179, 37);
            TestAllDnsListButton.TabIndex = 6;
            TestAllDnsListButton.Text = "T&est All DNS List";
            TestAllDnsListButton.UseVisualStyleBackColor = true;
            TestAllDnsListButton.Click += TestAllDnsListButton_Click;
            // 
            // UnsetDnsButton
            // 
            UnsetDnsButton.Location = new Point(215, 352);
            UnsetDnsButton.Name = "UnsetDnsButton";
            UnsetDnsButton.Size = new Size(179, 37);
            UnsetDnsButton.TabIndex = 7;
            UnsetDnsButton.Text = "&Unset DNS";
            UnsetDnsButton.UseVisualStyleBackColor = true;
            UnsetDnsButton.Click += UnsetDnsButton_Click;
            // 
            // CheckCurrentDnsButton
            // 
            CheckCurrentDnsButton.Location = new Point(215, 395);
            CheckCurrentDnsButton.Name = "CheckCurrentDnsButton";
            CheckCurrentDnsButton.Size = new Size(179, 37);
            CheckCurrentDnsButton.TabIndex = 8;
            CheckCurrentDnsButton.Text = "&Check System DNS";
            CheckCurrentDnsButton.UseVisualStyleBackColor = true;
            CheckCurrentDnsButton.Click += CheckCurrentDnsButton_Click;
            // 
            // CheckCensorshipButton
            // 
            CheckCensorshipButton.Location = new Point(19, 438);
            CheckCensorshipButton.Name = "CheckCensorshipButton";
            CheckCensorshipButton.Size = new Size(375, 37);
            CheckCensorshipButton.TabIndex = 9;
            CheckCensorshipButton.Text = "Chec&k Network To Bypass Sanctions";
            CheckCensorshipButton.UseVisualStyleBackColor = true;
            CheckCensorshipButton.Click += CheckCensorshipButton_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(12, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(135, 120);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 10;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(24, 208);
            label1.Name = "label1";
            label1.Size = new Size(123, 23);
            label1.TabIndex = 11;
            label1.Text = "Preferred DNS:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 244);
            label2.Name = "label2";
            label2.Size = new Size(135, 23);
            label2.TabIndex = 12;
            label2.Text = "Alternative DNS:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(29, 162);
            label3.Name = "label3";
            label3.Size = new Size(118, 23);
            label3.TabIndex = 13;
            label3.Text = "Service Name:";
            // 
            // TitleLabel
            // 
            TitleLabel.AutoSize = true;
            TitleLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            TitleLabel.Location = new Point(159, 12);
            TitleLabel.Name = "TitleLabel";
            TitleLabel.Size = new Size(102, 23);
            TitleLabel.TabIndex = 14;
            TitleLabel.Text = "DNSetter - ";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F);
            label5.Location = new Point(159, 52);
            label5.Name = "label5";
            label5.Size = new Size(208, 20);
            label5.TabIndex = 15;
            label5.Text = "Free DNS setter and tools app";
            // 
            // linkLabel1
            // 
            linkLabel1.ActiveLinkColor = Color.Brown;
            linkLabel1.AutoSize = true;
            linkLabel1.Font = new Font("Segoe UI", 9F);
            linkLabel1.Location = new Point(159, 84);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(89, 20);
            linkLabel1.TabIndex = 16;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Github Page";
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // linkLabel2
            // 
            linkLabel2.ActiveLinkColor = Color.Brown;
            linkLabel2.AutoSize = true;
            linkLabel2.Font = new Font("Segoe UI", 9F);
            linkLabel2.Location = new Point(159, 112);
            linkLabel2.Name = "linkLabel2";
            linkLabel2.Size = new Size(120, 20);
            linkLabel2.TabIndex = 17;
            linkLabel2.TabStop = true;
            linkLabel2.Text = "Owner Webpage";
            linkLabel2.LinkClicked += linkLabel2_LinkClicked;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(419, 496);
            Controls.Add(linkLabel2);
            Controls.Add(linkLabel1);
            Controls.Add(label5);
            Controls.Add(TitleLabel);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(pictureBox1);
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
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "DNSetter";
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
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
        private PictureBox pictureBox1;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label TitleLabel;
        private Label label5;
        private LinkLabel linkLabel1;
        private LinkLabel linkLabel2;
    }
}
