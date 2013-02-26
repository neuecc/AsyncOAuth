using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AsyncOAuth.ConsoleApp
{
    // a sample of hatena client
    public class HatenaClient
    {
        readonly string consumerKey;
        readonly string consumerSecret;
        readonly AccessToken accessToken;

        public HatenaClient(string consumerKey, string consumerSecret, AccessToken accessToken)
        {
            this.consumerKey = consumerKey;
            this.consumerSecret = consumerSecret;
            this.accessToken = accessToken;
        }

        // sample flow for Hatena authroize
        public async static Task<AccessToken> AuthorizeSample(string consumerKey, string consumerSecret)
        {
            // create authorizer
            var authorizer = new OAuthAuthorizer(consumerKey, consumerSecret);

            // get request token
            var tokenResponse = await authorizer.GetRequestToken(
                "https://www.hatena.com/oauth/initiate",
                new[] { new KeyValuePair<string, string>("oauth_callback", "oob") },
                new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("scope", "read_public,write_public,read_private,write_private") }));
            var requestToken = tokenResponse.Token;

            var pinRequestUrl = authorizer.BuildAuthorizeUrl("https://www.hatena.ne.jp/oauth/authorize", requestToken);

            // open browser and get PIN Code
            Process.Start(pinRequestUrl);

            // enter pin
            Console.WriteLine("ENTER PIN");
            var pinCode = Console.ReadLine();

            // get access token
            var accessTokenResponse = await authorizer.GetAccessToken("https://www.hatena.com/oauth/token", requestToken, pinCode);

            // save access token.
            var accessToken = accessTokenResponse.Token;
            Console.WriteLine("Key:" + accessToken.Key);
            Console.WriteLine("Secret:" + accessToken.Secret);

            return accessToken;
        }

        HttpClient CreateOAuthClient()
        {
            return OAuthUtility.CreateOAuthClient(consumerKey, consumerSecret, accessToken);
        }

        public async Task<string> GetMy()
        {
            var client = CreateOAuthClient();

            var json = await client.GetStringAsync("http://n.hatena.com/applications/my.json");
            return json;
        }

        public async Task<string> AppicationStart()
        {
            var client = CreateOAuthClient();

            var response = await client.PostAsync("http://n.hatena.com/applications/start", new StringContent("", Encoding.UTF8));
            return await response.Content.ReadAsStringAsync();
        }
    }
}