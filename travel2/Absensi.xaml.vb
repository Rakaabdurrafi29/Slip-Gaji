Imports System.Collections.ObjectModel
Imports System.IO
Imports System.Windows
Imports System.Xml.Serialization

Public Class AbsensiData
    Public Property No As String
    Public Property Nama As String
    Public Property Golongan As String
    Public Property TanggalMasuk As Date
    Public Property JamMasuk As String
    Public Property JamKeluar As String

    ' Properti untuk menyimpan data karyawan yang sesuai dengan ID yang dimasukkan
    Public Property EmployeeData As Employee
End Class

Partial Public Class Absensi
    Inherits Window

    Private ReadOnly absensiCollection As New ObservableCollection(Of AbsensiData)()
    Private dataFilePath As String = "AbsensiData.txt"
    ' Koleksi data karyawan
    Private ReadOnly employeeCollection As New ObservableCollection(Of Employee)()
    ' Properti untuk menyimpan data karyawan yang sesuai dengan ID yang dimasukkan
    Public Property EmployeeData As ObservableCollection(Of Employee)

    Public Sub New()
        InitializeComponent()
        ' Mengatur DataGrid untuk menggunakan koleksi absensi
        dataGrid.ItemsSource = absensiCollection
        ' Memuat data saat aplikasi dimuat
        LoadDataFromFile()
        ' Memuat data karyawan
        LoadEmployeeData()

        ' Menambahkan event handler untuk memproses tekanan tombol "Enter" pada txtEmployeeId
        AddHandler txtEmployeeId.PreviewKeyDown, AddressOf txtEmployeeId_PreviewKeyDown
    End Sub

    ' Method untuk memuat data karyawan
    Private Sub LoadEmployeeData()
        If File.Exists("employees.xml") Then
            Using fileStream As FileStream = File.OpenRead("employees.xml")
                Dim serializer As New XmlSerializer(GetType(List(Of Employee)))
                Dim loadedEmployees As List(Of Employee) = DirectCast(serializer.Deserialize(fileStream), List(Of Employee))
                For Each employee In loadedEmployees
                    employeeCollection.Add(employee)
                Next
            End Using
        End If
    End Sub

    Private Sub btnSave_Click(sender As Object, e As RoutedEventArgs)
        ' Menyimpan data ke dalam file
        SaveDataToFile()
        MessageBox.Show("Data berhasil disimpan!")
    End Sub

    Private Sub btnInput_Click(sender As Object, e As RoutedEventArgs)
        ' Simpan data absensi yang diinputkan oleh pengguna ke dalam objek AbsensiData
        Dim absensiData As New AbsensiData()
        absensiData.No = txtNo.Text
        absensiData.Nama = txtName.Text
        absensiData.Golongan = cmbGolongan.Text

        ' Memeriksa apakah SelectedDate tidak null sebelum mengonversi ke Date
        If dpTanggalMasuk.SelectedDate IsNot Nothing Then
            absensiData.TanggalMasuk = dpTanggalMasuk.SelectedDate.Value
        Else
            ' Jika SelectedDate null, mungkin Anda ingin memberikan nilai default atau menampilkan pesan kesalahan
            MessageBox.Show("Tanggal masuk tidak boleh kosong!")
            Return
        End If

        absensiData.JamMasuk = txtJamMasuk.Text
        absensiData.JamKeluar = txtJamKeluar.Text

        ' Cari data karyawan berdasarkan EmployeeId yang dimasukkan
        Dim employee As Employee = employeeCollection.FirstOrDefault(Function(emp) emp.EmployeeId = txtEmployeeId.Text)
        If employee IsNot Nothing Then
            absensiData.EmployeeData = employee
        Else
            MessageBox.Show("Employee ID not found!")
            Return
        End If

        ' Tambahkan data ke koleksi
        absensiCollection.Add(absensiData)

        ' Simpan data ke file setelah data ditambahkan
        SaveDataToFile()

        ' Tambahkan logika untuk tombol Input di sini
        MessageBox.Show("Data sedang diinput...")
    End Sub


    ' Event handler untuk memproses tekanan tombol "Enter" pada txtEmployeeId
    Private Sub txtEmployeeId_PreviewKeyDown(sender As Object, e As KeyEventArgs)
        If e.Key = Key.Enter Then
            Dim employeeId As String = txtEmployeeId.Text
            Dim employee As Employee = employeeCollection.FirstOrDefault(Function(emp) emp.EmployeeId = employeeId)
            If employee IsNot Nothing Then
                ' Mengisi data karyawan ke kotak teks lainnya
                txtName.Text = employee.Name
                cmbGolongan.Text = employee.Golongan
            Else
                MessageBox.Show("Employee ID not found!")
            End If
        End If
    End Sub

    Private Sub btnClear_Click(sender As Object, e As RoutedEventArgs)
        ' Tambahkan logika untuk tombol Clear di sini
        txtNo.Clear()
        txtName.Clear()
        cmbGolongan.SelectedIndex = -1
        dpTanggalMasuk.SelectedDate = Nothing
        txtJamMasuk.Clear()
        txtJamKeluar.Clear()
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
                    Dim absensiData As New AbsensiData() With {
                        .No = values(0),
                        .Nama = values(1),
                        .Golongan = values(2),
                        .TanggalMasuk = Date.Parse(values(3)),
                        .JamMasuk = values(4),
                        .JamKeluar = values(5)
                    }
                    absensiCollection.Add(absensiData)
                End While
            End Using
        End If
    End Sub

    Private Sub SaveDataToFile()
        ' Menyimpan data dari koleksi ke dalam file
        Using writer As New StreamWriter(dataFilePath)
            For Each absensiData As AbsensiData In absensiCollection
                writer.WriteLine($"{absensiData.No},{absensiData.Nama},{absensiData.Golongan},{absensiData.TanggalMasuk},{absensiData.JamMasuk},{absensiData.JamKeluar}")
            Next
        End Using
    End Sub
End Class
