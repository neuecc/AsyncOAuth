AsyncOAuth
==========

Portable Client Library and HttpClient based OAuth library, including all platform(for PCL as .NET 4.0, .NET 4.5, Silverlight4, Silverlight5, Windows Phone 7.5, Windows Phone 8.0, Windows Store Apps).

Install
---
using with NuGet
PM> Install-Package AsyncOAuth -Pre

Usage
---
at first, yout must initialize hash function(ApplicationStart etc...)

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

License
---
under MIT License