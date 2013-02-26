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
        const string consumerKey = "";
        const string consumerSecret = "";

        static async Task Run()
        {
            // initialize computehash function
            OAuthUtility.ComputeHash = (key, buffer) => { using (var hmac = new HMACSHA1(key)) { return hmac.ComputeHash(buffer); } };

            // sample, twitter access flow
            var accessToken = await TwitterClient.AuthorizeSample(consumerKey, consumerSecret);

            var client = new TwitterClient(consumerKey, consumerSecret, accessToken);

            var tl = await client.GetTimeline(10, 1);
            Console.WriteLine(tl);
        }

        static void Main(string[] args)
        {
            Run().Wait();
        }
    }
}