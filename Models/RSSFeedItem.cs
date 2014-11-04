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
using System.Net;

namespace WordPressStarterKit.Models
{
    public class RSSFeedItem
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = HttpUtility.HtmlDecode(value); }
        }

        public string Author { get; set; }

        private string _description;
        public string Description {
            get { return _description; }
            set { _description = HttpUtility.HtmlDecode(value); }
        }

        public string Tags { get; set; }

        public string ID { get; set; }

        public string Date { get; set; }

        public string subTitle { get; set; }
    }
}