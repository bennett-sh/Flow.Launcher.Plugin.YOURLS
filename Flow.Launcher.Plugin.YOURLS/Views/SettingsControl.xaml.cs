using System.Windows.Controls;

namespace Flow.Launcher.Plugin.YOURLS
{
    public partial class SettingsControl : UserControl
    {
        private readonly Settings Settings;

        public SettingsControl(Settings settings)
        {
            Settings = settings;
            InitializeComponent();
        }
    }
}
