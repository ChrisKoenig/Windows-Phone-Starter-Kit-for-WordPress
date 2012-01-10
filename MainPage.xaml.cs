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
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Tasks;
using WordPressStarterKit.Extensions;
using WordPressStarterKit.Models;
using WordPressStarterKit.Networking;

namespace WordPressStarterKit
{
    public partial class MainPage : PhoneApplicationPage
    {
        IBlogReader blogReader;
        AppConfig app = ((IBlogApp)Application.Current).AppValues;

        public MainPage() : this(null) { }

        public MainPage(IBlogReader BlogReader)
        {
            InitializeComponent();
            // poor mans IoC
            BlogReader.IfNullDoThis(() => BlogReader = Infrastructure.BlogReader());
            blogReader = BlogReader;

            panorama.SetBackgroundForTheme();

            Loaded += (s, e) =>
            {
                if (!NetworkInterface.GetIsNetworkAvailable() || NetworkInterface.NetworkInterfaceType == NetworkInterfaceType.None)
                {
                    MessageBox.Show("This application requires a network connection to function properly. Please fix your internet connection and re-launch the app.", "Network Error", MessageBoxButton.OK);
                }
                else
                {
                    this.DataContext = this;
                    //search section -- progress bar control
                    performanceProgressBar.Visibility = Visibility.Collapsed;
                    //category section -- progress bar control
                    performanceProgressBar2.Visibility = Visibility.Collapsed;
                    //load recent post

                    // UserInfo then RecientFeed then Categories
                    ReadUserinfo(() =>
                     GetRecentRssFeed(() =>
                         ReadCats(null)));
                }
            };
        }

        private void updateAppValues()
        {
            ((IBlogApp)Application.Current).AppValues = app;
        }

        protected internal void GetRecentRssFeed(Action OnComplete)
        {
            blogReader.GetRecentRss((results, ex) =>
            {
                if (ex != null)
                    return;

                List<RSSFeedItem> RSSFeedItems = results;
                homeList.Items.Clear();
                RSSFeedItems.ForEach(item => homeList.Items.Add(item));

                //preload About section
                OnComplete.IfNotNullInvoke();
            });
        }

        protected internal void ReadUserinfo(Action OnComplete)
        {
            blogReader.ReadUserinfo((results, ex) =>
            {
                if (ex != null)
                    return;

                List<UserInfo> blogInfo = results;

                aboutTxt.Text = "About " + blogInfo[0].displayName;
                Uri uri = new Uri(blogInfo[0].Avatar);
                aboutImg.Source = new BitmapImage(uri);
                bioTxt.Text = blogInfo[0].Bio;
                app.SiteAuthorName = blogInfo[0].displayName;
                app.SiteEmail = blogInfo[0].Email;

                updateAppValues();

                //preload Categories section
                OnComplete.IfNotNullInvoke();
            });
        }

        protected internal void ReadCats()
        {
            ReadCats(null);
        }

        protected internal void ReadCats(Action OnComplete)
        {
            blogReader.GetBlogCategories((results, ex) =>
            {
                if (ex != null)
                    return;

                List<CatFeedItem> catFeedItems = results;
                catList.Items.Clear();
                catFeedItems.ForEach(item => catList.Items.Add(item));

                OnComplete.IfNotNullInvoke();
            });
        }

        protected internal void ReadSubCat()
        {
            app.categoryState = "loaded";
            updateAppValues();

            blogReader.GetRssByCategory(app.cat_id, (results, ex) =>
            {
                if (ex != null)
                    return;

                List<RSSFeedItem> RSSFeedItems = results;
                catList.Items.Clear();
                RSSFeedItems.ForEach(item => catList.Items.Add(item));
                performanceProgressBar2.Visibility = Visibility.Collapsed;
            });
        }

        protected internal void BlogSearch(string SearchTerms)
        {
            blogReader.BlogSearch(SearchTerms, (results, ex) =>
            {
                if (ex != null)
                    return;

                List<RSSFeedItem> RSSFeedItems = results;
                searchList.Items.Clear();
                RSSFeedItems.ForEach(item => searchList.Items.Add(item));
                promptTxt.Text = (RSSFeedItems.Count == 0) ? "No result found." : promptTxt.Text = "";
                performanceProgressBar.Visibility = Visibility.Collapsed;
            });
        }

        private void homeList_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (homeList.SelectedIndex == -1)
                return;

            app.CurrentPost = (RSSFeedItem)homeList.SelectedItem;
            updateAppValues();

            NavTo("/DetailsPage.xaml?from_section=home");
            homeList.SelectedIndex = -1;
        }

        private void NavTo(string url)
        {
            NavigationService.Navigate(new Uri(url, UriKind.Relative));
        }

        //private void Cat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        private void catList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (catList.SelectedIndex == -1)
                return;

            if (app.categoryState == "")
            {
                // Load list of posts based on category //
                performanceProgressBar2.Visibility = Visibility.Visible;
                app.CurrentCategory = (CatFeedItem)catList.SelectedItem;
                catList.Items.Clear();
                app.cat_id = app.CurrentCategory.ID;
                updateAppValues();

                ReadSubCat();
            }
            else
            {
                // Load Details Page (blog post) //
                // app.CurrentCategory = (CatFeedItem)catList.SelectedItem;
                app.CurrentPost = (RSSFeedItem)catList.SelectedItem;
                catList.Items.Clear();
                app.categoryState = "";
                updateAppValues();

                NavTo("/DetailsPage.xaml?from_section=categories");

                ReadCats();
            }
            catList.SelectedIndex = -1;
        }

        // private void Search_SelectionChanged(object sender, SelectionChangedEventArgs e)
        private void searchList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (searchList.SelectedIndex == -1)
                return;

            app.CurrentPost = (RSSFeedItem)searchList.SelectedItem;
            app.keyWords = wpKeyword.Text;
            updateAppValues();

            NavTo("/DetailsPage.xaml?from_section=search");
            searchList.SelectedIndex = -1;
        }

        private void sendEmail(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailAuthor = new EmailComposeTask()
            {
                To = String.Format("<a href='mailto:{0}'>{0}</a>", app.SiteEmail),
                Subject = String.Format("{0}, message from your {1} WP7 app", app.SiteAuthorName, app.SiteTitle),
                Body = ""
            };
            emailAuthor.Show();
        }

        private void searchWP(object sender, RoutedEventArgs e)
        {
            performanceProgressBar.Visibility = Visibility.Visible;
            searchList.Items.Clear();
            app.keyWords = wpKeyword.Text;
            updateAppValues();

            BlogSearch(app.keyWords);
        }

        private void cat_refresh(object sender, RoutedEventArgs e)
        {
            app.categoryState = "";
            updateAppValues();

            ReadCats();
        }
    }
}