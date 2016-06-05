using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AsyncOAuth.ConsoleApp
{
    // a sample of ETrade client
    public class ETradeClient
    {
        readonly string consumerKey;
        readonly string consumerSecret;
        readonly AccessToken accessToken;

        public ETradeClient(string consumerKey, string consumerSecret, AccessToken accessToken)
        {
            this.consumerKey = consumerKey;
            this.consumerSecret = consumerSecret;
            this.accessToken = accessToken;
        }

        // sample flow for ETrade authroize
        public async static Task<AccessToken> AuthorizeSample(string consumerKey, string consumerSecret)
        {
            try
            {
                // create authorizer
                var authorizer = new OAuthAuthorizer(consumerKey, consumerSecret);
            
                // get request token
                List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>();
                args.Add(new KeyValuePair<string, string>("oauth_callback", "oob"));
              
                var tokenResponse = await authorizer.GetRequestToken("https://etws.etrade.com/oauth/request_token",args);
                var requestToken = tokenResponse.Token;
               

                //   var pinRequestUrl = authorizer.BuildAuthorizeUrl("https://us.etrade.com/e/t/etws/authorize", requestToken);
                string pinRequestUrl = "https://us.etrade.com/e/t/etws/authorize" + "?key=" + consumerKey + "&token=" + requestToken.Key;
                // open browser and get PIN Code
                Process.Start(pinRequestUrl);

                // enter pin
                Console.WriteLine("ENTER PIN");
                var pinCode = Console.ReadLine();

                // get access token
              
                var accessTokenResponse = await authorizer.GetAccessToken("https://etws.etrade.com/oauth/access_token", requestToken, pinCode);

                // save access token.
                var accessToken = accessTokenResponse.Token;
                Console.WriteLine("Access Granted: ");
                Console.WriteLine("  Access Key:" + accessToken.Key);
                Console.WriteLine("  Access Secret:" + accessToken.Secret);
                Console.WriteLine("===============================================");

                return accessToken;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception " + e.ToString());
            }

            return null;
        }

        public async Task<string> GetQuote()
        {
            try
            {


                var client = OAuthUtility.CreateOAuthClient(consumerKey, consumerSecret, accessToken);

                var json = await client.GetStringAsync("https://etwssandbox.etrade.com/market/sandbox/rest/quote/GOOG,MSFT.json?detailFlag=FUNDAMENTAL");

                return json;
            }
            catch (Exception e )
            {
                Console.WriteLine("Exception in GetQuote: " + e.ToString());
            }

            return null;
        }

       
    }
}