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
       

        const string consumerKey = "yourConsumerKey";
        const string consumerSecret = "yourConsumerToken";

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