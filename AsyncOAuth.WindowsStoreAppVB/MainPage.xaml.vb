Imports Windows.UI.Popups

Public NotInheritable Class MainPage
    Inherits Page

    Private Const ConsumerKey As String = "Ya69yklLcgAuxbSSft3Dw"
    Private Const ConsumerSecret As String = "M2EclIKFR3TUwceE59S0ctIXVsrLu8bndSl430A7M"
    Private Const AccessTokenKey As String = "14565539-IPM7XQisVYKK1xQ5RdOK3j8KRg3eXaISvAE8crceb"
    Private Const AccessTokenSecret As String = "FOK2eluHpsvnh25z7AnBhWW8zik0Baf8JC9dtfeU8"

    Private ReadOnly AccessToken As New AccessToken(AccessTokenKey, AccessTokenSecret)

    Private Async Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        Dim client = New TwitterClient(ConsumerKey, ConsumerSecret, AccessToken)

        Dim tl = Await client.GetTimeline(10, 1)
        Await New MessageDialog(tl).ShowAsync()
    End Sub
End Class
