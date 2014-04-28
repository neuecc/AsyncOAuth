using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using AsyncOAuth.WindowsPhone.Resources;

namespace AsyncOAuth.WindowsPhone
{
    public partial class MainPage : PhoneApplicationPage
    {
        // set your token
        const string consumerKey = "";
        const string consumerSecret = "";
        const string accessTokenKey = "";
        const string accessTokenSecret = "";

        readonly AccessToken accessToken = new AccessToken(accessTokenKey, accessTokenSecret);

        public MainPage()
        {
            InitializeComponent();

        }

        // atfirst, see App.xaml.cs
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var client = new TwitterClient(consumerKey, consumerSecret, accessToken);

            var tl = await client.GetTimeline(10, 1);
            MessageBox.Show(tl);
        }
    }
}