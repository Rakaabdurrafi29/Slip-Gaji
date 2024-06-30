Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports System.Windows
Imports System.Windows.Input
Imports System.IO
Imports System.Xml.Serialization
Imports Devart.Data.MySql
Imports System.Data

Public Class PenggajianKaryawan

    Inherits Window

    Public KONEKSI As MySqlConnection
    Public CMD As MySqlCommand
    Public DR As MySqlDataReader
    Public DA As MySqlDataAdapter
    Public DS As DataSet
    ' Definisikan koleksi untuk menyimpan data karyawan
    Private ReadOnly karyawanCollection As New ObservableCollection(Of Karyawan)()
    Private dataFilePath As String = "KaryawanData.txt"
    ' Koleksi data karyawan
    Private ReadOnly employeeCollection As New ObservableCollection(Of Employee)()

    ' Event handler untuk tombol "Open Data Penggajian"
    Private Sub OpenDataPenggajian_Click(sender As Object, e As RoutedEventArgs)
        Dim dataPenggajianWindow As New DataPenggajian()
        dataPenggajianWindow.Show()
    End Sub

    Public Sub New()
        InitializeComponent()
        ' Setel ItemsSource DataGrid ke koleksi listKaryawan
        dataGrid.ItemsSource = karyawanCollection
        ' Memuat data saat aplikasi dimuat
        LoadDataFromFile()
        ' Memuat data karyawan
        LoadEmployeeData()

        ' Menambahkan event handler untuk memproses tekanan tombol "Enter" pada txtEmployeeId
        AddHandler txtEmployeeId.PreviewKeyDown, AddressOf txtEmplooyeId_PreviewKeyDown
    End Sub

    ' Event handler untuk memproses tekanan tombol "Enter" pada txtEmployeeId
    Private Sub txtEmplooyeId_PreviewKeyDown(sender As Object, e As KeyEventArgs)
        If e.Key = Key.Enter Then
            Dim employeeID As String = txtEmployeeId.Text
            Dim employee As Employee = employeeCollection.FirstOrDefault(Function(emp) emp.EmployeeId = employeeID)
            If employee IsNot Nothing Then
                ' Mengisi data karyawan ke kotak teks lainnya
                txtName.Text = employee.Name
                txtEmployeeId.Text = employee.EmployeeId
                txtGajiBersih.Text = employee.GajiBersih
                txtGajiLembur.Text = employee.GajiLembur
                txtGajiPokok.Text = employee.GajiPokok
                txtPotonganAlpa.Text = employee.PotonganAlpa
                txtTunjanganJabatan.Text = employee.TunjanganJabatan
                txtTunjanganMakan.Text = employee.TunjanganMakan
                txtTunjanganTransportasi.Text = employee.TunjanganTransportasi
                cmbGolongan.Text = employee.Golongan
            Else
                MessageBox.Show("Employee name not found!")
            End If
        End If
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

    Private Sub btnInput_Click(sender As Object, e As RoutedEventArgs)
        ' Memeriksa status kehadiran terlebih dahulu
        Dim statusKehadiran As String = ""
        If chkAlpa.IsChecked = True Then
            statusKehadiran = "Alpa"
        ElseIf chkHadir.IsChecked = True Then
            statusKehadiran = "Hadir"
        ElseIf chkIzin.IsChecked = True Then
            statusKehadiran = "Izin"
        End If

        ' Simpan data absensi yang diinputkan oleh pengguna ke dalam objek Karyawan
        Dim karyawanData As New Karyawan()
        karyawanData.No = txtNo.Text
        karyawanData.Name = txtName.Text
        karyawanData.EmployeeId = txtEmployeeId.Text
        karyawanData.JumlahHariKerja = txtJumlahHariKerja.Text
        karyawanData.JumlahJamLembur = txtJumlahJamLembur.Text
        karyawanData.Golongan = cmbGolongan.Text
        karyawanData.GajiBersih = txtGajiBersih.Text
        karyawanData.GajiLembur = txtGajiLembur.Text
        karyawanData.GajiPokok = txtGajiPokok.Text
        karyawanData.TunjanganJabatan = txtTunjanganJabatan.Text
        karyawanData.TunjanganMakan = txtTunjanganMakan.Text
        karyawanData.TunjanganTransportasi = txtTunjanganTransportasi.Text

        ' Set nilai properti Alpa, Izin, atau Hadir berdasarkan checkbox yang dipilih
        karyawanData.Alpa = (statusKehadiran = "Alpa")
        karyawanData.Izin = (statusKehadiran = "Izin")
        karyawanData.Hadir = (statusKehadiran = "Hadir")

        ' Ambil nilai PotonganAlpa dari employee yang cocok
        Dim employee As Employee = employeeCollection.FirstOrDefault(Function(emp) emp.EmployeeId = karyawanData.EmployeeId)
        If employee IsNot Nothing Then
            karyawanData.PotonganAlpa = employee.PotonganAlpa

            ' Mengurangi gaji bersih dengan potongan alpa
            karyawanData.GajiBersih -= karyawanData.PotonganAlpa

        Else
            MessageBox.Show("Employee ID tidak ditemukan atau PotonganAlpa tidak diatur.")
            Return
        End If



        ' Tambahkan data ke koleksi
        karyawanCollection.Add(karyawanData)


        SaveDataToMySQL(karyawanData)

        ' Update tampilan tabel
        dataGrid.Items.Refresh()

        ' Update txtGajiBersih.Text
        txtGajiBersih.Text = karyawanData.GajiBersih

        SaveDataToFile()

        ' Tambahkan logika untuk tombol Input di sini
        MessageBox.Show("Data sedang diinput...")
    End Sub









    Private Sub LoadDataFromFile()
        ' Memuat data dari file ke dalam koleksi
        If File.Exists(dataFilePath) Then
            Using reader As New StreamReader(dataFilePath)
                Dim line As String
                While Not reader.EndOfStream
                    line = reader.ReadLine()
                    Dim values() As String = line.Split(","c)
                    Dim karyawanData As New Karyawan() With {
                    .No = values(0),
                    .EmployeeId = values(1),
                    .Name = values(2),
                    .Golongan = values(3),
                    .JumlahHariKerja = values(4),
                    .JumlahJamLembur = values(5),
                    .GajiLembur = values(6),
                    .GajiPokok = values(7),
                    .TunjanganJabatan = values(8),
                    .TunjanganMakan = values(9),
                    .TunjanganTransportasi = values(10),
                    .PotonganAlpa = values(11),
                    .GajiBersih = values(12),
                    .Alpa = Boolean.Parse(values(13)),
                    .Hadir = Boolean.Parse(values(14)),
                    .Izin = Boolean.Parse(values(15))
                }
                    karyawanCollection.Add(karyawanData)
                End While
            End Using
        End If
    End Sub


    Private Sub SaveDataToFile()
        ' Menyimpan data dari koleksi ke dalam file
        Using writer As New StreamWriter(dataFilePath)
            For Each karyawanData As Karyawan In karyawanCollection
                writer.WriteLine($"{karyawanData.No},{karyawanData.EmployeeId},{karyawanData.Name},{karyawanData.Golongan},{karyawanData.JumlahHariKerja},{karyawanData.JumlahJamLembur},{karyawanData.GajiLembur},{karyawanData.GajiPokok},{karyawanData.TunjanganJabatan},{karyawanData.TunjanganMakan},{karyawanData.TunjanganTransportasi},{karyawanData.PotonganAlpa},{karyawanData.GajiBersih},{karyawanData.Hadir},{karyawanData.Alpa},{karyawanData.Izin}")
            Next
        End Using
    End Sub

    ' Method yang dipanggil ketika aplikasi ditutup
    Private Sub PenggajianKaryawan_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        ' Menyimpan data ke dalam file saat aplikasi ditutup
        SaveDataToFile()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As RoutedEventArgs)
        SaveDataToFile()
        ' Tambahkan logika untuk tombol Close di sini
        Dim nextPage As New NextPage()
        nextPage.Show()
        Close()
    End Sub

    Private Sub dataGrid_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles dataGrid.SelectionChanged
        ' Pastikan ada item yang dipilih
        If dataGrid.SelectedItem IsNot Nothing Then
            ' Ambil item yang dipilih
            Dim selectedKaryawan As Karyawan = CType(dataGrid.SelectedItem, Karyawan)

            ' Tampilkan detail item yang dipilih
            MessageBox.Show($"Detail Karyawan: {vbCrLf} No: {selectedKaryawan.No}{vbCrLf} Nama: {selectedKaryawan.Name}")
        End If
    End Sub

    Private Sub SaveDataToMySQL(karyawanData As Karyawan)
        Dim connectionString As String = "Server=localhost;Port=3306;Database=travel;User=root;Password=;"
        Using connection As New MySqlConnection(connectionString)
            connection.Open()

            ' Memeriksa apakah kolom Golongan memiliki nilai
            If String.IsNullOrEmpty(karyawanData.Golongan) Then
                MessageBox.Show("Golongan harus diisi.")
                Return
            End If

            ' Gunakan nilai PotonganAlpa dari karyawanData
            Dim potonganAlpa As String = karyawanData.PotonganAlpa

            Dim employeeId As String = karyawanData.EmployeeId
            Dim sqlPotonganAlpa As String = "SELECT PotonganAlpa FROM tboidata WHERE EmployeeId = @EmployeeId"
            Using commandPotonganAlpa As New MySqlCommand(sqlPotonganAlpa, connection)
                commandPotonganAlpa.Parameters.AddWithValue("@EmployeeId", employeeId)
                Using reader As MySqlDataReader = commandPotonganAlpa.ExecuteReader()
                    If reader.Read() Then
                        potonganAlpa = Convert.ToInt32(reader("PotonganAlpa"))
                    End If
                End Using
            End Using

            ' Mengurangi gaji bersih dengan potongan alpa
            Dim gajiBersih As Integer = If(String.IsNullOrEmpty(karyawanData.GajiBersih), 0, Integer.Parse(karyawanData.GajiBersih))
            gajiBersih -= potonganAlpa

            ' Memastikan JumlahHariKerja tidak kosong
            If String.IsNullOrEmpty(karyawanData.JumlahHariKerja) Then
                karyawanData.JumlahHariKerja = "0" ' Atur nilai default jika diperlukan
            End If

            ' SQL untuk menyimpan data karyawan ke database
            Dim sql As String = "INSERT INTO tpenggajian (No, Name, Golongan, JumlahHariKerja, JumlahJamLembur, GajiLembur, GajiPokok, TunjanganJabatan, TunjanganMakan, TunjanganTransportasi, PotonganAlpa, GajiBersih, Alpa, Hadir, Izin) " &
                                "VALUES (@No, @Name, @Golongan, @JumlahHariKerja, @JumlahJamLembur, @GajiLembur, @GajiPokok, @TunjanganJabatan, @TunjanganMakan, @TunjanganTransportasi, @PotonganAlpa, @GajiBersih, @Alpa, @Hadir, @Izin)"

            ' Menyiapkan dan mengeksekusi perintah SQL
            Using command As New MySqlCommand(sql, connection)
                command.Parameters.AddWithValue("@No", karyawanData.No)
                command.Parameters.AddWithValue("@Name", karyawanData.Name)
                command.Parameters.AddWithValue("@Golongan", karyawanData.Golongan)
                command.Parameters.AddWithValue("@JumlahHariKerja", karyawanData.JumlahHariKerja)
                command.Parameters.AddWithValue("@JumlahJamLembur", karyawanData.JumlahJamLembur)
                command.Parameters.AddWithValue("@GajiLembur", karyawanData.GajiLembur)
                command.Parameters.AddWithValue("@GajiPokok", karyawanData.GajiPokok)
                command.Parameters.AddWithValue("@Alpa", karyawanData.Alpa)
                command.Parameters.AddWithValue("@Izin", karyawanData.Izin)
                command.Parameters.AddWithValue("@Hadir", karyawanData.Hadir)
                command.Parameters.AddWithValue("@TunjanganJabatan", karyawanData.TunjanganJabatan)
                command.Parameters.AddWithValue("@TunjanganMakan", karyawanData.TunjanganMakan)
                command.Parameters.AddWithValue("@TunjanganTransportasi", karyawanData.TunjanganTransportasi)
                command.Parameters.AddWithValue("@PotonganAlpa", potonganAlpa) ' Simpan potongan alpa dari database
                command.Parameters.AddWithValue("@GajiBersih", karyawanData.GajiBersih) ' Gaji bersih tanpa potongan
                ' Tambahkan parameter lainnya sesuai kebutuhan

                command.ExecuteNonQuery()
            End Using

        End Using
    End Sub






