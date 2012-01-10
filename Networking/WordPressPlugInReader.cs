using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using WordPressStarterKit.Extensions;
using WordPressStarterKit.Models;

namespace WordPressStarterKit.Networking
{
    public class WordPressPlugInReader : IBlogReader
    {
        INetworkWire network;
        AppConfig app;

        public WordPressPlugInReader(INetworkWire NetworkHelper, AppConfig appSettings)
        {
            network = NetworkHelper;
            app = appSettings;
        }

        public void GetRecentRss(Action<List<RSSFeedItem>, Exception> callback)
        {
            var uri = new Uri(app.SiteURL + "?feed=get_recent");
            ReadRss(uri, (results, ex) =>
                {
                    callback(results, ex);
                });
        }

        public void GetRssByCategory(string CategoryId, Action<List<RSSFeedItem>, Exception> callback)
        {
            var uri = new Uri(String.Format("{0}?feed=get_cat_feed&cat_id={1}", app.SiteURL, CategoryId));
            ReadRss(uri, (results, ex) =>
                {
                    callback(results, ex);
                });
        }

        protected internal void ReadRss(Uri rssUri, Action<List<RSSFeedItem>, Exception> callback)
        {
            callback.CheckNotNullThrowException();

            network.GetStringFromURL(rssUri, (results, ex) =>
                {
                    if (ex != null)
                    {
                        callback(null, ex);
                        return;
                    }

                    var xdoc = XDocument.Parse(results);
                    List<RSSFeedItem> RSSFeedItems = (from item in xdoc.Descendants("item")
                                                      select new RSSFeedItem()
                                                      {
                                                          Title = item.IfNullEmptyString("title"),
                                                          Author = item.IfNullEmptyString("author"),
                                                          Description = item.IfNullEmptyString("description"),
                                                          Tags = item.IfNullEmptyString("tags"),
                                                          ID = item.IfNullEmptyString("id"),
                                                          Date = item.IfNullEmptyString("pubDate"),
                                                          subTitle = String.Format("{0} | {1} | {2}", item.IfNullEmptyString("author"), item.IfNullEmptyString("pubDate"), item.IfNullEmptyString("tags")),
                                                      }).Take(10).ToList();
                    callback(RSSFeedItems, null);
                });
        }

        public void BlogSearch(string SearchTerms, Action<List<RSSFeedItem>, Exception> callback)
        {
            var url = string.Format("{0}?s={1}&feed=rss2&timestamp={2}", app.SiteURL, SearchTerms, DateTime.Now.Ticks);
            var uri = new Uri(url);
            ReadRss(uri, (results, ex) =>
            {
                callback(results, ex);
            });
        }

        public void GetBlogCategories(Action<List<CatFeedItem>, Exception> callback)
        {
            callback.CheckNotNullThrowException();

            var uri = new Uri(app.SiteURL + "?feed=categories");
            network.GetStringFromURL(uri, (results, ex) =>
            {
                if (ex != null)
                    callback(null, ex);

                XDocument xdoc = XDocument.Parse(results);
                List<CatFeedItem> catFeedItems = (from item in xdoc.Descendants("item")
                                                  select new CatFeedItem()
                                                  {
                                                      Title = item.IfNullEmptyString("title"),
                                                      ID = item.IfNullEmptyString("id"),
                                                      subTitle = ""
                                                  }).Take(10).ToList();
                callback(catFeedItems, null);
            });
        }

        public void ReadUserinfo(Action<List<UserInfo>, Exception> callback)
        {
            callback.CheckNotNullThrowException();

            var uri = new Uri(String.Format("{0}?feed=user_info&user_id={1}&datetime={2}", app.SiteURL, app.SiteAuthorBlogUserID, DateTime.Now.Ticks));
            network.GetStringFromURL(uri, (results, ex) =>
                {
                    if (ex != null)
                    {
                        callback(null, ex);
                        return;
                    }

                    XDocument xdoc = XDocument.Parse(results);
                    List<UserInfo> blogInfo = (from item in xdoc.Descendants("UserInfo")
                                               select new UserInfo()
                                               {
                                                   ID = item.IfNullEmptyString("UserID"),
                                                   displayName = item.IfNullEmptyString("DisplayName"),
                                                   Email = item.IfNullEmptyString("EmailAddress"),
                                                   Avatar = item.IfNullEmptyString("Gravatar"),
                                                   Bio = item.IfNullEmptyString("Bio"),
                                               }).Take(10).ToList();
                    callback(blogInfo, null);
                });
        }

        public void ReadRssComments(string PostId, Action<List<RSSCommentItem>, Exception> callback)
        {
            callback.CheckNotNullThrowException();

            var uri = new Uri(string.Format("{0}?feed=get_comments_feed&post_id={1}&timestamp={2}", app.SiteURL, PostId, DateTime.Now.Ticks));
            network.GetStringFromURL(uri, (results, ex) =>
            {
                if (ex != null)
                    callback(null, ex);

                XDocument xdoc = XDocument.Parse(results);

                List<RSSCommentItem> rssCommentItems = (from item in xdoc.Descendants("item")
                                                        select new RSSCommentItem()
                                                        {
                                                            Author = item.Element("author").Value.Trim(),
                                                            Email = item.Element("email").Value.Trim(),
                                                            Comment = item.Element("comment").Value.Trim()
                                                        }).Take(30).ToList();
                callback(rssCommentItems, null);
            });
        }
    }
}