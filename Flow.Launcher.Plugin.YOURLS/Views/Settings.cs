using System.IO;
using System.Text.Json;

namespace Flow.Launcher.Plugin.YOURLS_Shortener
{
    public class Settings
    {
        public string SignatureToken { get; set; } = "n7yqozz89k";
        public string Host { get; set; } = "https://url.example.com";
    }
}
