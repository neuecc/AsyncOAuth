AsyncOAuth
==========

Portable Client Library and HttpClient based OAuth library, including all platform(for PCL as .NET 4.0, .NET 4.5, Silverlight4, Silverlight5, Windows Phone 7.5, Windows Phone 8.0, Windows Store Apps).

Install
---
using with NuGet, [AsyncOAuth](https://nuget.org/packages/AsyncOAuth/)
```
PM> Install-Package AsyncOAuth
```

Usage
---
at first, you must initialize hash function(ApplicationStart etc...)

```csharp
// Silverlight, Windows Phone, Console, Web, etc...
OAuthUtility.ComputeHash = (key, buffer) => { using (var hmac = new HMACSHA1(key)) { return hmac.ComputeHash(buffer); } };
 
// Windows Store Apps
AsyncOAuth.OAuthUtility.ComputeHash = (key, buffer) =>
{
    var crypt = Windows.Security.Cryptography.Core.MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
    var keyBuffer = Windows.Security.Cryptography.CryptographicBuffer.CreateFromByteArray(key);
    var cryptKey = crypt.CreateKey(keyBuffer);
 
    var dataBuffer = Windows.Security.Cryptography.CryptographicBuffer.CreateFromByteArray(buffer);
    var signBuffer = Windows.Security.Cryptography.Core.CryptographicEngine.Sign(cryptKey, dataBuffer);
 
    byte[] value;
    Windows.Security.Cryptography.CryptographicBuffer.CopyToByteArray(signBuffer, out value);
    return value;
};
```

Create HttpClient with OAuthMessageHandler

```csharp
var client = new HttpClient(new OAuthMessageHandler("consumerKey", "consumerSecret", new AccessToken("accessToken", "accessTokenSecret")));
 
// shorthand(result is same above)
var client = OAuthUtility.CreateOAuthClient("consumerKey", "consumerSecret", new AccessToken("accessToken", "accessTokenSecret"));
```

operation same as HttpClient
```csharp
// Get
var json = await client.GetStringAsync("http://api.twitter.com/1.1/statuses/home_timeline.json?count=" + count + "&page=" + page);
 
// Post
var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("status", status) });
var response = await client.PostAsync("http://api.twitter.com/1.1/statuses/update.json", content);
var json = await response.Content.ReadAsStringAsync();
 
// Multi Post
var content = new MultipartFormDataContent();
content.Add(new StringContent(status), "\"status\"");
content.Add(new ByteArrayContent(media), "media[]", "\"" + fileName + "\"");
 
var response = await client.PostAsync("https://upload.twitter.com/1/statuses/update_with_media.json", content);
var json = await response.Content.ReadAsStringAsync();
```

Sample
---
more sample, please see AsyncOAuth.ConsoleApp(Twitter.cs, Hatena.cs), AsyncOAuth.WindowsStoreApp  
sample contains authorize flow

```csharp
// sample flow for Twitter authroize
public async static Task<AccessToken> AuthorizeSample(string consumerKey, string consumerSecret)
{
    // create authorizer
    var authorizer = new OAuthAuthorizer(consumerKey, consumerSecret);

    // get request token
    var tokenResponse = await authorizer.GetRequestToken("https://api.twitter.com/oauth/request_token");
    var requestToken = tokenResponse.Token;

    var pinRequestUrl = authorizer.BuildAuthorizeUrl("https://api.twitter.com/oauth/authorize", requestToken);

    // open browser and get PIN Code
    Process.Start(pinRequestUrl);

    // enter pin
    Console.WriteLine("ENTER PIN");
    var pinCode = Console.ReadLine();

    // get access token
    var accessTokenResponse = await authorizer.GetAccessToken("https://api.twitter.com/oauth/access_token", requestToken, pinCode);

    // save access token.
    var accessToken = accessTokenResponse.Token;
    Console.WriteLine("Key:" + accessToken.Key);
    Console.WriteLine("Secret:" + accessToken.Secret);

    return accessToken;
}
```

History
---
ver 0.8.4 - 2014-03-14
* fixed, generate wrong signature when POST and data contains "%21", "%2A", "%27", "%28" and "%29". thanks @karno #11
* update Microsoft.Bcl.Async to 1.0.165
* update Microsoft.Net.Http to 2.2.18

ver 0.8.3 - 2013-10-17
* fixed, generate wrong signature when data contains space(This enbug is from v.0.8.1)

ver 0.8.2 - 2013-09-30
* fixed, allow OAuth TokenResponse returns duplicate key  

ver 0.8.1 - 2013-09-30
* fixed, generate wrong OAuth signature when parameter have same key and value needs UrlEncode  
* modified, escape character in URLs. thanks @gjulianm #6  
* update HttpClient version - to 2.2.15.

ver 0.7.0 - 2013-06-24
* fixed, generate wrong OAuth signature when parameter-name mixed lower and upper.  
* fixed, generate wrong OAuth signature when get and querystring needs UrlEncode.

ver.0.6.4 - 2013-05-27
* update external library version.  
* improved:Token is serializable.

License
---
under [MIT License](http://opensource.org/licenses/MIT)
