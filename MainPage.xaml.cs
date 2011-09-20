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
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Tasks;
using WordPressStarterKit.Models;

namespace WordPressStarterKit
{
    public partial class MainPage : PhoneApplicationPage
    {
        //Enter your WordPress URL below
        public string siteURL = "http://blog.koenigweb.com/";
        //dynamic variables
        public string siteAuthor = "";
        public string siteEmail = "";
        public string categoryState = "";
        public string cat_id = "";

        public MainPage()
        {
            InitializeComponent();

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
                    ReadRss(new Uri(siteURL + "?feed=get_recent"));
                }
            };
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
                List<RSSFeedItem> RSSFeedItems = (from item in xdoc.Descendants("item")
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
                homeList.Items.Clear();
                RSSFeedItems.ForEach(item => homeList.Items.Add(item));
                //preload About section
                ReadUserinfo(new Uri(siteURL + "?feed=user_info&datetime=" + DateTime.Now.Ticks));
            };
            wclient.OpenReadAsync(rssUri);
        }

        public void ReadUserinfo(Uri rssUri)
        {
            WebClient wclient = new WebClient();
            wclient.OpenReadCompleted += (sender, e) =>
            {
                if (e.Error != null)
                    return;
                Stream str = e.Result;
                XDocument xdoc = XDocument.Load(str);
                List<UserInfo> blogInfo = (from item in xdoc.Descendants("UserInfo")
                                           select new UserInfo()
                                   {
                                       ID = item.Element("UserID").Value,
                                       displayName = item.Element("DisplayName").Value,
                                       Email = item.Element("EmailAddress").Value,
                                       Avatar = item.Element("Gravatar").Value,
                                       Bio = item.Element("Bio").Value,
                                   }).Take(10).ToList();
                str.Close();
                aboutTxt.Text = "About " + blogInfo[0].displayName;
                Uri uri = new Uri(blogInfo[0].Avatar);
                aboutImg.Source = new BitmapImage(uri);
                bioTxt.Text = blogInfo[0].Bio;
                siteAuthor = blogInfo[0].displayName;
                siteEmail = blogInfo[0].Email;
                //preload Categories section
                ReadCats(new Uri(siteURL + "?feed=categories&datetime=" + DateTime.Now.Ticks));
            };
            wclient.OpenReadAsync(rssUri);
        }

        public void ReadCats(Uri rssUri)
        {
            WebClient wclient = new WebClient();
            wclient.OpenReadCompleted += (sender, e) =>
            {
                if (e.Error != null)
                    return;
                Stream str = e.Result;
                XDocument xdoc = XDocument.Load(str);
                List<CatFeedItem> catFeedItems = (from item in xdoc.Descendants("item")
                                                  select new CatFeedItem()
                                                  {
                                                      Title = item.Element("title").Value,
                                                      ID = item.Element("id").Value,
                                                      subTitle = ""
                                                  }).Take(10).ToList();
                str.Close();
                catList.Items.Clear();
                catFeedItems.ForEach(item => catList.Items.Add(item));
            };
            wclient.OpenReadAsync(rssUri);
        }

        public void ReadSubCat(Uri rssUri)
        {
            categoryState = "loaded";
            WebClient wclient = new WebClient();
            wclient.OpenReadCompleted += (sender, e) =>
            {
                if (e.Error != null)
                    return;
                Stream str = e.Result;
                XDocument xdoc = XDocument.Load(str);
                List<RSSFeedItem> RSSFeedItems = (from item in xdoc.Descendants("item")
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
                catList.Items.Clear();
                RSSFeedItems.ForEach(item => catList.Items.Add(item));
                performanceProgressBar2.Visibility = Visibility.Collapsed;
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
                List<RSSFeedItem> RSSFeedItems = (from item in xdoc.Descendants("item")
                                                  select new RSSFeedItem()
                                                  {
                                                      Title = item.Element("title").Value,
                                                      Date = item.Element("pubDate").Value,
                                                      Description = item.Element("description").Value,
                                                  }).Take(10).ToList();
                str.Close();
                searchList.Items.Clear();
                RSSFeedItems.ForEach(item => searchList.Items.Add(item));
                if (RSSFeedItems.Count == 0)
                {
                    promptTxt.Text = "No result found.";
                }
                else
                {
                    promptTxt.Text = "";
                }
                //hide progress bar
                performanceProgressBar.Visibility = Visibility.Collapsed;
            };
            wclient.OpenReadAsync(rssUri);
        }

        private void homeList_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (homeList.SelectedIndex == -1)
                return;
            RSSFeedItem currentPost = (RSSFeedItem)homeList.SelectedItem;
            string output = string.Format("/DetailsPage.xaml?title={0}?&sub_title={1}&post_id={2}&site_url={3}&from_section=home", currentPost.Title, currentPost.subTitle, currentPost.ID, siteURL);
            NavigationService.Navigate(new Uri(output, UriKind.Relative));
            homeList.SelectedIndex = -1;
        }

        private void Cat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (catList.SelectedIndex == -1)
                return;
            if (categoryState == "")
            {
                performanceProgressBar2.Visibility = Visibility.Visible;
                CatFeedItem currentPost = (CatFeedItem)catList.SelectedItem;
                catList.Items.Clear();
                cat_id = currentPost.ID;
                ReadSubCat(new Uri(String.Format("{0}?feed=get_cat_feed&cat_id={1}", siteURL, currentPost.ID)));
            }
            else
            {
                RSSFeedItem currentPost = (RSSFeedItem)catList.SelectedItem;
                catList.Items.Clear();
                string output = string.Format("/DetailsPage.xaml?title={0}?&sub_title={1}&post_id={2}&site_url={3}&from_section=categories", currentPost.Title, currentPost.subTitle, currentPost.ID, siteURL);
                NavigationService.Navigate(new Uri(output, UriKind.Relative));
                categoryState = "";
                ReadCats(new Uri(siteURL + "?feed=categories"));
            }
            catList.SelectedIndex = -1;
        }

        private void Search_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (searchList.SelectedIndex == -1)
                return;
            RSSFeedItem currentPost = (RSSFeedItem)searchList.SelectedItem;
            string output = string.Format("/DetailsPage.xaml?title={0}?&sub_title={1}&site_url={2}&from_section=search&keyword={3}", currentPost.Title, currentPost.subTitle, siteURL, wpKeyword.Text);
            NavigationService.Navigate(new Uri(output, UriKind.Relative));
            searchList.SelectedIndex = -1;
        }

        private void sendEmail(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailAuthor = new EmailComposeTask();
            emailAuthor.To = String.Format("<a href='mailto:{0}'>{0}</a>", siteEmail);
            emailAuthor.Subject = siteAuthor + ", message from your WP7 WordPress Blog";
            emailAuthor.Body = "";
            emailAuthor.Show();
        }

        private void searchWP(object sender, RoutedEventArgs e)
        {
            performanceProgressBar.Visibility = Visibility.Visible;
            searchList.Items.Clear();
            string output = string.Format("{0}?s={1}&feed=rss2&timestamp={2}", siteURL, wpKeyword.Text, DateTime.Now.Ticks);
            ReadWP(new Uri(output));
        }

        private void cat_refresh(object sender, RoutedEventArgs e)
        {
            categoryState = "";
            ReadCats(new Uri(siteURL + "?feed=categories&timestamp=" + DateTime.Now.Ticks));
        }
    }
}