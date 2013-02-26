using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AsyncOAuth.WindowsStoreApp
{
    // a sample of twitter client
    public class TwitterClient
    {
        readonly string consumerKey;
        readonly string consumerSecret;
        readonly AccessToken accessToken;

        public TwitterClient(string consumerKey, string consumerSecret, AccessToken accessToken)
        {
            this.consumerKey = consumerKey;
            this.consumerSecret = consumerSecret;
            this.accessToken = accessToken;
        }

        // see console app sample

        // sample flow for Twitter authroize
        //public async static Task<AccessToken> AuthorizeSample(string consumerKey, string consumerSecret)
        //{
        //    // create authorizer
        //    var authorizer = new OAuthAuthorizer(consumerKey, consumerSecret);

        //    // get request token
        //    var tokenResponse = await authorizer.GetRequestToken("https://api.twitter.com/oauth/request_token");
        //    var requestToken = tokenResponse.Token;

        //    var pinRequestUrl = authorizer.BuildAuthorizeUrl("https://api.twitter.com/oauth/authorize", requestToken);

        //    // open browser and get PIN Code
        //    Process.Start(pinRequestUrl);

        //    // enter pin
        //    Console.WriteLine("ENTER PIN");
        //    var pinCode = Console.ReadLine();

        //    // get access token
        //    var accessTokenResponse = await authorizer.GetAccessToken("https://api.twitter.com/oauth/access_token", requestToken, pinCode);

        //    // save access token.
        //    var accessToken = accessTokenResponse.Token;
        //    Console.WriteLine("Key:" + accessToken.Key);
        //    Console.WriteLine("Secret:" + accessToken.Secret);

        //    return accessToken;
        //}

        public async Task<string> GetTimeline(int count, int page)
        {
            var client = OAuthUtility.CreateOAuthClient(consumerKey, consumerSecret, accessToken);

            var json = await client.GetStringAsync("http://api.twitter.com/1.1/statuses/home_timeline.json?count=" + count + "&page=" + page);
            return json;
        }

        public async Task<string> PostUpdate(string status)
        {
            var client = OAuthUtility.CreateOAuthClient(consumerKey, consumerSecret, accessToken);

            var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("status", status) });

            var response = await client.PostAsync("http://api.twitter.com/1.1/statuses/update.json", content);
            var json = await response.Content.ReadAsStringAsync();
            return json;
        }

        public async Task GetStream(Action<string> fetchAction)
        {
            var client = OAuthUtility.CreateOAuthClient(consumerKey, consumerSecret, accessToken);
            using (var stream = await client.GetStreamAsync("https://userstream.twitter.com/1.1/user.json"))
            using (var sr = new StreamReader(stream))
            {
                while (!sr.EndOfStream)
                {
                    var s = await sr.ReadLineAsync();
                    fetchAction(s);
                }
            }
        }

        public async Task<string> UpdateWithMedia(string status, byte[] media, string fileName)
        {
            var client = OAuthUtility.CreateOAuthClient(consumerKey, consumerSecret, accessToken);

            var content = new MultipartFormDataContent();
            content.Add(new StringContent(status), "\"status\"");
            content.Add(new ByteArrayContent(media), "media[]", "\"" + fileName + "\"");

            var response = await client.PostAsync("https://upload.twitter.com/1/statuses/update_with_media.json", content);
            var json = await response.Content.ReadAsStringAsync();
            return json;
        }
    }
}