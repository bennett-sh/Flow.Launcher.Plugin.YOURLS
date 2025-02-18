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
using Flow.Launcher.Plugin.YOURLS.ViewModels;
using Flow.Launcher.Plugin.YOURLS_Shortener;
using Flurl;
using Flurl.Http;

namespace Flow.Launcher.Plugin.YOURLS
{
    public class YOURLS : IPlugin, ISettingProvider
    {
        private PluginInitContext Context { get; set; }

        private static Settings _settings;
        private static SettingsViewModel _viewModel;
        public void Init(PluginInitContext context)
        {
            Context = context;
            _settings = context.API.LoadSettingJsonStorage<Settings>();
            _viewModel = new SettingsViewModel(_settings);
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
                Context.API.LogWarn("YOURLS", "URL: " + url);
                Context.API.LogWarn("YOURLS", "Second query: " + JsonSerializer.Serialize(query.SearchTerms));
                if (query.SecondSearch.Length > 0) customName = query.SecondToEndSearch;
            }
            else
            {
                return UnfinishedResult(
                    new Result()
                    {
                        Title = "Please provide a valid Url.",
                        SubTitle = "Syntax: [url] [<custom-name>]",
                        SubTitleToolTip = "Note: do not use spaces in any argument."
                    }
                );
            }

            return SingleResult(new Result()
            {
                Title = $"Shorten URL{(customName.Length > 0 ? $" ({customName})" : "")}",
                SubTitle = url,
                AsyncAction = async _ =>
                {
                    var shortUrl = await ShortenUrl(url, customName);
                    if (shortUrl == null) return false;
                    Context.API.CopyToClipboard(shortUrl, showDefaultNotification: false);
                    Context.API.ShowMsg("URL copied to clipboard.");
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
                signature = HashMD5(timestamp.ToString() + _settings.SignatureToken),
                timestamp
            }).PostAsync().ReceiveJson<IYOURLSResponse>();

            if(response.status == "fail")
            {
                Context.API.ShowMsg(response.message);
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
            return new SettingsView(_viewModel);
        }
    }
}