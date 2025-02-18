using System.Windows.Controls;

namespace Flow.Launcher.Plugin.YOURLS
{
    public partial class SettingsControl : UserControl
    {
        private Settings Settings { get; }

        public SettingsControl(Settings settings)
        {
            Settings = settings;
            InitializeComponent();
        }
    }
}
