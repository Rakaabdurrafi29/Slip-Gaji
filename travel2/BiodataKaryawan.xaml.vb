Imports System.Collections.ObjectModel
Imports System.IO
Imports System.Windows
Imports System.Xml.Serialization
Imports Microsoft.Win32
Imports System.Windows.Media.Imaging
Imports Devart.Data.MySql
Imports System.Data
Public Class BiodataKaryawan

    Public KONEKSI As MySqlConnection
    Public CMD As MySqlCommand
    Public DR As MySqlDataReader
    Public DA As MySqlDataAdapter
    Public DS As DataSet
    Private ReadOnly Property Employees As ObservableCollection(Of Employee)
    Private ReadOnly Property XmlFilePath As String = "employees.xml"
    Private ReadOnly Property ImageFolderPath As String = "EmployeeImages\"

    ' Event handler untuk tombol "Open Data Penggajian"
    Private Sub OpenDataBiodata_Click(sender As Object, e As RoutedEventArgs)
        Dim dataBiodataWindow As New DataBiodata()
        dataBiodataWindow.Show()
    End Sub

    Public Sub New()
        InitializeComponent()
        Employees = New ObservableCollection(Of Employee)()
        lstEmployees.ItemsSource = Employees

        ' Membuat folder untuk menyimpan gambar karyawan jika belum ada
        If Not Directory.Exists(ImageFolderPath) Then
            Directory.CreateDirectory(ImageFolderPath)
        End If

        ' Memuat data karyawan saat aplikasi dimulai
        LoadEmployees()
    End Sub

    Private Sub AddButton_Click(sender As Object, e As RoutedEventArgs)
        ' Mengatur nilai-nilai gaji dan tunjangan berdasarkan golongan
        Dim gajiPokok As Double = Double.Parse(txtGajiPokok.Text)
        Dim tunjanganJabatan As Double = Double.Parse(txtTunjanganJabatan.Text)
        Dim tunjanganMakan As Double = Double.Parse(txtTunjanganMakan.Text)
        Dim tunjanganTransportasi As Double = Double.Parse(txtTunjanganTransportasi.Text)
        Dim gajiLembur As Double = Double.Parse(txtGajiLembur.Text)
        Dim potonganAlpa As Double = Double.Parse(txtPotonganAlpa.Text)
        Dim gajiBersih As Double = Double.Parse(txtGajiBersih.Text)

        ' Membuat instansi baru dari kelas Employee dan menetapkan nilainya
        Dim newEmployee As New Employee() With {
            .EmployeeId = txtEmployeeId.Text,
            .Name = txtName.Text,
            .JobTitle = txtJobTitle.Text,
            .Golongan = cmbGolongan.Text,
            .Gender = cmbGender.Text,
            .HireDate = dpHireDate.SelectedDate,
            .Phone = txtPhone.Text,
            .Email = txtEmail.Text,
            .BirthDate = dpBirthDate.SelectedDate,
            .PhotoPath = SaveImageAndGetPath(), ' Simpan path gambar bersama dengan data karyawan
            .GajiPokok = gajiPokok,
            .TunjanganJabatan = tunjanganJabatan,
            .TunjanganMakan = tunjanganMakan,
            .TunjanganTransportasi = tunjanganTransportasi,
            .GajiLembur = gajiLembur,
            .PotonganAlpa = potonganAlpa,
            .GajiBersih = gajiBersih
        }

        ' Menambahkan instansi karyawan ke dalam koleksi
        Employees.Add(newEmployee)
        ' Menyimpan data karyawan
        SaveEmployees()
        ' Membersihkan formulir
        ClearForm()

        ' Call SaveDataToMySQL method passing the newly created employee
        SaveDataToMySQL(newEmployee)

    End Sub

    Private Sub SaveDataToMySQL(newEmployee As Employee)
        Dim connectionString As String = "Server=localhost;Port=3306;Database=travel;User=root;Password=;"
        Using connection As New MySqlConnection(connectionString)
            connection.Open()

            ' Your existing SQL INSERT command
            Dim sql As String = "INSERT INTO tboidata (EmployeeId, Name, JobTitle, Golongan, Gender, HireDate, Phone, Email, BirthDate, PhotoPath, GajiPokok, TunjanganJabatan, TunjanganMakan, TunjanganTransportasi, GajiLembur, PotonganAlpa, GajiBersih) " &
                            "VALUES (@EmployeeId, @Name, @JobTitle, @Golongan, @Gender, @HireDate, @Phone, @Email, @BirthDate, @PhotoPath, @GajiPokok, @TunjanganJabatan, @TunjanganMakan, @TunjanganTransportasi, @GajiLembur, @PotonganAlpa, @GajiBersih)"

            Using command As New MySqlCommand(sql, connection)
                command.Parameters.AddWithValue("@EmployeeId", newEmployee.EmployeeId)
                command.Parameters.AddWithValue("@Name", newEmployee.Name)
                command.Parameters.AddWithValue("@JobTitle", newEmployee.JobTitle)
                command.Parameters.AddWithValue("@Golongan", newEmployee.Golongan)
                command.Parameters.AddWithValue("@Gender", newEmployee.Gender)
                command.Parameters.AddWithValue("@HireDate", newEmployee.HireDate)
                command.Parameters.AddWithValue("@Phone", newEmployee.Phone)
                command.Parameters.AddWithValue("@Email", newEmployee.Email)
                command.Parameters.AddWithValue("@BirthDate", newEmployee.BirthDate)
                command.Parameters.AddWithValue("@PhotoPath", newEmployee.PhotoPath)
                command.Parameters.AddWithValue("@GajiPokok", newEmployee.GajiPokok)
                command.Parameters.AddWithValue("@TunjanganJabatan", newEmployee.TunjanganJabatan)
                command.Parameters.AddWithValue("@TunjanganMakan", newEmployee.TunjanganMakan)
                command.Parameters.AddWithValue("@TunjanganTransportasi", newEmployee.TunjanganTransportasi)
                command.Parameters.AddWithValue("@GajiLembur", newEmployee.GajiLembur)
                command.Parameters.AddWithValue("@PotonganAlpa", newEmployee.PotonganAlpa)
                command.Parameters.AddWithValue("@GajiBersih", newEmployee.GajiBersih)

                ' Execute the INSERT command
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub


    Private Sub ClearFormButton_Click(sender As Object, e As RoutedEventArgs)
        ClearForm()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As RoutedEventArgs)
        ' Tambahkan logika untuk tombol Close di sini
        Dim nextPage As New NextPage()
        nextPage.Show()
        Close()
    End Sub

    Private Sub ClearForm()
        txtEmployeeId.Text = String.Empty
        txtName.Text = String.Empty
        txtJobTitle.Text = String.Empty
        cmbGolongan.SelectedIndex = -1
        cmbGender.SelectedIndex = -1 ' Menambah baris untuk membersihkan ComboBox Gender
        dpHireDate.SelectedDate = Nothing
        txtPhone.Text = String.Empty
        txtEmail.Text = String.Empty
        dpBirthDate.SelectedDate = Nothing
        imgEmployeePhoto.Source = Nothing ' Hapus gambar dari elemen Image saat menghapus form

        ' Membersihkan nilai-nilai gaji
        txtGajiBersih.Text = String.Empty
        txtGajiLembur.Text = String.Empty
        txtGajiPokok.Text = String.Empty
        txtPotonganAlpa.Text = String.Empty
        txtTunjanganJabatan.Text = String.Empty
        txtTunjanganMakan.Text = String.Empty
        txtTunjanganTransportasi.Text = String.Empty
    End Sub


    Private Sub cmbGolongan_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If cmbGolongan.SelectedItem IsNot Nothing Then
            Dim selectedGolongan As String = DirectCast(cmbGolongan.SelectedItem, ComboBoxItem).Content.ToString()
            Select Case selectedGolongan
                Case "HRD"
                    txtJobTitle.Text = "HRD"
                    txtGajiPokok.Text = "10000000"
                    txtTunjanganJabatan.Text = "15000000"
                    txtTunjanganMakan.Text = "350000"
                    txtTunjanganTransportasi.Text = "500000"
                    txtGajiLembur.Text = "2000000"
                    txtPotonganAlpa.Text = "500000"
                Case "Manager"
                    txtJobTitle.Text = "Manager"
                    txtGajiPokok.Text = "8000000"
                    txtTunjanganJabatan.Text = "10000000"
                    txtTunjanganMakan.Text = "300000"
                    txtTunjanganTransportasi.Text = "300000"
                    txtGajiLembur.Text = "1000000"
                    txtPotonganAlpa.Text = "300000"
                Case "Staff"
                    txtJobTitle.Text = "Staff"
                    txtGajiPokok.Text = "5000000"
                    txtTunjanganJabatan.Text = "5000000"
                    txtTunjanganMakan.Text = "150000"
                    txtTunjanganTransportasi.Text = "250000"
                    txtGajiLembur.Text = "1000000"
                    txtPotonganAlpa.Text = "250000"
            End Select

            ' Hitung gaji bersih
            Dim gajiPokok As Double = Double.Parse(txtGajiPokok.Text)
            Dim tunjanganJabatan As Double = Double.Parse(txtTunjanganJabatan.Text)
            Dim tunjanganMakan As Double = Double.Parse(txtTunjanganMakan.Text)
            Dim tunjanganTransportasi As Double = Double.Parse(txtTunjanganTransportasi.Text)
            Dim gajiLembur As Double = Double.Parse(txtGajiLembur.Text)
            Dim potonganAlpa As Double = Double.Parse(txtPotonganAlpa.Text)
            Dim gajiBersih As Double = gajiPokok + tunjanganJabatan + tunjanganMakan + tunjanganTransportasi + gajiLembur - potonganAlpa

            txtGajiBersih.Text = gajiBersih.ToString()
        End If
    End Sub


    Private Sub LoadEmployees()
        If File.Exists(XmlFilePath) Then
            Using fileStream As FileStream = File.OpenRead(XmlFilePath)
                Dim serializer As New XmlSerializer(GetType(List(Of Employee)))
                Dim loadedEmployees As List(Of Employee) = DirectCast(serializer.Deserialize(fileStream), List(Of Employee))
                Employees.Clear()
                For Each employee In loadedEmployees
                    If Not String.IsNullOrEmpty(employee.PhotoPath) AndAlso File.Exists(employee.PhotoPath) Then
                        ' Memuat gambar dari path yang disimpan
                        Dim bitmapImage As New BitmapImage()
                        bitmapImage.BeginInit()
                        bitmapImage.UriSource = New Uri(employee.PhotoPath, UriKind.RelativeOrAbsolute)
                        bitmapImage.EndInit()
                        employee.Photo = bitmapImage
                    End If
                    Employees.Add(employee)
                Next
            End Using
        End If

        ' Setelah data karyawan dimuat, buat instance baru dari Absensi dan berikan data karyawan yang diperlukan
        'Dim absensiPage As New Absensi()
        'absensiPage.EmployeeData = Employees
        'absensiPage.Show()
    End Sub

    Private Sub SaveEmployees()
        Using fileStream As FileStream = File.Create(XmlFilePath)
            Dim serializer As New XmlSerializer(GetType(List(Of Employee)))
            serializer.Serialize(fileStream, Employees.ToList())
        End Using
    End Sub

    Private Function SaveImageAndGetPath() As String
        If imgEmployeePhoto.Source IsNot Nothing AndAlso TypeOf imgEmployeePhoto.Source Is BitmapImage Then
            ' Membuat path baru untuk gambar
            Dim newImagePath As String = Path.Combine(ImageFolderPath, $"{Guid.NewGuid()}.png")

            ' Menyimpan gambar
            Dim encoder As New PngBitmapEncoder()
            encoder.Frames.Add(BitmapFrame.Create(CType(imgEmployeePhoto.Source, BitmapImage)))
            Using fileStream As New FileStream(newImagePath, FileMode.Create)
                encoder.Save(fileStream)
            End Using

            Return newImagePath
        Else
            Return String.Empty
        End If
    End Function

    Private Sub Window_Closed(sender As Object, e As EventArgs)
        SaveEmployees() ' Simpan data saat aplikasi ditutup
    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        LoadEmployees() ' Muat data saat aplikasi dibuka
    End Sub

    Private Sub UploadPhotoButton_Click(sender As Object, e As RoutedEventArgs)
        Dim openFileDialog As New OpenFileDialog()
        openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*"
        If openFileDialog.ShowDialog() = True Then
            ' Mengambil path foto dari file yang dipilih
            Dim photoPath As String = openFileDialog.FileName

            ' Menampilkan gambar di elemen Image
            imgEmployeePhoto.Source = New BitmapImage(New Uri(photoPath))
        End If
    End Sub

    Private Sub SavePhotoButton_Click(sender As Object, e As RoutedEventArgs)
        If imgEmployeePhoto.Source IsNot Nothing AndAlso TypeOf imgEmployeePhoto.Source Is BitmapImage Then
            Dim newImagePath As String = Path.Combine(ImageFolderPath, $"{Guid.NewGuid()}.png")

            Dim encoder As New PngBitmapEncoder()
            encoder.Frames.Add(BitmapFrame.Create(CType(imgEmployeePhoto.Source, BitmapImage)))
            Using fileStream As New FileStream(newImagePath, FileMode.Create)
                encoder.Save(fileStream)
            End Using

            imgEmployeePhoto.Source = New BitmapImage(New Uri(newImagePath, UriKind.RelativeOrAbsolute)) ' Tampilkan gambar di elemen Image

        Else
            MessageBox.Show("No image to save.")
        End If
    End Sub

    Private Sub UpdateButton_Click(sender As Object, e As RoutedEventArgs)
        If lstEmployees.SelectedItem IsNot Nothing Then
            Dim selectedEmployee As Employee = TryCast(lstEmployees.SelectedItem, Employee)
            selectedEmployee.EmployeeId = txtEmployeeId.Text
            selectedEmployee.Name = txtName.Text
            selectedEmployee.JobTitle = txtJobTitle.Text
            selectedEmployee.Golongan = cmbGolongan.Text ' Menyesuaikan dengan .Golongan
            selectedEmployee.Gender = cmbGender.Text
            selectedEmployee.HireDate = dpHireDate.SelectedDate
            selectedEmployee.Phone = txtPhone.Text
            selectedEmployee.Email = txtEmail.Text
            selectedEmployee.BirthDate = dpBirthDate.SelectedDate
            selectedEmployee.GajiBersih = txtGajiBersih.Text
            selectedEmployee.GajiLembur = txtGajiLembur.Text
            selectedEmployee.GajiPokok = txtGajiPokok.Text
            selectedEmployee.PotonganAlpa = txtPotonganAlpa.Text
            selectedEmployee.TunjanganJabatan = txtTunjanganJabatan.Text
            selectedEmployee.TunjanganMakan = txtTunjanganMakan.Text
            selectedEmployee.TunjanganTransportasi = txtTunjanganTransportasi.Text

            SaveEmployees()

            ' Memperbarui ObservableCollection dengan data yang diperbarui
            Employees.Clear()
            LoadEmployees()

            ClearForm()
        Else
            MessageBox.Show("Please select an employee to update.")
        End If
    End Sub

    Private Sub DeleteButton_Click(sender As Object, e As RoutedEventArgs)
        If lstEmployees.SelectedItem IsNot Nothing Then
            Dim selectedEmployee As Employee = TryCast(lstEmployees.SelectedItem, Employee)
            Employees.Remove(selectedEmployee)
            SaveEmployees()
            ClearForm()
        Else
            MessageBox.Show("Please select an employee to delete.")
        End If
    End Sub
End Class

Public Class Employee
    Public Property EmployeeId As String
    Public Property Name As String
    Public Property JobTitle As String
    Public Property Golongan As String
    Public Property Gender As String
    Public Property HireDate As Date?
    Public Property Phone As String
    Public Property Email As String
    Public Property BirthDate As Date?
    Public Property PhotoPath As String
    <XmlIgnore>
    Public Property Photo As BitmapImage

    ' Properti-properti gaji
    Public Property GajiPokok As Double
    Public Property TunjanganJabatan As Double
    Public Property TunjanganMakan As Double
    Public Property TunjanganTransportasi As Double
    Public Property GajiLembur As Double
    Public Property PotonganAlpa As Double
    Public Property GajiBersih As Double
End Class


