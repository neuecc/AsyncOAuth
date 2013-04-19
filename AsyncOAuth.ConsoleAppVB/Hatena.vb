Imports System.Net.Http
Imports System.Text

Public Class HatenaClient
    Private ReadOnly ConsumerKey As String
    Private ReadOnly ConsumerSecret As String
    Private ReadOnly AccessToken As AccessToken

    Public Sub New(consumerKey As String, consumerSecret As String, accesstoken As AccessToken)
        Me.ConsumerKey = consumerKey
        Me.ConsumerSecret = consumerSecret
        Me.AccessToken = accesstoken
    End Sub

    'sample flow for Hatena authroize
    Public Shared Async Function AuthorizeSample(consumerkey As String, consumerSecret As String) As Task(Of AccessToken)
        ' create authorizer
        Dim authorizer As New OAuthAuthorizer(consumerkey, consumerSecret)

        ' get request token
        Dim tokenResponse = Await authorizer.GetRequestToken(
                                        "https://www.hatena.com/oauth/initiate",
                                        New KeyValuePair(Of String, String)() {New KeyValuePair(Of String, String)("oauth_callback", "oob")},
                                        New FormUrlEncodedContent(New KeyValuePair(Of String, String)() {New KeyValuePair(Of String, String)("scope", "read_public,write_public,read_private,write_private")}))

        Dim requestToken = tokenResponse.Token()
        Dim pinRequestUrl = authorizer.BuildAuthorizeUrl("https://www.hatena.ne.jp/oauth/authorize", requestToken)

        ' open browser and get PIN Code
        Process.Start(pinRequestUrl)

        ' enter pin
        Console.WriteLine("ENTER PIN")
        Dim pinCode = Console.ReadLine()

        ' get access token
        Dim accessTokenResponse = Await authorizer.GetAccessToken("https://www.hatena.com/oauth/token", requestToken, pinCode)

        ' save access token.
        Dim accessToken = accessTokenResponse.Token
        Console.WriteLine("Key:" & accessToken.Key)
        Console.WriteLine("Secret:" & accessToken.Secret)

        Return accessToken
    End Function

    Private Function CreateOAuthClient() As HttpClient
        Return OAuthUtility.CreateOAuthClient(ConsumerKey, ConsumerSecret, AccessToken)
    End Function

    Public Async Function GetMy() As Task(Of String)
        Dim client = CreateOAuthClient()
        Dim json = Await client.GetStringAsync("http://n.hatena.com/applications/my.json")
        Return json
    End Function

    Public Async Function ApplicationStart() As Task(Of String)
        Dim client = CreateOAuthClient()
        Dim response = Await client.PostAsync("http://n.hatena.com/applications/start", New StringContent("", Encoding.UTF8))
        Return Await response.Content.ReadAsStringAsync()
    End Function
End Class
