using System.Windows.Controls;

namespace Flow.Launcher.Plugin.YOURLS
{
    public partial class SettingsControl : UserControl
    {
        public Settings Settings { get; }

        public SettingsControl(Settings settings)
        {
            Settings = settings;
            InitializeComponent();
        }
    }
}
