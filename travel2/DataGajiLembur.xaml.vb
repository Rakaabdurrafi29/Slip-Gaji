Imports Devart.Data.MySql
Imports MySql.Data.MySqlClient
Imports System.Data

Public Class DataGajiLembur
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        ' Opsional: Panggil LoadData di sini jika Anda ingin memuat data secara otomatis saat jendela dimuat.
        ' LoadData()
    End Sub

    Private Sub LoadData()
        Dim connectionString As String = "Server=localhost;Port=3306;Database=travel;User=root;Password=;"
        Dim query As String = "SELECT `No`, `Nama`, `Golongan`, `TanggalMasuk`, `JamKerja`, `Lembur`, `GajiLembur`, `TotalGajiLembur`, `Employee` FROM `tgajilembur`"
        Dim connection As New MySqlConnection(connectionString)
        Dim command As New MySqlCommand(query, connection)
        Dim adapter As New MySqlDataAdapter(command)
        Dim table As New DataTable()

        Try
            connection.Open()
            adapter.Fill(table)

            If table.Rows.Count > 0 Then
                DataGrid1.ItemsSource = table.DefaultView
                MessageBox.Show("Data loaded successfully.")
            Else
                MessageBox.Show("No data found in the table.")
            End If
        Catch ex As MySqlException
            MessageBox.Show("Error: " & ex.Message)
        Finally
            connection.Close()
        End Try
    End Sub

    Private Sub OpenButton_Click(sender As Object, e As RoutedEventArgs)
        LoadData()
    End Sub
End Class
