Imports System.Collections.ObjectModel
Imports System.IO
Imports System.Windows
Imports Devart.Data.MySql
Imports System.Data
Imports System.ComponentModel
Public Class LemburData
    Public Property No As String
    Public Property Nama As String

    Public Property Employee As String
    Public Property Golongan As String
    Public Property TanggalMasuk As Date
    Public Property JamKerja As String
    Public Property Lembur As String

    Public Property GajiLembur As String

    Public Property TotalGajiLembur As Integer
End Class

Partial Public Class GajiLembur
    Inherits Window
    Public KONEKSI As MySqlConnection
    Public CMD As MySqlCommand
    Public DR As MySqlDataReader
    Public DA As MySqlDataAdapter
    Public DS As DataSet
    Private ReadOnly gajiLemburCollection As New ObservableCollection(Of LemburData)()
    Private dataFilePath As String = "GajiLemburData.txt"
    Private Sub OpenPenggajian_Click(sender As Object, e As RoutedEventArgs)
        Dim dataGajiLemburWindow As New DataGajiLembur()
        dataGajiLemburWindow.Show()
    End Sub
    Protected Overrides Sub OnClosing(e As CancelEventArgs)
        MyBase.OnClosing(e)
        ' Menyimpan data ke dalam file saat aplikasi ditutup
        SaveDataToFile()
    End Sub

    Public Sub New()
        InitializeComponent()
        ' Mengatur DataGrid untuk menggunakan koleksi gaji lembur
        dataGrid.ItemsSource = gajiLemburCollection
        ' Memuat data saat aplikasi dimuat
        LoadDataFromFile()
    End Sub

    Private Sub btnSave_Click(sender As Object, e As RoutedEventArgs)
        ' Menyimpan data ke dalam file
        SaveDataToFile()
        MessageBox.Show("Data berhasil disimpan!")
    End Sub

    Private Sub btnInput_Click(sender As Object, e As RoutedEventArgs)
        ' Simpan data lembur yang diinputkan oleh pengguna ke dalam objek LemburData
        Dim gajiLemburData As New LemburData()
        gajiLemburData.No = txtNo.Text
        gajiLemburData.Employee = txtEmployeeId.Text
        gajiLemburData.Nama = txtNama.Text
        gajiLemburData.Golongan = cmbGolongan.Text
        gajiLemburData.TanggalMasuk = dpTanggalMasuk.SelectedDate.Value
        gajiLemburData.JamKerja = txtJamKerja.Text
        gajiLemburData.Lembur = txtLembur.Text

        ' Hitung total gaji lembur berdasarkan jam lembur dan tarif lembur
        Dim tarifLembur As Integer = 0
        If gajiLemburData.Golongan = "HRD" Then
            gajiLemburData.GajiLembur = "5000"
            tarifLembur = 5000
        ElseIf gajiLemburData.Golongan = "Manager" Then
            gajiLemburData.GajiLembur = "10000"
            tarifLembur = 10000
        ElseIf gajiLemburData.Golongan = "Staff" Then
            gajiLemburData.GajiLembur = "15000"
            tarifLembur = 15000
        End If

        Dim lembur As Integer = If(Not String.IsNullOrEmpty(gajiLemburData.Lembur), Integer.Parse(gajiLemburData.Lembur), 0)
        Dim totalGajiLembur As Integer = tarifLembur * lembur

        ' Simpan total gaji lembur ke dalam objek LemburData
        gajiLemburData.TotalGajiLembur = totalGajiLembur

        ' Tambahkan data ke koleksi
        gajiLemburCollection.Add(gajiLemburData)

        ' Simpan data ke MySQL
        SaveDataToMySQL(gajiLemburData)

        ' Tambahkan logika untuk tombol Input di sini
        MessageBox.Show("Data sedang diinput...")

    End Sub

    Private Sub SaveDataToMySQL(gajiLemburData As LemburData)
        Dim connectionString As String = "Server=localhost;Port=3306;Database=travel;User=root;Password=;"
        Using connection As New MySqlConnection(connectionString)
            connection.Open()

            Dim sql As String = "INSERT INTO tgajilembur (No, Employee, Nama, Golongan, TanggalMasuk, JamKerja, Lembur, GajiLembur, TotalGajiLembur) " &
                            "VALUES (@No, @Employee, @Nama, @Golongan, @TanggalMasuk, @JamKerja, @Lembur, @GajiLembur, @TotalGajiLembur)"

            Using command As New MySqlCommand(sql, connection)
                command.Parameters.AddWithValue("@No", gajiLemburData.No)
                command.Parameters.AddWithValue("@Employee", gajiLemburData.Employee) ' Nilai ID karyawan
                command.Parameters.AddWithValue("@Nama", gajiLemburData.Nama)
                command.Parameters.AddWithValue("@Golongan", gajiLemburData.Golongan)
                command.Parameters.AddWithValue("@TanggalMasuk", gajiLemburData.TanggalMasuk)
                command.Parameters.AddWithValue("@JamKerja", gajiLemburData.JamKerja)
                command.Parameters.AddWithValue("@Lembur", gajiLemburData.Lembur)
                command.Parameters.AddWithValue("@GajiLembur", gajiLemburData.GajiLembur)
                command.Parameters.AddWithValue("@TotalGajiLembur", gajiLemburData.TotalGajiLembur)

                ' Jalankan perintah INSERT
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub



    Private Sub btnClear_Click(sender As Object, e As RoutedEventArgs)
        ' Tambahkan logika untuk tombol Clear di sini
        txtNo.Clear()
        txtNama.Clear()
        cmbGolongan.SelectedIndex = -1
        dpTanggalMasuk.SelectedDate = Nothing
        txtJamKerja.Clear()
        txtLembur.Clear()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As RoutedEventArgs)
        ' Tambahkan logika untuk tombol Close di sini
        Dim nextPage As New NextPage()
        nextPage.Show()
        Close()
    End Sub

    Private Sub LoadDataFromFile()
        ' Memuat data dari file ke dalam koleksi
        If File.Exists(dataFilePath) Then
            Using reader As New StreamReader(dataFilePath)
                Dim line As String
                While Not reader.EndOfStream
                    line = reader.ReadLine()
                    Dim values() As String = line.Split(","c)
                    Dim gajiLemburData As New LemburData()

                    ' Coba parse nilai TanggalMasuk
                    If Date.TryParse(values(4), gajiLemburData.TanggalMasuk) Then
                        ' Parsing berhasil
                        gajiLemburData.No = values(0)
                        gajiLemburData.Employee = values(1) ' Perbarui indeks untuk Employee ID
                        gajiLemburData.Nama = values(2)
                        gajiLemburData.Golongan = values(3)
                        gajiLemburData.JamKerja = values(5)
                        gajiLemburData.Lembur = values(6)
                        gajiLemburData.GajiLembur = values(7)

                        ' Coba parse nilai TotalGajiLembur
                        Dim totalGajiLembur As Integer
                        If Integer.TryParse(values(8), totalGajiLembur) Then
                            ' Parsing berhasil
                            gajiLemburData.TotalGajiLembur = totalGajiLembur
                        Else
                            ' Parsing gagal, mungkin nilai tidak valid
                            ' Lakukan penanganan kesalahan di sini, misalnya mengabaikan baris atau memberikan pesan kesalahan
                        End If

                        ' Tambahkan objek ke koleksi
                        gajiLemburCollection.Add(gajiLemburData)
                    Else
                        ' Parsing tanggal gagal, mungkin format tanggal tidak valid
                        ' Lakukan penanganan kesalahan di sini, misalnya mengabaikan baris atau memberikan pesan kesalahan
                    End If
                End While
            End Using
        End If
    End Sub



    Private Sub SaveDataToFile()
        ' Menyimpan data dari koleksi ke dalam file
        Using writer As New StreamWriter(dataFilePath)
            For Each gajiLemburData As LemburData In gajiLemburCollection
                writer.WriteLine($"{gajiLemburData.No},{gajiLemburData.Employee},{gajiLemburData.Nama},{gajiLemburData.Golongan},{gajiLemburData.TanggalMasuk},{gajiLemburData.JamKerja},{gajiLemburData.Lembur},{gajiLemburData.GajiLembur},{gajiLemburData.TotalGajiLembur}")
            Next
        End Using
    End Sub

End Class
