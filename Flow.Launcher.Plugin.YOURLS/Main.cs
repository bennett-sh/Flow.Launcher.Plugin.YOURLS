using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Flow.Launcher.Plugin;
using Flurl;
using Flurl.Http;

namespace Flow.Launcher.Plugin.YOURLS
{
    public class Main : IPlugin, ISettingProvider
    {
        private PluginInitContext _context { get; set; }
        private static Settings _settings;

        public void Init(PluginInitContext context)
        {
            _context = context;
            _settings = _context.API.LoadSettingJsonStorage<Settings>();
        }

        public List<Result> Query(Query query)
        {
            var clipboardText = GetClipboardText();
            var clipboardContainsUrl = IsValidUrl(clipboardText);

            string url = "";
            string customName = "";

            if (query.SecondSearch.Length < 1 && clipboardContainsUrl)
            {
                url = clipboardText;
                if (query.FirstSearch.Length > 0) customName = query.FirstSearch;
            }
            else if (query.FirstSearch.Length > 0 && IsValidUrl(query.FirstSearch))
            {
                url = query.FirstSearch;
                if (query.SecondSearch.Length > 0) customName = query.SecondSearch;
            }
            else
            {
                return UnfinishedResult(
                    new Result()
                    {
                        Title = "Please provide a valid Url.",
                        SubTitle = "Syntax: [url] [<custom-name>]",
                        SubTitleToolTip = "Note: do not use spaces in any argument.",
                        IcoPath = "icon.png"
                    }
                );
            }

            return SingleResult(new Result()
            {
                Title = $"Shorten URL{(customName.Length > 0 ? $" ({customName})" : "")}",
                SubTitle = url,
                IcoPath = "icon.png",
                AsyncAction = async _ =>
                {
                    var shortUrl = await ShortenUrl(url, customName);
                    if (shortUrl == null) return false;
                    _context.API.CopyToClipboard(shortUrl, showDefaultNotification: false);
                    _context.API.ShowMsg("URL copied to clipboard.");
                    return true;
                }
            });
        }

        private async Task<string?> ShortenUrl(string url, string? customName)
        {
            var timestamp = (long)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;
            var response = await _settings.Host.AppendPathSegment("yourls-api.php").SetQueryParams(new
            {
                action = "shorturl",
                url,
                keyword = customName,
                format = "json",
                signature = HashMD5(timestamp.ToString() + _settings.SignatureToken).ToLower(),
                timestamp
            }).PostAsync().ReceiveJson<YOURLSResponse>();

            if(response.status == "fail")
            {
                _context.API.ShowMsg(response.message);
                return null;
            }

            return response.shorturl;
        }

        private string HashMD5(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                return Convert.ToHexString(hashBytes);
            }
        }

        private string GetClipboardText()
        {
            var result = string.Empty;
            var thread = new Thread(() =>
            {
                try
                {
                    if (Clipboard.ContainsText())
                    {
                        result = Clipboard.GetText();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Clipboard operation failed: " + ex.Message);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return result;
        }

        private bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uri)
              && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        private List<Result> SingleResult(Result result)
        {
            var resultList = new List<Result>();
            resultList.Add(result);
            return resultList;
        }

        private List<Result> UnfinishedResult(Result result)
        {
            result.Action = _ => { return false; };
            return SingleResult(result);
        }

        public Control CreateSettingPanel()
        {
            return new SettingsControl(_settings);
        }
    }
}