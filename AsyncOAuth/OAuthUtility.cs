using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace AsyncOAuth
{
    public static class OAuthUtility
    {
        public delegate byte[] HashFunction(byte[] key, byte[] buffer);

        private static readonly Random random = new Random();

        /// <summary>
        /// <para>hashKey -> buffer -> hashedBytes</para>
        /// <para>ex:</para>
        /// <para>ComputeHash = (key, buffer) => { using (var hmac = new HMACSHA1(key)) { return hmac.ComputeHash(buffer); } };</para>
        /// <para>ex(WinRT): </para>
        /// <para>ComputeHash = (key, buffer) =></para>
        /// <para>{</para>
        /// <para>&#160;&#160;&#160;&#160;var crypt = Windows.Security.Cryptography.Core.MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");</para>
        /// <para>&#160;&#160;&#160;&#160;var keyBuffer = Windows.Security.Cryptography.CryptographicBuffer.CreateFromByteArray(key);</para>
        /// <para>&#160;&#160;&#160;&#160;var cryptKey = crypt.CreateKey(keyBuffer);</para>
        /// <para>&#160;</para>
        /// <para>&#160;&#160;&#160;&#160;var dataBuffer = Windows.Security.Cryptography.CryptographicBuffer.CreateFromByteArray(buffer);</para>
        /// <para>&#160;&#160;&#160;&#160;var signBuffer = Windows.Security.Cryptography.Core.CryptographicEngine.Sign(cryptKey, dataBuffer);</para>
        /// <para>&#160;</para>
        /// <para>&#160;&#160;&#160;&#160;byte[] value;</para>
        /// <para>&#160;&#160;&#160;&#160;Windows.Security.Cryptography.CryptographicBuffer.CopyToByteArray(signBuffer, out value);</para>
        /// <para>&#160;&#160;&#160;&#160;return value;</para>
        /// <para>};</para>
        /// </summary>
        public static HashFunction ComputeHash { private get; set; }

        static string GenerateSignature(string consumerSecret, Uri uri, HttpMethod method, Token token, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            if (ComputeHash == null)
            {
                throw new InvalidOperationException("ComputeHash is null, must initialize before call OAuthUtility.HashFunction = /* your computeHash code */ at once.");
            }

            var hmacKeyBase = consumerSecret.UrlEncode() + "&" + ((token == null) ? "" : token.Secret).UrlEncode();

            var queryParams = Utility.ParseQueryString(uri.GetComponents(UriComponents.Query | UriComponents.KeepDelimiter, UriFormat.UriEscaped));

            var stringParameter = parameters
                .Where(x => x.Key.ToLower() != "realm")
                .Concat(queryParams)
                .OrderBy(p => p.Key, StringComparer.Ordinal)
                .ThenBy(p => p.Value, StringComparer.Ordinal)
                .Select(p => p.Key.UrlEncode() + "=" + p.Value.UrlEncode())
                .ToString("&");
            var signatureBase = method.ToString() +
                "&" + uri.GetComponents(UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.Unescaped).UrlEncode() +
                "&" + stringParameter.UrlEncode();

            var hash = ComputeHash(Encoding.UTF8.GetBytes(hmacKeyBase), Encoding.UTF8.GetBytes(signatureBase));
            return Convert.ToBase64String(hash).UrlEncode();
        }

        public static IEnumerable<KeyValuePair<string, string>> BuildBasicParameters(string consumerKey, string consumerSecret, string url, HttpMethod method, Token token = null, IEnumerable<KeyValuePair<string, string>> optionalParameters = null)
        {
            Precondition.NotNull(url, "url");

            var parameters = new List<KeyValuePair<string, string>>(capacity: 7)
            {
                new KeyValuePair<string,string>("oauth_consumer_key", consumerKey),
                new KeyValuePair<string,string>("oauth_nonce", random.Next().ToString() ),
                new KeyValuePair<string,string>("oauth_timestamp", DateTime.UtcNow.ToUnixTime().ToString() ),
                new KeyValuePair<string,string>("oauth_signature_method", "HMAC-SHA1" ),
                new KeyValuePair<string,string>("oauth_version", "1.0" )
            };
            if (token != null) parameters.Add(new KeyValuePair<string, string>("oauth_token", token.Key));
            if (optionalParameters == null) optionalParameters = Enumerable.Empty<KeyValuePair<string, string>>();

            var signature = GenerateSignature(consumerSecret, new Uri(url), method, token, parameters.Concat(optionalParameters));

            parameters.Add(new KeyValuePair<string, string>("oauth_signature", signature));

            return parameters;
        }

        public static HttpClient CreateOAuthClient(string consumerKey, string consumerSecret, AccessToken accessToken, IEnumerable<KeyValuePair<string, string>> optionalOAuthHeaderParameters = null)
        {
            return new HttpClient(new OAuthMessageHandler(consumerKey, consumerSecret, accessToken, optionalOAuthHeaderParameters));
        }


        public static HttpClient CreateOAuthClient(HttpMessageHandler innerHandler, string consumerKey, string consumerSecret, AccessToken accessToken, IEnumerable<KeyValuePair<string, string>> optionalOAuthHeaderParameters = null)
        {
            return new HttpClient(new OAuthMessageHandler(innerHandler, consumerKey, consumerSecret, accessToken, optionalOAuthHeaderParameters));
        }
    }
}