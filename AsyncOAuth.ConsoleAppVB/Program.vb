Imports System.Security.Cryptography

Class Program
    ' set your token
    Private Const ConsumerKey As String = ""
    Private Const ConsumerSecret As String = ""

    Public Shared Async Function Run() As Task
        ' initialize computehash function
        OAuthUtility.ComputeHash = Function(key, buffer)
                                       Using hmac = New HMACSHA1(key)
                                           Return hmac.ComputeHash(buffer)
                                       End Using
                                   End Function
        ' sample, twitter access flow
        Dim accessToken = Await TwitterClient.AuthorizeSample(ConsumerKey, ConsumerSecret)
        Dim client = New TwitterClient(ConsumerKey, ConsumerSecret, accessToken)
        Dim tl = Await client.GetTimeline(10, 1)
        Console.WriteLine(tl)
    End Function

    Public Shared Sub Main(args As String())
        Run.Wait()
    End Sub
End Class
