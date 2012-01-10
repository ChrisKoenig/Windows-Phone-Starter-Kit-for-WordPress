using System;
using System.Net;
using WordPressStarterKit.Extensions;

namespace WordPressStarterKit.Networking
{
    public class NetworkWire : INetworkWire
    {
        public void GetStringFromURL(Uri uri, Action<string, Exception> callback)
        {
            var c = new WebClient();
          //  c.AllowReadStreamBuffering = false;
            c.DownloadStringCompleted += (sender, e) =>
            {
                if (e.Error == null)
                    callback(e.Result.scrubHTML(), null);
                else
                    callback(null, e.Error);
            };

            c.DownloadStringAsync(uri);
        }
    }
}
