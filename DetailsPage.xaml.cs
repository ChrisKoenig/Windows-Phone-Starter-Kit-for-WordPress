//
//    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
//    Use of this sample source code is subject to the terms of the Microsoft license
//    agreement under which you licensed this sample source code and is provided AS-IS.
//    If you did not accept the terms of the license agreement, you are not authorized
//    to use this sample source code.  For the terms of the license, please see the
//    license agreement between you and Microsoft.
//
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Navigation;
using System.Xml.Linq;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using WordPressStarterKit.Models;

namespace WordPressStarterKit
{
    public partial class PivotPage1 : PhoneApplicationPage
    {
        public string post_id;
        public string cat_id;
        public string site_url;
        public string sub_title;
        public string description;
        public string selitem;
        public string title;

        public PivotPage1()
        {
            InitializeComponent();
            //hidden progress bar control -- used in add comment section
            performanceProgressBar3.Visibility = Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadCommentsForPost();
        }

        private void LoadCommentsForPost()
        {
            string from_section = "", keyword = "";
            NavigationContext.QueryString.TryGetValue("title", out title);
            NavigationContext.QueryString.TryGetValue("sub_title", out sub_title);
            NavigationContext.QueryString.TryGetValue("post_id", out post_id);
            NavigationContext.QueryString.TryGetValue("cat_id", out cat_id);
            NavigationContext.QueryString.TryGetValue("site_url", out site_url);
            NavigationContext.QueryString.TryGetValue("from_section", out from_section);
            NavigationContext.QueryString.TryGetValue("keyword", out keyword);
            pivot.Title = title;
            // loading get recents a second time. Passing WordPress post details between XAML pages can easily exceed the 2050 character limit.
            if (from_section == "home")
            {
                ReadRss(new Uri(site_url + "?feed=get_recent&datetime=" + DateTime.Now.Ticks));
            }
            else if (from_section == "categories")
            {
                ReadSubCat(new Uri(String.Format("{0}?feed=get_cat_feed&cat_id={1}&datetime={2}", site_url, cat_id, DateTime.Now.Ticks)));
            }
            else if (from_section == "search")
            {
                button2.Visibility = Visibility.Collapsed;
                string output = string.Format("{0}?s={1}&feed=rss2&datetime={2}", site_url, keyword, DateTime.Now.Ticks);
                ReadWP(new Uri(output));
            }
        }

        public void ReadRss(Uri rssUri)
        {
            WebClient wclient = new WebClient();
            wclient.OpenReadCompleted += (sender, e) =>
            {
                if (e.Error != null)
                    return;
                Stream str = e.Result;
                XDocument xdoc = XDocument.Load(str);
                List<RSSFeedItem> rssFeedItems = (from item in xdoc.Descendants("item")
                                                  select new RSSFeedItem()
                                                  {
                                                      Title = item.Element("title").Value,
                                                      Author = item.Element("author").Value,
                                                      Description = item.Element("description").Value,
                                                      Tags = item.Element("tags").Value,
                                                      ID = item.Element("id").Value,
                                                      Date = item.Element("pubDate").Value,
                                                      subTitle = String.Format("{0} | {1} | {2}", item.Element("author").Value, item.Element("pubDate").Value, item.Element("tags").Value),
                                                  }).Take(10).ToList();
                str.Close();
                for (int i = rssFeedItems.Count - 1; i >= 0; i--)
                {
                    if (rssFeedItems[i].ID == post_id)
                    {
                        description = Uri.UnescapeDataString(rssFeedItems[i].Description);
                        description = description.Replace("+", " ");
                        webBrowser1.NavigateToString(WebBrowserHelper.WrapHtml(description, webBrowser1.ActualWidth));
                        postTitleTxt.Text = rssFeedItems[i].Title;
                        postSubTitleTxt.Text = rssFeedItems[i].subTitle;
                        postTitleTxt2.Text = rssFeedItems[i].Title;
                        postSubTitleTxt2.Text = rssFeedItems[i].subTitle;
                        //load comments feed
                        string output = string.Format("{0}?feed=get_comments_feed&post_id={1}&timestamp={2}", site_url, post_id, DateTime.Now.Ticks);
                        ReadRssComments(new Uri(output));
                    }
                };
            };
            wclient.OpenReadAsync(rssUri);
        }

        public void ReadRssComments(Uri rssUri)
        {
            WebClient wclient = new WebClient();
            wclient.OpenReadCompleted += (sender, e) =>
            {
                if (e.Error != null)
                    return;

                Stream str = e.Result;
                XDocument xdoc = XDocument.Load(str);
                List<RSSCommentItem> rssCommentItems = (from item in xdoc.Descendants("item")
                                                        select new RSSCommentItem()
                                                        {
                                                            Author = item.Element("author").Value.Trim(),
                                                            Email = item.Element("email").Value.Trim(),
                                                            Comment = item.Element("comment").Value.Trim()
                                                        }).Take(30).ToList();
                str.Close();
                commentList.Items.Clear();
                rssCommentItems.ForEach(item => commentList.Items.Add(item));
                nocommentTxt.Visibility = rssCommentItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            };
            wclient.OpenReadAsync(rssUri);
        }

