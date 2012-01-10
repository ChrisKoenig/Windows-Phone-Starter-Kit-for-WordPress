using System;
using System.Windows;
using WordPressStarterKit.Models;
using WordPressStarterKit.Networking;

namespace WordPressStarterKit
{
    public class AppConfig
    {
        // Initial Values
        public AppConfig()
        {
            SiteAuthorName = "Site Author";
            // SiteURL = "http://blog.koenigweb.com/";
            SiteURL = @"http://chriskoenig.net";
            SiteTitle = "Chris' Blog!";
            SiteEmail = @"chris.koenig@microsoft.com";
            SiteAuthorBlogUserID = 0; // Default is likely 0, but not always
            categoryState = string.Empty;
            cat_id = string.Empty;
            keyWords = string.Empty;
        }

        public RSSFeedItem CurrentPost { get; set; }

        public CatFeedItem CurrentCategory { get; set; }

        public string SiteURL { get; private set; }

        public string SiteAuthorName { get; set; }

        public string SiteTitle { get; set; }

        public int SiteAuthorBlogUserID { get; private set; }

        public string SiteEmail { get; set; }

        public string categoryState { get; set; }

        public string cat_id { get; set; }

        public string keyWords { get; set; }
    }
}