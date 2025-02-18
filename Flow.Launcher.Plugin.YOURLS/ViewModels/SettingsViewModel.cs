using Flow.Launcher.Plugin.YOURLS_Shortener;
using System.Collections.Generic;
using System.Linq;

namespace Flow.Launcher.Plugin.YOURLS.ViewModels
{
    public class SettingsViewModel : BaseModel
    {
        public SettingsViewModel(Settings settings)
        {
            Settings = settings;
        }

        public Settings Settings { get; init; }
    }
}
