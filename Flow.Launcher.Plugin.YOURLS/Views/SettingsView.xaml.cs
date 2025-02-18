using Flow.Launcher.Plugin.YOURLS.ViewModels;
using Flow.Launcher.Plugin.YOURLS_Shortener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Flow.Launcher.Plugin.YOURLS
{
    public partial class SettingsView : UserControl
    {
        private readonly SettingsViewModel _viewModel;
        private readonly Settings _settings;

        public SettingsView(SettingsViewModel viewModel)
        {
            _settings = _viewModel.Settings;
            _viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
