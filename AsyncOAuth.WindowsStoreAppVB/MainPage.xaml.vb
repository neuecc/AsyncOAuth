Imports Windows.UI.Popups

Public NotInheritable Class MainPage
    Inherits Page

    Private Const ConsumerKey As String = ""
    Private Const ConsumerSecret As String = ""
    Private Const AccessTokenKey As String = ""
    Private Const AccessTokenSecret As String = ""

    Private ReadOnly AccessToken As New AccessToken(AccessTokenKey, AccessTokenSecret)

    Private Async Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        Dim client = New TwitterClient(ConsumerKey, ConsumerSecret, AccessToken)

        Dim tl = Await client.GetTimeline(10, 1)
        Await New MessageDialog(tl).ShowAsync()
    End Sub
End Class
