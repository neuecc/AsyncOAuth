using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AsyncOAuth.ConsoleApp
{
    class Program
    {
        // set your token
        /*Dear Graham Briggs,
                   This message is regarding the E*TRADE API.
                   Your key and secret for the sandbox environment are as follows:
                       oauth_consumer_key: b3de705c7e73dba19ed2cd406f24ea00 
                       consumer_secret: 41486fa59ab14b564d294b8b18e3fcb7	*/

        const string consumerKey = "b3de705c7e73dba19ed2cd406f24ea00";
        const string consumerSecret = "41486fa59ab14b564d294b8b18e3fcb7";

        static async Task Run()
        {
            // initialize computehash function
            OAuthUtility.ComputeHash = (key, buffer) => { using (var hmac = new HMACSHA1(key)) { return hmac.ComputeHash(buffer); } };

            // sample, ETrade access flow
            Console.WriteLine("ASyncOauth ETrade Sample: ");
            System.Threading.Thread.Sleep(1000);

            Console.WriteLine("Getting access token");
            var accessToken = await ETradeClient.AuthorizeSample(consumerKey, consumerSecret);

            if ( accessToken == null )
            {
                Console.WriteLine("Error getting access token");
                return;
            }

            //  create the client with the access token and consumer key
            var client = new ETradeClient(consumerKey, consumerSecret, accessToken);

            //  Get Quote:
            Console.WriteLine("Getting Quotes from sandbox server: ");

            var quoteResponse = await client.GetQuote();

            Console.WriteLine(quoteResponse);
        }

        static void Main(string[] args)
        {
            Run().Wait();

            //  Exit
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}