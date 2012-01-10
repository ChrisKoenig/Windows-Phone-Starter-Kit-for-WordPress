using System;

namespace WordPressStarterKit.Networking
{
    public interface INetworkWire
    {
        void GetStringFromURL(Uri uri, Action<string, Exception> callback);
    }
}
