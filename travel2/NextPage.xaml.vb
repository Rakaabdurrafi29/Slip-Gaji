Imports System.Windows

Partial Public Class NextPage
    Inherits Window

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub btnAbsensi_Click(sender As Object, e As RoutedEventArgs)
        ' Tambahkan logika untuk tombol Absensi di sini
        MessageBox.Show("Anda menekan tombol Absensi")
        ' Navigasi ke halaman baru setelah login berhasil
        Dim absensiPage As New Absensi()
        absensiPage.Show()
        Close() ' Menutup halaman login setelah login berhasil
    End Sub

    Private Sub btnPenggajian_Click(sender As Object, e As RoutedEventArgs)
        ' Tambahkan logika untuk tombol Penggajian Karyawan di sini
        MessageBox.Show("Anda menekan tombol Penggajian Karyawan")
        ' Navigasi ke halaman baru setelah login berhasil
        Dim penggajiankaryawanPage As New PenggajianKaryawan()
        penggajiankaryawanPage.Show()
        Close() ' Menutup halaman login setelah login berhasil
    End Sub

    Private Sub btnGajiLembur_Click(sender As Object, e As RoutedEventArgs)
        ' Tambahkan logika untuk tombol Gaji Lembur per Hari di sini
        MessageBox.Show("Anda menekan tombol Gaji Lembur per Hari")
        ' Navigasi ke halaman baru setelah login berhasil
        Dim gajilemburPage As New GajiLembur()
        gajilemburPage.Show()
        Close() ' Menutup halaman login setelah login berhasil
    End Sub

    Private Sub btnBiodata_Click(sender As Object, e As RoutedEventArgs)
        ' Tambahkan logika untuk tombol Biodata Karyawan di sini
        MessageBox.Show("Anda menekan tombol Biodata Karyawan")
        ' Navigasi ke halaman baru setelah login berhasil
        Dim biodatakaryawanPage As New BiodataKaryawan()
        biodatakaryawanPage.Show()
        Close() ' Menutup halaman login setelah login berhasil
    End Sub

    Private Sub btnExit_Click(sender As Object, e As RoutedEventArgs)
        ' Tambahkan logika untuk tombol Exit di sini
        Close()
    End Sub
End Class