        public void ReadWP(Uri rssUri)
        {
            WebClient wclient = new WebClient();
            wclient.OpenReadCompleted += (sender, e) =>
            {
                if (e.Error != null)
                    return;
                Stream str = e.Result;
                XDocument xdoc = XDocument.Load(str);
                List<RSSFeedItem> rssFeedItems = (from item in xdoc.Descendants("item")
                                                  select new RSSFeedItem()
                                                  {
                                                      Title = item.Element("title").Value,
                                                      Date = item.Element("pubDate").Value,
                                                      Description = item.Element("description").Value,
                                                  }).Take(10).ToList();
                // close
                str.Close();

                for (int i = rssFeedItems.Count - 1; i >= 0; i--)
                {
                    if (i == 0)
                    {
                        description = Uri.UnescapeDataString(rssFeedItems[i].Description);
                        description = description.Replace("+", " ");
                        webBrowser1.NavigateToString(WebBrowserHelper.WrapHtml(description, webBrowser1.ActualWidth));
                        postTitleTxt.Text = rssFeedItems[i].Title;
                        postSubTitleTxt.Text = rssFeedItems[i].subTitle;
                        postTitleTxt2.Text = rssFeedItems[i].Title;
                        postSubTitleTxt2.Text = rssFeedItems[i].subTitle;
                    }
                }
            };
            wclient.OpenReadAsync(rssUri);
        }

        public void ReadSubCat(Uri rssUri)
        {
            WebClient wclient = new WebClient();
            wclient.OpenReadCompleted += (sender, e) =>
            {
                if (e.Error != null)
                    return;
                Stream str = e.Result;
                XDocument xdoc = XDocument.Load(str);
                List<RSSFeedItem> rssFeedItems = (from item in xdoc.Descendants("item")
                                                  select new RSSFeedItem()
                                                  {
                                                      Title = item.Element("title").Value,
                                                      Author = item.Element("author").Value,
                                                      Description = item.Element("description").Value,
                                                      Tags = item.Element("tags").Value,
                                                      ID = item.Element("id").Value,
                                                      Date = item.Element("pubDate").Value,
                                                      subTitle = String.Format("{0} | {1} | {2}", item.Element("author").Value, item.Element("pubDate").Value, item.Element("tags").Value),
                                                  }).Take(10).ToList();
                str.Close();

                for (int i = rssFeedItems.Count - 1; i >= 0; i--)
                {
                    if (rssFeedItems[i].ID == post_id)
                    {
                        description = System.Uri.UnescapeDataString(rssFeedItems[i].Description);
                        description = description.Replace("+", " ");
                        webBrowser1.NavigateToString(WebBrowserHelper.WrapHtml(description, webBrowser1.ActualWidth));
                        postTitleTxt.Text = rssFeedItems[i].Title;
                        postSubTitleTxt.Text = rssFeedItems[i].subTitle;
                        postTitleTxt2.Text = rssFeedItems[i].Title;
                        postSubTitleTxt2.Text = rssFeedItems[i].subTitle;
                    }
                }
            };
            wclient.OpenReadAsync(rssUri);
        }

        private void submitComment(object sender, RoutedEventArgs e)
        {
            performanceProgressBar3.Visibility = Visibility.Visible;
            var commentEsc = commentTxt.Text;
            string output = string.Format("{0}?feed=add_comment&post_id={1}&name={2}&comment={3}&email={2}&timestamp={4}", site_url, post_id, System.Uri.EscapeDataString(emailTxt.Text), System.Uri.EscapeDataString(commentEsc), DateTime.Now.Ticks);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(output, UriKind.Absolute));
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.BeginGetRequestStream(RequestReady, request);
        }

        private void RequestReady(IAsyncResult asyncResult)
        {
            HttpWebRequest request = asyncResult.AsyncState as HttpWebRequest;
            this.Dispatcher.BeginInvoke(() => request.BeginGetResponse(ResponseReady, request));
        }

        private void ResponseReady(IAsyncResult asyncResult)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                promptTxt.Text = "Comment submitted!";
                //hide progress bar control
                performanceProgressBar3.Visibility = Visibility.Collapsed;
                string output = string.Format("{0}?feed=get_comments_feed&post_id={1}&time={2}", site_url, post_id, DateTime.Now.Ticks);
                ReadRssComments(new Uri(output));
                commentTxt.Text = "";
                emailTxt.Text = "";
            });
        }

        private void WebBrowser1_ScriptNotify(object sender, NotifyEventArgs e)
        {
            webBrowser1.Dispatcher.BeginInvoke(() => WebBrowserHelper.OpenBrowser(e.Value));
        }

        private void launchSite(object sender, RoutedEventArgs e)
        {
            var url = String.Format("{0}?p={1}", site_url, post_id);
            WebBrowserTask webBrowserTask = new WebBrowserTask { URL = url };
            webBrowserTask.Show();
        }

        private void LightDarkImageButton_Click(object sender, RoutedEventArgs e)
        {
            LoadCommentsForPost();
        }
    }
}