using System;
using WordPressStarterKit.Networking;
using System.Windows;

namespace WordPressStarterKit
{
    public static class Infrastructure
    {
        public static IBlogReader BlogReader()
        {
            return new WordPressPlugInReader(new NetworkWire(), ((IBlogApp)Application.Current).AppValues);
        }
    }
}
