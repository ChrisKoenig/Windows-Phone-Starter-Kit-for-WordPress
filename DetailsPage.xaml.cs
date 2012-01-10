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
using WordPressStarterKit.Networking;
using WordPressStarterKit.Extensions;

namespace WordPressStarterKit
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        public string post_id;
        public string cat_id;
        public string site_url;
        public string sub_title;
        public string description;
        public string selitem;
        public string title;

        private IBlogReader blogReader;
        private AppConfig app;
        
        public DetailsPage() : this (null)
        {
            InitializeComponent();
        }
        public DetailsPage(IBlogReader BlogReader)
        {
            // Poor man's DI
            BlogReader.IfNullDoThis(() => BlogReader = Infrastructure.BlogReader());
            blogReader = BlogReader;

            InitializeComponent();
            //hidden progress bar control -- used in add comment section
            performanceProgressBar3.Visibility = Visibility.Collapsed;
            app = ((IBlogApp)Application.Current).AppValues;

            pivot.SetBackgroundForTheme();
        }

        private void updateAppValues()
        {
            ((IBlogApp)Application.Current).AppValues = app;
        }
        protected RSSFeedItem CurrentPost
        {
            get { return app.CurrentPost; }
            set
            {
                if (app.CurrentPost != value)
                {
                    app.CurrentPost = value;
                    updateAppValues();
                }
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            app = ((IBlogApp)Application.Current).AppValues;
            LoadPost();
        }
   private void LoadPost()
        {
            string from_section = "";
            string keyword = "";
            //CurrentPost = ((IBlogValues)Application.Current).AppValues
            if (CurrentPost == null)
                MessageBox.Show("No current post to show! #sadPanda");
            else
            {
                pivot.Title = CurrentPost.Title;
                webBrowser1.NavigateToString(WebBrowserHelper.WrapHtml(app.CurrentPost.Description, webBrowser1.ActualWidth));
                postTitleTxt.Text = app.CurrentPost.Title;
                postSubTitleTxt.Text = app.CurrentPost.subTitle;
                postTitleTxt2.Text = app.CurrentPost.Title;
                postSubTitleTxt2.Text = app.CurrentPost.subTitle;

                LoadComments(app.CurrentPost.ID);
            }
        }

        public void LoadComments(string postId)
        {
            blogReader.ReadRssComments(postId, (results, ex)=>
            {
                if(ex != null)
                return;

                List<RSSCommentItem> rssCommentItems = results;
                commentList.Items.Clear();
                rssCommentItems.ForEach(item => commentList.Items.Add(item));
                nocommentTxt.Visibility = rssCommentItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        private void submitComment(object sender, RoutedEventArgs e)
        {
            performanceProgressBar3.Visibility = Visibility.Visible;
            var commentEsc = commentTxt.Text;
            string output = string.Format("{0}?feed=add_comment&post_id={1}&name={2}&comment={3}&email={2}&timestamp={4}", app.SiteURL, app.CurrentPost.ID, System.Uri.EscapeDataString(emailTxt.Text), System.Uri.EscapeDataString(commentEsc), DateTime.Now.Ticks);
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
                string output = string.Format("{0}?feed=get_comments_feed&post_id={1}&time={2}", app.SiteURL, app.CurrentPost.ID, DateTime.Now.Ticks);
               // ReadRssComments(new Uri(output));
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
            var uri = new Uri(String.Format("{0}?p={1}", app.SiteURL, app.CurrentPost.ID));
            WebBrowserTask webBrowserTask = new WebBrowserTask { Uri = uri };
            webBrowserTask.Show();
        }

        private void LightDarkImageButton_Click(object sender, RoutedEventArgs e)
        {
            LoadPost();
        }
    }
}