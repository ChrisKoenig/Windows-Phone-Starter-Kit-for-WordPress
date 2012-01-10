using System;
using System.Collections.Generic;
using WordPressStarterKit.Models;

namespace WordPressStarterKit.Networking
{
    public interface IBlogReader
    {
        // void ReadRss(Uri rssUrl, Action<List<RSSFeedItem>, Exception> callback);
        void GetRecentRss(Action<List<RSSFeedItem>, Exception> callback);
        void GetRssByCategory(string CategoryId, Action<List<RSSFeedItem>, Exception> callback);
        void BlogSearch(string SearchTerms, Action<List<RSSFeedItem>, Exception> callback);
        void ReadUserinfo(Action<List<UserInfo>, Exception> callback);
        void GetBlogCategories(Action<List<CatFeedItem>, Exception> callback);
        void ReadRssComments(string PostId, Action<List<RSSCommentItem>, Exception> callback);
    }
}
