namespace DNSetter
{
    partial class DnsList
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DnsList));
            panel1 = new Panel();
            CloseButton = new Button();
            panel2 = new Panel();
            DnsListGrid = new DataGridView();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DnsListGrid).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(CloseButton);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 451);
            panel1.Name = "panel1";
            panel1.Size = new Size(839, 52);
            panel1.TabIndex = 0;
            // 
            // CloseButton
            // 
            CloseButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            CloseButton.Location = new Point(308, 6);
            CloseButton.Name = "CloseButton";
            CloseButton.Size = new Size(197, 34);
            CloseButton.TabIndex = 0;
            CloseButton.Text = "&Close";
            CloseButton.UseVisualStyleBackColor = true;
            CloseButton.Click += CloseButton_Click;
            // 
            // panel2
            // 
            panel2.Controls.Add(DnsListGrid);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(839, 451);
            panel2.TabIndex = 1;
            // 
            // DnsListGrid
            // 
            DnsListGrid.AllowUserToAddRows = false;
            DnsListGrid.AllowUserToDeleteRows = false;
            DnsListGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DnsListGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DnsListGrid.Dock = DockStyle.Fill;
            DnsListGrid.Location = new Point(0, 0);
            DnsListGrid.Name = "DnsListGrid";
            DnsListGrid.ReadOnly = true;
            DnsListGrid.RowHeadersWidth = 51;
            DnsListGrid.Size = new Size(839, 451);
            DnsListGrid.TabIndex = 0;
            // 
            // DnsList
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = CloseButton;
            ClientSize = new Size(839, 503);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimizeBox = false;
            Name = "DnsList";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "DNS List Ping Result";
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DnsListGrid).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Button CloseButton;
        private Panel panel2;
        private DataGridView DnsListGrid;
    }
}