End Class

' Class untuk merepresentasikan data karyawan
Public Class Karyawan
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub OnPropertyChanged(<CallerMemberName> Optional ByVal propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Private _no As String
    Public Property No As String
        Get
            Return _no
        End Get
        Set(ByVal value As String)
            _no = value
            OnPropertyChanged()
        End Set
    End Property

    Private _employeeId As String
    Public Property EmployeeId As String
        Get
            Return _employeeId
        End Get
        Set(ByVal value As String)
            _employeeId = value
            OnPropertyChanged()
        End Set
    End Property

    Private _name As String
    Public Property Name As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
            OnPropertyChanged()
        End Set
    End Property

    Private _golongan As String
    Public Property Golongan As String
        Get
            Return _golongan
        End Get
        Set(ByVal value As String)
            _golongan = value
            OnPropertyChanged()
        End Set
    End Property

    Private _jumlahHariKerja As String
    Public Property JumlahHariKerja As String
        Get
            Return _jumlahHariKerja
        End Get
        Set(ByVal value As String)
            _jumlahHariKerja = value
            OnPropertyChanged()
        End Set
    End Property

    Private _jumlahJamLembur As String
    Public Property JumlahJamLembur As String
        Get
            Return _jumlahJamLembur
        End Get
        Set(ByVal value As String)
            _jumlahJamLembur = value
            OnPropertyChanged()
        End Set
    End Property

    Private _gajiLembur As String
    Public Property GajiLembur As String
        Get
            Return _gajiLembur
        End Get
        Set(ByVal value As String)
            _gajiLembur = value
            OnPropertyChanged()
        End Set
    End Property

    Private _penguranganGajiBersih As String
    Public Property PenguranganGajiBersih As String
        Get
            Return _penguranganGajiBersih
        End Get
        Set(ByVal value As String)
            _penguranganGajiBersih = value
            OnPropertyChanged()
        End Set
    End Property


    Private _gajiPokok As String
    Public Property GajiPokok As String
        Get
            Return _gajiPokok
        End Get
        Set(ByVal value As String)
            _gajiPokok = value
            OnPropertyChanged()
        End Set
    End Property

    Private _tunjanganJabatan As String
    Public Property TunjanganJabatan As String
        Get
            Return _tunjanganJabatan
        End Get
        Set(ByVal value As String)
            _tunjanganJabatan = value
            OnPropertyChanged()
        End Set
    End Property

    Private _tunjanganMakan As String
    Public Property TunjanganMakan As String
        Get
            Return _tunjanganMakan
        End Get
        Set(ByVal value As String)
            _tunjanganMakan = value
            OnPropertyChanged()
        End Set
    End Property

    Private _tunjanganTransportasi As String
    Public Property TunjanganTransportasi As String
        Get
            Return _tunjanganTransportasi
        End Get
        Set(ByVal value As String)
            _tunjanganTransportasi = value
            OnPropertyChanged()
        End Set
    End Property

    Private _potonganAlpa As String
    Public Property PotonganAlpa As String
        Get
            Return _potonganAlpa
        End Get
        Set(ByVal value As String)
            _potonganAlpa = value
            OnPropertyChanged()
        End Set
    End Property

    Private _gajiBersih As String
    Public Property GajiBersih As String
        Get
            Return _gajiBersih
        End Get
        Set(ByVal value As String)
            _gajiBersih = value
            OnPropertyChanged()
        End Set
    End Property

    ' Properti untuk Alpa, Izin, dan Hadir
    Private _alpa As Boolean
    Public Property Alpa As Boolean
        Get
            Return _alpa
        End Get
        Set(ByVal value As Boolean)
            _alpa = value
            OnPropertyChanged()
        End Set
    End Property

    Private _izin As Boolean
    Public Property Izin As Boolean
        Get
            Return _izin
        End Get
        Set(ByVal value As Boolean)
            _izin = value
            OnPropertyChanged()
        End Set
    End Property

    Private _hadir As Boolean
    Public Property Hadir As Boolean
        Get
            Return _hadir
        End Get
        Set(ByVal value As Boolean)
            _hadir = value
            OnPropertyChanged()
        End Set
    End Property

    ' Properti untuk menyimpan data karyawan yang sesuai dengan ID yang dimasukkan
    Public Property EmployeeData As Employee
End Class
