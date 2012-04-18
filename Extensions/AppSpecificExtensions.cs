using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WordPressStarterKit.Extensions
{
    public static class AppSpecificExtensions
    {
        public static void SetBackgroundForTheme(this Control c)
        {
            // adjust background for light theme'd phones (default bg is for dark)
            if ((Visibility.Visible == (Visibility)Application.Current.Resources["PhoneLightThemeVisibility"]))
            {
                ImageBrush ib = (ImageBrush)Application.Current.Resources["PanoramaBackgroundImage-light"];
                c.Background = ib;
            }
        }

        [Obsolete("This functionality has been replaced by code in the plugin.", true)]
        public static string scrubHTML(this string input)
        {
            // used because XDoc.Parse doesn't handle &'s very well....

            return input.Replace("&ndash;", "-")
                .Replace("&hellip;", "...")
                .Replace("&ldquo;", "\"")
                .Replace("&rdquo;", "\"")
                .Replace("&amp;", "&")
                .Replace("&amp", "&")
                .Replace("&#38;", "|")
                .Replace("&#124;", "|");
        }
    }
}