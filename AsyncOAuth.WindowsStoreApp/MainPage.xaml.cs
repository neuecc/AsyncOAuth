using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace AsyncOAuth.WindowsStoreApp
{
    public sealed partial class MainPage : Page
    {
        // set your token
        const string consumerKey = "";
        const string consumerSecret = "";
        const string accessTokenKey = "";
        const string accessTokenSecret = "";

        readonly AccessToken accessToken = new AccessToken(accessTokenKey, accessTokenSecret);

        public MainPage()
        {
            this.InitializeComponent();
        }

        // atfirst, see App.xaml.cs
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var client = new TwitterClient(consumerKey, consumerSecret, accessToken);

            var tl = await client.GetTimeline(10, 1);
            await new MessageDialog(tl).ShowAsync();
        }
    }
}
