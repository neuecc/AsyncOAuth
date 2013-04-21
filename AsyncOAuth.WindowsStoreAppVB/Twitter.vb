Imports System.IO
Imports System.Net.Http

'  a sample of twitter client
Public Class TwitterClient
    Private ReadOnly ConsumerKey As String
    Private ReadOnly ConsumerSecret As String
    Private ReadOnly AccessToken As AccessToken

    Public Sub New(consumerKey As String, consumerSecret As String, accesstoken As AccessToken)
        Me.ConsumerKey = consumerKey
        Me.ConsumerSecret = consumerSecret
        Me.AccessToken = accesstoken
    End Sub

    ' see console app sample

    ''sample flow for Twitter authroize
    'Public Shared Async Function AuthorizeSample(consumerkey As String, consumerSecret As String) As Task(Of AccessToken)
    '    ' create authorizer
    '    Dim authorizer As New OAuthAuthorizer(consumerkey, consumerSecret)

    '    ' get request token
    '    Dim tokenResponse = Await authorizer.GetRequestToken("https://api.twitter.com/oauth/request_token")
    '    Dim requestToken = tokenResponse.Token()

    '    Dim pinRequestUrl = authorizer.BuildAuthorizeUrl("https://api.twitter.com/oauth/authorize", requestToken)

    '    ' open browser and get PIN Code
    '    Process.Start(pinRequestUrl)

    '    ' enter pin
    '    Console.WriteLine("ENTER PIN")
    '    Dim pinCode = Console.ReadLine()

    '    ' get access token
    '    Dim accessTokenResponse = Await authorizer.GetAccessToken("https://api.twitter.com/oauth/access_token", requestToken, pinCode)

    '    ' save access token.
    '    Dim accessToken = accessTokenResponse.Token
    '    Console.WriteLine("Key:" & accessToken.Key)
    '    Console.WriteLine("Secret:" & accessToken.Secret)

    '    Return accessToken
    'End Function

    Public Async Function GetTimeline(count As Integer, page As Integer) As Task(Of String)
        Dim client = OAuthUtility.CreateOAuthClient(Me.ConsumerKey, Me.ConsumerSecret, Me.AccessToken)

        Dim json = Await client.GetStringAsync("http://api.twitter.com/1.1/statuses/home_timeline.json?count=" & count & "&page=" & page)
        Return json
    End Function

    Public Async Function PostUpdate(status As String) As Task(Of String)
        Dim client = OAuthUtility.CreateOAuthClient(Me.ConsumerKey, Me.ConsumerSecret, Me.AccessToken)

        Dim content = New FormUrlEncodedContent(New KeyValuePair(Of String, String)() {New KeyValuePair(Of String, String)("status", status)})

        Dim response = Await client.PostAsync("http://api.twitter.com/1.1/statuses/update.json", content)
        Dim json = Await response.Content.ReadAsStringAsync()
        Return json
    End Function

    Public Async Function GetStream(fetchAction As Action(Of String)) As Task
        Dim client = OAuthUtility.CreateOAuthClient(Me.ConsumerKey, Me.ConsumerSecret, Me.AccessToken)

        client.Timeout = System.Threading.Timeout.InfiniteTimeSpan  'set infinite timespan
        Using stream = Await client.GetStreamAsync("https://userstream.twitter.com/1.1/user.json")
            Using sr = New StreamReader(stream)
                Do While (Not sr.EndOfStream)
                    Dim s = Await sr.ReadLineAsync()
                    fetchAction(s)
                Loop
            End Using
        End Using
    End Function

    Public Async Function UpdateWithMedia(status As String, media As Byte(), fileName As String) As Task(Of String)
        Dim client = OAuthUtility.CreateOAuthClient(Me.ConsumerKey, Me.ConsumerSecret, Me.AccessToken)

        Dim content = New MultipartFormDataContent()
        content.Add(New StringContent(status), """status""")
        content.Add(New ByteArrayContent(media), "media()", """" & fileName & """")

        Dim response = Await client.PostAsync("https://upload.twitter.com/1/statuses/update_with_media.json", content)
        Dim json = Await response.Content.ReadAsStringAsync()
        Return json
    End Function

End Class
