using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow.Launcher.Plugin.YOURLS
{
    internal class YOURLSResponse
    {
        public string status { get; set; }
        public string code { get; set; }
        public string message { get; set; }
        public string errorCode { get; set; }
        public string statusCode { get; set; }
        public string title { get; set; }
        public string shorturl { get; set; }
        public UrlInformation url { get; set; }
    }

    internal class UrlInformation
    {
        public string keyword { get; set; }
        public string url { get; set; }
        public string title { get; set; }
        public string date { get; set; }
        public string ip { get; set; }
        public int clicks { get; set; }
    }
}
