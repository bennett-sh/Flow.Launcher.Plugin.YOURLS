using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Flow.Launcher.Plugin.YOURLS
{
    public class Settings : INotifyPropertyChanged
    {
        public string _signatureToken = "n7yqozz89k";
        public string _host = "https://url.example.com";

        public string SignatureToken
        {
            get => _signatureToken;
            set
            {
                _signatureToken = value;
                OnPropertyChanged();
            }
        }

        public string Host
        {
            get => _host;
            set
            {
                _host = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
