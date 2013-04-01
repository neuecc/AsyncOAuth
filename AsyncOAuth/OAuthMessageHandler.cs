using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq;

namespace AsyncOAuth
{
    // idea is based on http://blogs.msdn.com/b/henrikn/archive/2012/02/16/extending-httpclient-with-oauth-to-access-twitter.aspx
    public class OAuthMessageHandler : DelegatingHandler
    {
        string consumerKey;
        string consumerSecret;
        Token token;
        IEnumerable<KeyValuePair<string, string>> parameters;

        public OAuthMessageHandler(string consumerKey, string consumerSecret, Token token = null, IEnumerable<KeyValuePair<string, string>> optionalOAuthHeaderParameters = null)
            : this(new HttpClientHandler(), consumerKey, consumerSecret, token, optionalOAuthHeaderParameters)
        {
        }

        public OAuthMessageHandler(HttpMessageHandler innerHandler, string consumerKey, string consumerSecret, Token token = null, IEnumerable<KeyValuePair<string, string>> optionalOAuthHeaderParameters = null)
            : base(innerHandler)
        {
            this.consumerKey = consumerKey;
            this.consumerSecret = consumerSecret;
            this.token = token;
            this.parameters = optionalOAuthHeaderParameters ?? Enumerable.Empty<KeyValuePair<string, string>>();
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var sendParameter = parameters;
            if (request.Method == HttpMethod.Post)
            {
                // form url encoded content
                if (request.Content is FormUrlEncodedContent)
                {
                    var extraParameter = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var parsed = Utility.ParseQueryString(extraParameter);
                    sendParameter = sendParameter.Concat(parsed.Select(x => new KeyValuePair<string, string>(x.Key.UrlDecode(), x.Value.UrlDecode())));
                }
            }

            var headerParams = OAuthUtility.BuildBasicParameters(
                consumerKey, consumerSecret,
                request.RequestUri.ToString(), request.Method, token,
                sendParameter);
            headerParams = headerParams.Concat(parameters);

            var header = headerParams.Select(p => p.Key + "=" + p.Value.Wrap("\"")).ToString(",");
            request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", header);

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}