Imports System.Windows

Partial Public Class MainWindow
    Inherits Window

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub btnLogin_Click(sender As Object, e As RoutedEventArgs)
        Dim username As String = txtUsername.Text
        Dim password As String = txtPassword.Password

        If username = "julio" AndAlso password = "admin" Then
            MessageBox.Show("Login Berhasil!")

            ' Navigasi ke halaman baru setelah login berhasil
            Dim nextPage As New NextPage()
            nextPage.Show()
            Close() ' Menutup halaman login setelah login berhasil
        Else
            MessageBox.Show("Username atau Password salah!")
        End If
    End Sub

    Private Sub btnExit_Click(sender As Object, e As RoutedEventArgs)
        Application.Current.Shutdown()
    End Sub

End Class
