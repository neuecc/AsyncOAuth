NotInheritable Class App
    Inherits Application

    Public Sub New()
        InitializeComponent()

        ' must initialize hash function
        AsyncOAuth.OAuthUtility.ComputeHash = Function(key, buffer)
                                                  Dim crypt = Windows.Security.Cryptography.Core.MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1")
                                                  Dim keyBuffer = Windows.Security.Cryptography.CryptographicBuffer.CreateFromByteArray(key)
                                                  Dim cryptKey = crypt.CreateKey(keyBuffer)

                                                  Dim dataBuffer = Windows.Security.Cryptography.CryptographicBuffer.CreateFromByteArray(buffer)
                                                  Dim signBuffer = Windows.Security.Cryptography.Core.CryptographicEngine.Sign(cryptKey, dataBuffer)

                                                  Dim value As Byte()
                                                  Windows.Security.Cryptography.CryptographicBuffer.CopyToByteArray(signBuffer, value)
                                                  Return value
                                              End Function
    End Sub

    Protected Overrides Sub OnLaunched(args As Windows.ApplicationModel.Activation.LaunchActivatedEventArgs)
        Dim rootFrame As Frame = Window.Current.Content

        If rootFrame Is Nothing Then
            rootFrame = New Frame()
            If args.PreviousExecutionState = ApplicationExecutionState.Terminated Then
            End If
            Window.Current.Content = rootFrame
        End If
        If rootFrame.Content Is Nothing Then
            If Not rootFrame.Navigate(GetType(MainPage), args.Arguments) Then
                Throw New Exception("Failed to create initial page")
            End If
        End If

        Window.Current.Activate()
    End Sub

    Private Sub OnSuspending(sender As Object, e As SuspendingEventArgs) Handles Me.Suspending
        Dim deferral As SuspendingDeferral = e.SuspendingOperation.GetDeferral()
        deferral.Complete()
    End Sub

End Class
