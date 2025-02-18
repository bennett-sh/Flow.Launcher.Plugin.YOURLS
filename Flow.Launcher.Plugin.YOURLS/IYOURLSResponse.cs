using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow.Launcher.Plugin.YOURLS
{
    internal interface IYOURLSResponse
    {
        string status { get; }
        string code { get; }
        string message { get; }
        string errorCode { get; }
        string statusCode { get; }
        string title { get; }
        string shorturl { get; }
        IUrlInformation url { get; }
    }

    internal interface IUrlInformation
    {
        string keyword { get; }
        string url { get; }
        string title { get; }
        string date { get; }
        string ip { get; }
        int clicks { get; }
    }
}
