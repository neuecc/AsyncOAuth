using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AsyncOAuth
{
    /// <summary>OAuth Authorization Client</summary>
    public class OAuthAuthorizer
    {
        readonly string consumerKey;
        readonly string consumerSecret;

        public OAuthAuthorizer(string consumerKey, string consumerSecret)
        {
            this.consumerKey = consumerKey;
            this.consumerSecret = consumerSecret;
        }

        async Task<TokenResponse<T>> GetTokenResponse<T>(string url, OAuthMessageHandler handler, HttpContent postValue, Func<string, string, T> tokenFactory) where T : Token
        {
            var client = new HttpClient(handler);

            var response = await client.PostAsync(url, postValue ?? new FormUrlEncodedContent(Enumerable.Empty<KeyValuePair<string, string>>())).ConfigureAwait(false);
            var tokenBase = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new HttpRequestException(response.StatusCode + ":" + tokenBase); // error message
            }

            var splitted = tokenBase.Split('&').Select(s => s.Split('=')).ToDictionary(s => s.First(), s => s.Last());
            var token = tokenFactory(splitted["oauth_token"].UrlDecode(), splitted["oauth_token_secret"].UrlDecode());
            var extraData = splitted.Where(kvp => kvp.Key != "oauth_token" && kvp.Key != "oauth_token_secret")
                .ToLookup(kvp => kvp.Key, kvp => kvp.Value);
            return new TokenResponse<T>(token, extraData);
        }

        /// <summary>construct AuthrizeUrl + RequestTokenKey</summary>
        public string BuildAuthorizeUrl(string authUrl, RequestToken requestToken)
        {
            Precondition.NotNull(authUrl, "authUrl");
            Precondition.NotNull(requestToken, "accessToken");

            return authUrl + "?oauth_token=" + requestToken.Key;
        }

        /// <summary>asynchronus get RequestToken</summary>
        public Task<TokenResponse<RequestToken>> GetRequestToken(string requestTokenUrl, IEnumerable<KeyValuePair<string, string>> parameters = null, HttpContent postValue = null)
        {
            Precondition.NotNull(requestTokenUrl, "requestTokenUrl");

            var handler = new OAuthMessageHandler(consumerKey, consumerSecret, token: null, optionalOAuthHeaderParameters: parameters);
            return GetTokenResponse(requestTokenUrl, handler, postValue, (key, secret) => new RequestToken(key, secret));
        }

        /// <summary>asynchronus get GetAccessToken</summary>
        public Task<TokenResponse<AccessToken>> GetAccessToken(string accessTokenUrl, RequestToken requestToken, string verifier, IEnumerable<KeyValuePair<string, string>> parameters = null, HttpContent postValue = null)
        {
            Precondition.NotNull(accessTokenUrl, "accessTokenUrl");
            Precondition.NotNull(requestToken, "requestToken");
            Precondition.NotNull(verifier, "verifier");

            var verifierParam = new KeyValuePair<string, string>("oauth_verifier", verifier.Trim());

            if (parameters == null) parameters = Enumerable.Empty<KeyValuePair<string, string>>();
            var handler = new OAuthMessageHandler(consumerKey, consumerSecret, token: requestToken, optionalOAuthHeaderParameters: parameters.Concat(new[] { verifierParam }));

            return GetTokenResponse(accessTokenUrl, handler, postValue, (key, secret) => new AccessToken(key, secret));
        }
    }
}