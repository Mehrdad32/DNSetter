namespace DNSetter;

internal static class UiTheme
{
    private static bool HighContrast => SystemInformation.HighContrast;
    public static Color Background => HighContrast ? SystemColors.Control : Color.FromArgb(244, 246, 249);
    public static Color Surface => SystemColors.Window;
    public static Color Text => SystemColors.WindowText;
    public static Color Muted => HighContrast ? SystemColors.WindowText : Color.FromArgb(91, 105, 123);
    public static Color ReadOnly => HighContrast ? SystemColors.Control : Color.FromArgb(239, 243, 248);
    public static Color Header => HighContrast ? SystemColors.Control : Color.FromArgb(23, 37, 58);
    public static Color HeaderText => HighContrast ? SystemColors.ControlText : Color.White;
    public static Color HeaderMuted => HighContrast ? SystemColors.ControlText : Color.FromArgb(199, 213, 233);
    public static Color Accent => HighContrast ? SystemColors.Highlight : Color.FromArgb(28, 91, 194);

    public static void Apply(Control root, Button? primary = null)
    {
        foreach (Control control in root.Controls)
        {
            if (control is Button button)
            {
                button.AutoSize = true;
                button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                button.MinimumSize = new Size(100, 36);
                button.Padding = new Padding(10, 4, 10, 4);
                button.Margin = new Padding(0, 0, 8, 4);
                button.UseVisualStyleBackColor = true;
                if (button == primary && !HighContrast)
                {
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    button.BackColor = Accent;
                    button.ForeColor = Color.White;
                    button.UseVisualStyleBackColor = false;
                }
            }
            if (control is LinkLabel link)
            {
                link.LinkColor = Accent;
                link.ActiveLinkColor = Accent;
                link.VisitedLinkColor = Accent;
            }
            if (control is Label label) label.UseMnemonic = false;
            Apply(control, primary);
        }
    }
}
